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

public class TestCustomerStatementAnalysisController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/testCustomerStatementAnalysis", GetMbsStatementHandler)
            //.ExcludeFromDescription()
            .WithName("Get Mbs Statement")
            .WithTags("Customer");
    }
    
    private async Task<IResult> GetMbsStatementHandler(IMediator mediator, [FromBody]TestGetMbsStatementQuery model)
    {
        var result = await mediator.Send(model);
        return result.ToOk(x => x);
    }
}

public sealed record TestGetMbsStatementQuery : IRequest<Result<string>>
{
    public string Model{get; set;}
    public Guid CustomerId { get; set; }
}

public sealed class TestGetMbsStatementHandler : IRequestHandler<TestGetMbsStatementQuery, Result<string>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    private readonly IIndicina _indicina;

    public TestGetMbsStatementHandler(TrivistaDbContext trivistaDbContext, IIndicina indicina)
    {
        _trivistaDbContext = trivistaDbContext;
        _indicina = indicina;
    }
    
    public async Task<Result<string>> Handle(TestGetMbsStatementQuery request, CancellationToken cancellationToken)
    {
        var customer = await _trivistaDbContext.Customer
                                    .AsNoTracking()
                                    .FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);
        
        var indicinaResponse = await _indicina.ProcessStatement(new BankStatementRequest()
        {
            Customer = new()
            {
                Id = Guid.NewGuid().ToString(),
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Phone = customer.PhoneNumber
            },
            BankStatement = new()
            {
                Type = "mbs",
                Content = new()
                {
                    Message = "Successful",
                    Result = request.Model,
                    Status = "00"
                }
            }
        });

        if (!string.IsNullOrEmpty(indicinaResponse))
        {
            return indicinaResponse;
        }
        
        return new Result<string>(ExceptionManager.Manage("Statement", "Unable to get statement"));
    }
}