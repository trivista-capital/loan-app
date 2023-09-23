using Trivista.LoanApp.ApplicationCore.Interfaces;

namespace Trivista.LoanApp.ApplicationCore.DomainEvents;

public static class CustomerEvents
{
    public class CustomerCreatedEvent: IDomainEvent
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
    }
}