using System;

namespace ChitMeo.Mediator.Tests;

public class LoggingBehaviorTests
{
    [Fact]
    public async Task HandleAsync_ShouldLogPreAndPostProcessing()
    {
        // Arrange
        var request = new TestRequest();
        RequestHandlerDelegate<object> nextDelegate = () => Task.FromResult(new object());
        var behavior = new LoggingBehavior<TestRequest, object>();

        using var consoleOutput = new StringWriter();
        Console.SetOut(consoleOutput);

        // Act
        await behavior.HandleAsync(request, nextDelegate, CancellationToken.None);

        // Assert
        var output = consoleOutput.ToString().TrimEnd();
        Assert.Contains("Pre-processing TestRequest", output);
        Assert.Contains("Post-processing TestRequest", output);
    }
}

public class TestRequest : IRequest<object> { }