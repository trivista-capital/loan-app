using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanConfiguration;

public class GetActiveLoanConfigurationController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/getActiveLoanConfiguration", GetActiveGetLoanConfigurationHandler)
            .WithName("GetActiveLoanConfiguration")
            .RequireAuthorization()
            .WithTags("Loan Configuration");
    }

    private async Task<IResult> GetActiveGetLoanConfigurationHandler(IMediator mediator)
    {
        var response = await mediator.Send(new ActiveLoanConfigurationQuery());
        return response.ToOk(x=>x);
    }
}

public class GetActiveLoanConfigurationDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal InterestRate { get; set; }
    public decimal MaximumLoanAmount{ get; set;}
    public decimal MaximumTenure{ get; set;}
    
    public bool IsDefault{ get; set;}
    
    public decimal MinimumSalary{ get; set;}

    public static GetActiveLoanConfigurationDto ToGetLoanConfigurationDto(Loan loan)
    {
        return new GetActiveLoanConfigurationDto()
        {
            Id = loan.Id,
            Name = loan.Name,
            InterestRate = loan.InterestRate,
            MaximumTenure = loan.MaximumTenure,
            MaximumLoanAmount = loan.MaximumLoanAmount,
            IsDefault = loan.IsDefault,
            MinimumSalary = loan.MinimumSalary
        };
    }
}

public record ActiveLoanConfigurationQuery: IRequest<Result<GetActiveLoanConfigurationDto>>;

public record ActiveLoanConfigurationQueryHandler : IRequestHandler<ActiveLoanConfigurationQuery, Result<GetActiveLoanConfigurationDto>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    public ActiveLoanConfigurationQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    public async Task<Result<GetActiveLoanConfigurationDto>> Handle(ActiveLoanConfigurationQuery request, CancellationToken cancellationToken)
    {
        var loanConfigFromDb = await _trivistaDbContext.Loan.FirstOrDefaultAsync(x=>x.IsDefault == true, cancellationToken);
        var loanConfigResponse = GetActiveLoanConfigurationDto.ToGetLoanConfigurationDto(loanConfigFromDb);
        return loanConfigResponse;
    }
}