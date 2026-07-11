namespace ChitMeo.Mediator.Benchmarks;

public class PingNotificationHandler : INotificationHandler<PingNotification>
{
    public Task HandleAsync(PingNotification notification, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
