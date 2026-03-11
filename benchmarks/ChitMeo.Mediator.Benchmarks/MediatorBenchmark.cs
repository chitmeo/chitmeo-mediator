namespace ChitMeo.Mediator.Benchmarks;

using BenchmarkDotNet.Attributes;

using Microsoft.Extensions.DependencyInjection;

[MemoryDiagnoser]
public class MediatorBenchmark
{
    private IMediator _mediator = null!;
    private PingHandler _handler = null!;
    private Ping _request = new();

    [GlobalSetup]
    public void Setup()
    {
        var services = new ServiceCollection();

        services.AddMediator();

        services.AddScoped<IRequestHandler<Ping, string>, PingHandler>();

        var provider = services.BuildServiceProvider();

        _mediator = provider.GetRequiredService<IMediator>();
        _handler = new PingHandler();
    }

    [Benchmark]
    public async Task<string> Mediator_Send()
    {
        return await _mediator.SendAsync(_request);
    }

    [Benchmark]
    public async Task<string> Direct_Call()
    {
        return await _handler.HandleAsync(_request, default);
    }

    [Benchmark]
    public async Task<string> Reflection_Invoke()
    {
        var method = typeof(PingHandler).GetMethod("HandleAsync");

        var task = (Task<string>)method!.Invoke(_handler, new object[] { _request, default })!;

        return await task;
    }
}