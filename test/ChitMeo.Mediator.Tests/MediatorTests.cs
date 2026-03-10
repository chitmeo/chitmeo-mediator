using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Mediator.Tests;

public class MediatorTests
{
    [Fact]
    public async Task SendAsync_Should_Invoke_Handler()
    {
        var services = new ServiceCollection();

        services.AddMediator();

        services.AddScoped<IRequestHandler<Ping, string>, PingHandler>();

        var provider = services.BuildServiceProvider();

        var mediator = provider.GetRequiredService<IMediator>();

        var result = await mediator.SendAsync(new Ping());

        Assert.Equal("Pong", result);
    }
}
