using Carter;
using FluentValidation;
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
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.Customer
{
    public class SetCustomerLocation : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/Customer/setCustomerLocation", HandleCustomerLocation)
             .WithName("Customer Location")
             .WithTags("Customer")
             .RequireAuthorization()
             .RequireCors("AllowSpecificOrigins");
        }

        private async Task<IResult> HandleCustomerLocation(IMediator mediator, [FromBody] SetCustomerLocationCommand model)
        {
            var result = await mediator.Send(model);
            return result.ToOk(x => x);
        }
    }

    public class SetCustomerLocationCommandValidation : AbstractValidator<SetCustomerLocationCommand>
    {
        public SetCustomerLocationCommandValidation()
        {
            RuleFor(x => x.Location).NotEqual("string").NotEmpty().NotNull().WithMessage("Location must be set");
        }
    }

    public sealed record SetCustomerLocationCommand(Guid CustomerId, string Location) : IRequest<Result<Unit>>;

    public sealed class SetCustomerLocationCommandHandler : IRequestHandler<SetCustomerLocationCommand, Result<Unit>>
    {
        private readonly TrivistaDbContext _trivistaDbContext;
        private readonly ILogger<SetCustomerLocationCommandHandler> _logger;

        public SetCustomerLocationCommandHandler(TrivistaDbContext trivistaDbContext, ILogger<SetCustomerLocationCommandHandler> logger)
        {
            _trivistaDbContext = trivistaDbContext;
            _logger = logger;
        }

        public async Task<Result<Unit>> Handle(SetCustomerLocationCommand request, CancellationToken cancellationToken)
        {
            var validator = new SetCustomerLocationCommandValidation();
            var exceptionResult = await TrivistaValidationException<SetCustomerLocationCommandValidation, SetCustomerLocationCommand>
                .ManageException<Unit>(validator, request, cancellationToken, Unit.Value);
            if (!exceptionResult.IsSuccess)
                return exceptionResult;

            var customer = await _trivistaDbContext.Customer.FirstOrDefaultAsync(x => x.Id == request.CustomerId, cancellationToken);
            if (customer == null)
                return new Result<Unit>(ExceptionManager.Manage("Customer", "Customer does not exist"));

            customer.SetLocation(request.Location);

            _trivistaDbContext.Customer.Update(customer);

            var savedLoanRequestResponse = await _trivistaDbContext.SaveChangesAsync(cancellationToken);

            if (savedLoanRequestResponse > 0)
            {
                _logger.LogInformation("Customer location is set successfully");
                return Unit.Value;
            }

            return new Result<Unit>(ExceptionManager.Manage("Customer", "Unable to save customer location"));
        }
    }
}