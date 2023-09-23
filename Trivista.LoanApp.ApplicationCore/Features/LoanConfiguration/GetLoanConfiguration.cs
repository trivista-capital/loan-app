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

public class GetLoanConfigurationController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/getLoanConfigurationById/{id}", GetGetLoanConfigurationHandler)
            .WithName("GetLoanConfigurationById")
            .WithTags("Loan Configuration");
    }

    private async Task<IResult> GetGetLoanConfigurationHandler(IMediator mediator, [FromRoute]int id)
    {
        var response = await mediator.Send(new LoanConfigurationQuery(id));
        return response.ToOk(x=>x);
    }
}

public class GetLoanConfigurationDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal InterestRate { get; set; }
    public decimal MaximumLoanAmount{ get; set;}
    public decimal MaximumTenure{ get; set;}
    public bool IsDefault{ get; set;}
    public decimal MinimumSalary{ get; set;}

    public static GetLoanConfigurationDto ToGetLoanConfigurationDto(Loan loan)
    {
        return new GetLoanConfigurationDto()
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

public record LoanConfigurationQuery(int Id): IRequest<Result<GetLoanConfigurationDto>>;

public record LoanConfigurationQueryHandler : IRequestHandler<LoanConfigurationQuery, Result<GetLoanConfigurationDto>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    public LoanConfigurationQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    public async Task<Result<GetLoanConfigurationDto>> Handle(LoanConfigurationQuery request, CancellationToken cancellationToken)
    {
        var loanConfigFromDb = await _trivistaDbContext.Loan.FirstOrDefaultAsync(x=>x.Id == request.Id, cancellationToken);
        var loanConfigResponse = GetLoanConfigurationDto.ToGetLoanConfigurationDto(loanConfigFromDb);
        return loanConfigResponse;
    }
}