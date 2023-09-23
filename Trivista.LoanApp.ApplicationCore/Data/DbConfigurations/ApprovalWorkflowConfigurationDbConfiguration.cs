using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class ApprovalWorkflowConfigurationDbConfiguration: IEntityTypeConfiguration<ApprovalWorkflowConfiguration>
{
    public void Configure(EntityTypeBuilder<ApprovalWorkflowConfiguration> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Action).IsRequired().HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.HasMany(x => x.Roles)
            .WithOne(x => x.ApprovalWorkflowConfiguration)
            .HasForeignKey(x => x.ApprovalWorkflowConfigurationId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}