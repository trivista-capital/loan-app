using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;

public class EditLoanRequestDisbursementStatusController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("loanRequest/{id}/changeLoanDisbursementStatus", EditLoanRequestDisbursementStatusHandler).
            WithName("Change loan request status").
            WithTags("Admin");
    }

    private static async Task<IResult> EditLoanRequestDisbursementStatusHandler(IMediator mediator, string id, CChangeLoanRequestStatus command)
    {
        var requestId = Guid.Parse(id);
        CChangeLoanRequestStatusCommand cmd = new(requestId, command.Status);
        var result = await mediator.Send(cmd);

        return result.ToOk(x => x);
    }
}

public sealed record CChangeLoanRequestStatus(DisbursedLoanStatus Status);

public sealed record CChangeLoanRequestStatusCommand(Guid Id, DisbursedLoanStatus Status) : IRequest<Result<Unit>>;