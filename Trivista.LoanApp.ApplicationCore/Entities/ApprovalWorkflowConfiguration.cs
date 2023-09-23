namespace Trivista.LoanApp.ApplicationCore.Entities;

public sealed class ApprovalWorkflowConfiguration : BaseEntity<Guid>
{
    internal ApprovalWorkflowConfiguration() { }

    private ApprovalWorkflowConfiguration(string action, List<ApprovalWorkflowApplicationRoleConfiguration> workFlowRoles)
    {
        Action = action;
        Roles = workFlowRoles;
        DateCreated = DateTime.UtcNow;
        Created = DateTime.UtcNow;
    }

    public string Action { get; private set; }

    public DateTime DateCreated { get; private set; }

    public List<ApprovalWorkflowApplicationRoleConfiguration> Roles { get; set; }
        = new List<ApprovalWorkflowApplicationRoleConfiguration>();

    protected override void When(object @event)
    {
        throw new NotImplementedException();
    }

    public class Factory
    {
        public static ApprovalWorkflowConfiguration Build(string action,
            List<ApprovalWorkflowApplicationRoleConfiguration> workFlowRoles)
        {
            return new ApprovalWorkflowConfiguration(action, workFlowRoles);
        }
    }
}