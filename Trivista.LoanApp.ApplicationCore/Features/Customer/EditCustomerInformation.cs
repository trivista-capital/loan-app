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
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.Customer;

public class EditCustomerInformation: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/updateCustomer/{id}", UpdateCustomerHandler)
            .WithName("UpdateCustomer")
            .RequireAuthorization()
            .WithTags("Customer");
    }

    private async Task<IResult> UpdateCustomerHandler(IMediator mediator, Guid id, [FromBody]EditCustomerCommand customer)
    {
        var result = await mediator.Send(customer);
        return result.ToOk(x => x);
    }
}

public class EditCustomerValidation : AbstractValidator<EditCustomerCommand>
{
    public EditCustomerValidation()
    {
       
    }
}

public sealed record EditCustomerCommand(Guid Id, string FirstName, string MiddleName, string LastName, string Email,
    string PhoneNumber, string Sex, string Dob, Guid RoleId, string Address, string Bvn, string City, string Country,
    string Occupation, string State, string PostalCode): IRequest<Result<bool>>;

public sealed record EditCustomerCommandHandler : IRequestHandler<EditCustomerCommand, Result<bool>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    private readonly ILogger<AddCustomerCommandHandler> _logger;
    public EditCustomerCommandHandler(TrivistaDbContext trivistaDbContext, ILogger<AddCustomerCommandHandler> logger)
    {
        _trivistaDbContext = trivistaDbContext;
        _logger = logger;
    }
    
    public async Task<Result<bool>> Handle(EditCustomerCommand request, CancellationToken cancellationToken)
    {
        var validator = new EditCustomerValidation();
        var exceptionResult = await TrivistaValidationException<EditCustomerValidation, EditCustomerCommand>
            .ManageException<bool>(validator, request, cancellationToken, true);
        if (!exceptionResult.IsSuccess)
            return exceptionResult;
        
        var customer = await _trivistaDbContext.Customer.FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
        if(customer == null)
            return new Result<bool>(ExceptionManager.Manage("Customer Update", "Unable to update customer"));
        
        customer.SetFirstName(request.FirstName).SetLastName(request.LastName).SetAddress(request.Address).SetBvn(request.Bvn)
            .SetCity(request.City).SetCountry(request.Country).SetDob(request.Dob).SetOccupation(request.Occupation).SetSex(request.Sex)
            .SetState(request.State).SetMiddleName(request.MiddleName).SetPhoneNumber(request.PhoneNumber).SetPostCode(request.PostalCode)
            .SetEmail(request.Email);
        
        await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Customer with Id: {Email} updated successfully", request.Email);

        return true;
    }
}