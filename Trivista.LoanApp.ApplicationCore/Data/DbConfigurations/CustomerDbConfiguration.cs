using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class CustomerDbConfiguration: IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        var userId = Guid.Parse("363b37a0-c306-4472-a405-4b576334cca0");
        var roleId = Guid.Parse("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d");

        var customer = Entities.Customer.Factory.Build(userId, "Babafemi", "Ibitolu", "femi.ibitolu@gmail.com", "",
                                                       "Male", "", roleId.ToString(), "Staff");

        builder.HasData(customer);
        
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FirstName).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.MiddleName).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.LastName).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.Email).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.PhoneNumber).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.Sex).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.Dob).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.Bvn).IsRequired(false).HasMaxLength(20).HasColumnType("nvarchar(20)");
        builder.Property(x => x.Occupation).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.Address).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.Country).IsRequired(false).HasMaxLength(100).HasColumnType("nvarchar(100)");
        builder.Property(x => x.State).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.City).IsRequired(false).HasMaxLength(200).HasColumnType("nvarchar(200)");
        builder.Property(x => x.PostCode).IsRequired(false).HasMaxLength(20).HasColumnType("nvarchar(20)");
        builder.Property(x => x.RoleId).IsRequired(false).HasColumnType("nvarchar(200)");
        builder.Property(x => x.UserType).IsRequired(false).HasColumnType("nvarchar(200)");
        builder.Property(x => x.MbsRequestStatementResponseCode).HasColumnType("int");
        builder.Property(x => x.MbsBankStatement).IsRequired(false).HasColumnType("nvarchar(max)");
        builder.Property(x => x.BankStatementAnalysis).IsRequired(false).HasColumnType("nvarchar(max)");
        //Customer Remitta Information
        builder.OwnsOne(x => x.CustomerRemitterInformation).Property(x => x.IsRemittaUser).HasColumnType("bit");
        builder.OwnsOne(x => x.CustomerRemitterInformation).Property(x => x.AverageSixMonthsSalary).HasColumnType("decimal(18, 2)");
        builder.OwnsOne(x => x.CustomerRemitterInformation).Property(x => x.OtherLoansCollected).HasColumnType("decimal(18, 2)");
        //Proof of address
        builder.OwnsOne(x => x.ProfilePicture).Property(x => x.ProfilePictureFile).HasColumnType("nvarchar(max)");
        builder.OwnsOne(x => x.ProfilePicture).Property(x => x.ProfilePictureFileLength).HasColumnType("int");
        builder.OwnsOne(x => x.ProfilePicture).Property(x => x.ProfilePictureFileName).HasColumnType("nvarchar(500)");
        builder.OwnsOne(x => x.ProfilePicture).Property(x => x.ProfilePictureFileType).HasColumnType("nvarchar(100)");
        
        //builder.OwnsOne(x => x.CustomerRemitterInformation, builder => { builder.ToJson();});
        
        //Relationships
        builder.HasMany(x => x.LoanRequests).WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasMany(x => x.Tickets).WithOne(x => x.Customer)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}