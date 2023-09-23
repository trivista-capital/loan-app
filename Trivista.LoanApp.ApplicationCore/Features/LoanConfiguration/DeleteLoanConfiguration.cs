using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanConfiguration;

public sealed class DeleteLoanConfigurationController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/deleteLoanConfiguration/{id}", DeleteLoanConfigurationHandler)
            .WithName("DeleteLoanConfiguration")
            .WithTags("Loan Configuration");
    }

    private async Task<IResult> DeleteLoanConfigurationHandler(IMediator mediator, [FromRoute] int id)
    {
        var result = await mediator.Send(new DeleteLoanConfigurationCommand(id));
        return result.ToOk(x => x.Item2);
    }
}

public sealed record DeleteLoanConfigurationCommand(int Id): IRequest<Result<(bool, string)>>;

public record DeleteLoanConfigurationCommandHandler : IRequestHandler<DeleteLoanConfigurationCommand, Result<(bool, string)>>
{
    private readonly TrivistaDbContext _trivistaDbContext;

    public DeleteLoanConfigurationCommandHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }

    public async Task<Result<(bool, string)>> Handle(DeleteLoanConfigurationCommand request, CancellationToken cancellationToken)
    {
        var loanConfigFromDb = await _trivistaDbContext.Loan.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (loanConfigFromDb == null)
            return new Result<(bool, string)>((false, "No loan configuration found with such id"));
        _trivistaDbContext.Loan.Remove(loanConfigFromDb);
        var isDeleted = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        if (isDeleted > 0)
            return (true, "Loan configuration deleted successfully");
        return (false, "Unable to delete loan configuration");
    }
}