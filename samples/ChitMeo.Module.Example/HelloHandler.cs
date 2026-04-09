using ChitMeo.Mediator;

namespace ChitMeo.Module.Example;

public class HelloHandler : IRequestHandler<HelloRequest, string>
{
    public Task<string> HandleAsync(
        HelloRequest request,
        CancellationToken cancellationToken)
    {
        System.Console.WriteLine("Handling HelloRequest");
        return Task.FromResult("");
    }
}