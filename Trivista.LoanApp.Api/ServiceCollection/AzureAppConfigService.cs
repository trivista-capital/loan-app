using Azure.Identity;
using Microsoft.Extensions.Options;
using Trivista.LoanApp.ApplicationCore.Commons.Options;

namespace Trivista.LoanApp.Api.ServiceCollection;

public static class AzureAppConfigService
{
    public static WebApplicationBuilder BootstrapAppConfig(this WebApplicationBuilder builder)
    {
        //builder.Services.Configure<AzureAppConfigOption>(builder.Configuration.GetSection("AzureAppConfigOption"));
        if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging() || builder.Environment.IsProduction())
        {
            //var serviceProvider = builder.Services.BuildServiceProvider().GetRequiredService<IOptions<AzureAppConfigOption>>();
            var azureAppConfigConnectionString = "Endpoint=https://trivistadevappconfig.azconfig.io;Id=ukDQ;Secret=l6jDWnNyTci++m0veFECfdfY6Nelo9htsZERtzhpuKE="; //serviceProvider.Value.ConnectionString;
            builder.Configuration.AddAzureAppConfiguration(options =>
            {
                options.Connect(azureAppConfigConnectionString).ConfigureRefresh((refreshOptions) =>
                {
                    // indicates that all configuration should be refreshed when the given key has changed.
                    refreshOptions.Register(key: "sentinel", refreshAll: true);
                    refreshOptions.SetCacheExpiration(TimeSpan.FromSeconds(5));
                }).UseFeatureFlags();
            });
        }

        return builder;
    }
}