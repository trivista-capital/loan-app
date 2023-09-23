using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class RoleConfiguration: IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        var roleId = Guid.Parse("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d");
        var role = ApplicationRole.Factory.Create(roleId, "SuperAdmin", "Default super admin role");

        builder.HasData(role);

        builder.Property(x => x.Name).HasColumnType("nvarchar(200)").IsRequired();
        builder.Property(x => x.Description).HasColumnType("nvarchar(400)");
    }
}