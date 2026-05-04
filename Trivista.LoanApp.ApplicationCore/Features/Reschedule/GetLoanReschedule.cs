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

namespace xTrivista.LoanApp.ApplicationCore.Features.Reschedule;

public class GetLoanRescheduleController:ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/getRepaymentSchedule/{id}", GetRepaymentScheduleHandler)
            .WithName("GetRepaymentSchedule")
            .RequireAuthorization()
            .WithTags("Customer");
    }

    private static async Task<IResult> GetRepaymentScheduleHandler(IMediator mediator, Guid id)
    {
        var result = await mediator.Send(new LoanRepaymentScheduleQuery(id));
        return result.ToOk(x => x);
    }
}

public sealed record LoanRepaymentScheduleResponse
{
    public Guid Id { get; set; }
    
    public string RepaymentType { get; set; }

    public DateTime DueDate { get; set; }
    
    public decimal RepaymentAmount { get; set; }
    
    public string PaymentType { get; set; }
    
    public string Status { get; set; }
    
    public bool IsDue { get; set; }

    public ICollection<FailedAttemptsResponse> FailedAttempts { get; set; } = new List<FailedAttemptsResponse>();

    public static explicit operator LoanRepaymentScheduleResponse(RepaymentSchedule schedule)
    {
        return new LoanRepaymentScheduleResponse()
        {
            Id = schedule.Id,
            RepaymentType = EnumHelpers.Convert(schedule.RepaymentType),
            IsDue = schedule.IsDue,
            DueDate = schedule.DueDate,
            RepaymentAmount = schedule.RepaymentAmount,
            PaymentType = EnumHelpers.Convert(schedule.PaymentType),
            Status = EnumHelpers.Convert(schedule.Status),
            FailedAttempts = schedule.FailedPaymentAttempts.Select(x=> new FailedAttemptsResponse(x.Amount, x.RepaymentSchedule.ToString())).ToList()
        };
    }
}

public sealed record FailedAttemptsResponse(decimal Amount, string Status);

public sealed record LoanRepaymentScheduleQuery(Guid Id): IRequest<Result<LoanRepaymentScheduleResponse>>;

public sealed class LoanRepaymentScheduleQueryHandler : IRequestHandler<LoanRepaymentScheduleQuery,
    Result<LoanRepaymentScheduleResponse>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public LoanRepaymentScheduleQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }  
    
    public async Task<Result<LoanRepaymentScheduleResponse>> Handle(LoanRepaymentScheduleQuery request, CancellationToken cancellationToken)
    {
        var repaymentScheduleFromDb = await _trivistaDbContext.RepaymentSchedule
            .Include(x=>x.FailedPaymentAttempts).
            FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (repaymentScheduleFromDb == null)
            return new Result<LoanRepaymentScheduleResponse>();
        var repaymentSchedule = (LoanRepaymentScheduleResponse)repaymentScheduleFromDb;
        return repaymentSchedule;
    }
}