using Carter;
using FluentValidation;
using FluentValidation.Results;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Trivista.LoanApp.ApplicationCore.Commons.Model;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanConfiguration;

public sealed class ConfigureLoanController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/configureLoan", HandleLoanConfiguration)
            .WithName("ConfigureLoan")
            .WithTags("Loan Configuration");
    }

    private async Task<IResult> HandleLoanConfiguration(IMediator mediator, [FromBody]ConfigureLoanCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToOk(b => (b.message));
    }
}

public class ConfigureLoanValidation : AbstractValidator<ConfigureLoanCommand>
{
    public ConfigureLoanValidation()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty().WithMessage("Loan name must be set");
        RuleFor(x => x.InterestRate).GreaterThan(0).WithMessage("Loan interest rate must be set");
        RuleFor(x => x.MaximumTenure).GreaterThan(0).WithMessage("Maximum tenure must be set");
        RuleFor(x => x.MaximumLoanAmount).GreaterThan(0).WithMessage("Maximum loan amount must be set");
        RuleFor(x => x.MinimumSalary).GreaterThan(0).WithMessage("Minimum salary must be set");
    }
}
public sealed record ConfigureLoanCommand(string Name, decimal InterestRate, decimal MaximumLoanAmount, decimal MaximumTenure, decimal MinimumSalary, bool IsDefault): IRequest<Result<(string message, bool isSuccessful)>>;

public sealed record ConfigureLoanCommandHandler : IRequestHandler<ConfigureLoanCommand, Result<(string message, bool isSuccessful)>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    public ConfigureLoanCommandHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<(string message, bool isSuccessful)>> Handle(ConfigureLoanCommand request, CancellationToken cancellationToken)
    {
        var validator = new ConfigureLoanValidation();
        var exceptionResult = await TrivistaValidationException<ConfigureLoanValidation, ConfigureLoanCommand>
            .ManageException<(string message, bool isSuccessful)>(validator, request, cancellationToken, ("", true));
        if (!exceptionResult.IsSuccess)
            return exceptionResult;
        var loanObject = Loan.Factory.Create(request.Name, request.InterestRate, request.MaximumLoanAmount,
            request.MaximumTenure, request.MinimumSalary, request.IsDefault);
        await _trivistaDbContext.Loan.AddAsync(loanObject, cancellationToken);
        var result = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        if (result > 0)
        {
           return ("Loan configured successful", true);
        }
        return ("Unable to configure loan", false);
    }
}