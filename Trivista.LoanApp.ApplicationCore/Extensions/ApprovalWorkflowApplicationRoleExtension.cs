using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Extensions;

public static class ApprovalWorkflowApplicationRoleExtension
{
    public static bool IsLastApproval(this List<ApprovalWorkflowApplicationRole> approvalWorkflowApplicationRoles, Guid workflowId)
    {
        var numberOfApprovals = approvalWorkflowApplicationRoles.Count(x => x.IsApproved);
        var totalNumberOfApprovals = approvalWorkflowApplicationRoles.Count(x => x.ApprovalWorkflowId == workflowId);
        return ((totalNumberOfApprovals - 1).Equals(numberOfApprovals)) ? true : false;
    }
}