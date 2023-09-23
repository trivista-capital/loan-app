using Carter;
using LanguageExt.Common;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Commons.Pagination;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.TicketManagement;

public class AllTickets: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/tickets", TicketsHandler)
            .WithName("Get all tickets")
            .WithTags("Ticket Management");
        //.RequireAuthorization();
    }

    private static async Task<IResult> TicketsHandler(IMediator mediator, 
                                                      [FromQuery]Guid? customerId, 
                                                      [FromQuery]int pageNumber, 
                                                      [FromQuery]int itemsPerPage)
    {
        var result = await mediator.Send(new GetTicketsQuery(customerId, pageNumber, itemsPerPage));
        return result.ToOk(x => x);
    }
}

public sealed class GetTicketsDto
{
    public Guid Id { get; set; }
    
    public Guid CustomerId { get; set; }
    
    public string Customer { get; set; }
    public string Status { get; set; }
    public string Email { get; set; }
    public DateTime Created { get; set; }

    public static implicit operator GetTicketsDto(Ticket ticket)
    {
        return new GetTicketsDto()
        {
            Id = ticket.Id,
            Status = EnumHelpers.Convert(ticket.Status),
            Email = ticket.Email,
            CustomerId = ticket.CustomerId,
            Customer = $"{ticket?.Customer?.FirstName} {ticket?.Customer?.LastName}",
            Created = ticket.Created
        };
    }
}

public sealed record GetTicketsQuery(Guid? CustomerId, int PageNumber, int ItemsPerPage) : IRequest<Result<PaginationInfo<GetTicketsDto>>>;

public sealed class GetTicketsQueryHandler : IRequestHandler<GetTicketsQuery, Result<PaginationInfo<GetTicketsDto>>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public GetTicketsQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<PaginationInfo<GetTicketsDto>>> Handle(GetTicketsQuery request, CancellationToken cancellationToken)
    {
        var tickets = Enumerable.Empty<Ticket>().AsQueryable();
        
        tickets = _trivistaDbContext.Ticket.OrderBy(x=>x.Created).Include(x=>x.Customer).AsQueryable();
        
        
        if (request.CustomerId.Equals(default))
        {
            tickets = tickets.Select(x => x).AsQueryable();
        }
        else
        {
            tickets = tickets.Where(x=>x.CustomerId == request.CustomerId);
        }
        
        var pagedResult = await PaginationData.PaginateAsync(tickets, request.PageNumber, request.ItemsPerPage);
        
        if(!tickets.Any())
            return new PaginationInfo<GetTicketsDto>(new List<GetTicketsDto>(), 
                                    pagedResult.CurrentPage, 
                                    pagedResult.PageSize, 
                                    pagedResult.TotalItems, 
                                    pagedResult.TotalPages);

        var result = tickets.Select(ticket => (GetTicketsDto)ticket).ToList();
        
        return new PaginationInfo<GetTicketsDto>(result, 
                                                    pagedResult.CurrentPage, 
                                                    pagedResult.PageSize, 
                                                    pagedResult.TotalItems, 
                                                    pagedResult.TotalPages);
        
        
    }
}