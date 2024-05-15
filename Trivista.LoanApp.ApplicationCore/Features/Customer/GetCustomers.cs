using System.Reflection;
using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Pagination;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.Customer;

public sealed class GetCustomersController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/customers", GetCustomersHandler)
            .WithName("Get Customers")
            .WithTags("Customer")
            .RequireAuthorization()
            .RequireCors("AllowSpecificOrigins");
    }

    private static async Task<IResult> GetCustomersHandler(IMediator mediator, 
        [FromQuery]string? firstName, 
        [FromQuery]string? lastName, 
        [FromQuery]string? email,
        [FromQuery]string? remita,
        [FromQuery]int pageNumber,
        [FromQuery]int itemsPerPage)
    {
        var result = await mediator.Send(new GetCustomersQuery(firstName, lastName, email, remita, pageNumber, itemsPerPage));
        return result.ToOk(x => x);
    }
}

public class GetCustomersRequestDto
{
    public string FirstName { get; set; }
    
    public string LastName { get; set; }
    
    public string Email { get; set; }
    
    public string Remitta { get; set; }
}

public sealed record GetCustomersDto
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
    
    public string Location { get; set; }
    
    public bool IsRemittaUser { get; set; }

    public string PostalCode { get; set; }

    public static explicit operator GetCustomersDto(Entities.Customer customer)
    {
        return new GetCustomersDto()
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            MiddleName = customer.MiddleName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Sex = customer.Sex,
            Dob = customer.Dob,
            Bvn = string.IsNullOrEmpty(customer.Bvn) ? string.Empty : customer.Bvn,
            Occupation = customer.Occupation,
            Address = customer.Address,
            Country = customer.Country,
            State = customer.State,
            City = customer.City,
            PostalCode = customer.PostCode,
            Location = customer.Location,
            IsRemittaUser = customer.CustomerRemitterInformation == null ? false : customer.CustomerRemitterInformation.IsRemittaUser!
        };
    }
}
public sealed record GetCustomersQuery(string FirstName, string LastName, string Email, string Remitta, int PageNumber, int ItemsPerPage): IRequest<Result<PaginationInfo<GetCustomersDto>>>;

public sealed class GetCustomersQueryHandler : IRequestHandler<GetCustomersQuery, Result<PaginationInfo<GetCustomersDto>>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    public GetCustomersQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }

    public async Task<Result<PaginationInfo<GetCustomersDto>>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        IQueryable<Entities.Customer> loanRequestList = Enumerable.Empty<Entities.Customer>().AsQueryable();
        
        loanRequestList = _trivistaDbContext.Customer.Include(x=>x.CustomerRemitterInformation).AsQueryable();
        
        if (!string.IsNullOrEmpty(request.FirstName))
        {
            loanRequestList = loanRequestList.Where(x => x.FirstName == request.FirstName);
        }
        
        if (!string.IsNullOrEmpty(request.LastName))
        {
            loanRequestList = loanRequestList.Where(x => x.LastName == request.LastName);
        }
        
        if (!string.IsNullOrEmpty(request.Email))
        {
            loanRequestList = loanRequestList.Where(x => x.Email == request.Email);
        }
        
        if (!string.IsNullOrEmpty(request.Remitta))
        {
            bool isRemittaUser = Convert.ToBoolean(request.Remitta);
            loanRequestList = loanRequestList.Where(x => x.CustomerRemitterInformation.IsRemittaUser == isRemittaUser);
        }

        loanRequestList = loanRequestList.Where(x=>x.UserType == "Customer");
        
        var pagedResult = await PaginationData.PaginateAsync(loanRequestList, request.PageNumber, request.ItemsPerPage);

        List<GetCustomersDto> customers = loanRequestList.Cast<GetCustomersDto?>().ToList();

        return new PaginationInfo<GetCustomersDto>(customers, 
                                                     pagedResult.CurrentPage, 
                                                     pagedResult.PageSize, 
                                                     pagedResult.TotalItems, 
                                                     pagedResult.TotalPages);
    }
}