using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class ApprovalWorkflowApplicationRoleConfigurationDbConfiguration: IEntityTypeConfiguration<ApprovalWorkflowApplicationRoleConfiguration>
{
    public void Configure(EntityTypeBuilder<ApprovalWorkflowApplicationRoleConfiguration> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.RoleId).HasColumnType("uniqueidentifier");
        builder.Ignore(x => x.RoleName);
        builder.Property(x => x.Created).HasColumnType("datetime2");
        builder.HasOne(x => x.ApprovalWorkflowConfiguration).WithMany(x => x.Roles)
            .HasForeignKey(x => x.ApprovalWorkflowConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
