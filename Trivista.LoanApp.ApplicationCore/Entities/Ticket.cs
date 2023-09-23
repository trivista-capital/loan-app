using Trivista.LoanApp.ApplicationCore.Commons.Enums;

namespace Trivista.LoanApp.ApplicationCore.Entities;

public class Ticket: BaseEntity<Guid>
{
    internal Ticket() { }
    private Ticket(Guid id, Customer customer, string email)
    {
        Id = id;
        
        Customer = customer;

        Email = email;
        
        Created = DateTime.UtcNow;

        Status = TicketStatus.Open;
        
        Created = DateTime.UtcNow;
        
        
    }
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; }
    public string Email { get; set; }
    public TicketStatus Status { get; private set; }
    public List<TicketMessages> Messages { get; set; } = new List<TicketMessages>();

    public Ticket AddMessageToTicket(string message, Ticket ticket, TicketInitiator initiator, Guid initiatorId)
    {
        Messages.Add(TicketMessages.Factory.Build(Guid.NewGuid(), ticket, initiator, message, initiatorId));
        return this;
    }
    
    public Ticket CloseTicket()
    {
        this.Status = TicketStatus.Closed;
        return this;
    }
    
    public Ticket OpenTicket()
    {
        this.Status = TicketStatus.Open;
        return this;
    }

    public class Factory
    {
        public static Ticket Build(Guid id, Customer customer, string email)
        {
            return new Ticket(id, customer, email);
        }
    }
    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
}