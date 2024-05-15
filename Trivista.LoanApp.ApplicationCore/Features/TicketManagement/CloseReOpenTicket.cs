using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.TicketManagement;

public class CloseReOpenTicketController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/closeOpenTicket", TicketsHandler)
            .WithName("Close open ticket")
            .RequireAuthorization()
            .WithTags("Ticket Management");
    }

    private static async Task<IResult> TicketsHandler(IMediator mediator, [FromBody]CloseOpenTicketCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToOk(x => x);
    }
}

public sealed record CloseOpenTicketCommand(Guid TicketId, TicketStatus status) : IRequest<Result<Unit>>;

public sealed class CloseOpenTicketCommandHandler : IRequestHandler<CloseOpenTicketCommand, Result<Unit>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public CloseOpenTicketCommandHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<Unit>> Handle(CloseOpenTicketCommand request, CancellationToken cancellationToken)
    {
        var tickets = await _trivistaDbContext.Ticket
                            .FirstOrDefaultAsync(x=>x.Id == request.TicketId, cancellationToken);
        if (request.status == TicketStatus.Open)
        {
            tickets.OpenTicket();
        }
        else
        {
            tickets.CloseTicket();
        }

        var ticketStatus = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        if (ticketStatus > 0) return Unit.Value;
        return new Result<Unit>(ExceptionManager.Manage("Ticket", "Unable to perform action"));
    }
}