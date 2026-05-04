using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using Serilog.Core;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Commons.Options;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.BankCode;
using Trivista.LoanApp.ApplicationCore.Features.Dto;
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
            .WithTags("Customer")
        .RequireAuthorization();
    }

    private static async Task<IResult> ApproveLoanHandler(IMediator mediator, ApproveLoanByCustomerCommand command)
    {
        var response = await mediator.Send(command);
        return response.ToOk(x => x);
    }
}

public sealed record ApproveLoanByCustomerCommand(Guid CustomerId,Guid LoanRequestId): IRequest<Result<RecovaResponse>>;

public sealed record ApproveLoanByCustomerCommandHandler: IRequestHandler<ApproveLoanByCustomerCommand, Result<RecovaResponse>>
{
    //private readonly TrivistaDbContext _trivistaDbContext;

    //private readonly TokenManager _token;

    //private readonly ILogger<ApproveLoanCommandHandler> _logger;

    //private readonly IPayStackService _payStackService;

    //private readonly IRemittaService _remittaService;

    //private readonly IMbsService _mbsService;
    private readonly TrivistaDbContext _trivistaDbContext;
    private readonly IRecovaService _recovaService;
    private readonly ILogger<ApproveLoanByCustomerCommandHandler> _logger;

    public ApproveLoanByCustomerCommandHandler(
        TrivistaDbContext trivistaDbContext,
        IRecovaService recovaService,
        ILogger<ApproveLoanByCustomerCommandHandler> logger)
    {
        this._trivistaDbContext = trivistaDbContext;
        this._recovaService = recovaService;
        this._logger = logger;
    }


    //public ApproveLoanByCustomerCommandHandler(
    //    TrivistaDbContext trivistaDbContext, 
    //    ILogger<ApproveLoanCommandHandler> logger, 
    //    IPayStackService payStackService, 
    //    IRemittaService remittaService, 
    //    IMbsService mbsService,
    //    TokenManager token)
    //{
    //    _trivistaDbContext = trivistaDbContext;
    //    _logger = logger;
    //    _payStackService = payStackService;
    //    _remittaService = remittaService;
    //    _mbsService = mbsService;
    //    _token = token;
    //}

    public async Task<Result<RecovaResponse>> Handle(ApproveLoanByCustomerCommand request, CancellationToken cancellationToken)
    {
        try
        {

            var loanRequest = await _trivistaDbContext.LoanRequest.Include(x => x.Customer)
                                                  .Include(x => x.SalaryDetails)
                                                  .Include(x => x.ApprovalWorkflow)
                                                  .ThenInclude(x => x.ApprovalWorkflowApplicationRole)
                                                  .Include(x => x.RepaymentSchedules)
                                                  .AsSplitQuery()
                                                  .FirstOrDefaultAsync(x => x.Id == request.LoanRequestId
                                                  && x.CustomerId == request.CustomerId, cancellationToken);

            var recovaRequest = loanRequest!.Customer!.ToRecovaRequest(loanRequest!, loanRequest.SalaryDetails.BankCode);
            var recovaResponse = await _recovaService.CreateConsent(recovaRequest);
            //if (recovaResponse!.RequestStatus != "Initiated" || recovaResponse!.RequestStatus != "AwaitingConfirmation")
            //{
            //    this._logger.LogWarning("Unable to process request with Recova with status: {Status}", recovaResponse.RequestStatus);
            //    return new Result<RecovaResponse>(ExceptionManager.Manage("Loan Approval", "Unable to disburse loan to customer"));
            //}
            if (recovaResponse!.RequestStatus != "AwaitingConfirmation")
            {
                this._logger.LogWarning("Unable to process request with Recova with status: {Status}", recovaResponse.RequestStatus);
                return new Result<RecovaResponse>(ExceptionManager.Manage("Loan Approval", "Unable to disburse loan to customer"));
            }
            return recovaResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occured while customer initiated loan approval");
            return new Result<RecovaResponse>(ExceptionManager.Manage("Customer Loan Approval", "Something went wrong, please contact support"));
        }

        //var userEmail = _token.GetEmail();
        //var transactionReferenceNumber = Guid.NewGuid();
        //var customer = await _trivistaDbContext.Customer.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);
        //if (customer == null)
        //{
        //    _logger.LogWarning("Customer is null");
        //    return new Result<Unit>(ExceptionManager.Manage("Loan Approval", "Unable to approve loan"));
        //}

        //var loanRequest = await _trivistaDbContext.LoanRequest
        //                                          .Include(x=>x.ApprovalWorkflow)
        //                                          .ThenInclude(x=>x.ApprovalWorkflowApplicationRole)
        //                                          .Include(x=>x.RepaymentSchedules)
        //                                          .AsSplitQuery()
        //                                          .FirstOrDefaultAsync(x => x.Id == request.LoanRequestId, cancellationToken);
        //if (loanRequest == null)
        //    return new Result<Unit>(ExceptionManager.Manage("Customer Loan Approval", "Loan request not found"));

        ////Call payStack to disburse money in customer account

        //var accountDetails = await _payStackService.ResolveAccount(loanRequest.SalaryDetails.SalaryAccountNumber, loanRequest.SalaryDetails.BankCode);

        //if(!accountDetails.Status)
        //    return new Result<Unit>(ExceptionManager.Manage("Customer Loan Approval", accountDetails.Message));

        //var paySackRecipientResponse = await _payStackService.TransferRecipient(new TransferRecipientRequestDto()
        //{
        //    AccountNumber = accountDetails.Data.AccountNumber,
        //    BankCode = loanRequest.SalaryDetails.BankCode,
        //    Name = accountDetails.Data.AccountName,
        //    Currency = "NGN"
        //});

        //if(!paySackRecipientResponse.Status)
        //    return new Result<Unit>(ExceptionManager.Manage("Customer Loan Approval", paySackRecipientResponse.Message));

        //var payment = await _payStackService.Transfer(new TransferRequestDto()
        //{
        //    Source = "balance",
        //    Amount = loanRequest.LoanDetails.LoanAmount,
        //    Reason = loanRequest.LoanDetails.purpose,
        //    Recipient = paySackRecipientResponse.Data.RecipientCode,
        //    Reference = transactionReferenceNumber.ToString()
        //});

        //if(!payment.Status)
        //{
        //    loanRequest.SetProviderAccountStatus();
        //    _logger.LogInformation("Unable to approve loan for user: {User}", userEmail);
        //    _logger.LogInformation(payment.Message);
        //    return new Result<Unit>(ExceptionManager.Manage("Customer Loan Approval", "Something went wrong, please contact support"));
        //}

        //var disbursementApproval = DisbursementApproval.Factory.Build(Guid.NewGuid(), loanRequest, "", payment.Data.TransferCode, transactionReferenceNumber.ToString());

        //await _trivistaDbContext.DisbursementApproval.AddAsync(disbursementApproval, cancellationToken);

        //loanRequest.ApproveLoanByCustomer();

        //var result = await _trivistaDbContext.SaveChangesAsync(cancellationToken);

        //return result < 0 ? new Result<Unit>(ExceptionManager.Manage("Repayment Schedule", "Unable to approve loan request.")) : Unit.Value;
    }
}