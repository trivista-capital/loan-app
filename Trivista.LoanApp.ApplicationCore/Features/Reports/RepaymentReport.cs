using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.Reports;

public sealed class RepaymentReport: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/report/repayment", RepaymentReportHandler)
            .WithName("Repayment Report")
            .WithTags("Report");
    }

    private async Task<IResult> RepaymentReportHandler(IMediator mediator, [FromQuery]ScheduleStatus status, [FromQuery]string? startDate, [FromQuery]string? endDate)
    {
        var result = await mediator.Send(new RepaymentReportQuery(status, startDate, endDate));
        return result.ToOk(x => x);
    }
}

public sealed class RepaymentReportViewModel
{
    public int TotalNumberOfLoans { get; set; }
    
    public decimal TotalLoanBalance { get; set; }

    public ICollection<TotalNumberOfRepaymentLoansOverTimeViewModel> TotalNumberOfLoansOverTimeViewModel { get; set; }
        = new List<TotalNumberOfRepaymentLoansOverTimeViewModel>();
}

public sealed record TotalNumberOfRepaymentLoansOverTimeViewModel
{
    public int Count { get; set; }
    
    public DateTime Date { get; set; }
}

public sealed record RepaymentReportQuery(ScheduleStatus status, string startDate, string endDate) : IRequest<Result<RepaymentReportViewModel>>;

public sealed class RepaymentReportQueryHandler : IRequestHandler<RepaymentReportQuery, Result<RepaymentReportViewModel>>
{
    private readonly TrivistaDbContext _trivistaDbContext;

    public RepaymentReportQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<RepaymentReportViewModel>> Handle(RepaymentReportQuery request, CancellationToken cancellationToken)
    {
        var loanRequestWithStatus =  _trivistaDbContext.LoanRequest.AsNoTrackingWithIdentityResolution()
                                     .Include(x=>x.RepaymentSchedules)
                                     .Where(x => x.RepaymentSchedules.Any(x=>x.Status == request.status));
        
        if (!string.IsNullOrEmpty(request.startDate) && !string.IsNullOrEmpty(request.endDate))
        {
            var startDate = Convert.ToDateTime(request.startDate);
            var endDate = Convert.ToDateTime(request.endDate); //1/07/2023 - 5/07/2023
            
            loanRequestWithStatus = loanRequestWithStatus.Where(x => x.Created.Date >= startDate.Date && x.Created.Date <= endDate.Date).OrderBy(x=>x.Created);
        }

        var loans = await loanRequestWithStatus.ToListAsync(cancellationToken);

        var groupedByDate =  loans.Select(x=>x.Created);
        var repaymenSchedules = loanRequestWithStatus.SelectMany(x => x.RepaymentSchedules.ToList());
        var report = new RepaymentReportViewModel
        {
            TotalNumberOfLoans = loans.Count(),
            TotalLoanBalance = repaymenSchedules.Where(x=>x.Status == ScheduleStatus.Unpaid).Sum(x => x.Amount),
            TotalNumberOfLoansOverTimeViewModel = groupedByDate.Select(x => new TotalNumberOfRepaymentLoansOverTimeViewModel()
            {
                Count = loans.Count(x=>x.Created.Date == x.Created.Date),
                Date = x.Date
            }).Distinct().ToList()
        };

        return report;
    }
}