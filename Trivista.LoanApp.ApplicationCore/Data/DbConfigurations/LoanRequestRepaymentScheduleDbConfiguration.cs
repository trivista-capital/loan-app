using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class LoanRequestRepaymentScheduleDbConfiguration: IEntityTypeConfiguration<RepaymentSchedule>
{
    public void Configure(EntityTypeBuilder<RepaymentSchedule> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Amount).IsRequired().HasColumnType("decimal(18, 2)");
        builder.Property(x => x.RepaymentAmount).IsRequired().HasColumnType("decimal(18, 2)");
        builder.Property(x => x.LoanBalance).HasColumnType("decimal(18, 2)");
        builder.Property(x => x.RepaymentType).HasConversion<string>().IsRequired(true);
        builder.Property(x => x.Status).HasConversion<string>().IsRequired(true);
        builder.Property(x => x.DueDate).IsRequired(true).HasColumnType("DateTime2");
        builder.Property(x => x.PaymentType).IsRequired(true).HasConversion<string>();
        builder.HasMany(x => x.FailedPaymentAttempts).WithOne(x => x.RepaymentSchedule)
            .HasForeignKey(x => x.RepaymentScheduleId).OnDelete(DeleteBehavior.Cascade);
        // builder.HasOne<Transaction>(x => x.Transaction)
        //     .WithOne(x => x.RepaymentSchedule)
        //     .HasForeignKey<Transaction>(x=>x.RepaymentScheduleId)
        //     .OnDelete(DeleteBehavior.Cascade);
    }
}