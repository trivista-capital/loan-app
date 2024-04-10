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
            .WithTags("Report")
        .RequireAuthorization();
    }

    private async Task<IResult> RepaymentReportHandler(IMediator mediator, [FromQuery]ScheduleStatus status, [FromQuery]string? startDate, [FromQuery]string? endDate)
    {
        var result = await mediator.Send(new RepaymentReportQuery(status, startDate, endDate));
        return result.ToOk(x => x);
    }
}

public sealed class RepaymentReportViewModel
{
    public int TotalNumberOfRepayment { get; set; }
    
    public decimal TotalRepaymentBalance { get; set; }

    public ICollection<TotalNumberOfRepaymentLoansOverTimeViewModel> TotalNumberOfLoansOverTimeViewModel { get; set; }
        = new List<TotalNumberOfRepaymentLoansOverTimeViewModel>();
}

public sealed record TotalNumberOfRepaymentLoansOverTimeViewModel
{
    public decimal RepaymentAmount { get; set; }
    
    public Guid LoanRequestId { get; set; }
    
    public string Status { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTime DateCreated { get; set; }
    
    public string RepaymentType { get; set; }
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
        var loanRequestWithStatus =  _trivistaDbContext
                                                          .LoanRequest
                                                          .Include(x=>x.RepaymentSchedules.Where(x => x.Status == request.status))
                                                          .AsNoTrackingWithIdentityResolution();
        
        if (!string.IsNullOrEmpty(request.startDate) && !string.IsNullOrEmpty(request.endDate))
        {
            var startDate = Convert.ToDateTime(request.startDate);
            var endDate = Convert.ToDateTime(request.endDate); //1/07/2023 - 5/07/2023
            
            loanRequestWithStatus = loanRequestWithStatus.Where(x => x.Created.Date >= startDate.Date && x.Created.Date < endDate.Date.AddDays(1)).OrderBy(x=>x.Created);
        }

        var loans = await loanRequestWithStatus.ToListAsync(cancellationToken);

        //var groupedByDate =  loans.OrderBy(x => x.Created).Select(x=>x.Created);
        var repaymentSchedules = loans.SelectMany(x => x.RepaymentSchedules.OrderBy(x => x.Created).ToList());
        var report = new RepaymentReportViewModel
        {
            TotalNumberOfRepayment = repaymentSchedules.Count(),
            TotalRepaymentBalance = repaymentSchedules.Sum(x => x.RepaymentAmount),
            
            TotalNumberOfLoansOverTimeViewModel = repaymentSchedules.Select(x => new TotalNumberOfRepaymentLoansOverTimeViewModel()
            {
                Amount = x.Amount,
                DateCreated = x.Created,
                RepaymentAmount = x.RepaymentAmount,
                Status = x.Status.ToString(),
                RepaymentType = x.RepaymentType.ToString(),
                LoanRequestId = x.LoanRequestId
            }).Distinct().ToList()
        };

        return report;
    }
}