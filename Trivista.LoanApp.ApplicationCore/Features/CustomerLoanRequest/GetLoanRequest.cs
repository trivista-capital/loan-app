using Carter;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Specifications;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;

public sealed class GetLoanRequestController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/loanRequests/{id}", HandleRequestLoan)
            .WithName("RequestLoanById")
            .WithTags("Loan Request");
    }
    
    private async Task<IResult> HandleRequestLoan(IMediator mediator, [FromQuery]Guid id)
    {
        var result = await mediator.Send(new GetLoanRequestQuery(id));
        return result.ToOk(x => x);
    }
}

public sealed record GetSingleLoanRequestDto
{
    public Guid Id { get; set; }
    public string Bvn { get; set; }
    public string CustomerName { get; set; }
    public Guid CustomerId { get; set; }
    public string LoanApplicationStatus { get; set; }
    public string DisbursedLoanStatus { get; set; }
    public decimal InterestRate { get; set; }
    public GetkycDetailsDto kycDetails { get; set; }
    public GetLoanDetailsDto LoanDetails { get; set; }
    public GetSalaryDetailsDto SalaryDetails { get; set; }
    public ApprovalWorkflowDto ApprovalWorkFlow { get; set; }
    public bool IsRepaid { get; set; }
    public DateTime? MaturityDate { get; set; }
    
    public DateTime? DateLoanPaid { get; set; }
}

public sealed record GetSinglekycDetailsDto
{
    public string CustomerFirstName { get; set; }
    public string CustomerMiddleName { get; set; }
    public string CustomerLastName { get; set; }
    public string CustomerEmail { get; set; }
    public string CustomerAddress { get; set; }
    public string CustomerCity { get; set; }
    public string CustomerState { get; set; }
    public string CustomerCountry { get; set; }
    
    public string CustomerPostalCode { get; set; }
    
    public string CustomerOccupation { get; set; }
    
    public string CustomerPhoneNumber { get; set; }

    public static explicit operator GetSinglekycDetailsDto(kycDetails kycDetails)
    {
        return new GetSinglekycDetailsDto()
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

public sealed record GetSingleLoanDetailsDto
{
    public decimal LoanAmount { get; set; }
    public int tenure { get; set; }
    public string purpose { get; set; }
    public RepaymentScheduleType RepaymentScheduleType { get; set; }

    public static explicit operator GetSingleLoanDetailsDto(LoanDetails loanDetails)
    {
        return new GetSingleLoanDetailsDto()
        {
            LoanAmount = loanDetails.LoanAmount,
            tenure = loanDetails.tenure,
            purpose = loanDetails.purpose,
            RepaymentScheduleType = loanDetails.RepaymentScheduleType
        };
    }
}

public sealed record GetSingleSalaryDetailsDto
{
    public decimal AverageMonthlyNetSalary { get; set; }
    public string SalaryAccountNumber { get; set; }
    public string BankName { get; set; }
    public string AccountName { get; set; }

    public static explicit operator GetSingleSalaryDetailsDto(SalaryDetails salaryDetails)
    {
        return new GetSingleSalaryDetailsDto()
        {
            AverageMonthlyNetSalary = salaryDetails.AverageMonthlyNetSalary,
            SalaryAccountNumber = salaryDetails.SalaryAccountNumber,
            BankName = salaryDetails.BankName,
            AccountName = salaryDetails.AccountName,
        };
    }
}

public class GetLoanRequestQueryValidation : AbstractValidator<GetLoanRequestQuery>
{
    public GetLoanRequestQueryValidation()
    {
        //Kyc details validation
        RuleFor(x => x.Id).NotEqual(Guid.Parse("00000000-0000-0000-0000-000000000000"))
            .NotNull().NotEmpty().WithMessage("Loan request Id is invalid");
    }
}
public sealed record GetLoanRequestQuery(Guid Id): IRequest<Result<GetSingleLoanRequestDto>>;

public sealed class GetLoanRequestQueryHandler : IRequestHandler<GetLoanRequestQuery, Result<GetSingleLoanRequestDto>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public GetLoanRequestQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }

    public async Task<Result<GetSingleLoanRequestDto>> Handle(GetLoanRequestQuery request, CancellationToken cancellationToken)
    {
        var predicate = CustomerLoanRequestSpecification.WhereId(request.Id);
        var loanRequestFromDb = await _trivistaDbContext.LoanRequest
            .Include(x=>x.ApprovalWorkflow)
            .ThenInclude(x=>x.ApprovalWorkflowApplicationRole)
            .FirstOrDefaultAsync(predicate, cancellationToken);
        
        if (loanRequestFromDb == null)
            return new Result<GetSingleLoanRequestDto>(ExceptionManager.Manage("Loan Request", "Loan request does not exist"));
        
        var lastRepaymentSchedule = await _trivistaDbContext.RepaymentSchedule.Where(x=>x.LoanRequestId == request.Id && x.Status == ScheduleStatus.Paid)
                                                            .OrderBy(x=>x.DueDate)
                                                            .FirstOrDefaultAsync(cancellationToken);
        
        var loanRequest = new GetSingleLoanRequestDto()
        {
            Id = loanRequestFromDb.Id,
            Bvn = loanRequestFromDb.Bvn,
            CustomerId = loanRequestFromDb.CustomerId,
            InterestRate = loanRequestFromDb.Interest,
            CustomerName = $"{loanRequestFromDb.kycDetails.CustomerFirstName} {loanRequestFromDb.kycDetails.CustomerLastName}",
            SalaryDetails = (GetSalaryDetailsDto)loanRequestFromDb.SalaryDetails,
            kycDetails = (GetkycDetailsDto)loanRequestFromDb.kycDetails,
            LoanDetails = (GetLoanDetailsDto)loanRequestFromDb.LoanDetails,
            LoanApplicationStatus = loanRequestFromDb.LoanApplicationStatus.ToString(),
            DisbursedLoanStatus = loanRequestFromDb.DisbursedLoanStatus.ToString(),
            ApprovalWorkFlow = (ApprovalWorkflowDto)loanRequestFromDb.ApprovalWorkflow,
            IsRepaid = lastRepaymentSchedule != null,
            MaturityDate = lastRepaymentSchedule?.DueDate,
            DateLoanPaid = loanRequestFromDb.DateLoanPaid
        };
        return loanRequest;
    }
}