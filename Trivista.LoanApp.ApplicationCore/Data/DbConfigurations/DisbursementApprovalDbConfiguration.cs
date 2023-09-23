using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class DisbursementApprovalDbConfiguration: IEntityTypeConfiguration<DisbursementApproval>
{
    public void Configure(EntityTypeBuilder<DisbursementApproval> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Otp).IsRequired(false).HasColumnType("nvarchar(10)");
        builder.Property(x => x.TransferCode).IsRequired(false).HasColumnType("nvarchar(200)");
        builder.Property(x => x.TransactionReference).IsRequired(false).HasColumnType("nvarchar(50)");
        builder.Property(x => x.IsSuccessful).HasColumnType("bit");
        builder.Property(x => x.Status).HasConversion<string>();
        builder.Property(x => x.Created).HasColumnType("datetime2");
        builder.Property(x => x.Deleted).HasColumnType("datetime2");
        builder.Property(x => x.DeletedBy).HasColumnType("uniqueidentifier");
    }
}