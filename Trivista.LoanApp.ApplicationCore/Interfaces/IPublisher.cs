namespace Trivista.LoanApp.ApplicationCore.Interfaces;

public interface IPublisher
{
    Task Publish<T>(T message);
    Task SendToQueueAsync<T>(T message, string queue, CancellationToken cancellationToken = default);
}