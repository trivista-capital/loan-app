namespace Trivista.LoanApp.Api.ServiceCollection;

public static class AuthorizationService
{
    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(authOptions =>
        {
            //authOptions.AddPolicy("UserCanAddRole", AuthorizationPolicies.CanCreateRole());
            authOptions.AddPolicy("UserCanAddRole", policyBuilder =>
            {
                policyBuilder.RequireAuthenticatedUser();
                //policyBuilder.AddRequirements(new MustHavePermissionToEditRoleRequirement());
            });
        });
        return services;
    }
}