using Trivista.LoanApp.ApplicationCore.Commons.Enums;

namespace Trivista.LoanApp.ApplicationCore.Entities;

public sealed class ApprovalWorkflow: BaseEntity<Guid>
{
    internal ApprovalWorkflow() { }
    
    private ApprovalWorkflow(ApprovalWorkflowConfiguration approvalWorkflowConfiguration)
    {
        ApprovalWorkflowConfiguration = approvalWorkflowConfiguration;
        Created = DateTime.UtcNow;
    }
    
    public bool IsApproved { get; set; }
    public DateTime DateApproved { get; set; }
    public DateTime DateRejected { get; set; }

    public string ApprovedBy { get; set; }

    public string RejectedBy { get; set; }

    public ApprovalWorkflowConfiguration ApprovalWorkflowConfiguration { get; set; }
    public ICollection<ApprovalWorkflowApplicationRole> ApprovalWorkflowApplicationRole { get; set; }
        = new List<ApprovalWorkflowApplicationRole>();

    public ApprovalWorkflow SetApprovalWorkflowApplicationRole(
        ApprovalWorkflowApplicationRole approvalWorkflowApplicationRole)
    {
        ApprovalWorkflowApplicationRole.Add(approvalWorkflowApplicationRole);
        return this;
    }
    
    public ApprovalWorkflow SetApproval(List<ApprovalWorkflowApplicationRole> roles, ApprovalWorkflowApplicationRole role, string approvedBy, LoanRequest loanRequest)
    {
        var isLast = Trivista.LoanApp.ApplicationCore.Entities.ApprovalWorkflowApplicationRole.IsLastApproval(roles, this.Id);
        if (isLast)
        {
            role.Approve(approvedBy);
            IsApproved = true;
            DateApproved = DateTime.UtcNow;
            loanRequest.ApproveLoan();
        }
        else
        {
            role.Approve(approvedBy);
        }
        return this;
    }
    
    public ApprovalWorkflow SetApprovalIfLastApprover(List<ApprovalWorkflowApplicationRole> roles, ApprovalWorkflowApplicationRole role, string approvedBy, LoanRequest loanRequest)
    {
        role.Approve(approvedBy);
        IsApproved = true;
        DateApproved = DateTime.UtcNow;
        loanRequest.ApproveLoan();
        return this;
    }

    public ApprovalWorkflow RejectLoan(string rejectedBy)
    {
        this.DateRejected = DateTime.UtcNow;
        this.IsApproved = false;
        this.RejectedBy = rejectedBy;
        return this;
    }
    
    public ApprovalWorkflow Approve()
    {
        this.DateRejected = DateTime.UtcNow;
        this.IsApproved = false;
        return this;
    }

    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
    
    public class Factory
    {
        public static ApprovalWorkflow Build(ApprovalWorkflowConfiguration approvalWorkflowConfiguration)
        {
            return new ApprovalWorkflow(approvalWorkflowConfiguration);
        }
    }
}