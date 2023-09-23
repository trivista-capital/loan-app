using Trivista.LoanApp.ApplicationCore.DomainEvents;
using Trivista.LoanApp.ApplicationCore.Interfaces;

namespace Trivista.LoanApp.ApplicationCore.DomainEvents;

public static class LoanEvents
{
    public class LoanEdited: IDomainEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    
    public class LoanDeleted: IDomainEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
    
    public class LoanCreated: IDomainEvent
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}