using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class LoanRequestDbConfiguration: IEntityTypeConfiguration<LoanRequest>
{
    public void Configure(EntityTypeBuilder<LoanRequest> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Bvn).IsRequired().HasMaxLength(20).HasColumnType("nvarchar(20)");
        builder.Property(x => x.DateLoanDisbursed).HasColumnType("datetime2(7)");
        builder.Property(x => x.DateLoanPaid).HasColumnType("datetime2(7)");
        builder.Property(x => x.Interest).HasColumnType("decimal(18, 2)");
        //Kyc details config
        builder.OwnsOne(x => x.kycDetails).Property(x=>x.CustomerFirstName).HasColumnType("nvarchar(100)");
        builder.OwnsOne(x => x.kycDetails).Property(x => x.CustomerMiddleName).HasColumnType("nvarchar(100)");
        builder.OwnsOne(x => x.kycDetails).Property(x => x.CustomerLastName).HasColumnType("nvarchar(100)");
        builder.OwnsOne(x => x.kycDetails).Property(x => x.CustomerAddress).HasColumnType("nvarchar(300)");
        builder.OwnsOne(x => x.kycDetails).Property(x => x.CustomerCity).HasColumnType("nvarchar(100)");
        builder.OwnsOne(x => x.kycDetails).Property(x => x.CustomerCountry).HasColumnType("nvarchar(50)");
        builder.OwnsOne(x => x.kycDetails).Property(x => x.CustomerEmail).HasColumnType("nvarchar(300)");
        builder.OwnsOne(x => x.kycDetails).Property(x => x.CustomerOccupation).HasColumnType("nvarchar(300)");
        builder.OwnsOne(x => x.kycDetails).Property(x => x.CustomerState).HasColumnType("nvarchar(100)");
        builder.OwnsOne(x => x.kycDetails).Property(x => x.CustomerPhoneNumber).HasColumnType("nvarchar(300)");
        builder.OwnsOne(x => x.kycDetails).Property(x => x.CustomerPostalCode).HasColumnType("nvarchar(100)");
        //Salary details config
        builder.OwnsOne(x => x.SalaryDetails).Property(x => x.SalaryAccountNumber).HasColumnType("nvarchar(12)");
        builder.OwnsOne(x => x.SalaryDetails).Property(x => x.AccountName).HasColumnType("nvarchar(70)");
        builder.OwnsOne(x => x.SalaryDetails).Property(x => x.BankName).HasColumnType("nvarchar(80)");
        builder.OwnsOne(x => x.SalaryDetails).Property(x => x.AverageMonthlyNetSalary).HasColumnType("decimal(18, 2)");
        builder.OwnsOne(x => x.SalaryDetails).Property(x => x.BankCode).HasMaxLength(8);
        //Proof of address
        builder.OwnsOne(x => x.ProofOfAddress).Property(x => x.ProofOFAddressFile).HasColumnType("nvarchar(max)");
        builder.OwnsOne(x => x.ProofOfAddress).Property(x => x.ProofOFAddressFileLength).HasColumnType("int");
        builder.OwnsOne(x => x.ProofOfAddress).Property(x => x.ProofOFAddressFileName).HasColumnType("nvarchar(500)");
        builder.OwnsOne(x => x.ProofOfAddress).Property(x => x.ProofOFAddressFileType).HasColumnType("nvarchar(100)");
        //Loan Details
        builder.OwnsOne(x => x.LoanDetails).Property(x => x.LoanAmount).HasColumnType("decimal(18, 2)");
        builder.OwnsOne(x => x.LoanDetails).Property(x => x.purpose).HasColumnType("nvarchar(400)");
        builder.OwnsOne(x => x.LoanDetails).Property(x => x.tenure).HasColumnType("int");
        builder.OwnsOne(x => x.LoanDetails).Property(x => x.LoanBalance).HasColumnType("decimal(18, 2)");
        builder.OwnsOne(x => x.LoanDetails).Property(x => x.RepaymentScheduleType).HasColumnType("decimal(18, 2)");

        builder.HasOne(x => x.ApprovalWorkflow).WithOne()
            .HasForeignKey<LoanRequest>(x=>x.ApprovalWorkflowId).OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.RepaymentSchedules).WithOne(x => x.LoanRequest).HasForeignKey(x => x.LoanRequestId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}