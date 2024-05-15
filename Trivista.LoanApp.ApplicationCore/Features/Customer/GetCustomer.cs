using Carter;
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

namespace Trivista.LoanApp.ApplicationCore.Features.Customer;

public sealed class GetCustomerController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/customers/Profile/{id}", GetCustomersHandler)
            .WithName("Get Customer Profile")
            .WithTags("Customer")
            .RequireAuthorization()
            .RequireCors("AllowSpecificOrigins");
    }

    private static async Task<IResult> GetCustomersHandler(IMediator mediator, [FromRoute]Guid id)
    {
        var result = await mediator.Send(new GetCustomerQuery(id));
        return result.ToOk(x => x);
    }
}

public sealed record GetCustomerDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Sex { get; set; }
    public string Dob { get; set; }
    
    public string Bvn { get; set; }
    
    public string Occupation { get; set; }
    
    public string Address { get; set; }
    
    public string Country { get; set; }
    
    public string State { get; set; }
    
    public string City { get; set; }
    
    public string PostalCode { get; set; }

    public string Location { get; set; }

    public bool? IsRemittaUser { get; set; }

    public static explicit operator GetCustomerDto(Entities.Customer customer)
    {
        return new GetCustomerDto()
        {
            Id = customer.Id,
            FirstName = customer?.FirstName,
            MiddleName = customer?.MiddleName,
            LastName = customer?.LastName,
            Email = customer?.Email,
            PhoneNumber = customer?.PhoneNumber,
            Sex = customer?.Sex,
            Dob = customer?.Dob,
            Bvn = customer?.Bvn,
            Occupation = customer?.Occupation,
            Address = customer?.Address,
            Country = customer?.Country,
            State = customer?.State,
            City = customer?.City,
            PostalCode = customer?.PostCode,
            Location = customer.Location,
            IsRemittaUser = customer?.CustomerRemitterInformation.IsRemittaUser
        };
    }
}
public sealed record GetCustomerQuery(Guid Id): IRequest<Result<GetCustomerDto>>;

public sealed class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, Result<GetCustomerDto>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    public GetCustomerQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    public async Task<Result<GetCustomerDto>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var customerFrmDb = await _trivistaDbContext.Customer.FirstOrDefaultAsync(x=>x.Id == request.Id, 
                                cancellationToken);
        if(customerFrmDb == null || customerFrmDb.Id == default)
            return new Result<GetCustomerDto>(ExceptionManager.Manage("Get Customer", "Customer not found"));
        var customer = (GetCustomerDto)customerFrmDb;

        return customer;
    }
}