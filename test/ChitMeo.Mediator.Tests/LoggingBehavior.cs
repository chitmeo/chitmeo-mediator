namespace ChitMeo.Mediator.Tests;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Pre-processing
        System.Console.WriteLine($"Pre-processing {typeof(TRequest).Name}");

        var response = await next();

        // Post-processing
        System.Console.WriteLine($"Post-processing {typeof(TRequest).Name}");

        return response;
    }
}
