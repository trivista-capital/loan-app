using Microsoft.AspNetCore.Mvc;
using Trivista.LoanApp.ApplicationCore.Commons.DIConfiguration;

namespace Trivista.LoanApp.Api.ServiceCollection;

public static class OptionsService
{
    public static IServiceCollection ConfigureOptions(this IServiceCollection services)
    {
        services.Configure<ApiBehaviorOptions>(option =>
        {
            //option.InvalidModelStateResponseFactory = ErrorResult.GenerateErrorResponse;
        });
        return services;
    }
}