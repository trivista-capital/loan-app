using Carter;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Commons.Pagination;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.Customer;

namespace Trivista.LoanApp.ApplicationCore.Features.Reports;

public class LoanReportController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/report/loan", LoanReportHandler)
            .WithName("Loan Report")
            .WithTags("Report")
        .RequireAuthorization();
    }

    private async Task<IResult> LoanReportHandler(IMediator mediator, [FromQuery]DisbursedLoanStatus status, [FromQuery]string? startDate, [FromQuery]string? endDate)
    {
        var result = await mediator.Send(new LoanReportQuery(status, startDate, endDate));
        return result.ToOk(x => x);
    }
}

public sealed class LoanReportViewModel
{
    public int TotalNumberOfLoans { get; set; }
    
    public decimal TotalLoanBalance { get; set; }

    public ICollection<TotalNumberOfLoansOverTimeViewModel> TotalNumberOfLoansOverTimeViewModel { get; set; }
        = new List<TotalNumberOfLoansOverTimeViewModel>();
}

public sealed record TotalNumberOfLoansOverTimeViewModel
{
    public int Count { get; set; }
    public DateTime DateCreated { get; set; }
    public string LoanApplicationStatus { get; set; }
    public string DisbursedLoanStatus { get; set; }
    public DateTime? DateLoanDisbursed { get; set; }
    public Guid CustomerId { get; set; }

    public LoanDetailsViewModel LoanDetailsViewModel { get; set; } = new();
}

public sealed record LoanDetailsViewModel
{
    public decimal LoanAmount { get; set; }
    public decimal LoanBalance { get; set; }
    public int tenure { get; set; }
    public string purpose { get; set; }
    
    public string RepaymentScheduleType { get; set; }
}

public sealed record LoanReportQuery(DisbursedLoanStatus status, string startDate, string endDate) : IRequest<Result<LoanReportViewModel>>;

public sealed class LoanReportQueryHandler : IRequestHandler<LoanReportQuery, Result<LoanReportViewModel>>
{
    private readonly TrivistaDbContext _trivistaDbContext;

    public LoanReportQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<LoanReportViewModel>> Handle(LoanReportQuery request, CancellationToken cancellationToken)
    {
        var loanRequestWithStatus =  _trivistaDbContext
                                                                    .LoanRequest
                                                                    .Include(x => x.LoanDetails)
                                                                    .AsNoTrackingWithIdentityResolution()
                                                                    .Where(x => x.DisbursedLoanStatus == request.status);
        
        if (!string.IsNullOrEmpty(request.startDate) && !string.IsNullOrEmpty(request.endDate))
        {
            var startDate = Convert.ToDateTime(request.startDate);
            var endDate = Convert.ToDateTime(request.endDate); //1/07/2023 - 5/07/2023
            
            loanRequestWithStatus = loanRequestWithStatus.Where(x => x.Created.Date >= startDate.Date && x.Created.Date < endDate.Date.AddDays(1)).OrderBy(x=>x.Created);
        }

        var loans = await loanRequestWithStatus.ToListAsync(cancellationToken);
        
        var report = new LoanReportViewModel
        {
            TotalNumberOfLoans = loans.Count(),
            TotalLoanBalance = loans.Sum(x => x.LoanDetails.LoanAmount),
            TotalNumberOfLoansOverTimeViewModel = loans.OrderBy(x => x.Created).Select(x => new TotalNumberOfLoansOverTimeViewModel()
            {
                Count = loans.Count(x=>x.Created.Date == x.Created.Date),
                DateCreated = x.Created,
                LoanApplicationStatus = x.LoanApplicationStatus.ToString(),
                DisbursedLoanStatus = x.DisbursedLoanStatus.ToString(),
                DateLoanDisbursed = x.DateLoanDisbursed,
                CustomerId = x.CustomerId,
                LoanDetailsViewModel = new LoanDetailsViewModel()
                {
                    LoanAmount = x.LoanDetails.LoanAmount,
                    LoanBalance = x.LoanDetails.LoanBalance,
                    tenure = x.LoanDetails.tenure,
                    purpose = x.LoanDetails.purpose,
                    RepaymentScheduleType = x.LoanDetails.RepaymentScheduleType.ToString()
                }
            }).Distinct().ToList()
        };

        return report;
    }
}

