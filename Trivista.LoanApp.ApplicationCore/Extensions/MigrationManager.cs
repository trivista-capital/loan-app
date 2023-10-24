using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Trivista.LoanApp.ApplicationCore.Data.Context;

namespace Trivista.LoanApp.ApplicationCore.Extensions;

public static class MigrationManager
{
    public static WebApplication MigrateDatabase(this WebApplication webapp)
    {
        using (var scope = webapp.Services.CreateScope())
        {
            using (var appContext = scope.ServiceProvider.GetRequiredService<TrivistaDbContext>())
            {
                try
                {
                    var conn = appContext.Database.GetConnectionString();
                    appContext.Database.Migrate();
                }
                catch (Exception ex)
                {
                    //Log errors or do anything you think it's needed
                    throw;
                }
            }
        }
        return webapp;
    }
}