namespace Trivista.LoanApp.ApplicationCore.Entities;

public class Account: BaseEntity<Guid>
{
    public Account(Guid id)
    {
        Id = id;
    }
    
    public decimal Balance { get; set; }
    
    public Customer Customer { get; set; }

    public List<Transaction> Transactions { get; set; }
            = new List<Transaction>();

    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
}