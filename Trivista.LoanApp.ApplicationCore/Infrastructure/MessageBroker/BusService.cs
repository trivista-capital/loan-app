using MassTransit;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Trivista.LoanApp.ApplicationCore.Infrastructure.MessageBroker;

public class BusService: IHostedService
{
    private readonly IBusControl _busControl;
    private readonly ILogger<BusService> _logger;

    public BusService(IBusControl busControl, ILogger<BusService> logger)
    {
        _busControl = busControl;
        _logger = logger;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Started bus for loan Module");

        return _busControl.StartAsync(cancellationToken);
    }
        
    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopped bus for loan Module");

        return _busControl.StopAsync(cancellationToken);
    }
}