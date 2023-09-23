using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.TicketManagement;

public sealed class TicketController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/ticket/{id}", TicketsHandler)
            .WithName("Get ticket")
            .WithTags("Ticket Management");
    }

    private static async Task<IResult> TicketsHandler(IMediator mediator, [FromRoute]Guid id)
    {
        var result = await mediator.Send(new GetTicketQuery(id));
        return result.ToOk(x => x);
    }
}

public sealed class GetTicketDto
{
    public Guid Id { get; set; }
    
    public Guid CustomerId { get; set; }
    
    public string Customer { get; set; }
    public string Status { get; set; }
    public string Email { get; set; }
    
    public DateTime Created { get; set; }

    public List<GetTicketMessages> Messages { get; set; }
        = new List<GetTicketMessages>();

    public static implicit operator GetTicketDto(Ticket ticket)
    {
        return new GetTicketDto()
        {
            Id = ticket.Id,
            Status = EnumHelpers.Convert(ticket.Status),
            Email = ticket.Email,
            Customer = $"{ticket?.Customer?.FirstName} {ticket?.Customer?.LastName}",
            Created = ticket.Created,
            Messages = ticket.Messages
                                .Select(x=> new GetTicketMessages(x.TicketId, x.Message, EnumHelpers.Convert(x.Initiator), 
                                    x.InitiatorId))
                                .ToList()
        };
    }
}

public sealed record GetTicketMessages(Guid TicketId, string Message, string Initiator, Guid InitiatorId);

public sealed record GetTicketQuery(Guid Id) : IRequest<Result<GetTicketDto>>;

public sealed class GetTicketQueryHandler : IRequestHandler<GetTicketQuery, Result<GetTicketDto>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public GetTicketQueryHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<GetTicketDto>> Handle(GetTicketQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _trivistaDbContext.Ticket
                        .Include(x=>x.Customer)
                        .Include(x=>x.Messages)
                        .Where(x=>x.Id == request.Id)
                        .FirstOrDefaultAsync(cancellationToken);
        if(ticket == null)
            return null;
        return (GetTicketDto)ticket;
    }
}