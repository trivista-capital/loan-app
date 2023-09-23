using Carter;
using FluentValidation;
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
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Exceptions;
using Trivista.LoanApp.ApplicationCore.Extensions;

namespace Trivista.LoanApp.ApplicationCore.Features.TicketManagement;

public class LogTicketController: ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/logTicket", LogTicketHandler)
            .WithName("Log ticket")
            .WithTags("Ticket Management");
            //.RequireAuthorization();
    }

    private static async Task<IResult> LogTicketHandler(IMediator mediator, [FromBody] AddTicketDto ticket)
    {
        var result = await mediator.Send(new AddTicketCommand(ticket));
        return result.ToOk(x => x);
    }
}

public sealed class AddTicketDto
{
    public Guid CustomerId { get; set; }
    public string CustomerEmail { get; set; }
}

public class AddTicketCommandValidation : AbstractValidator<AddTicketCommand>
{
    public AddTicketCommandValidation()
    {
        RuleFor(x => x.Ticket.CustomerId).Must(BeAValidGuid).WithMessage("CustomerId must be set");
        RuleFor(x => x.Ticket.CustomerEmail).NotEqual("string").NotNull().NotEmpty().WithMessage("Email must be set");
    }
    private bool BeAValidGuid(Guid id)
    {
        if (id.Equals(default))
            return false;
        return true;
    }
}

public sealed record AddTicketCommand(AddTicketDto Ticket) : IRequest<Result<Unit>>;

public sealed class AddTicketCommandHandler : IRequestHandler<AddTicketCommand, Result<Unit>>
{
    private readonly TrivistaDbContext _trivistaDbContext;

    private readonly IPublisher _publisher;
    
    public AddTicketCommandHandler(TrivistaDbContext trivistaDbContext, IPublisher publisher)
    {
        _trivistaDbContext = trivistaDbContext;
        _publisher = publisher;
    }
    
    public async Task<Result<Unit>> Handle(AddTicketCommand request, CancellationToken cancellationToken)
    {
        var ticketId = Guid.NewGuid();
        var customer = await _trivistaDbContext.Customer.FirstOrDefaultAsync(x => x.Id == request.Ticket.CustomerId, cancellationToken);
        
        if(customer is null)
            return new Result<Unit>(ExceptionManager.Manage("Ticket", "Customer is not valid"));
        
        var ticket = Ticket.Factory.Build(ticketId, customer, request.Ticket.CustomerEmail);
        await _trivistaDbContext.Ticket.AddAsync(ticket, cancellationToken);
        var isTicketSaved = await _trivistaDbContext.SaveChangesAsync(cancellationToken);
        
        if (isTicketSaved < 1) return new Result<Unit>(ExceptionManager.Manage("Ticket", "Unable to log ticket"));
        
        _publisher.Publish(new NewTicketRaisedEvent()
        {
            To = customer.Email,
            AdminName = "",
            TicketId = Guid.NewGuid(),
            CustomerName = $"{customer.FirstName} {customer.LastName}",
            DateAndTimeOfTicket = DateTime.UtcNow,
            IssueCategory = "",
            DescriptionOfIssue = ""
        });
        return Unit.Value;
    }
}