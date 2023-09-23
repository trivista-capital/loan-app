using Trivista.LoanApp.ApplicationCore.Commons.Enums;

namespace Trivista.LoanApp.ApplicationCore.Entities;

public class Transaction: BaseEntity<Guid>
{
    internal Transaction() { }
    private Transaction(Guid id, string transactionReference, decimal amount, string payload, RepaymentStatus status, 
        bool isSuccessful, TransactionType transactionType, Guid loanRequestId)
    {
        Id = id;
        TransactionReference = transactionReference;
        Amount = amount;
        Payload = payload;
        Status = status;
        IsSuccessful = isSuccessful;
        Created = DateTime.UtcNow;
        TransactionType = transactionType;
        LoanRequestId = loanRequestId;
    }
    
    public string TransactionReference { get; set; }
    public decimal Amount { get; private set; }
    public string Payload { get; private set; }
    public RepaymentStatus Status { get; private set; }
    public bool IsSuccessful { get; private set; }
    public TransactionType TransactionType { get; private set; }
    public Guid? RepaymentScheduleId { get; private set; }
    
    public Guid? LoanRequestId { get; private set; }
    public Guid CustomerId { get; private set; }
    public Customer Customer { get; private set; }
    
    public class Factory
    {
        public static Transaction Build(Guid id, string transactionReference, decimal amount, string payload,
            RepaymentStatus status,
            bool isSuccessful, TransactionType transactionType, Guid loanRequestId)
        {
            return new Transaction( id, transactionReference, amount, payload, status, 
                 isSuccessful, transactionType, loanRequestId);
        }
    }

    public Transaction SetReschedulePayment(RepaymentSchedule reschedule)
    {
        this.RepaymentScheduleId = reschedule.Id;
        return this;
    }
    
    public Transaction SetCustomer(Customer customer)
    {
        this.Customer = customer;
        return this;
    }
    
    public Transaction SetDisbursement(Customer customer)
    {
        this.Customer = customer;
        return this;
    }
    
    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
}