using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Commons.Options;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.BankCode;
using Trivista.LoanApp.ApplicationCore.Features.LoanApproval;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;
using Trivista.LoanApp.ApplicationCore.Services.Payment;

namespace Trivista.LoanApp.ApplicationCore.Features.Customer;

public class RegenerateOtpFromPaystack : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/customer/approval/regeneratePayStackOtp/{id}", RegenerateOtpFromPaystackHandler)
            .WithName("Regenerate PayStackOtp")
            .WithTags("Customer")
        .RequireAuthorization();
    }

    private static async Task<IResult> RegenerateOtpFromPaystackHandler(IMediator mediator, RegenerateOtpFromPaystackCommand command)
    {
        var response = await mediator.Send(command);
        return response.ToOk(x => x);
    }
}

public sealed record RegenerateOtpFromPaystackCommand(Guid CustomerId,Guid LoanRequestId): IRequest<Result<Unit>>;

public sealed record RegenerateOtpFromPaystackCommandHandler : IRequestHandler<RegenerateOtpFromPaystackCommand, Result<Unit>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    private readonly TokenManager _token;

    private readonly ILogger<ApproveLoanCommandHandler> _logger;
    
    private readonly IPayStackService _payStackService;
    
    private readonly IRemittaService _remittaService;

    private readonly IMbsService _mbsService;


    public RegenerateOtpFromPaystackCommandHandler(
        TrivistaDbContext trivistaDbContext, 
        ILogger<ApproveLoanCommandHandler> logger, 
        IPayStackService payStackService, 
        IRemittaService remittaService, 
        IMbsService mbsService,
        TokenManager token)
    {
        _trivistaDbContext = trivistaDbContext;
        _logger = logger;
        _payStackService = payStackService;
        _remittaService = remittaService;
        _mbsService = mbsService;
        _token = token;
    }
    
    public async Task<Result<Unit>> Handle(RegenerateOtpFromPaystackCommand request, CancellationToken cancellationToken)
    {
        var userEmail = _token.GetEmail();

        var transactionReferenceNumber = Guid.NewGuid();
        var customer = await _trivistaDbContext.Customer.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);
        if (customer == null)
        {
            _logger.LogWarning("Customer is null");
            return new Result<Unit>(ExceptionManager.Manage("Loan Approval", "Unable to approve loan"));
        }
        
        var loanRequest = await _trivistaDbContext.LoanRequest
                                                  .Include(x=>x.ApprovalWorkflow)
                                                  .ThenInclude(x=>x.ApprovalWorkflowApplicationRole)
                                                  .Include(x=>x.RepaymentSchedules)
                                                  .AsSplitQuery()
                                                  .FirstOrDefaultAsync(x => x.Id == request.LoanRequestId, cancellationToken);
        if (loanRequest == null)
            return new Result<Unit>(ExceptionManager.Manage("Customer Loan Approval", "Loan request not found"));

        var doesApprovalExist = await _trivistaDbContext.DisbursementApproval.AsNoTracking().
            Where(x => x.LoanRequestId == loanRequest.Id).Select(x => x).FirstOrDefaultAsync(cancellationToken);
        if (doesApprovalExist != null)
        {
            if(doesApprovalExist.Status == DisbursedLoanStatus.Disbursed)
                return new Result<Unit>(ExceptionManager.Manage("Loan Approval", "Loan has alrady been disbursed"));
            _trivistaDbContext.DisbursementApproval.Remove(doesApprovalExist);
        }

        //Call payStack to disburse money in customer account
        var accountDetails = await _payStackService.ResolveAccount(loanRequest.SalaryDetails.SalaryAccountNumber, loanRequest.SalaryDetails.BankCode);
        
        if(!accountDetails.Status)
            return new Result<Unit>(ExceptionManager.Manage("Customer Loan Approval", accountDetails.Message));
        
        var paySackRecipientResponse = await _payStackService.TransferRecipient(new TransferRecipientRequestDto()
        {
            AccountNumber = accountDetails.Data.AccountNumber,
            BankCode = loanRequest.SalaryDetails.BankCode,
            Name = accountDetails.Data.AccountName,
            Currency = "NGN"
        });
        
        if(!paySackRecipientResponse.Status)
            return new Result<Unit>(ExceptionManager.Manage("Customer Loan Approval", paySackRecipientResponse.Message));
        
        var payment = await _payStackService.Transfer(new TransferRequestDto()
        {
            Source = "balance",
            Amount = loanRequest.LoanDetails.LoanAmount,
            Reason = loanRequest.LoanDetails.purpose,
            Recipient = paySackRecipientResponse.Data.RecipientCode,
            Reference = transactionReferenceNumber.ToString()
        });
        
        if(!payment.Status)
        {
            loanRequest.SetProviderAccountStatus();
            _logger.LogInformation("Unable to approve loan for user: {User}", userEmail);
            _logger.LogInformation(payment.Message);
            return new Result<Unit>(ExceptionManager.Manage("Customer Loan Approval", "Something went wrong, please contact support"));
        }

        var disbursementApproval = DisbursementApproval.Factory.Build(Guid.NewGuid(), loanRequest, "", payment.Data.TransferCode, transactionReferenceNumber.ToString());
        
        await _trivistaDbContext.DisbursementApproval.AddAsync(disbursementApproval, cancellationToken);

        var result = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        
        return result < 0 ? new Result<Unit>(ExceptionManager.Manage("Loan request", "Unable to gegerate paystack token.")) : Unit.Value;
    }
}