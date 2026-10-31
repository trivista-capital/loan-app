using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Trivista.LoanApp.Api.ServiceCollection;

public static class AuthenticatinService
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = configuration.GetSection("IdentityOptions").GetSection("IdentityUrl").Value;
                options.Audience = "travisterApiLoanApi";
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new()
                {
                    NameClaimType = "given_name",
                    RoleClaimType = "role",
                    ValidTypes = new[]{ "at+jwt" }
                };

            });
        return services;
    }
}