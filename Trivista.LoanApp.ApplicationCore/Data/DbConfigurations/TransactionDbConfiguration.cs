using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class TransactionDbConfiguration: IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.TransactionReference).IsRequired().HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18, 2)");
        builder.Property(x => x.Payload).IsRequired().HasColumnType("nvarchar(max)");
        builder.Property(x => x.Status).IsRequired().HasColumnType("int");
        builder.Property(x => x.IsSuccessful).HasColumnType("bit").HasDefaultValue(false);
        builder.Property(x => x.Created).HasColumnType("datetime2");
        builder.Property(x => x.Deleted).HasColumnType("datetime2");
        builder.Property(x => x.DeletedBy).HasColumnType("uniqueidentifier");
        builder.Property(x => x.TransactionType).IsRequired(true).HasConversion<string>();
        builder.Property(x => x.RepaymentScheduleId).IsRequired(false).HasColumnType("uniqueidentifier");
        builder.Property(x => x.LoanRequestId).IsRequired(true).HasColumnType("uniqueidentifier");
    }
}