using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using Carter;
using Microsoft.IdentityModel.Logging;
using Serilog;
using Trivista.LoanApp.Api.ServiceCollection;
using Trivista.LoanApp.ApplicationCore.Commons.DIConfiguration;
using Trivista.LoanApp.ApplicationCore.Commons.Helpers;
using Trivista.LoanApp.ApplicationCore.Extensions;


// Add services to the container.
Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();
Log.Information("Starting up");
try
{
    var builder = WebApplication.CreateBuilder(args);

    var environmnt = builder.Environment.EnvironmentName;
    
    IdentityModelEventSource.ShowPII = true;

    builder.Services.AddControllers().AddJsonOptions(x =>
    {
        // serialize enums as strings in api responses (e.g. Role)
        x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
    //builder.BootstrapAppConfig();
    //builder.Services.AddAzureAppConfiguration();
    builder.Services.InjectOptions(builder.Configuration);
    //builder.BootstrapAppInsight(builder.Services);
    builder.Services.AddCarter();
    builder.Services.ConfigureCors();
    builder.Services.AddHttpContextAccessor();
    builder.Services.AddAccessTokenManagement();
    JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
    //builder.Services.ServicePointManager
    builder.Services.ConfigureAuthentication(builder.Configuration);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.ConfigureSwagger();
    builder.Services.ConfigureAuthorization();
    // builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
    builder.Services.InjectApplicationServices(builder.Configuration);
    //builder.Services.InitializeLoanConfiguration();
    builder.Services.ConfigureOptions();
    builder.ConfigureSerilog();
    builder.Services.AddSingleton<TokenManager>();

    var app = builder.Build();
    app.MigrateDatabase();
    app.MapCarter();

    //Configure the HTTP request pipeline.
    app.MapSwagger();
    if (app.Environment.IsLocal() || app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
    {
        app.UseSwagger();
        
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Trivista Loan Management API"));
    }
    
    if (app.Environment.IsDevelopment() || app.Environment.IsStaging() || app.Environment.IsProduction())
    {
        //app.UseAzureAppConfiguration();
    }
    

    app.UseHttpsRedirection();

    app.UseCors("AllowSpecificOrigins");

    app.UseAuthentication();
    
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception ex) when (ex.GetType().Name is not "StopTheHostException")
{
    Log.Fatal(ex, "Unhandled exception");
}
finally
{
    Log.Information("Api down complete");
    Log.CloseAndFlush();
}
public partial class Program { }