using ChitMeo.Mediator;

namespace ChitMeo.Module.Example;

public class HelloNotificationHandler : INotificationHandler<HelloNotification>
{
    public Task HandleAsync(HelloNotification notification, CancellationToken cancellationToken)
    {
        System.Console.WriteLine($"Handling HelloNotification: {notification.Message}");
        return Task.CompletedTask;
    }
}
