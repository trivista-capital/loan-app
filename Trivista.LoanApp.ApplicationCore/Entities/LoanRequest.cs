using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Trivista.LoanApp.ApplicationCore.Commons.Enums;
using Trivista.LoanApp.ApplicationCore.Entities.ValueObjects;

namespace Trivista.LoanApp.ApplicationCore.Entities;

public sealed class LoanRequest: BaseEntity<Guid>
{
    internal LoanRequest() { }
    
    private LoanRequest(Guid id, string bvn, Customer customer, ApprovalWorkflow approvalWorkflow, SalaryDetails salaryDetails, 
        LoanDetails loanDetails, kycDetails kycDetails, ProofOfAddress proofOfAddress,  decimal interestRate)
    {
        Id = id;
        Bvn = bvn;
        LoanApplicationStatus = LoanApplicationStatus.Pending;
        DisbursedLoanStatus = DisbursedLoanStatus.None;
        Customer = customer;
        ApprovalWorkflow = approvalWorkflow;
        SalaryDetails = salaryDetails;
        LoanDetails = loanDetails;
        this.kycDetails = kycDetails;
        Created = DateTime.UtcNow;
        ProofOfAddress = proofOfAddress;
    }
    public string Bvn { get; private set; }
    public LoanApplicationStatus LoanApplicationStatus { get; private set; }
    public DateTime? DateLoanPaid { get; set; }
    public DisbursedLoanStatus DisbursedLoanStatus { get; private set; }
    public DateTime? DateLoanDisbursed { get; set; }
    public kycDetails kycDetails { get; private set; }
    public SalaryDetails SalaryDetails { get; private set; }
    public ProofOfAddress ProofOfAddress { get; private set; }
    public LoanDetails LoanDetails { get; private set; } = new LoanDetails();
    public decimal Interest { get; private set; }
    public Guid CustomerId { get; set; }
    public Customer Customer { get; private set; }
    public Guid ApprovalWorkflowId { get; set; }
    public ApprovalWorkflow ApprovalWorkflow { get; set; }
    public ICollection<RepaymentSchedule> RepaymentSchedules { get; set; } = new List<RepaymentSchedule>();

    private List<object> Events { get; set; }

    public LoanRequest OverrideDefaultInterestRate(Loan loan, decimal newRate)
    {
        this.Interest = loan.InterestRate;
        return this;
    }
    
    public LoanRequest SetProofOfAddress(ProofOfAddress proofOfAddress)
    {
        this.ProofOfAddress = proofOfAddress;
        return this;
    }
    
    
    public LoanRequest SetRejectionDateLoan()
    {
        this.LoanApplicationStatus = LoanApplicationStatus.Rejected;
        return this;
    }

    public LoanRequest RejectLoan(string rejectedBy)
    {
        this.ApprovalWorkflow.RejectLoan(rejectedBy);
        return this;
    }
    
    public LoanRequest SetInterestRate(decimal interest)
    {
        this.Interest = interest;
        return this;
    }
    
    public LoanRequest SetLoanStatus(decimal loanBalance)
    {
        if (loanBalance < 1)
        {
            this.LoanApplicationStatus = LoanApplicationStatus.Paid;  
            this.DateLoanPaid = DateTime.UtcNow;
        }
        return this;
    }
    
    public LoanRequest SetLoanDisbursedStatus()
    {
        this.DisbursedLoanStatus = DisbursedLoanStatus.Disbursed;  
        this.DateLoanDisbursed = DateTime.UtcNow;
        return this;
    }
    
    public LoanRequest ChangeLoanAmount(decimal amount)
    {
        this.LoanDetails.LoanAmount = amount;
        return this;
    }
    
    public LoanRequest SetLoanBalance(decimal balance)
    {
        LoanDetails.LoanBalance = balance;
        return this;
    }
    
    public LoanRequest ApproveLoan()
    {
        this.LoanApplicationStatus = LoanApplicationStatus.Approved;
        return this;
    }
    
    public LoanRequest ApproveLoanByCustomer()
    {
        this.LoanApplicationStatus = LoanApplicationStatus.Active;
        return this;
    }

    public class Factory
    {
        public static LoanRequest Build(Guid id, string bvn, Customer customer, ApprovalWorkflow approvalWorkflow,
            SalaryDetails salaryDetails, LoanDetails loanDetails, kycDetails kycDetails, ProofOfAddress proofOfAddress, decimal interestRate)
        {
            return new LoanRequest(id, bvn, customer, approvalWorkflow, salaryDetails, loanDetails, kycDetails, proofOfAddress, interestRate);
        }
    }
    protected override void When(object @event)
    {
        switch (@event)
        {
            
        }
    }
}