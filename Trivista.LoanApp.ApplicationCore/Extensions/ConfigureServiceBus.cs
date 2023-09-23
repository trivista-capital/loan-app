using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Trivista.LoanApp.ApplicationCore.Infrastructure.MessageBroker;
using Trivista.LoanApp.ApplicationCore.Infrastructure.MessageBroker.Consumers;

namespace Trivista.LoanApp.ApplicationCore.Extensions;

public static class ConfigureServiceBus
{
        public static IServiceCollection ConfigureBus(this IServiceCollection services, IConfiguration config)
        {
            var username = config.GetSection("RabbitMqSettings").GetSection("UserName").Value;
            var password = config.GetSection("RabbitMqSettings").GetSection("Password").Value;
            var rabbitMqUrl = config.GetSection("RabbitMqSettings").GetSection("Uri").Value; 
            
            services.AddMassTransit(x =>
            {
                x.AddConsumer<CreateCustomerConsumer>();
                x.AddBus(provider => Bus.Factory.CreateUsingRabbitMq(cfg =>
                {
                    cfg.Host(new Uri(rabbitMqUrl), h => {
                        h.Username(username);
                        h.Password(password);
                    });
                    cfg.ReceiveEndpoint("CreateCustomerQueue", e =>
                    {
                        e.Durable = true;
                        e.PrefetchCount = 16;
                        e.UseMessageRetry(r => r.Interval(3, 2000));
                        e.UseCircuitBreaker(cb =>
                        {
                            cb.TrackingPeriod = TimeSpan.FromMinutes(1);
                            cb.TripThreshold = 15;
                            cb.ActiveThreshold = 10;
                            cb.ResetInterval = TimeSpan.FromMinutes(3);
                        });
                        e.Consumer<CreateCustomerConsumer>(provider);
                        e.DiscardSkippedMessages();
                        e.DiscardFaultedMessages();
                    });
                }));
            });
            services.AddSingleton<IHostedService, BusService>();
            return services;
        }
}