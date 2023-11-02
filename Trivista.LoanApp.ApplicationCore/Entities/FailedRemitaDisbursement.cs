namespace Trivista.LoanApp.ApplicationCore.Entities;

public class FailedRemitaDisbursement : BaseEntity<long>
{
    internal FailedRemitaDisbursement() { }

    public FailedRemitaDisbursement(Guid loanRequestId, string payLoad)
    {
        LoanRequestId = loanRequestId;
        Payload = payLoad;
        Created = DateTime.UtcNow;
    }

    public Guid LoanRequestId { get; set; }

    public bool IsReProcessed { get; set; }

    public bool IsSuccessful { get; set; }

    public string Payload { get; set; }

    public sealed class Factory
    {
        public static FailedRemitaDisbursement Build(Guid loanRequestId, string payLoad)
        {
            return new FailedRemitaDisbursement(loanRequestId, payLoad);
        }
    }


    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
}