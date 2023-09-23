using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class TicketMessagesDbConfiguration: IEntityTypeConfiguration<TicketMessages>
{
    public void Configure(EntityTypeBuilder<TicketMessages> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Message).IsRequired(true).HasColumnType("nvarchar(max)");
        builder.Property(x => x.InitiatorId);
        builder.Property(x => x.Initiator).HasConversion<string>();
    }
}