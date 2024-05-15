using Carter;
using FluentValidation;
using LanguageExt.Common;
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

namespace Trivista.LoanApp.ApplicationCore.Features.Customer;

public class RequestMbsStatementController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/requestMbsStatement", RequestMbsStatementHandler)
            .WithName("Request Mbs Statement")
            .RequireAuthorization()
            .WithTags("Customer");
    }
    
    private async Task<IResult> RequestMbsStatementHandler(IMediator mediator, [FromBody]RequestMbsStatementQuery model)
    {
        var result = await mediator.Send(model);
        return result.ToOk(x => x);
    }
}

public class RequestMbsStatementQueryValidation : AbstractValidator<RequestMbsStatementQuery>
{
    public RequestMbsStatementQueryValidation()
    {
        RuleFor(x => x.CustomerId).NotNull().NotEmpty().WithMessage("CustomerId must be set");
        RuleFor(x => x.Model.BankId).NotNull().NotEmpty().WithMessage("Bank must be valid");
        RuleFor(x => x.Model.StartDate).NotNull().NotEmpty().WithMessage("Start date must be set");
        RuleFor(x => x.Model.EndDate).NotNull().NotEmpty().WithMessage("End date must be set");
        RuleFor(x => x.Model.Role).NotNull().NotEmpty().WithMessage("Role must be set");
    }
}

public sealed record RequestMbsStatementQuery : IRequest<Result<RequestStatementResponseDto>>
{
    public RequestStatementRequestDto Model{get; set;}
    
    public Guid CustomerId { get; set; }
}

public sealed class RequestMbsStatementHandler : IRequestHandler<RequestMbsStatementQuery, Result<RequestStatementResponseDto>>
{
    private readonly IMbsService _mbsService;

    private readonly TrivistaDbContext _trivistaDbContext;

    public RequestMbsStatementHandler(IMbsService mbsService, TrivistaDbContext trivistaDbContext)
    {
        _mbsService = mbsService;
        _trivistaDbContext = trivistaDbContext;
    }
    public async Task<Result<RequestStatementResponseDto>> Handle(RequestMbsStatementQuery request, CancellationToken cancellationToken)
    {
        var validator = new RequestMbsStatementQueryValidation();
        var exceptionResult = await TrivistaValidationException<RequestMbsStatementQueryValidation, RequestMbsStatementQuery>
            .ManageException<RequestStatementResponseDto>(validator, request, cancellationToken, new RequestStatementResponseDto());
        
        if (!exceptionResult.IsSuccess)
            return exceptionResult;

        var customer = await _trivistaDbContext.Customer
                                               .AsNoTracking()
                                               .FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);
        
        if(customer == null)
            return new Result<RequestStatementResponseDto>(ExceptionManager.Manage("Statement", "Customer is invalid"));
        
        var result = await _mbsService.RequestStatement(request.Model);
        if (result.Status == "00")
        {
            customer.SetMbsCode(result.Result);
            _trivistaDbContext.Customer.Update(customer);
            await _trivistaDbContext.SaveChangesAsync(cancellationToken);
            return result;
        }
        return new Result<RequestStatementResponseDto>(ExceptionManager.Manage("Statement", "Unable to request statement, please try again later"));
    }
}