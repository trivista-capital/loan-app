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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Enums;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Features.Account;
using Trivista.LoanApp.ApplicationCore.Features.LoanApproval;
using Trivista.LoanApp.ApplicationCore.Filters;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;
using Trivista.LoanApp.ApplicationCore.Services.Payment;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Webhook
{
    public class RecovaMandateCreationApa : ICarterModule// Recova first calls this, then we debit customer
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/directDebitMandate", RecovaDirectDebitCommandHandler)
            .WithName("Debit mandate")
            .WithTags("Recova");
        }

        private async Task<IResult> RecovaDirectDebitCommandHandler(IMediator mediator,
            IApiKeyValidator apiKeyValidator, 
            HttpContext context,
            [FromBody]RecovaDirectDebitCommand model)
        {
            const string ApiKeyHeaderName = "X-API-Key";
            string apiKey = context.Request.Headers[ApiKeyHeaderName]!;

            var isKeyValid = await apiKeyValidator.IsValid(apiKey);
            if (!isKeyValid)
            {
                return Results.Unauthorized();
            }
            var result = await mediator.Send(model);
            return result.ToOk(x => x);
        }
    }

    public sealed record RecovaDirectDebitCommand(string LoanReference, string InstitutionCode): IRequest<Result<bool>>;

    public sealed record RecovaDirectDebitCommandHandler : IRequestHandler<RecovaDirectDebitCommand, Result<bool>>
    {
        private readonly TrivistaDbContext _trivistaDbContext;

        private readonly ILogger<RecovaDirectDebitCommandHandler> _logger;

        private readonly IPayStackService _payStackService;

        public RecovaDirectDebitCommandHandler(TrivistaDbContext trivistaDbContext,
        ILogger<RecovaDirectDebitCommandHandler> logger,
        IPayStackService payStackService)
        {
            _trivistaDbContext = trivistaDbContext;
            _logger = logger;
            _payStackService = payStackService;
        }

        public async Task<Result<bool>> Handle(RecovaDirectDebitCommand request, CancellationToken cancellationToken)
        {
            //Use the loan reference which is the loan id. Aalso call paystack

            try
            {
                if(string.IsNullOrEmpty(request.LoanReference))
                    return new Result<bool>(ExceptionManager.Manage("Loan Approval", "Loanreference can not be null or empty"));

                if (EnumHelpers.IsStringValidGuid(request.LoanReference))
                    return new Result<bool>(ExceptionManager.Manage("Loan Approval", "Loanreference is invalid"));

                var loanId = Guid.Parse(request.LoanReference);

                var loanRequest = await _trivistaDbContext.LoanRequest.Include(x => x.Customer)
                                                              .Include(x => x.ApprovalWorkflow)
                                                              .ThenInclude(x => x.ApprovalWorkflowApplicationRole)
                                                              .Include(x => x.RepaymentSchedules)
                                                              .AsSplitQuery()
                                                              .FirstOrDefaultAsync(x => x.Id == loanId, cancellationToken);

                var command = JsonConvert.SerializeObject(request);
                loanRequest!.SetDirectDebitCommand(command);
                loanRequest!.ApproveIsDirectDebitCommand();

                var userEmail = loanRequest!.Customer.Email;

                var transactionReferenceNumber = Guid.NewGuid();
                var customer = await _trivistaDbContext.Customer.AsNoTracking().FirstOrDefaultAsync(x => x.Id == loanRequest!.Customer.Id, cancellationToken);
                if (customer == null)
                {
                    _logger.LogWarning("Customer is null");
                    return new Result<bool>(ExceptionManager.Manage("Loan Approval", "Unable to approve loan"));
                }

                //Call payStack to disburse money in customer account

                var accountDetails = await _payStackService.ResolveAccount(loanRequest.SalaryDetails.SalaryAccountNumber, loanRequest.SalaryDetails.BankCode);

                if (!accountDetails.Status)
                    return new Result<bool>(ExceptionManager.Manage("Customer Loan Approval", accountDetails.Message));

                var paySackRecipientResponse = await _payStackService.TransferRecipient(new TransferRecipientRequestDto()
                {
                    AccountNumber = accountDetails.Data.AccountNumber,
                    BankCode = loanRequest.SalaryDetails.BankCode,
                    Name = accountDetails.Data.AccountName,
                    Currency = "NGN"
                });

                if (!paySackRecipientResponse.Status)
                    return new Result<bool>(ExceptionManager.Manage("Customer Loan Approval", paySackRecipientResponse.Message));

                var payment = await _payStackService.Transfer(new TransferRequestDto()
                {
                    Source = "balance",
                    Amount = loanRequest.LoanDetails.LoanAmount,
                    Reason = loanRequest.LoanDetails.purpose,
                    Recipient = paySackRecipientResponse.Data.RecipientCode,
                    Reference = transactionReferenceNumber.ToString()
                });

                if (!payment.Status)
                {
                    loanRequest.SetProviderAccountStatus();
                    _logger.LogInformation("Unable to approve loan for user: {User}", userEmail);
                    _logger.LogInformation(payment.Message);
                    return new Result<bool>(ExceptionManager.Manage("Customer Loan Approval", "Something went wrong, please contact support"));
                }

                var disbursementApproval = DisbursementApproval.Factory.Build(Guid.NewGuid(), loanRequest, "", payment.Data.TransferCode, transactionReferenceNumber.ToString());

                await _trivistaDbContext.DisbursementApproval.AddAsync(disbursementApproval, cancellationToken);

                loanRequest.ApproveLoanByCustomer();

                var finalSavedResult = await _trivistaDbContext.SaveChangesAsync(cancellationToken);

                if (finalSavedResult > 1)
                {
                    return true;
                }
                return new Result<bool>(ExceptionManager.Manage("Recover mandate", "Process was not complete"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occured while recova called RecovaDirectDebitCommandHandler");
                return new Result<bool>(ExceptionManager.Manage("Recover mandate", "An error occured"));
            }
        }
    }
}
