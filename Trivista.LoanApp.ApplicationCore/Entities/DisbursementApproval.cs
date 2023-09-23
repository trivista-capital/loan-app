using Trivista.LoanApp.ApplicationCore.Commons.Enums;

namespace Trivista.LoanApp.ApplicationCore.Entities;

public class DisbursementApproval: BaseEntity<Guid>
{
    internal DisbursementApproval() { }
    private DisbursementApproval(Guid id, LoanRequest loanRequest, string otp, string transferCode, string transactionReference)
    {
        Id = id;
        LoanRequest = loanRequest;
        Otp = otp;
        TransferCode = transferCode;
        Created = DateTime.UtcNow;
        IsSuccessful = false;
        Status = DisbursedLoanStatus.None;
        TransactionReference = transactionReference;
    }
    
    public Guid LoanRequestId { get; private set; }
    public LoanRequest LoanRequest { get; private set; }
    public string Otp { get; private set; }
    public string  TransferCode { get; private set; }
    public string  TransactionReference { get; private set; }
    public bool IsSuccessful { get; private set; }
    public DisbursedLoanStatus Status { get; private set; }

    public class Factory
    {
        public static DisbursementApproval Build(Guid id, LoanRequest loanRequest, string otp, string transferCode, string transactionReference)
        {
            return new DisbursementApproval(id, loanRequest, otp, transferCode, transactionReference);
        }
    }
    
    public DisbursementApproval SetOtp(string otp)
    {
        Otp = otp;
        return this;
    }

    public DisbursementApproval ApproveLoan()
    {
        IsSuccessful = true;
        Status = DisbursedLoanStatus.Disbursed;
        return this;
    }
    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
}