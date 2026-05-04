using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.Reschedule;

public class GetLoanRescheduleByLoanRequestIdController:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/repaymentSchedule/LoanRequest/{id}",
                GetRepaymentScheduleHandler)
            .WithName("GetRepaymentScheduleForLoan")
            .RequireAuthorization()
            .WithTags("Customer");
    }

    private static async Task<IResult> GetRepaymentScheduleHandler(IMediator mediator, Guid id)
    {
        var result = await mediator.Send(new LoanRepaymentScheduleByLoanRequestIdQuery(id));
        return result.ToOk(x => x);
    }
}

public sealed record RepaymentScheduleResponse
{
    public Guid Id { get; set; }
    
    public string RepaymentType { get; set; }
    
    public string Status { get; set; }
    
    public DateTime DueDate { get; set; }
    
    public decimal RepaymentAmount { get; set; }
    
    public string PaymentType { get; set; }
    
    public bool IsDue { get; set; }
    
    public DateTime Created { get; set; }

    public ICollection<RepaymentScheduleFailedAttemptsResponse> FailedAttempts { get; set; }
        = new List<RepaymentScheduleFailedAttemptsResponse>();

    public static explicit operator RepaymentScheduleResponse(RepaymentSchedule schedule)
    {
        return new RepaymentScheduleResponse()
        {
            Id = schedule.Id,
            RepaymentType = EnumHelpers.Convert(schedule.RepaymentType),
            Status = EnumHelpers.Convert(schedule.Status),
            DueDate = schedule.DueDate,
            RepaymentAmount = schedule.RepaymentAmount,
            PaymentType = EnumHelpers.Convert(schedule.PaymentType),
            IsDue = schedule.IsDue,
            FailedAttempts = schedule.FailedPaymentAttempts.Select(x=> 
                new RepaymentScheduleFailedAttemptsResponse(x.Amount, x.RepaymentSchedule.ToString())).ToList()
        };
    }
}

public sealed record RepaymentScheduleFailedAttemptsResponse(decimal Amount, string Status);

public sealed record LoanRepaymentScheduleByLoanRequestIdQuery(Guid Id): IRequest<Result<List<RepaymentScheduleResponse>>>;

public sealed class LoanRepaymentScheduleByLoanRequestIdQueryHandler : IRequestHandler<LoanRepaymentScheduleByLoanRequestIdQuery,
    Result<List<RepaymentScheduleResponse>>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public LoanRepaymentScheduleByLoanRequestIdQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }  
    
    public async Task<Result<List<RepaymentScheduleResponse>>> Handle(LoanRepaymentScheduleByLoanRequestIdQuery request, CancellationToken cancellationToken)
    {
        var schedules = new List<RepaymentScheduleResponse>();

        var repaymentSchedulesFromDb = await _trivistaDbContext.RepaymentSchedule.
            Include(x=>x.FailedPaymentAttempts)
            .OrderByDescending(x=>x.DueDate)
            .Where(x => x.LoanRequestId == request.Id)
            .ToListAsync(cancellationToken);
        if (!repaymentSchedulesFromDb.Any())
            return new List<RepaymentScheduleResponse>();
        foreach (var schedule in repaymentSchedulesFromDb)
        {
            schedules.Add((RepaymentScheduleResponse)schedule);
        }
        return schedules;
    }
}