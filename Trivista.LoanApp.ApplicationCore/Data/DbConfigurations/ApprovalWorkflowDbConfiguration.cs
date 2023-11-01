using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class ApprovalWorkflowDbConfiguration : IEntityTypeConfiguration<ApprovalWorkflow>
{
    public void Configure(EntityTypeBuilder<ApprovalWorkflow> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ApprovedBy).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.RejectedBy).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.HasMany(x => x.ApprovalWorkflowApplicationRole)
            .WithOne(x => x.ApprovalWorkflow)
            .HasForeignKey(x => x.ApprovalWorkflowId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}