using Carter;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;
using Trivista.LoanApp.ApplicationCore.Services.Payment;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanApproval;

public class DisbursementApprovalController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/disbursement/approval/{id}", FinalLoanApproveHandler)
            .WithName("Loan disbursement By Admin")
            .WithTags("Admin");
        //.RequireAuthorization();
    }

    private static async Task<IResult> FinalLoanApproveHandler(IMediator mediator, Guid id, [FromBody]Approval command)
    {
        Approval cmd = new(id, command.Otp);
        var response = await mediator.Send(new DisbursementApprovalCommand(cmd));
        return response.ToOk(x => x);
    }
}

public sealed record Approval(Guid Id, string Otp);

public class DisbursementApprovalCommandValidation : AbstractValidator<DisbursementApprovalCommand>
{
    public DisbursementApprovalCommandValidation()
    {
        RuleFor(x => x.Command.Otp).NotNull().NotEmpty().WithMessage("OTP code must be set");
    }
}

public sealed record DisbursementApprovalCommand(Approval Command): IRequest<Result<Unit>>;

public sealed record DisbursementApprovalCommandHandler: IRequestHandler<DisbursementApprovalCommand, Result<Unit>>
{
    private readonly TrivistaDbContext _trivistaDbContext;

    private readonly ILogger<DisbursementApprovalCommandHandler> _logger;

    private readonly IPayStackService _payStackService;

    private readonly IPublisher _publisher;

    private readonly IMbsService _mbsService;

    private readonly IRemittaService _remittaService;

    public DisbursementApprovalCommandHandler(TrivistaDbContext trivistaDbContext,
        ILogger<DisbursementApprovalCommandHandler> logger,
        IPayStackService payStackService,
        IPublisher publisher,
        IMbsService mbsService,
        IRemittaService remittaService)
    {
        _trivistaDbContext = trivistaDbContext;
        _logger = logger;
        _payStackService = payStackService;
        _publisher = publisher;
        _mbsService = mbsService;
        _remittaService = remittaService;
    }

    public async Task<Result<Unit>> Handle(DisbursementApprovalCommand request, CancellationToken cancellationToken)
    {
        var validator = new DisbursementApprovalCommandValidation();
        var exceptionResult = await TrivistaValidationException<DisbursementApprovalCommandValidation, DisbursementApprovalCommand>
            .ManageException<Unit>(validator, request, cancellationToken, Unit.Value);

        if (!exceptionResult.IsSuccess)
            return exceptionResult;

        var approval = await _trivistaDbContext
                             .DisbursementApproval.Include(x => x.LoanRequest)
                             .ThenInclude(x => x.Customer)
                             .FirstOrDefaultAsync(x => x.Id == request.Command.Id, cancellationToken);

        if (approval == null)
            return new Result<Unit>(ExceptionManager.Manage("Loan Approval", "No loan to approve"));

        var account = await _payStackService.FinalizeTransfer(new FinalTransferRequestDto()
        {
            Otp = request.Command.Otp,
            TransferCode = approval.TransferCode
        });

        if (account.Status)
            return new Result<Unit>(ExceptionManager.Manage("Loan Approval",
                account.Message));

        approval.SetOtp(request.Command.Otp);

        approval.ApproveLoan();

        var transaction = Transaction.Factory.Build(Guid.NewGuid(), approval.TransactionReference, approval.LoanRequest.LoanDetails.LoanAmount,
                "", RepaymentStatus.Unpaid, true, TransactionType.Disbursement, approval.LoanRequest.Id)
            .SetCustomer(approval.LoanRequest.Customer);

        var loanRequest = await _trivistaDbContext.LoanRequest
                            .Include(x => x.RepaymentSchedules)
                            .Include(x => x.ApprovalWorkflow)
                            .ThenInclude(x => x.ApprovalWorkflowApplicationRole)
                            .Where(x => x.Id == approval.LoanRequestId)
                            .Select(x => x)
                            .AsSplitQuery()
                            .FirstOrDefaultAsync(cancellationToken);

        loanRequest!.SetLoanDisbursedStatus();

        _trivistaDbContext.LoanRequest.Update(loanRequest);

        await _trivistaDbContext.Transaction.AddAsync(transaction, cancellationToken);

        var saveChanges = await _trivistaDbContext.SaveChangesAsync(cancellationToken);

        if (saveChanges <= 0)
            return new Result<Unit>(ExceptionManager.Manage("Loan Approval",
                "Unable to approve loan, please try again later"));

        var roleId = loanRequest.ApprovalWorkflow.ApprovalWorkflowApplicationRole.FirstOrDefault()!.RoleId;

        var staff = await _trivistaDbContext.Customer.Where(x => x.RoleId == roleId.ToString()).Select(x => x).FirstOrDefaultAsync(cancellationToken);

        await _publisher.Publish(new LoanDisbursedEvent()
        {
            AdminName = $"{staff?.FirstName} {staff!.LastName}",
            AdminEmail = staff!.Email,
            CustomerName = $"{approval.LoanRequest.Customer.FirstName} {approval.LoanRequest.Customer.LastName}",
            InterestRate = approval.LoanRequest.Interest,
            LoanAmount = approval.LoanRequest.LoanDetails.LoanAmount,
            LoanTenure = approval.LoanRequest.LoanDetails.tenure,
            RepaymentScheduleType = approval.LoanRequest.RepaymentSchedules.FirstOrDefault()!.RepaymentType.ToString()
        }, cancellationToken);

        await Task.Run(async () =>
        {
           _ = await InitiateRemitaDisbursement(_trivistaDbContext, _remittaService, loanRequest);
        });

        return Unit.Value;

    }

    private async Task<(LoanDisbursementResponseDto?, string)> InitiateRemitaDisbursement(TrivistaDbContext context, IRemittaService remitaService, LoanRequest  loanRequest)
    {
        //Call payStack to disburse money in customer account   
        var banksService = await _mbsService.SelectActiveRequestBanks();

        var bank = banksService.Result.Where(x => x.Name == loanRequest.SalaryDetails.BankName).Select(x => x).FirstOrDefault();
        if (bank == null)
            return (new LoanDisbursementResponseDto(), "Unable to validate customer bank, please try again later.");


        var request = new LoanDisbursementRequestDto()
        {
            CustomerId = loanRequest!.CustomerId.ToString(),
            PhoneNumber = loanRequest.Customer.PhoneNumber,
            AccountNumber = loanRequest.SalaryDetails.SalaryAccountNumber,
            Currency = "NGN",
            LoanAmount = loanRequest.LoanDetails.LoanAmount.ToString(),
            CollectionAmount = loanRequest.RepaymentSchedules.OrderBy(x => x.DueDate).Select(x => x.RepaymentAmount).FirstOrDefault().ToString(),
            DateOfDisbursement = DateTime.UtcNow.ToString(),
            DateOfCollection = loanRequest.RepaymentSchedules.OrderBy(x => x.DueDate).Select(x => x.DueDate).FirstOrDefault().ToString(),
            TotalCollectionAmount = loanRequest.RepaymentSchedules.Sum(x => x.RepaymentAmount).ToString(),
            NumberOfRepayments = loanRequest.RepaymentSchedules.Count.ToString(),
            BankCode = bank.SortCode
        };
        var disbursement = await remitaService.DisburseLoan(request);

        if(disbursement.Message.ToLower() != "Successful".ToLower() && disbursement.Status != "00")
        {
            //Log to data base here for retry.
            var failedRemitaDisbursementObject = FailedRemitaDisbursement.Factory.Build(loanRequest.Id, JsonConvert.SerializeObject(request));

            await context.FailedRemitaDisbursement.AddAsync(failedRemitaDisbursementObject);

            await context.SaveChangesAsync();
        }

        return (disbursement, "Successful");
    }
}