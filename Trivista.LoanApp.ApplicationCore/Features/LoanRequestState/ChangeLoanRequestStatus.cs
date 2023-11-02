using Carter;
using FluentValidation;
using LanguageExt;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.LoanApproval;
using Unit = MediatR.Unit;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanRequestState;

public class ChangeLoanRequestStatusController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        //app.MapPost("/loanRequest/{id}/changeDisburses", ChangeLoanRequestStatusHandl)
        //    .WithName("Change loan request s")
        //    .WithTags("Admin");
    }

    //private static async Task<IResult> ChangeLoanRequestStatusHandl(IMediator mediator,
    //    string id,
    //    [FromBody] ChangeLoanRequestStatus command)
    //{
    //    var requestId = Guid.Parse(id);
    //    ChangeLoanRequestStatusCommand cmd = new(requestId, command.Status);
    //    var response = await mediator.Send(cmd);
    //    return response.ToOk(x => x);
    //}
}

public sealed record ChangeLoanRequestStatus(DisbursedLoanStatus Status);

public sealed record ChangeLoanRequestStatusCommand(Guid Id, DisbursedLoanStatus Status) : IRequest<Result<Unit>>;

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
        if (request.Status is not DisbursedLoanStatus.WrittenOff || request.Status is not DisbursedLoanStatus.Delinquent)
            return new Result<Unit>(ExceptionManager.Manage("Loan request change", "Invalid request. Status can only be WrittenOff or Deliquent"));

        var loanRequest = await _trivistaDbContext.
                                    LoanRequest.
                                    Where(x => x.Id == request.Id).
                                    Select(x => x).
                                    FirstOrDefaultAsync(cancellationToken);

        if (loanRequest == null)
            return new Result<Unit>(ExceptionManager.Manage("Loan request change", "No loan to found"));

        loanRequest!.SetDisbursementStatus(request.Status);

        var result = await _trivistaDbContext.SaveChangesAsync(cancellationToken);

        if (result > 0)
            return Unit.Value;

        return new Result<Unit>(ExceptionManager.Manage("Loan request change", "Unable to chamge loan status"));

    }
}

