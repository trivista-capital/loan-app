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
            .FirstOrDefaultAsync(x => x.Id == request.Command.Id, cancellationToken);
        
        if (approval == null)
            return new Result<Unit>(ExceptionManager.Manage("Loan Approval", "No loan to approve"));
        
        var loanRequest = await _trivistaDbContext.LoanRequest
            .Include(x => x.Customer)
            .Include(x => x.RepaymentSchedules)
            .Include(x => x.ApprovalWorkflow)
            .ThenInclude(x => x.ApprovalWorkflowApplicationRole)
            .AsSplitQuery()
            .Where(x => x.Id == approval.LoanRequestId)
            .Select(x => x)
            .FirstOrDefaultAsync(cancellationToken);
        
        var transaction = Transaction.Factory.Build(Guid.NewGuid(), approval.TransactionReference, approval.LoanRequest.LoanDetails.LoanAmount,
                "", RepaymentStatus.Unpaid, true, TransactionType.Disbursement, approval.LoanRequest.Id)
            .SetCustomer(approval.LoanRequest.Customer);
        
        var (disbursementResult, _) = await InitiateRemitaDisbursement(loanRequest!);

        if (disbursementResult!.Status != "00")
            return new Result<Unit>(ExceptionManager.Manage("Loan Approval", "Unable to disburse loan to customer"));
        
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
            CustomerName = $"{approval.LoanRequest.Customer!.FirstName} {approval.LoanRequest.Customer.LastName}",
            InterestRate = approval.LoanRequest.Interest,
            LoanAmount = approval.LoanRequest.LoanDetails.LoanAmount,
            LoanTenure = approval.LoanRequest.LoanDetails.tenure,
            RepaymentScheduleType = approval.LoanRequest.RepaymentSchedules.FirstOrDefault()!.RepaymentType.ToString()
        }, cancellationToken);
        
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
        
        return Unit.Value;
    }

    private async Task<(LoanDisbursementResponseDto?, string)> InitiateRemitaDisbursement(LoanRequest  loanRequest)
    {
        _logger.LogInformation("Entered the InitiateRemitaDisbursement method");
        var dateOfDisbursement = DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss") + "+0000";
        var dateOfCollection = loanRequest.RepaymentSchedules.OrderBy(x => x.DueDate).Select(x => x.DueDate)
            .FirstOrDefault().ToString("dd-MM-yyyy HH:mm:ss") + "+0000";
        
        var remitaMandateResponse = await _remittaService.SalaryHistory(new GetSalaryHistoryRequestDto()
        {
            FirstName = loanRequest.Customer.FirstName,
            LastName = loanRequest.Customer.LastName,
            MiddleName = loanRequest.Customer.MiddleName,
            AccountNumber = loanRequest.SalaryDetails.SalaryAccountNumber,
            BankCode = loanRequest.SalaryDetails.BankCode,
            Bvn = loanRequest.Customer.Bvn
        }, Guid.NewGuid().ToString());

        if(remitaMandateResponse == null || !remitaMandateResponse.HasData || remitaMandateResponse.Status.ToUpper() != "success".ToUpper())
        {
            _logger.LogError("Unable to get response from remita service in CheckRemitaStatusHandler");
            return (new LoanDisbursementResponseDto(), "");
        }
        
        _logger.LogInformation("Building the LoanDisbursementRequestDto");
        var request = new LoanDisbursementRequestDto()
        {
            CustomerId = remitaMandateResponse.Data.CustomerId,
            PhoneNumber = loanRequest.Customer.PhoneNumber,
            AccountNumber = loanRequest.SalaryDetails.SalaryAccountNumber,
            Currency = "NGN",
            LoanAmount = loanRequest.LoanDetails.LoanAmount,
            CollectionAmount = loanRequest.RepaymentSchedules.OrderBy(x => x.DueDate).Select(x => x.RepaymentAmount).FirstOrDefault(),
            DateOfDisbursement = dateOfDisbursement,
            DateOfCollection = dateOfCollection,
            TotalCollectionAmount = loanRequest.RepaymentSchedules.Sum(x => x.RepaymentAmount),
            NumberOfRepayments = loanRequest.RepaymentSchedules.Count,
            BankCode = loanRequest.SalaryDetails.BankCode
        };
        _logger.LogInformation("Remita disbursement payload is: {Payload}", JsonConvert.SerializeObject(request));
        _logger.LogInformation("Calling remita DisburseLoan service");
        var disbursement = await _remittaService.DisburseLoan(request);
       
        _logger.LogInformation("Called remita DisburseLoan service");
        if (disbursement != null)
        {
            _logger.LogInformation("disbursement object from remita service is not null");
            if(disbursement.Message.ToLower() != "Successful".ToLower() && disbursement.Status != "00")
            {
                _logger.LogInformation("disbursement is not successful");
                //Log to data base here for retry.
                var failedRemitaDisbursementObject = FailedRemitaDisbursement.Factory.Build(loanRequest.Id, JsonConvert.SerializeObject(request));

                await _trivistaDbContext.FailedRemitaDisbursement.AddAsync(failedRemitaDisbursementObject);

                await _trivistaDbContext.SaveChangesAsync();
                _logger.LogInformation("Saved failed response from remita");
                
                return (new LoanDisbursementResponseDto(), "");
            }
        }

        if (disbursement == null)
        {
            _logger.LogInformation("Remita returned null");
            //Log to data base here for retry.
            var failedRemitaDisbursementObject = FailedRemitaDisbursement.Factory.Build(loanRequest.Id, JsonConvert.SerializeObject(request));

            await _trivistaDbContext.FailedRemitaDisbursement.AddAsync(failedRemitaDisbursementObject);

            await _trivistaDbContext.SaveChangesAsync();   
            _logger.LogInformation("Remita returned null and is saved in database");
            
            return (new LoanDisbursementResponseDto(), "");
        }

        _logger.LogInformation("Successful response from renita is: {RemitaDisbursement}", disbursement);
        return (disbursement, "Successful");
    }
}