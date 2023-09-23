using System.Net.Http.Headers;
using System.Reflection;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Trivista.LoanApp.ApplicationCore.Commons.DIConfiguration;
using Trivista.LoanApp.ApplicationCore.Commons.Options;
using Trivista.LoanApp.ApplicationCore.Data;
using Trivista.LoanApp.ApplicationCore.Data.Context;
using Trivista.LoanApp.ApplicationCore.Entities;
using Trivista.LoanApp.ApplicationCore.Features.LoanCalculation;
using Trivista.LoanApp.ApplicationCore.Infrastructure.Http;
using Trivista.LoanApp.ApplicationCore.Infrastructure.MessageBroker;
using Trivista.LoanApp.ApplicationCore.Interfaces;
using Trivista.LoanApp.ApplicationCore.Services.Mail;
using Trivista.LoanApp.ApplicationCore.Services.Payment;

namespace Trivista.LoanApp.ApplicationCore.Extensions;

public static class DependencyInjection
{
    public static IServiceCollection InjectApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("TrivistaDbConnection");
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            // cfg.AddBehavior<IPipelineBehavior<Ping, Pong>, PingPongBehavior>();
            // cfg.AddOpenBehavior(typeof(GenericBehavior<,>));
        });
        services.AddDbContext<TrivistaDbContext>(x =>
        {
            x.UseSqlServer(connectionString, sqlServerOptionsAction: option =>
            {
                option.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);
            } );
            x.EnableDetailedErrors(true);
        });
        // using var dbContext = services.BuildServiceProvider().GetService<TrivistaDbContext>();
        // var loan = InitializeLoan(dbContext).GetAwaiter().GetResult();
        // services.InitializeLoanConfiguration(x =>
        // {
        //     x.InterestRate = loan.InterestRate;
        //     x.MaximumTenure = loan.MaximumTenure;
        //     x.MaximumLoanAmount = loan.MaximumLoanAmount;
        //     x.SetName(loan.Name);
        // });
        
        // services.AddTransient<Interfaces.IPublisher, Publisher>();
        // services.ConfigureBus(configuration);
        services.AddTransient<ISmileIdService, SmileIdService>();
        services.AddTransient<IPayStackService, PayStackService>();
        services.AddTransient<IMailBuilder, MailBuilder>();
        services.AddTransient<IMailService, MailService>();
        services.AddTransient<IRemittaService, RemittaService>();
        services.AddTransient<IMbsService, MbsService>();
        services.AddTransient<IIndicina, Indicina>();
        services.AddHttpClient<ISmileIdService, SmileIdService>((provider, client) =>
        {
            var serviceProvider = provider.GetService<IOptions<SmileIdOption>>(); 
            client.BaseAddress = new Uri(serviceProvider!.Value.Url);
        });
        
        services.AddHttpClient<IPayStackService, PayStackService>((provider, client) =>
        {
            var serviceProvider = provider.GetService<IOptions<PayStackOption>>(); 
            client.BaseAddress = new Uri(serviceProvider!.Value.BaseUrl);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", serviceProvider.Value.SecretKey);
        }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15)
        }).SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        
        services.AddHttpClient<IRemittaService, RemittaService>((provider, client) =>
        {
            var serviceProvider = provider.GetService<IOptions<RemittaOption>>(); 
            client.BaseAddress = new Uri(serviceProvider!.Value.BaseUrl);
        }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15)
        }).SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        
        services.AddHttpClient<IMbsService, MbsService>((provider, client) =>
        {
            var serviceProvider = provider.GetService<IOptions<MbsOption>>(); 
            client.BaseAddress = new Uri(serviceProvider!.Value.BaseUrl);
            client.DefaultRequestHeaders.Add("Client-ID", serviceProvider.Value.ClientId);
            client.DefaultRequestHeaders.Add("Client-Secret", serviceProvider.Value.ClientSecret);
        }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15)
        }).SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        
        services.AddHttpClient<IMailService, MailService>((provider, client) =>
        {
            var serviceProvider = provider.GetService<IConfiguration>();
            string baseAddy = serviceProvider.GetSection("TrivistaMailServiceBaseAddress").Value;
            client.BaseAddress = new Uri(baseAddy);
        }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15)
        }).SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        
        services.AddHttpClient<IIndicina, Indicina>((provider, client) =>
        {
            var serviceProvider = provider.GetService<IConfiguration>();
            string baseAddy = serviceProvider.GetSection("IndicinaOption").GetSection("BaseUrl").Value;
            client.BaseAddress = new Uri(baseAddy);
        }).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler()
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15)
        }).SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        
        return services;
    }
    
    private static async Task<Loan> InitializeLoan(TrivistaDbContext ctx)
    {
        var loanEntity = await ctx.Loan.FirstOrDefaultAsync();
        var loan = Loan.Factory.Build();
        loan.SetName(loanEntity.Name);
        loan.SetInterestRate(loanEntity.InterestRate);
        loan.SetMaximumTenure(loanEntity.MaximumTenure);
        loan.SetMaximumLoanAmount(loanEntity.MaximumLoanAmount);
        return loan;
    }
}