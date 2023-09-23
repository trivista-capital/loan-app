using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Trivista.LoanApp.ApplicationCore.Extensions;
using Trivista.LoanApp.ApplicationCore.Features.CustomerLoanRequest;

namespace Trivista.LoanApp.ApplicationCore.IntegrationTest.Setup;

public class BaseTestSetup : WebApplicationFactory<Program>, IAsyncLifetime
{
    private IConfiguration Configuration { get; set; }
    public HttpClient HttpClient { get; private set; } = default!; 

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        Configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        builder.ConfigureAppConfiguration(config =>
        {
            config.AddConfiguration(Configuration);
        });
        
        //app.MapCarter();

        builder.ConfigureTestServices(services =>
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssemblyContaining<LoanRequestController>();
                cfg.Lifetime = ServiceLifetime.Scoped;
            });
            services.InjectApplicationServices(Configuration);
            services.InitializeLoanConfiguration();
            //services.AddCarter();
        });
    }
    
    public async Task InitializeAsync()
    {
        HttpClient = CreateHttpClient();
    }

    private HttpClient CreateHttpClient()
    {
        return this.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
        });
    }

    public new async Task DisposeAsync()
    {
        
    }
}