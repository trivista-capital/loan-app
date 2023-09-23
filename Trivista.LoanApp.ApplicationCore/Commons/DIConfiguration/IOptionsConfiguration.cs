using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trivista.LoanApp.ApplicationCore.Commons.Options;

namespace Trivista.LoanApp.ApplicationCore.Commons.DIConfiguration;

public static class IOptionsConfiguration
{
    public static IServiceCollection InjectOptions(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<LoanConfiguration>(configuration.GetSection("LoanConfiguration"));
        services.Configure<AzureAppInsightOption>(configuration.GetSection("AzureAppInsightOption"));
        services.Configure<AzureAppConfigOption>(configuration.GetSection("AzureAppConfigOption"));
        services.Configure<SmileIdOption>(configuration.GetSection("SmileIdOption"));
        services.Configure<PayStackOption>(configuration.GetSection("PayStackOption"));
        services.Configure<MailOptions>(configuration.GetSection("MailOptions"));
        services.Configure<RemittaOption>(configuration.GetSection("RemittaOption"));
        services.Configure<MbsOption>(configuration.GetSection("MbsOption"));
        services.Configure<IndicinaOption>(configuration.GetSection("IndicinaOption"));
        return services;
    }
}