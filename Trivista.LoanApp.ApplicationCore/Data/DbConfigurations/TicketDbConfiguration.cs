using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class TicketDbConfiguration: IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).IsRequired(false).HasColumnType("nvarchar(200)");
        builder.Property(x => x.Status).HasConversion<string>();
        builder.Property(x => x.Created).HasColumnType("datetime2");
        builder.Property(x => x.Deleted).HasColumnType("datetime2");
        builder.Property(x => x.DeletedBy).HasColumnType("uniqueidentifier");
        builder.HasMany(x => x.Messages).WithOne(x => x.Ticket)
            .HasForeignKey(x => x.TicketId).OnDelete(DeleteBehavior.Cascade);
    }
}