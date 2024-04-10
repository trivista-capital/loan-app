using System.Reflection;
using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Commons.Pagination;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;

public sealed class GetLoanRequestsController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/getLoanRequests", HandleGetLoanRequests)
            .WithName("GetLoanRequests")
            .WithTags("Loan Request")
            .RequireAuthorization();
    }
    
    private async Task<IResult> HandleGetLoanRequests(IMediator mediator, [FromQuery]Guid? roleId,
                                                                          [FromQuery]LoanApplicationStatus? status,
                                                                          [FromQuery]string? email,
                                                                          [FromQuery]int hierarchy,
                                                                          [FromQuery]int pageNumber = 1,
                                                                          [FromQuery]int itemsPerPage = 10)
    {
        
        var result = await mediator.Send(new GetLoanRequestsQuery(roleId, status, email, hierarchy, pageNumber, itemsPerPage));
        return result.ToOk(b => (b));
    }
}

public sealed class LoanRequestsWrapper
{
    public int TotalPendingLoan { get; set; }
    
    public int TotalRejected { get; set; }
    
    public int TotalApproved { get; set; }
    
    public PaginationInfo<GetLoanRequests> GetLoanRequests { get; set; }
}
public sealed record GetLoanRequests
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
    public DateTime? DateLoanPaid { get; set; }
    public DateTime? DateCreated { get; set; }
    public DateTime? DueDate { get; set; }

    public string Statement { get; set; }

    public static explicit operator GetLoanRequests(LoanRequest loanRequest)
    {
        return new GetLoanRequests()
        {
            Id = loanRequest.Id,
            Bvn = loanRequest.Bvn,
            DateLoanPaid = loanRequest.DateLoanPaid,
            CustomerId = loanRequest.CustomerId,
            InterestRate = loanRequest.Interest,
            CustomerName = $"{loanRequest.kycDetails.CustomerFirstName} {loanRequest.kycDetails.CustomerLastName}",
            LoanApplicationStatus = EnumHelpers.Convert(loanRequest.LoanApplicationStatus),
            DisbursedLoanStatus = EnumHelpers.Convert(loanRequest.DisbursedLoanStatus),
            DateCreated = loanRequest.Created
        };
    }
}

public sealed record GetkycDetailsDto
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

    public static explicit operator GetkycDetailsDto(kycDetails kycDetails)
    {
        return new GetkycDetailsDto()
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

public sealed record GetLoanDetailsDto
{
    public decimal LoanAmount { get; set; }
    public decimal LoanBalance { get; set; }
    public int tenure { get; set; }
    public string purpose { get; set; }
    public RepaymentScheduleType RepaymentScheduleType { get; set; }

    public static explicit operator GetLoanDetailsDto(LoanDetails loanDetails)
    {
        return new GetLoanDetailsDto()
        {
            LoanAmount = loanDetails.LoanAmount,
            tenure = loanDetails.tenure,
            purpose = loanDetails.purpose,
            RepaymentScheduleType = loanDetails.RepaymentScheduleType,
            LoanBalance = loanDetails.LoanBalance
        };
    }
}

public sealed record GetSalaryDetailsDto
{
    public decimal AverageMonthlyNetSalary { get; set; }
    public string SalaryAccountNumber { get; set; }
    public string BankName { get; set; }
    public string AccountName { get; set; }

    public static explicit operator GetSalaryDetailsDto(SalaryDetails salaryDetails)
    {
        return new GetSalaryDetailsDto()
        {
            AverageMonthlyNetSalary = salaryDetails.AverageMonthlyNetSalary,
            SalaryAccountNumber = salaryDetails.SalaryAccountNumber,
            BankName = salaryDetails.BankName,
            AccountName = salaryDetails.AccountName,
        };
    }
}

public sealed record ApprovalWorkflowDto
{
    public Guid Id { get; set; }
    public bool IsApproved { get; set; }
    
    public DateTime DateApproved { get; set; }
    
    public DateTime DateRejected { get; set; }

    public List<ApprovalWorkflowApplicationRoleDto> ApprovalWorkflowApplicationRoleDto { get; set; }
        = new List<ApprovalWorkflowApplicationRoleDto>();

    public static explicit operator ApprovalWorkflowDto(ApprovalWorkflow config)
    {
        return new ApprovalWorkflowDto()
        {
            Id = config.Id,
            IsApproved = config.IsApproved,
            DateApproved = config.DateApproved,
            DateRejected = config.DateRejected,
            
            ApprovalWorkflowApplicationRoleDto = config.ApprovalWorkflowApplicationRole.Select(x =>
                new ApprovalWorkflowApplicationRoleDto()
                {
                    Id = x.Id,
                    RoleId = x.RoleId,
                    IsApproved = x.IsApproved,
                    ApprovedBy = x.ApprovedBy,
                    RejectedBy = x.RejectedBy,
                    DateApproved = x.DateApproved,
                    DateRejected = x.DateRejected,
                    Hierarchy = x.Hierarchy
                }).ToList()
        };
    }
}

public sealed class ApprovalWorkflowApplicationRoleDto
{
    public Guid Id { get; set; }
    public Guid RoleId { get; set; }
    public bool IsApproved { get; set; }
    public string ApprovedBy { get; set; }
    public string RejectedBy { get; set; }
    public DateTime DateApproved { get; set; }
    public DateTime DateRejected { get; set; }
    public int Hierarchy { get;  set; }
}

public class GetLoanRequestsDto
{
    public Guid RoleId { get; set; }
    public LoanApplicationStatus Status { get; set; }
    public string Email { get; set; }
    public int Hierarchy { get; set; }
}

public sealed record GetLoanRequestsQuery(Guid? RoleId, LoanApplicationStatus? Status, string Email, int Hierarchy, int PageNumber, int ItemsPerPage): IRequest<Result<LoanRequestsWrapper>>;

public sealed class GetLoanRequestsQueryHandler : IRequestHandler<GetLoanRequestsQuery, Result<LoanRequestsWrapper>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    private readonly TokenManager _token;
    public GetLoanRequestsQueryHandler(TrivistaDbContext trivistaDbContext, TokenManager token)
    {
        _trivistaDbContext = trivistaDbContext;
        _token = token;
    }

    public async Task<Result<LoanRequestsWrapper>> Handle(GetLoanRequestsQuery request, CancellationToken cancellationToken)
    {
        LoanRequestsWrapper wrapper = new();
        var roleId = _token.GetRoleId();
        var email = _token.GetEmail();

        if (string.IsNullOrEmpty(roleId))
            return new Result<LoanRequestsWrapper>(ExceptionManager.Manage("Loan Request", "Role can not be specified"));
        
        var listOfLoanRequests = new List<GetLoanRequests>();
        
        IQueryable<LoanRequest> loanRequestList = Enumerable.Empty<LoanRequest>().AsQueryable();
        
        loanRequestList = _trivistaDbContext.LoanRequest.Include(x => x.Customer)
            .Include(x => x.ApprovalWorkflow)
            .ThenInclude(x => x.ApprovalWorkflowApplicationRole)
            //.Include(x => x.RepaymentSchedules.OrderBy(x => x.DueDate))
            .OrderByDescending(x => x.Created)
            .AsQueryable();
        
        if (roleId != default)
        {
            loanRequestList = loanRequestList.Where(x=>x.ApprovalWorkflow.ApprovalWorkflowApplicationRole.Any(x=>x.RoleId == Guid.Parse(roleId)));
        }
        if (request.Status != null)
        {
            loanRequestList = loanRequestList.Where(x=>x.LoanApplicationStatus == request.Status);
        }
        if (!string.IsNullOrEmpty(request.Email))
        {
            loanRequestList = loanRequestList.Where(x=>x.kycDetails.CustomerEmail == request.Email);
        }
        loanRequestList = loanRequestList.Where(x=>x.ApprovalWorkflow.ApprovalWorkflowApplicationRole.Any(x=>x.Hierarchy == request.Hierarchy));
        
        var pagedResult = await PaginationData.PaginateAsync(loanRequestList, request.PageNumber, request.ItemsPerPage);

        foreach (var loanRequest in loanRequestList)
        {
            GetLoanRequests loanRequestDto = (GetLoanRequests)loanRequest;
            loanRequestDto.Statement = loanRequest.Customer.MbsBankStatement;
            loanRequestDto.kycDetails = (GetkycDetailsDto)loanRequest.kycDetails;
            loanRequestDto.LoanDetails = (GetLoanDetailsDto)loanRequest.LoanDetails;
            loanRequestDto.SalaryDetails = (GetSalaryDetailsDto)loanRequest.SalaryDetails;
            loanRequestDto.ApprovalWorkFlow = (ApprovalWorkflowDto)loanRequest.ApprovalWorkflow;
            listOfLoanRequests.Add(loanRequestDto);
        }

        wrapper.TotalApproved = listOfLoanRequests.Count(x => x.LoanApplicationStatus == "Approved");
        wrapper.TotalPendingLoan = listOfLoanRequests.Count(x => x.LoanApplicationStatus == "Pending");
        wrapper.TotalRejected = listOfLoanRequests.Count(x => x.LoanApplicationStatus == "Rejected");
        wrapper.GetLoanRequests = new PaginationInfo<GetLoanRequests>(listOfLoanRequests, 
                                                                        pagedResult.CurrentPage, 
                                                                        pagedResult.PageSize, 
                                                                        pagedResult.TotalItems, 
                                                                        pagedResult.TotalPages);

        return wrapper;
    }
}