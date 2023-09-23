using System;
using Carter;
using FluentValidation;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.Customer;

namespace Trivista.LoanApp.ApplicationCore.Features.Role
{
    public class AddApplicationRoleController : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/addRole", AddHandler)
                .WithName("AddRole")
                .WithTags("Role")
            .ExcludeFromDescription();
        }

        private async Task<IResult> AddHandler(IMediator mediator, ILogger<AddApplicationRoleController> logger, [FromBody] CreateRoleCommand command)
        {
            logger.LogInformation("Entered the AddRoleHandler");
            var result = await mediator.Send(command);
            logger.LogInformation("Exiting the AddRoleHandler");
            return result.ToOk(x => x);
        }
    }

    public class CreateRoleCommandValidation : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidation()
        {
            RuleFor(x => x.RoleName).NotNull().NotEmpty().WithMessage("Role name must be set");
        }
    }

    public sealed record CreateRoleCommand(Guid Id, string RoleName, string Description): IRequest<Result<bool>>;

    public sealed record CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, Result<bool>>
    {
        private readonly TrivistaDbContext _trivistaDbContext;
        private readonly ILogger<CreateRoleCommandHandler> _logger;

        public CreateRoleCommandHandler(TrivistaDbContext trivistaDbContext, ILogger<CreateRoleCommandHandler> logger)
        {
            _trivistaDbContext = trivistaDbContext;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Entered the Command handler for creating roles");
            var validator = new CreateRoleCommandValidation();
            var exceptionResult = await TrivistaValidationException<CreateRoleCommandValidation, CreateRoleCommand>
                .ManageException<bool>(validator, request, cancellationToken, true);
            if (!exceptionResult.IsSuccess)
                return exceptionResult;
            _logger.LogInformation("Creating role via factory");
            var role = ApplicationRole.Factory.Create(request.Id, request.RoleName, request.Description);
            _logger.LogInformation("Adding role to memory");
            await _trivistaDbContext.ApplicationRole.AddAsync(role, cancellationToken);
            _logger.LogInformation("Saving role to db");
            await _trivistaDbContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Role with Id: {RequestId} created successfully", request.Id);

            return true;
        }
    }
}

