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

public sealed class EditLoanConfigurationController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("editLoanConfiguration/{id}", EditLoanConfigurationHandler)
            .WithName("EditLoanConfiguration")
            .RequireAuthorization()
            .WithTags("Loan Configuration");
    }

    private async Task<IResult> EditLoanConfigurationHandler(IMediator mediator, [FromRoute]int id,
        [FromBody] EditLoanConfigurationCommand command)
    {
        var response = await mediator.Send(command);
        return response.ToOk(x => x.Item2);
    }
}

public sealed record EditLoanConfigurationCommand(int Id, string Name, decimal InterestRate, decimal MaximumTenure, decimal MaximumLoanAmount, bool IsDefault, decimal MinimumSalary): IRequest<Result<(bool, string)>>;

public sealed record EditLoanConfigurationCommandHandler : IRequestHandler<EditLoanConfigurationCommand, Result<(bool, string)>>
{
    private readonly TrivistaDbContext _trivistaDbContext;

    public EditLoanConfigurationCommandHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<(bool, string)>> Handle(EditLoanConfigurationCommand request, CancellationToken cancellationToken)
    {
        var existingActiveLoan = await _trivistaDbContext.Loan.FirstOrDefaultAsync(x => x.IsDefault, cancellationToken);
        if (existingActiveLoan != null)
        {
            existingActiveLoan.SetDefaultLoan(false);
        }
        var loanConfigFromDb = await _trivistaDbContext.Loan.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if (loanConfigFromDb == null)
            return new Result<(bool, string)>((false, "No lona has been configured"));
        loanConfigFromDb.SetName(request.Name).SetInterestRate(request.InterestRate).SetMaximumTenure(request.MaximumTenure)
            .SetMaximumLoanAmount(request.MaximumLoanAmount).SetDefaultLoan(request.IsDefault)
            .SetMinimumSalary(request.MinimumSalary);
        var isSaved = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        if(isSaved > 0)
            return (true, "Loan configuration edited successfully");
        return (false, "Unable to edit loan configuration");
    }
}