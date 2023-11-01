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
using Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest.EventHandlers;
using Trivista.LoanApp.ApplicationCore.Features.LoanApproval;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;

public sealed class LoanRequestController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        try
        {
            app.MapPost("/requestLoan", HandleRequestLoan)
           .WithName("RequestLoan")
           .WithTags("Loan Request")
           .RequireAuthorization();
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    
    private async Task<IResult> HandleRequestLoan(IMediator mediator, [FromBody]RequestLoanCommand model)
    {
        try
        {
            var result = await mediator.Send(model);
            return result.ToOk(x => x);
        }
        catch (Exception ex)
        {

            throw;
        }
    }
}

public sealed record kycDetailsDto
{
    public string CustomerFirstName { get; set; }
    public string CustomerMiddleName { get; set; }
    public string CustomerLastName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerDob { get; set; }
    public string CustomerAddress { get; set; }
    public string CustomerCity { get; set; }
    public string CustomerSex { get; set; }
    public string CustomerState { get; set; }
    public string CustomerCountry { get; set; }
    public string CustomerPostalCode { get; set; }
    public string CustomerOccupation { get; set; }
    public string CustomerPhoneNumber { get; set; }

    public static explicit operator kycDetails(kycDetailsDto kycDetails)
    {
        return new kycDetails()
        {
            CustomerFirstName = kycDetails.CustomerFirstName, //Required
            CustomerMiddleName = kycDetails.CustomerMiddleName,
            CustomerLastName = kycDetails.CustomerLastName, //Required
            CustomerEmail = kycDetails.CustomerEmail, //Required
            CustomerAddress = kycDetails.CustomerAddress, //Required
            CustomerCity = kycDetails.CustomerCity, //Required
            CustomerState = kycDetails.CustomerState, //Required
            CustomerCountry = kycDetails.CustomerCountry, //Required
            CustomerPostalCode = kycDetails.CustomerPostalCode,
            CustomerOccupation = kycDetails.CustomerOccupation, //Required
            CustomerPhoneNumber = kycDetails.CustomerPhoneNumber //Required
        };
    }
}

public sealed record ProofOfAddressDto
{
    public string ProofOFAddressFileName { get; set; }
    
    public string ProofOFAddressFileType { get; set; }
    
    public long ProofOFAddressFileLength { get; set; }
    public string ProofOFAddressFile { get; set; }

    public static explicit operator ProofOfAddress(ProofOfAddressDto proofOfAddress)
    {
        return new ProofOfAddress()
        {
            ProofOFAddressFileName = proofOfAddress.ProofOFAddressFileName, //Required
            ProofOFAddressFileType = proofOfAddress.ProofOFAddressFileType,
            ProofOFAddressFileLength = proofOfAddress.ProofOFAddressFileLength, //Required
            ProofOFAddressFile = proofOfAddress.ProofOFAddressFile, //Required
        };
    }
}

public sealed record LoanDetailsDto
{
    public decimal LoanAmount { get; set; }
    public int tenure { get; set; }
    public string purpose { get; set; }
    public RepaymentScheduleType RepaymentScheduleType { get; set; }

    public static explicit operator LoanDetails(LoanDetailsDto loanDetails)
    {
        return new LoanDetails()
        {
            LoanAmount = loanDetails.LoanAmount,
            tenure = loanDetails.tenure,
            purpose = loanDetails.purpose,
            RepaymentScheduleType = loanDetails.RepaymentScheduleType
        };
    }
}

public sealed record SalaryDetailsDto
{
    public decimal AverageMonthlyNetSalary { get; set; }
    public string SalaryAccountNumber { get; set; }
    public string BankName { get; set; }
    public string AccountName { get; set; }

    public static explicit operator SalaryDetails(SalaryDetailsDto salaryDetails)
    {
        return new SalaryDetails()
        {
            AverageMonthlyNetSalary = salaryDetails.AverageMonthlyNetSalary,
            SalaryAccountNumber = salaryDetails.SalaryAccountNumber,
            BankName = salaryDetails.BankName,
            AccountName = salaryDetails.AccountName
        };
    }
}

//public sealed record LoaRequestResponse(decimal MonthlyRepaymentAmount, decimal TotalRepaymentAmount, int Tenure, decimal PossibleLoanAMount);
public class RequestLoanCommandValidation : AbstractValidator<RequestLoanCommand>
{
    public RequestLoanCommandValidation()
    {
        //Kyc details validation
        RuleFor(x => x.Bvn).NotEqual("string").NotNull().NotEmpty().WithMessage("Bvn must be set");
        RuleFor(x => x.kycDetails.CustomerFirstName).NotEqual("string").NotNull().NotEmpty().WithMessage("First name must be set");
        RuleFor(x => x.kycDetails.CustomerLastName).NotEqual("string").NotNull().NotEmpty().WithMessage("Last name must be set");
        RuleFor(x => x.kycDetails.CustomerDob).NotEqual("string").NotNull().NotEmpty().WithMessage("Dob must be set");
        RuleFor(x => x.kycDetails.CustomerEmail).NotEqual("string").NotNull().NotEmpty().WithMessage("Email must be set");
        RuleFor(x => x.kycDetails.CustomerAddress).NotEqual("string").NotNull().NotEmpty().WithMessage("Address must be set");
        RuleFor(x => x.kycDetails.CustomerCity).NotEqual("string").NotNull().NotEmpty().WithMessage("City must be set");
        RuleFor(x => x.kycDetails.CustomerSex).NotEqual("string").NotNull().NotEmpty().WithMessage("Sex must be set");
        RuleFor(x => x.kycDetails.CustomerState).NotEqual("string").NotNull().NotEmpty().WithMessage("State must be set");
        RuleFor(x => x.kycDetails.CustomerCountry).NotEqual("string").NotNull().NotEmpty().WithMessage("Country must be set");
        RuleFor(x => x.kycDetails.CustomerOccupation).NotEqual("string").NotNull().NotEmpty().WithMessage("Occupation must be set");
        RuleFor(x => x.kycDetails.CustomerPhoneNumber).NotEqual("string").NotNull().NotEmpty().WithMessage("Phone number must be set");
        //Loan details validation
        RuleFor(x => x.LoanDetails.LoanAmount).GreaterThan(0).WithMessage("Loan amount must be set");
        RuleFor(x => x.LoanDetails.tenure).GreaterThan(0).WithMessage("Tenure must be set");
        RuleFor(x => x.LoanDetails.purpose).NotEqual("string").NotNull().NotEmpty().WithMessage("Purpose must be set");
        //RuleFor(x => x.LoanDetails.RepaymentScheduleType)..WithMessage("Repayment schedule must be set");
        //Salary details validation
        RuleFor(x => x.SalaryDetails.AverageMonthlyNetSalary).GreaterThan(0).WithMessage("Average monthly salary must be set");
        RuleFor(x => x.SalaryDetails.SalaryAccountNumber).NotEqual("string").NotNull().NotEmpty().WithMessage("Salary account number must be set");
        RuleFor(x => x.SalaryDetails.BankName).NotEqual("string").NotNull().NotEmpty().WithMessage("Bank name must be set");
        RuleFor(x => x.SalaryDetails.AccountName).NotEqual("string").NotNull().NotEmpty().WithMessage("Account name must be set");
    }
}

public sealed record RequestLoanCommand(Guid CustomerId, string Bvn, kycDetailsDto kycDetails, LoanDetailsDto LoanDetails, SalaryDetailsDto SalaryDetails, ProofOfAddressDto ProofOfAddressDto, bool IsRemita) : IRequest<Result<bool>>;

public sealed class RequestLoanCommandHandler : IRequestHandler<RequestLoanCommand, Result<bool>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    private readonly ILogger<RequestLoanCommandHandler> _logger;
    private readonly IPublisher _publisher;
    public RequestLoanCommandHandler(TrivistaDbContext trivistaDbContext, ILogger<RequestLoanCommandHandler> logger, IPublisher publisher)
    {
        _trivistaDbContext = trivistaDbContext;
        _logger = logger;
        _publisher = publisher;
    }

    public async Task<Result<bool>> Handle(RequestLoanCommand request, CancellationToken cancellationToken)
    {
            var validator = new RequestLoanCommandValidation();
            var exceptionResult = await TrivistaValidationException<RequestLoanCommandValidation, RequestLoanCommand>
                .ManageException<bool>(validator, request, cancellationToken, true);
            if (!exceptionResult.IsSuccess)
                return exceptionResult;
            
            var doesCustomerHaveAnyLoan = await _trivistaDbContext.LoanRequest
                                                 .AsNoTracking()
                                                 .Where(x => x.CustomerId == request.CustomerId)
                                                 .Select(x => x)
                
                                                 .FirstOrDefaultAsync(cancellationToken);

            if (doesCustomerHaveAnyLoan != null)
            {
                switch (doesCustomerHaveAnyLoan!.LoanApplicationStatus)
                {
                    case LoanApplicationStatus.Active:
                    case LoanApplicationStatus.Approved:
                    case LoanApplicationStatus.Pending:
                        return new Result<bool>(ExceptionManager.Manage("Loan Request", "Customer can not request for loan, while another is pending"));
                }
        }
            
            
            var customer = await _trivistaDbContext.Customer.FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);
            
            if (customer == null)
                return new Result<bool>(ExceptionManager.Manage("Customer", "Customer does not exist"));
            
            customer.SetBvn(request.Bvn).SetDob(request.kycDetails.CustomerDob).SetSex(request.kycDetails.CustomerSex)
                                        .SetOccupation(request.kycDetails.CustomerOccupation).SetPhoneNumber(request.kycDetails.CustomerPhoneNumber)
                                        .SetAddress(request.kycDetails.CustomerAddress).SetCountry(request.kycDetails.CustomerCountry).SetState(request.kycDetails.CustomerState)
                                        .SetCity(request.kycDetails.CustomerCity).SetPostCode(request.kycDetails.CustomerPostalCode).IsRemittaUser(request.IsRemita);

            var defaultLoan = await _trivistaDbContext.Loan.AsNoTracking().Where(x => x.IsDefault).Select(x=>x).FirstOrDefaultAsync(cancellationToken);
            
            if(defaultLoan == null)
            {
                _logger.LogInformation("Unable to process lona request, loan needs to be configured");
                return new Result<bool>(ExceptionManager.Manage("Loan Request", "Unable to process loan request, please try again"));
            }

            var kycDetails = (kycDetails)request.kycDetails;
            var loanDetails = (LoanDetails)request.LoanDetails;
            var salaryDetails = (SalaryDetails)request.SalaryDetails;
            var proofOfAddress = (ProofOfAddress)request.ProofOfAddressDto;
            var loanRequestId = Guid.NewGuid();

            var approvalWorkFlowConfiguration = await _trivistaDbContext.ApprovalWorkflowConfiguration
                                                .FirstOrDefaultAsync(x => x.Action == ApprovalTypes.LoanRequest.ToString(), cancellationToken);
            
            if (approvalWorkFlowConfiguration == null)
                return new Result<bool>(ExceptionManager.Manage("Loan Request", "Workflow has not been configured"));
        
            var approvalWorkFlow = ApprovalWorkflow.Factory.Build(approvalWorkFlowConfiguration);
            
            var approvalConfigurationRoles = await _trivistaDbContext.ApprovalWorkflowApplicationRoleConfiguration
                                                                                    .Where(x => x.ApprovalWorkflowConfiguration.Id == approvalWorkFlowConfiguration.Id)
                                                                                    .Select(x => x)
                                                                                    .ToListAsync(cancellationToken);
            foreach (var configRole in approvalConfigurationRoles)
            {
                approvalWorkFlow.SetApprovalWorkflowApplicationRole(ApprovalWorkflowApplicationRole.Factory.Build(configRole.RoleId, "", approvalWorkFlow.Id, configRole.Hierarchy));   
            }
            
            var loanRequest = LoanRequest.Factory.Build(loanRequestId, request.Bvn, customer, approvalWorkFlow, salaryDetails, loanDetails, kycDetails, proofOfAddress, defaultLoan.InterestRate)
                .SetInterestRate(defaultLoan.InterestRate);
            
            if (loanRequest == null)
                return new Result<bool>(ExceptionManager.Manage("Loan Request", "Something happened while building your loan request"));
            
            await _trivistaDbContext.LoanRequest.AddAsync(loanRequest, cancellationToken);
            var savedLoanRequestResponse = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
            if (savedLoanRequestResponse > 0)
            {
                //To customer
                await _publisher.Publish(new LoanRequestedEvent()
                {
                    To = loanRequest!.kycDetails!.CustomerEmail,
                    Name = $"{loanRequest.kycDetails?.CustomerFirstName} {loanRequest.kycDetails?.CustomerLastName}",
                    LoanAmount = loanRequest.LoanDetails.LoanAmount,
                    Purpose = loanRequest!.LoanDetails!.purpose
                });

                //To Admin
                foreach (var role in approvalConfigurationRoles)
                {
                    var staff = await _trivistaDbContext.Customer.Where(x => x.RoleId == role.RoleId.ToString()).Select(x=>x).FirstOrDefaultAsync(cancellationToken);
                    if (staff != null)
                    {
                        await _publisher.Publish(new NewLoanRequestNotificationEvent()
                        {
                            To = staff.Email, //get admin email,
                            AdminName = $"{staff.FirstName} {staff.LastName}", //get admin name,
                            CustomerName = $"{request.kycDetails.CustomerFirstName} {request.kycDetails.CustomerLastName}",
                            LoanAmount = request.LoanDetails.LoanAmount,
                            LoanPurpose = request.LoanDetails.purpose
                        });    
                    }
                }
                return true;
            }
            
            return false;
    }
}