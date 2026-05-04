using Carter;
using LanguageExt.Common;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.TicketManagement;

public class AddMessageToTicketController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/addMessage", LogMessageHandler)
            .WithName("Add Message")
            .RequireAuthorization()
            .WithTags("Ticket Management");
    }

    private static async Task<IResult> LogMessageHandler(IMediator mediator, [FromBody] AddMessageToTicketCommand command)
    {
        var result = await mediator.Send(command);
        return result.ToOk(x => x);
    }
}

public sealed record AddMessageToTicketCommand(Guid TicketId, string Message, TicketInitiator Initiator, Guid InitiatorId) : IRequest<Result<Unit>>;

public sealed class AddMessageToTicketCommandHandler : IRequestHandler<AddMessageToTicketCommand, Result<Unit>>
{
    private readonly TrivistaDbContext _trivistaDbContext;
    
    public AddMessageToTicketCommandHandler(TrivistaDbContext trivistaDbContext)
    {
        _trivistaDbContext = trivistaDbContext;
    }
    
    public async Task<Result<Unit>> Handle(AddMessageToTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _trivistaDbContext.Ticket.FirstOrDefaultAsync(x => x.Id == request.TicketId, cancellationToken);
        if(ticket is null)
            return new Result<Unit>(ExceptionManager.Manage("Ticket", "Ticket does not exist"));
        
        var ticketMessage = ticket.AddMessageToTicket(request.Message, ticket, request.Initiator, request.InitiatorId);
        await _trivistaDbContext.TicketMessage.AddRangeAsync(ticketMessage.Messages, cancellationToken);
        var isTicketSaved = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        if (isTicketSaved > 0) return Unit.Value;
        return new Result<Unit>(ExceptionManager.Manage("Ticket", "Unable to log ticket"));
    }
}