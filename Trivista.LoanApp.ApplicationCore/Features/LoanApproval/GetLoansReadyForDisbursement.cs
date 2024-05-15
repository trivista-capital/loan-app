using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Pagination;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.Role;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanApproval;

public class GetLoansReadyForDisbursement: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/loans/getLoansForDisbursement", GetLoansForDisbursementHandler)
            .WithName("Get Loans For Disbursement")
            .RequireAuthorization()
            .WithTags("Admin");
    }

    private static async Task<IResult> GetLoansForDisbursementHandler(IMediator mediator, [FromQuery]int pageNumber = 1, [FromQuery]int itemPerPage = 10)
    {
        var result = await mediator.Send(new GetLoansReadyForDisbursementQuery(pageNumber, itemPerPage));
        return result.ToOk(x => x);
    }
}

public sealed record GetLoansReadyForDisbursementDto(Guid Id, 
                                                     string CustomerName, 
                                                     decimal LoanAmount, 
                                                     int Tenure, 
                                                     string RepaymentScheduleType, 
                                                     string LoanApplicationStatus);

public sealed record GetLoansReadyForDisbursementQuery
    (int pageNumber, int itemPerPage) : IRequest<Result<PaginationInfo<GetLoansReadyForDisbursementDto>>>;

public sealed class GetLoansReadyForDisbursementQueryHandler : IRequestHandler<GetLoansReadyForDisbursementQuery,
    Result<PaginationInfo<GetLoansReadyForDisbursementDto>>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public GetLoansReadyForDisbursementQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<PaginationInfo<GetLoansReadyForDisbursementDto>>> Handle(GetLoansReadyForDisbursementQuery request, CancellationToken cancellationToken)
    {
        List<GetLoansReadyForDisbursementDto> loansList = new List<GetLoansReadyForDisbursementDto>();
        
        var loansToBeDisbursed = _trivistaDbContext
                                                             .DisbursementApproval.Where(x => !x.IsSuccessful)
                                                             .Include(x=>x.LoanRequest)
                                                             .ThenInclude(x=>x.RepaymentSchedules)
                                                             .Include(x=>x.LoanRequest)
                                                             .ThenInclude(x=>x.Customer)
                                                             .AsNoTrackingWithIdentityResolution();
        
        var pagedResult = await PaginationData.PaginateAsync(loansToBeDisbursed, request.pageNumber, request.itemPerPage);
        
        foreach (var loan in loansToBeDisbursed)
        {
            
            GetLoansReadyForDisbursementDto loanToDisbursed = new (loan.Id, 
                                                    $"{loan.LoanRequest.Customer.FirstName} {loan.LoanRequest.Customer.LastName}",
                                                    loan.LoanRequest.LoanDetails.LoanAmount,
                                                    loan.LoanRequest.LoanDetails.tenure,
                                                    loan.LoanRequest.RepaymentSchedules.FirstOrDefault().RepaymentType.ToString(),
                                                    loan.LoanRequest.LoanApplicationStatus.ToString());
            loansList.Add(loanToDisbursed);
        }

        return new PaginationInfo<GetLoansReadyForDisbursementDto>(loansList, 
                                                                     pagedResult.CurrentPage, 
                                                                     pagedResult.PageSize, 
                                                                     pagedResult.TotalItems, 
                                                                     pagedResult.TotalPages);
    }
}
    
    