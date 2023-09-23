using Microsoft.Extensions.Options;
using Trivista.LoanApp.ApplicationCore.Commons.Options;

namespace Trivista.LoanApp.Api.ServiceCollection;

public static class ApplicationInsightService
{
    public static void BootstrapAppInsight(this WebApplicationBuilder builder, IServiceCollection services)
    {
        if (builder.Environment.IsDevelopment() || builder.Environment.IsStaging() || builder.Environment.IsProduction())
        {
            var serviceProvider = services.BuildServiceProvider().GetService<IOptions<AzureAppInsightOption>>(); 
            var azureAppInsightConnectionString = serviceProvider.Value.ConnectionString;
            builder.Services.AddApplicationInsightsTelemetry(options =>
            {
                options.ConnectionString = azureAppInsightConnectionString;
            });
        }
    }
}