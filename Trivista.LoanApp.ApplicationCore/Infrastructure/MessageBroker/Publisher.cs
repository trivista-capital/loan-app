using MassTransit;
using Trivista.LoanApp.ApplicationCore.Interfaces;

namespace Trivista.LoanApp.ApplicationCore.Infrastructure.MessageBroker;

public class Publisher: IPublisher
{
    private readonly IBus _bus;
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public Publisher(IBus bus)
    {
        _bus = bus;
    }

    public async Task Publish<T>(T message)
    {
        await Task.Run(() =>
        {
            _bus.Publish(message, _cancellationTokenSource.Token);
        });
    }
        
    public async Task SendToQueueAsync<T>(T message, string queue, CancellationToken cancellationToken = default)
    {
        var endpoint = await _bus.GetSendEndpoint(new Uri($"queue:{queue}"));

        await endpoint.Send(message, cancellationToken);
    }
}