using Carter;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Commons.Options;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest.EventHandlers;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;
using ValidationFailure = FluentValidation.Results.ValidationFailure;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanApproval;

public sealed class ApproveLoan: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/approval/LoanRequest/{id}", ApproveLoanHandler)
            .WithName("Approve Loan")
            .WithTags("Admin")
        .RequireAuthorization();
    }

    private static async Task<IResult> ApproveLoanHandler(IMediator mediator, ApproveLoanCommand command)
    {
        var response = await mediator.Send(command);
        return response.ToOk(x => x);
    }
}

public sealed record ApproveLoanCommand(Guid Id, decimal interestRate, decimal LoanAmount): IRequest<Result<Unit>>;

public sealed record ApproveLoanCommandHandler: IRequestHandler<ApproveLoanCommand, Result<Unit>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    private readonly TokenManager _token;

    private readonly ILogger<ApproveLoanCommandHandler> _logger;

    private readonly IPublisher _publisher;

    private readonly IRemittaService _remittaService;

    private readonly RemittaOption _remittaOption;
    public ApproveLoanCommandHandler(TrivistaDbContext trivistaDbContext, TokenManager token, ILogger<ApproveLoanCommandHandler> logger, IPublisher publisher, IRemittaService remittaService,
        IOptions<RemittaOption> remittaOption)
    {
        _trivistaDbContext = trivistaDbContext;
        _token = token;
        _logger = logger;
        _publisher = publisher;
        _remittaService = remittaService;
        _remittaOption = remittaOption.Value;
    }
    
    public async Task<Result<Unit>> Handle(ApproveLoanCommand request, CancellationToken cancellationToken)
    {
        var roleId = _token.GetRoleId();
        if (string.IsNullOrEmpty(roleId))
        {
            _logger.LogWarning("Role is null");
            return new Result<Unit>(ExceptionManager.Manage("Loan Approval", "Approver role is not known. Please logout and try again, else contact the admin"));   
        }
        
        var userId = _token.GetUserId();
        if (string.IsNullOrEmpty(userId))
        {
            _logger.LogWarning("Customer id is null");
            return new Result<Unit>(ExceptionManager.Manage("Loan Approval", "Approver is not known. Please logout and try again, else contact the admin"));
        }
        
        var loanRequest = await _trivistaDbContext.LoanRequest
                                                  .Include(x=>x.Customer)
                                                  .Include(x=>x.ApprovalWorkflow)
                                                  .ThenInclude(x=>x.ApprovalWorkflowApplicationRole)
                                                  .Include(x=>x.RepaymentSchedules)
                                                  .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        
        if (loanRequest == null)
            return new Result<Unit>(ExceptionManager.Manage("Repayment Schedule", "Loan request not found"));

        var roles = loanRequest.ApprovalWorkflow.ApprovalWorkflowApplicationRole.OrderBy(x=>x.Hierarchy).ToList();
        
        foreach (var role in roles)
        {
            if (!role.IsApproved)
            {
                if (role.RoleId != Guid.Parse(roleId))
                    return new Result<Unit>(ExceptionManager.Manage("Repayment Schedule", "Unable to approve request, Please notify other approvers"));
                await SetApproval(roles, role, Guid.Parse(userId), loanRequest, request, cancellationToken, loanRequest.ApprovalWorkflow.Id, _publisher);
                break;
            }
            if (role.IsApproved && role.RoleId == Guid.Parse(roleId))
            {
                return new Result<Unit>(ExceptionManager.Manage("Repayment Schedule", "Unable to approve, as its already approved by you"));
            }
        }

        var result = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        
        return result < 0 ? new Result<Unit>(ExceptionManager.Manage("Repayment Schedule", "Unable to approve loan request")) : Unit.Value;
    }
    
    private async Task<Result<Unit>> SetApproval(List<ApprovalWorkflowApplicationRole> roles, ApprovalWorkflowApplicationRole role, Guid approvedBy, LoanRequest loanRequest, 
                                                    ApproveLoanCommand request, CancellationToken cancellationToken, Guid workflowId, IPublisher publisher)
    {
        var isLast = ApprovalWorkflowApplicationRole.IsLastApproval(roles, workflowId);
        if (isLast)
        {
            loanRequest.SetInterestRate(request.interestRate).ChangeLoanAmount(request.LoanAmount < 1 ? loanRequest.LoanDetails.LoanAmount : request.LoanAmount);
            
            var loan = await _trivistaDbContext.Loan.FirstOrDefaultAsync(x => x.IsDefault, new CancellationToken());
            
            if (loan == null)
                return new Result<Unit>(ExceptionManager.Manage("Repayment Schedule", "Loan not Configured"));
            
            var interest = Convert.ToDecimal((loan.InterestRate / 100) * request.LoanAmount);
            
            var loanTotalRepaymentAmount = Loan.TotalRepaymentAmount(interest, request.LoanAmount);
        
            loanRequest.SetLoanBalance(loanTotalRepaymentAmount);
            
            var repaymentSchedule = RepaymentSchedule.Factory.GenerateLoanSchedule(loanTotalRepaymentAmount,
                loanRequest.LoanDetails.RepaymentScheduleType,
                loanRequest.Id, loanRequest.LoanDetails.tenure);
        
            if(!repaymentSchedule.Any())
                return new Result<Unit>(ExceptionManager.Manage("Loan Request", "Unable to generate repayment schedule, please try approving again"));
            
            role.Approve(approvedBy);
            
            var workflow = await _trivistaDbContext.ApprovalWorkflow.FirstOrDefaultAsync(x => x.Id == workflowId, new CancellationToken());
            
            workflow.SetApprovalIfLastApprover(roles, role, approvedBy, loanRequest);

            await _trivistaDbContext.RepaymentSchedule.AddRangeAsync(repaymentSchedule, new CancellationToken());

            publisher.Publish(new LoanApprovedByAdminSucceededEvent()
            {
                To = loanRequest.Customer.Email,
                Name = $"{loanRequest?.Customer?.FirstName} {loanRequest?.Customer?.LastName}",
                InterestRate = interest, 
                LoanAmount = loanTotalRepaymentAmount, 
                LoanTenure = loanRequest.LoanDetails.tenure
            });
        }
        else
        {
            role.Approve(approvedBy);
        }

        return Unit.Value;
    }
}