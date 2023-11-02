using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public partial class CustomerDbConfiguration
{
    public class FailedRemitaDisbursementDbConfiguration : IEntityTypeConfiguration<FailedRemitaDisbursement>
    {
        public void Configure(EntityTypeBuilder<FailedRemitaDisbursement> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.LoanRequestId);
            builder.Property(x => x.IsReProcessed);
            builder.Property(x => x.IsSuccessful);
            builder.Property(x => x.Payload).IsRequired().HasColumnType("nvarchar(max)");
        }
    }
}