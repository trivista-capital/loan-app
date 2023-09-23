namespace Trivista.LoanApp.ApplicationCore.Entities;

public sealed class ApprovalWorkflowApplicationRoleConfiguration: BaseEntity<int>
{
    public ApprovalWorkflowApplicationRoleConfiguration(Guid roleId, bool canOverrideAllApprovals, int hierarchy)
    {
        RoleId = roleId;
        CanOverrideAllApprovals = canOverrideAllApprovals;
        this.Created = DateTime.UtcNow;
        Hierarchy = hierarchy;
    }
    public Guid RoleId { get; private set; }
    
    public string RoleName { get; set; }
    public bool CanOverrideAllApprovals { get; set; }
    
    public int Hierarchy { get; private set; }
    public Guid ApprovalWorkflowConfigurationId { get; set; }
    public ApprovalWorkflowConfiguration ApprovalWorkflowConfiguration { get; set; }
    
    public ApprovalWorkflowApplicationRoleConfiguration SetRoleId(Guid roleId)
    {
        this.RoleId = roleId;
        return this;
    }
    
    public ApprovalWorkflowApplicationRoleConfiguration SetHierarchy(int hierarchy)
    {
        this.Hierarchy = hierarchy;
        return this;
    }
    
    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }
}