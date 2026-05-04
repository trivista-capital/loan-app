using Carter;
using FluentValidation;
using LanguageExt.Common;
using LanguageExt.Pipes;
using MassTransit.Mediator;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;
using IMediator = MediatR.IMediator;
using Unit = LanguageExt.Unit;

namespace Trivista.LoanApp.ApplicationCore.Features.Customer;

public class GetCustomerStatementAnalysisController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/getCustomerStatementAnalysis/{customerId}", GetMbsStatementHandler)
            .WithName("Get Statement analysis")
            .RequireAuthorization()
            .WithTags("Admin");
    }
    
    private async Task<IResult> GetMbsStatementHandler(IMediator mediator, [FromRoute] Guid customerId)
    {
        var result = await mediator.Send(new GetMbsStatementQuery(){ CustomerId = customerId});
        return result.ToOk(x => x);
    }
}

public sealed record GetMbsStatementQuery : IRequest<Result<string>>
{
    public Guid CustomerId { get; set; }
}

public sealed class GetMbsStatementHandler : IRequestHandler<GetMbsStatementQuery, Result<string>>
{
    private readonly TrivistaDbContext _trivistaDbContext;

    public GetMbsStatementHandler(TrivistaDbContext trivistaDbContext, IIndicina indicina)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<string>> Handle(GetMbsStatementQuery request, CancellationToken cancellationToken)
    {
        var customer = await _trivistaDbContext.Customer
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);
        if(customer == null)
            return new Result<string>(ExceptionManager.Manage("Statement", "Customer is invalid"));
        
        if(string.IsNullOrEmpty(customer.BankStatementAnalysis))
            return new Result<string>(ExceptionManager.Manage("Statement", "Customer statement has not been processed"));

        return customer.BankStatementAnalysis;
    }
}