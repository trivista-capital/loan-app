using Trivista.LoanApp.ApplicationCore.Commons.Enums;

namespace Trivista.LoanApp.ApplicationCore.Entities;

public class TicketMessages: BaseEntity<Guid>
{
    internal TicketMessages() { }
    private TicketMessages(Guid id, Ticket ticket, TicketInitiator initiator,  string message, Guid initiatorId)
    {
        Id = id;
        Ticket = ticket;
        Message = message;
        Created = DateTime.UtcNow;
        Initiator = initiator;
        InitiatorId = initiatorId;
    }
    
    public Guid TicketId { get; private set; }
    public Ticket Ticket { get; private set; }
    public string Message { get; private set; }
    public TicketInitiator Initiator { get; set; }
    public Guid InitiatorId { get; set; }

    public class Factory
    {
        public static TicketMessages Build(Guid id, Ticket ticket, TicketInitiator initiator, string message, Guid initiatorId)
        {
            return new TicketMessages(id, ticket, initiator, message, initiatorId);
        }
    }
    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
}