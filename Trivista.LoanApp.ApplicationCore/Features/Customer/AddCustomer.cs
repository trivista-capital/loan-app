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
using Trivista.LoanApp.ApplicationCore.Enums;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;

namespace Trivista.LoanApp.ApplicationCore.Features.Customer;

public sealed class AddCustomerController : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/addCustomer", AddCustomerHandler)
            .WithName("AddCustomer")
            .WithTags("Customer")
            .ExcludeFromDescription();
    }

    private async Task<IResult> AddCustomerHandler(IMediator mediator, [FromBody]AddCustomerCommand customer)
    {
        var result = await mediator.Send(customer);
        return result.ToOk(x => x);
    }
}

public class AddCustomerValidation : AbstractValidator<AddCustomerCommand>
{
    public AddCustomerValidation()
    {
        RuleFor(x => x.FirstName).NotNull().NotEmpty().WithMessage("First name must be set");
        RuleFor(x => x.LastName).NotNull().NotEmpty().WithMessage("Last name must be set");
        RuleFor(x => x.Email).NotNull().NotEmpty().WithMessage("Email must be set");
    }
}

public sealed record AddCustomerCommand(Guid Id, string FirstName, string MiddleName, string LastName, string Email,
    string PhoneNumber, string Sex, string Dob, Guid RoleId, string Address, string UserType): IRequest<Result<bool>>;

public sealed record AddCustomerCommandHandler : IRequestHandler<AddCustomerCommand, Result<bool>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    private readonly ILogger<AddCustomerCommandHandler> _logger;
    private readonly IRemittaService _remittaService;
    public AddCustomerCommandHandler(TrivistaDbContext trivistaDbContext, ILogger<AddCustomerCommandHandler> logger, IRemittaService remittaService)
    {
        _trivistaDbContext = trivistaDbContext;
        _logger = logger;
        _remittaService = remittaService;
    }

    public async Task<Result<bool>> Handle(AddCustomerCommand request, CancellationToken cancellationToken)
    {
        var validator = new AddCustomerValidation();
        var exceptionResult = await TrivistaValidationException<AddCustomerValidation, AddCustomerCommand>
            .ManageException<bool>(validator, request, cancellationToken, true);
        if (!exceptionResult.IsSuccess)
            return exceptionResult;
        var customer = Entities.Customer.Factory.Build(request.Id, request.FirstName, request.LastName, request.Email,
                                        request.PhoneNumber, request.Sex, request.PhoneNumber, request.RoleId.ToString(), request.UserType)
                                        .SetMiddleName(request.MiddleName).SetAddress(request.Address)
                                        .SetCustomerRemittance(new Entities.ValueObjects.CustomerRemitterInformation()
                                        {
                                            IsRemittaUser = RemittaUser.NotDetermined.ToString(),
                                            OtherLoansCollected = 0,
                                            AverageSixMonthsSalary = 0,
                                        });

        await _trivistaDbContext.Customer.AddAsync(customer, cancellationToken);
        await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Customer with Id: {RequestId} created successfully", request.Id);

        return true;
    }
}