using Trivista.LoanApp.ApplicationCore.Commons.Enums;

namespace Trivista.LoanApp.ApplicationCore.Entities;

public sealed class FailedPaymentAttempts: BaseEntity<int>
{
    internal FailedPaymentAttempts(){ }

    public FailedPaymentAttempts(decimal amount, PaymentAttemptStatus paymentAttemptStatus, Guid repaymentScheduleId)
    {
        Amount = amount;
        Status = paymentAttemptStatus;
        RepaymentScheduleId = repaymentScheduleId;
        Created = DateTime.UtcNow;
    }
    
    public decimal Amount { get; private set; }
    
    public PaymentAttemptStatus Status { get; private set; }
    
    public Guid RepaymentScheduleId { get; private set; }
    
    public RepaymentSchedule RepaymentSchedule { get; set; }

    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
}