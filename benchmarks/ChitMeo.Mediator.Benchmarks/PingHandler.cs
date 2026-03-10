namespace ChitMeo.Mediator.Benchmarks;

public class PingHandler : IRequestHandler<Ping, string>
{
    public Task<string> HandleAsync(Ping request, CancellationToken cancellationToken)
    {
        return Task.FromResult("Pong");
    }
}
