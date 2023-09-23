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
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
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
    
    public DisbursementApprovalCommandHandler(TrivistaDbContext trivistaDbContext, ILogger<DisbursementApprovalCommandHandler> logger, IPayStackService payStackService, IPublisher publisher)
    {
        _trivistaDbContext = trivistaDbContext;
        _logger = logger;
        _payStackService = payStackService;
        _publisher = publisher;
    }
    
    public async Task<Result<Unit>> Handle(DisbursementApprovalCommand request, CancellationToken cancellationToken)
    {
        var validator = new DisbursementApprovalCommandValidation();
        var exceptionResult = await TrivistaValidationException<DisbursementApprovalCommandValidation, DisbursementApprovalCommand>
            .ManageException<Unit>(validator, request, cancellationToken, Unit.Value);
        
        if (!exceptionResult.IsSuccess)
            return exceptionResult;

        var approval = await _trivistaDbContext
                             .DisbursementApproval.Include(x=>x.LoanRequest)
                             .ThenInclude(x=>x.Customer)
                             .FirstOrDefaultAsync(x => x.Id == request.Command.Id, cancellationToken);
        
        if(approval == null)
            return new Result<Unit>(ExceptionManager.Manage("Loan Approval", "No loan to approve"));

        var account = await _payStackService.FinalizeTransfer(new FinalTransferRequestDto()
        {
            Otp = request.Command.Otp,
            TransferCode = approval.TransferCode
        });

        if (account.Status != true)
            return new Result<Unit>(ExceptionManager.Manage("Loan Approval",
                account.Message));
        
        approval.SetOtp(request.Command.Otp);
        
        approval.ApproveLoan();
        
        var transaction = Transaction.Factory.Build(Guid.NewGuid(), approval.TransactionReference, approval.LoanRequest.LoanDetails.LoanAmount,
                "", RepaymentStatus.Unpaid, true, TransactionType.Disbursement, approval.LoanRequest.Id)
            .SetCustomer(approval.LoanRequest.Customer);
        
        var loanRequest = await _trivistaDbContext.LoanRequest
                            .Include(x=>x.RepaymentSchedules)
                            .Include(x=>x.ApprovalWorkflow)
                            .ThenInclude(x=>x.ApprovalWorkflowApplicationRole)
                            .Where(x => x.Id == approval.LoanRequestId)
                            .Select(x => x)
                            .AsSplitQuery()
                            .FirstOrDefaultAsync(cancellationToken);

        loanRequest.SetLoanDisbursedStatus();

        _trivistaDbContext.LoanRequest.Update(loanRequest);
        
        await _trivistaDbContext.Transaction.AddAsync(transaction, cancellationToken);
        
        var saveChanges = await _trivistaDbContext.SaveChangesAsync(cancellationToken);

        if (saveChanges <= 0)
            return new Result<Unit>(ExceptionManager.Manage("Loan Approval",
                "Unable to approve loan, please try again later"));
        
        var roleId = loanRequest.ApprovalWorkflow.ApprovalWorkflowApplicationRole.FirstOrDefault().RoleId;
        
        var staff = await _trivistaDbContext.Customer.Where(x => x.RoleId == roleId.ToString()).Select(x=>x).FirstOrDefaultAsync(cancellationToken);

        await _publisher.Publish(new LoanDisbursedEvent()
        {
            AdminName = $"{staff?.FirstName} {staff.LastName}",
            AdminEmail = staff?.Email,
            CustomerName = $"{approval.LoanRequest.Customer.FirstName} {approval.LoanRequest.Customer.LastName}",
            InterestRate = approval.LoanRequest.Interest,
            LoanAmount = approval.LoanRequest.LoanDetails.LoanAmount,
            LoanTenure = approval.LoanRequest.LoanDetails.tenure,
            RepaymentScheduleType = approval.LoanRequest.RepaymentSchedules.FirstOrDefault().RepaymentType.ToString()
        }, cancellationToken);
        return Unit.Value;

    }
}