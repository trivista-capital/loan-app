using Carter;
using LanguageExt.Common;
using LanguageExt.SomeHelp;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.LoanRequestState;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;

public class EditLoanRequestDisbursementStatusController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("loanRequest/{id}/changeLoanDisbursementStatus", EditLoanRequestDisbursementStatusHandler).
            WithName("Change loan request status").
            WithTags("Admin");
    }

    private static async Task<IResult> EditLoanRequestDisbursementStatusHandler(IMediator mediator, string id, ChangeLoanRequestStatus command)
    {
        var requestId = Guid.Parse(id);
        ChangeLoanRequestStatusCommand cmd = new(requestId, command.Status);
        var result = await mediator.Send(cmd);

        return result.ToOk(x => x);
    }
}

public sealed record ChangeLoanRequestStatus(string Status);

public sealed record ChangeLoanRequestStatusCommand(Guid Id, string Status) : IRequest<Result<Unit>>;

public sealed class ChangeLoanRequestStatusCommandHandler : IRequestHandler<ChangeLoanRequestStatusCommand, Result<Unit>>
{
    private readonly TrivistaDbContext _trivistaDbContext;

    private readonly ILogger<ChangeLoanRequestStatusCommandHandler> _logger;

    public ChangeLoanRequestStatusCommandHandler(TrivistaDbContext trivistaDbContext,
        ILogger<ChangeLoanRequestStatusCommandHandler> logger)
    {
        _trivistaDbContext = trivistaDbContext;
        _logger = logger;
    }

    public async Task<Result<Unit>> Handle(ChangeLoanRequestStatusCommand request, CancellationToken cancellationToken)
    {
        if(string.IsNullOrEmpty(request.Status))
            return new Result<Unit>(ExceptionManager.Manage("Loan request change", "Status can not be null or empty"));
        
        var status = (DisbursedLoanStatus)Enum.Parse(typeof(DisbursedLoanStatus), request.Status);
        var listOfEnums = new List<DisbursedLoanStatus>() {
            DisbursedLoanStatus.WrittenOff,
            DisbursedLoanStatus.Delinquent
        };
        
        if(!listOfEnums.Contains(status))
            return new Result<Unit>(ExceptionManager.Manage("Loan request change", "Invalid request. Status can only be WrittenOff or Deliquent"));

        var loanRequest = await _trivistaDbContext.
            LoanRequest.
            Where(x => x.Id == request.Id).
            Select(x => x).
            FirstOrDefaultAsync(cancellationToken);

        if (loanRequest == null)
            return new Result<Unit>(ExceptionManager.Manage("Loan request change", "No loan to found"));

        loanRequest!.SetDisbursementStatus(status);

        var result = await _trivistaDbContext.SaveChangesAsync(cancellationToken);

        if (result > 0)
            return Unit.Value;

        return new Result<Unit>(ExceptionManager.Manage("Loan request change", "Unable to chamge loan status"));

    }
}