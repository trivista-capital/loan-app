using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trivista.LoanApp.ApplicationCore.Entities;

namespace Trivista.LoanApp.ApplicationCore.Data.DbConfigurations;

public class CLientApiKeyConfigurationDbConfiguration : IEntityTypeConfiguration<ClientApiKeyConfiguration>
{
    public void Configure(EntityTypeBuilder<ClientApiKeyConfiguration> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Email).HasColumnType("nvarchar(400)").IsRequired(true);
        builder.Property(x => x.ApiKey).HasColumnType("nvarchar(3000)").IsRequired(true);
        builder.Property(x => x.Created).HasColumnType("datetime2");
    }
}
