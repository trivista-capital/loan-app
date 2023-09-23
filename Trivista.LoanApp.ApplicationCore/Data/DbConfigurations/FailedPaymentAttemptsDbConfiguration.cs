using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class FailedPaymentAttemptsDbConfiguration: IEntityTypeConfiguration<FailedPaymentAttempts>
{
    public void Configure(EntityTypeBuilder<FailedPaymentAttempts> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18, 2)");
        builder.Property(x => x.Status).IsRequired(true).HasColumnType("nvarchar");
    }
}