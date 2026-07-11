using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Mediator.Tests;

public class PublishTests
{
    public class Ping : INotification
    {
        public string? Message { get; set; }
    }


    public class PongHandler : INotificationHandler<Ping>
    {

        public Task HandleAsync(Ping notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("PongHandler received: " + notification.Message);
            return Task.CompletedTask;
        }
    }

    public class PungHandler : INotificationHandler<Ping>
    {

        public Task HandleAsync(Ping notification, CancellationToken cancellationToken)
        {
            Console.WriteLine("PungHandler received: " + notification.Message);
            return Task.CompletedTask;
        }
    }

    private readonly IMediator _mediator;

    public PublishTests()
    {
        var services = new ServiceCollection();
        services.AddMediator(options =>
        {
            options.ModulePrefix = "ChitMeo.Mediator.Tests";
        });
        services.AddScoped<INotificationHandler<Ping>, PongHandler>();
        services.AddScoped<INotificationHandler<Ping>, PungHandler>();
        _mediator = services.BuildServiceProvider().GetRequiredService<IMediator>();
    }

    [Fact]
    public async Task PublishAsync_Should_Invoke_All_Notification_Handlers()
    {
        // Arrange
        var notification = new Ping { Message = "Ping" };
        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);
        // Act
        await _mediator.PublishAsync(notification);

        // Assert
        var output = consoleOutput.ToString().TrimEnd();
        Assert.Contains("PungHandler received: Ping", output);
        Assert.Contains("PongHandler received: Ping", output);
    }
}