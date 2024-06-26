namespace Trivista.LoanApp.ApplicationCore.Entities;

public sealed class ApprovalWorkflowApplicationRole: BaseEntity<Guid>
{
    internal ApprovalWorkflowApplicationRole() { }

    private ApprovalWorkflowApplicationRole(Guid roleId, string approvedBy, Guid approvalWorkflowId, int hierarchy)
    {
        RoleId = roleId;
        ApprovedBy = approvedBy;
        ApprovalWorkflowId = approvalWorkflowId;
        DateApproved = DateTime.UtcNow;
        IsApproved = false;
        Hierarchy = hierarchy;
        Created = DateTime.UtcNow;
    }
    
    private ApprovalWorkflowApplicationRole(Guid roleId, string rejectedBy, Guid approvalWorkflowId, bool rejection, int hierarchy)
    {
        RoleId = roleId;
        RejectedBy = rejectedBy;
        ApprovalWorkflowId = approvalWorkflowId;
        DateRejected = DateTime.UtcNow;
        IsApproved = false;
        Hierarchy = hierarchy;
        Created = DateTime.UtcNow;
    }
    public Guid RoleId { get; private set; }
    public bool IsApproved { get; private set; }
    public string? ApprovedBy { get; private set; } = default!;
    public string? RejectedBy { get; private set; } = default!;
    public DateTime DateApproved { get; private set; }
    public DateTime DateRejected { get; private set; }
    public Guid ApprovalWorkflowId { get; private set; }
    public int Hierarchy { get; private set; }
    
    public ApprovalWorkflow ApprovalWorkflow { get; private set; }
    
    public class Factory
    {
        public static ApprovalWorkflowApplicationRole Build()
        {
            return new ApprovalWorkflowApplicationRole();
        }
        
        public static ApprovalWorkflowApplicationRole Build(Guid roleId, string approvedBy, Guid approvalWorkflowId, int hierarchy)
        {
            return new ApprovalWorkflowApplicationRole(roleId, approvedBy, approvalWorkflowId, hierarchy);
        }
    }
    
    public ApprovalWorkflowApplicationRole Approve(string approvedBy)
    {
        this.ApprovedBy = approvedBy;
        this.DateApproved = DateTime.UtcNow;
        this.IsApproved = true;
        return this;
    }
    
    
    
    public ApprovalWorkflowApplicationRole Reject(string rejectedBy)
    {
        this.RejectedBy = rejectedBy;
        this.DateRejected = DateTime.UtcNow;
        return this;
    }
    
    public static bool IsLastApproval(List<ApprovalWorkflowApplicationRole> approvalWorkflowApplicationRoles, Guid workflowId)
    {
        var numberOfApprovedRequests = approvalWorkflowApplicationRoles.Count(x => x.IsApproved);
        var totalNumberOfApprovals = approvalWorkflowApplicationRoles.Count(x => x.ApprovalWorkflowId == workflowId);
        return ((totalNumberOfApprovals - 1).Equals(numberOfApprovedRequests)) ? true : false;
    }
    
    public ApprovalWorkflowApplicationRole SetRejectedBy(string rejectedBy)
    {
        this.RejectedBy = rejectedBy;
        return this;
    }

    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
}