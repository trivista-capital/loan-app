using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using System.Threading;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Enums;

namespace Trivista.LoanApp.ApplicationCore.Extensions;

public static class MigrationManager
{
    public static WebApplication MigrateDatabase(this WebApplication webapp)
    {
        using var scope = webapp.Services.CreateScope();
        using var appContext = scope.ServiceProvider.GetRequiredService<TrivistaDbContext>();
        try
        {
            var roleId = Guid.Parse("3e7d9440-48d7-4174-b9c5-0ea5be7d9e7d");
            var userId = Guid.Parse("363b37a0-c306-4472-a405-4b576334cca0");
            appContext.Database.Migrate();
            var doesRoleExist = appContext.ApplicationRole.FirstOrDefaultAsync(x => x.Name.ToLower() == "SUPERADMIN".ToLower()).GetAwaiter().GetResult();
            if (doesRoleExist == null)
            {
                var role = ApplicationRole.Factory.Create(roleId, "SUPERADMIN", "Super admin role");
                appContext.ApplicationRole.AddAsync(role).GetAwaiter().GetResult();
                appContext.SaveChangesAsync().GetAwaiter().GetResult();
            }
            var doesUserExist = appContext.Customer.FirstOrDefaultAsync(x => x.Email.ToLower() == "tgslimited@gmail.com".ToLower()).GetAwaiter().GetResult();
            if (doesUserExist == null)
            {
                var customer = Entities.Customer.Factory.Build(userId, 
                    "Admin", "Admin", "tgslimited@gmail.com",
                    "", "", "",
                    roleId.ToString(), "Staff").SetMiddleName("Admin")
                    .SetAddress("").SetCustomerRemittance(new Entities.ValueObjects.CustomerRemitterInformation()
                    {
                        IsRemittaUser = RemittaUser.NotRemittaUser.ToString(),
                        OtherLoansCollected = 0,
                        AverageSixMonthsSalary = 0,
                    });
                appContext.Customer.AddAsync(customer).GetAwaiter().GetResult();
                appContext.SaveChangesAsync().GetAwaiter().GetResult();
            }

        }
        catch (Exception ex)
        {
            // Log errors or do anything you think it's needed
            // logger.LogError(ex, "An error occured while creating role and user");
        }

        return webapp;
    }
}