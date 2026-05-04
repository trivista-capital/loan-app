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
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Helper;
using Trivista.LoanApp.ApplicationCore.Specifications;
using EnumHelpers = Trivista.LoanApp.ApplicationCore.Commons.Helpers.EnumHelpers;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;

public sealed class ByCustomerIdController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/loanRequests/Customer/{id}", HandleRequestLoan)
            .WithName("RequestLoanByCustomerId")
            .RequireAuthorization()
            .WithTags("Customer");
    }
    
    private async Task<IResult> HandleRequestLoan(IMediator mediator, [FromQuery]Guid id)
    {
        var result = await mediator.Send(new GetLoanRequestByCustomerIdQuery(id));
        return result.ToOk(x => x);
    }
}

public sealed record GetSingleLoanRequestByCustomerIdDto
{
    public Guid Id { get; set; }
    public string Bvn { get; set; }
    public string CustomerName { get; set; }
    public Guid CustomerId { get; set; }
    public string LoanApplicationStatus { get; set; }
    public string DisbursedLoanStatus { get; set; }
    public decimal InterestRate { get; set; }
    
    public DateTime? DateLoanPaid { get; set; }
    public GetSinglekycDetailsByCustomerIdDto kycDetails { get; set; }
    
    public GetSingleLoanDetailsByCustomerIdDto LoanDetails { get; set; }
    
    public GetSingleSalaryDetailsByCustomerIdDto SalaryDetails { get; set; }
    public ApprovalWorkflowDto ApprovalWorkFlow { get; set; }
    public DateTime? DateCreated { get; set; }
    public DateTime? DueDate { get; set; }

    public static explicit operator GetSingleLoanRequestByCustomerIdDto(LoanRequest loanRequest)
    {
        return new GetSingleLoanRequestByCustomerIdDto()
        {
            Id = loanRequest.Id,
            Bvn = loanRequest.Bvn,
            CustomerName = $"{loanRequest.kycDetails.CustomerFirstName} {loanRequest.kycDetails.CustomerLastName}",
            CustomerId = loanRequest.CustomerId, 
            InterestRate = loanRequest.Interest,
            LoanApplicationStatus = EnumHelpers.Convert(loanRequest.LoanApplicationStatus),
            DisbursedLoanStatus = EnumHelpers.Convert(loanRequest.DisbursedLoanStatus),
            DateLoanPaid = loanRequest.DateLoanPaid,
            DateCreated = loanRequest.Created
        };
    }
}

public sealed record GetSinglekycDetailsByCustomerIdDto
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

    public static explicit operator GetSinglekycDetailsByCustomerIdDto(kycDetails kycDetails)
    {
        return new GetSinglekycDetailsByCustomerIdDto()
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

public sealed record GetSingleLoanDetailsByCustomerIdDto
{
    public decimal LoanAmount { get; set; }
    public int tenure { get; set; }
    public string purpose { get; set; }
    public string RepaymentScheduleType { get; set; }
    
    public decimal LoanBalance { get; set; }

    public static explicit operator GetSingleLoanDetailsByCustomerIdDto(LoanDetails loanDetails)
    {
        return new GetSingleLoanDetailsByCustomerIdDto()
        {
            LoanAmount = loanDetails.LoanAmount,
            tenure = loanDetails.tenure,
            purpose = loanDetails.purpose,
            RepaymentScheduleType = EnumHelpers.Convert(loanDetails.RepaymentScheduleType),
            LoanBalance = loanDetails.LoanBalance
        };
    }
}

public sealed record GetSingleSalaryDetailsByCustomerIdDto
{
    public decimal AverageMonthlyNetSalary { get; set; }
    public string SalaryAccountNumber { get; set; }
    public string BankName { get; set; }
    public string AccountName { get; set; }

    public static explicit operator GetSingleSalaryDetailsByCustomerIdDto(SalaryDetails salaryDetails)
    {
        return new GetSingleSalaryDetailsByCustomerIdDto()
        {
            AverageMonthlyNetSalary = salaryDetails.AverageMonthlyNetSalary,
            SalaryAccountNumber = salaryDetails.SalaryAccountNumber,
            BankName = salaryDetails.BankName,
            AccountName = salaryDetails.AccountName,
        };
    }
}

public class GetLoanRequestByCustomerIdQueryValidation : AbstractValidator<GetLoanRequestByCustomerIdQuery>
{
    public GetLoanRequestByCustomerIdQueryValidation()
    {
        //Kyc details validation
        RuleFor(x => x.Id).NotEqual(Guid.Parse("00000000-0000-0000-0000-000000000000"))
            .NotNull().NotEmpty().WithMessage("Customer is invalid");
    }
}
public sealed record GetLoanRequestByCustomerIdQuery(Guid Id): IRequest<Result<List<GetSingleLoanRequestByCustomerIdDto>>>;

public sealed class GetLoanRequestByCustomerIdQueryHandler : IRequestHandler<GetLoanRequestByCustomerIdQuery, Result<List<GetSingleLoanRequestByCustomerIdDto>>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public GetLoanRequestByCustomerIdQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }

    public async Task<Result<List<GetSingleLoanRequestByCustomerIdDto>>> Handle(GetLoanRequestByCustomerIdQuery request, CancellationToken cancellationToken)
    {
        var list = new List<GetSingleLoanRequestByCustomerIdDto>();
        
        var predicate = CustomerLoanRequestSpecification.WhereCustomerId(request.Id);
        
        var loanRequestsFromDb = await _trivistaDbContext.LoanRequest
            .Include(x=>x.Customer)
            .Include(x=>x.ApprovalWorkflow)
            .ThenInclude(x=>x.ApprovalWorkflowApplicationRole)
            .OrderByDescending(x=>x.Created)
            .Where(predicate)
            .ToListAsync(cancellationToken);
        
        foreach (var loanRequest in loanRequestsFromDb)
        {
            var loanRequestDto = (GetSingleLoanRequestByCustomerIdDto)loanRequest;
            loanRequestDto.kycDetails = (GetSinglekycDetailsByCustomerIdDto)loanRequest.kycDetails;
            loanRequestDto.LoanDetails = (GetSingleLoanDetailsByCustomerIdDto)loanRequest.LoanDetails;
            loanRequestDto.SalaryDetails = (GetSingleSalaryDetailsByCustomerIdDto)loanRequest.SalaryDetails;
            loanRequestDto.ApprovalWorkFlow = (ApprovalWorkflowDto)loanRequest.ApprovalWorkflow;
            list.Add(loanRequestDto);
        }
        return list;
    }
}