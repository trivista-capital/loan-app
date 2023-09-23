namespace Trivista.LoanApp.Api.ServiceCollection;

public static class CorsService
{
    public static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(name: "AllowSpecificOrigins",
                policy  =>
                {
                    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
        });
        return services;
    }
}