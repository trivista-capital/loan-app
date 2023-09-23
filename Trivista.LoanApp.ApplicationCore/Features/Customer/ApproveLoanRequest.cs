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

public class ApproveLoanRequest: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/customer/approval/LoanRequest/{id}", ApproveLoanHandler)
            .WithName("Approve Loan By Customer")
            .WithTags("Customer");
        //.RequireAuthorization();
    }

    private static async Task<IResult> ApproveLoanHandler(IMediator mediator, ApproveLoanByCustomerCommand command)
    {
        var response = await mediator.Send(command);
        return response.ToOk(x => x);
    }
}

public sealed record ApproveLoanByCustomerCommand(Guid CustomerId,Guid LoanRequestId): IRequest<Result<Unit>>;

public sealed record ApproveLoanByCustomerCommandHandler: IRequestHandler<ApproveLoanByCustomerCommand, Result<Unit>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    private readonly TokenManager _token;

    private readonly ILogger<ApproveLoanCommandHandler> _logger;
    
    private readonly IPayStackService _payStackService;
    
    private readonly IRemittaService _remittaService;

    private readonly RemittaOption _remittaOption;

    private readonly IMbsService _mbsService;

    public ApproveLoanByCustomerCommandHandler(TrivistaDbContext trivistaDbContext, TokenManager token, ILogger<ApproveLoanCommandHandler> logger, IPayStackService payStackService, 
        IRemittaService remittaService, IMbsService mbsService)
    {
        _trivistaDbContext = trivistaDbContext;
        _token = token;
        _logger = logger;
        _payStackService = payStackService;
        _remittaService = remittaService;
        _mbsService = mbsService;
    }
    
    public async Task<Result<Unit>> Handle(ApproveLoanByCustomerCommand request, CancellationToken cancellationToken)
    {
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
        
        //Call payStack to disburse money in customer account   
        var banksService = await _mbsService.SelectActiveRequestBanks();

        var bank = banksService.Result.Where(x => x.Name == loanRequest.SalaryDetails.BankName).Select(x=>x).FirstOrDefault();
        if(bank == null)
            return new Result<Unit>(ExceptionManager.Manage("Customer Loan Approval", "Unable to verify customer bank"));

        var accountDetails = await _payStackService.ResolveAccount(loanRequest.SalaryDetails.SalaryAccountNumber, bank.SortCode);
        
        if(!accountDetails.Status)
            return new Result<Unit>(ExceptionManager.Manage("Customer Loan Approval", accountDetails.Message));
        
        var paySackRecipientResponse = await _payStackService.TransferRecipient(new TransferRecipientRequestDto()
        {
            AccountNumber = accountDetails.Data.AccountNumber,
            BankCode = bank.SortCode,
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
            return new Result<Unit>(ExceptionManager.Manage("Customer Loan Approval", payment.Message));
        
        var disbursementApproval = DisbursementApproval.Factory.Build(Guid.NewGuid(), loanRequest, "", payment.Data.TransferCode, transactionReferenceNumber.ToString());
        
        await _trivistaDbContext.DisbursementApproval.AddAsync(disbursementApproval, cancellationToken);
        
        loanRequest.ApproveLoanByCustomer();
        
        // startDate = loanRequest.RepaymentSchedules.OrderBy(x=>x.DueDate).FirstOrDefault().DueDate.ToString(),
        //EndDate = loanRequest.RepaymentSchedules.OrderByDescending(x=>x.DueDate).FirstOrDefault().DueDate.ToString(),
        //MaxNoOfDebits = loanRequest.RepaymentSchedules.Count
        
        // //Call Remitta
        // var remittaMandateResponse = await _remittaService.SalaryHistory(new GetSalaryHistoryRequestDto()
        // {
        //     FirstName = loanRequest.kycDetails.CustomerFirstName,
        //     LastName = loanRequest.kycDetails.CustomerLastName,
        //     MiddleName = loanRequest.kycDetails.CustomerMiddleName,
        //     AccountNumber = loanRequest.SalaryDetails.SalaryAccountNumber,
        //     BankCode = bank.SortCode,
        //     Bvn = loanRequest.Bvn
        // }, loanRequest.Id.ToString());
        //
        // if (remittaMandateResponse is { HasData: true } && remittaMandateResponse.Status.ToUpper() == "success".ToUpper() && remittaMandateResponse.HasData)
        // {
        //     var customerFromDb = await _trivistaDbContext.Customer.FirstOrDefaultAsync(x => x.Id == loanRequest.CustomerId, new CancellationToken());
        //     customerFromDb.IsRemittaUser();
        //     //customerFromDb.SetRemittaMandate(remittaMandateResponse);
        //     
        //     var result = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        //
        //     return result < 0 ? new Result<Unit>(ExceptionManager.Manage("Repayment Schedule", "Unable to approve loan request.")) : Unit.Value;  
        // }

        var result = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        
        return result < 0 ? new Result<Unit>(ExceptionManager.Manage("Repayment Schedule", "Unable to approve loan request.")) : Unit.Value;
    }
}