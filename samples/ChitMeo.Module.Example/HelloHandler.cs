using ChitMeo.Mediator;

namespace ChitMeo.Module.Example;

public class HelloHandler : IRequestHandler<HelloRequest, string>
{
    public Task<string> HandleAsync(
        HelloRequest request,
        CancellationToken cancellationToken)
    {
        return Task.FromResult("Hello ChitMeo Mediator");
    }
}