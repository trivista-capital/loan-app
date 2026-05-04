using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.LoanConfiguration;

public class GetLoanConfigurationsController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/getLoanConfigurations", GetGetLoanConfigurationsHandler)
            .WithName("GetLoanConfigurations")
            .RequireAuthorization()
            .WithTags("Loan Configuration");
    }

    private async Task<IResult> GetGetLoanConfigurationsHandler(IMediator mediator)
    {
        var response = await mediator.Send(new LoanConfigurationsQuery());
        return response.ToOk(x=>x);
    }
}

public class GetLoanConfigurationsDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal InterestRate { get; set; }
    public decimal MaximumLoanAmount{ get; set;}
    public decimal MaximumTenure{ get; set;}
    
    public bool IsDefault{ get; set;}
    
    public decimal MinimumSalary{ get; set;}

    public static List<GetLoanConfigurationsDto> ToGetLoanConfigurationsDto(List<Loan> loans)
    {
        return loans.Select(x => new GetLoanConfigurationsDto()
        {
            Id = x.Id,
            Name = x.Name,
            InterestRate = x.InterestRate,
            MaximumTenure = x.MaximumTenure,
            MaximumLoanAmount = x.MaximumLoanAmount,
            IsDefault = x.IsDefault,
            MinimumSalary = x.MinimumSalary
        }).ToList();
    }
}

public record LoanConfigurationsQuery(): IRequest<Result<List<GetLoanConfigurationsDto>>>;

public record LoanConfigurationsQueryHandler : IRequestHandler<LoanConfigurationsQuery, Result<List<GetLoanConfigurationsDto>>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    public LoanConfigurationsQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    public async Task<Result<List<GetLoanConfigurationsDto>>> Handle(LoanConfigurationsQuery request, CancellationToken cancellationToken)
    {
        var loanConfigFromDb = await _trivistaDbContext.Loan.ToListAsync(cancellationToken);
        var lanConfigResponse = GetLoanConfigurationsDto.ToGetLoanConfigurationsDto(loanConfigFromDb);
        return lanConfigResponse;
    }
}