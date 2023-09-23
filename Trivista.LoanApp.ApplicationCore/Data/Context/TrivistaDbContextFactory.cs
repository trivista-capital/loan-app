using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Trivista.LoanApp.ApplicationCore.Data.Context;

public class TrivistaDbContextFactory : IDesignTimeDbContextFactory<TrivistaDbContext>
{
    public TrivistaDbContext CreateDbContext(string[] args)
    {
        var envName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("secrets.json", optional: true)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{envName}.json", optional: true)
            .AddUserSecrets(Assembly.GetExecutingAssembly(), optional:true)
            .Build();
        
        var connectionString = configuration.GetConnectionString("TrivistaDbConnection");
        var optionsBuilder = new DbContextOptionsBuilder<TrivistaDbContext>();
        optionsBuilder.UseSqlServer(connectionString);
        return new TrivistaDbContext(optionsBuilder.Options);
    }
}