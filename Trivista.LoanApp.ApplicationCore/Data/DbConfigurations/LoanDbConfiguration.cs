using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class LoanDbConfiguration: IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired().HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.InterestRate).IsRequired().HasColumnType("decimal(18, 2)");
        builder.Property(x => x.MinimumSalary).IsRequired().HasColumnType("decimal(18, 2)");
        builder.Property(x => x.MaximumTenure).IsRequired().HasColumnType("int");
        builder.Property(x => x.MaximumLoanAmount).IsRequired().HasColumnType("decimal(18, 2)");
        builder.Property(x => x.LastModifiedBy).HasColumnType("uniqueidentifier");
        builder.Property(x => x.Created).HasColumnType("datetime2");
        builder.Property(x => x.Deleted).HasColumnType("datetime2");
        builder.Property(x => x.IsDeleted).HasColumnType("bit").HasDefaultValue(false);
        builder.Property(x => x.DeletedBy).HasColumnType("uniqueidentifier");
    }
}