using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Mediator;

/// <summary>
/// Mediator implementation for handling requests and dispatching them to appropriate handlers.
/// </summary>
public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    private static readonly ConcurrentDictionary<Type, (Type handlerType, MethodInfo method)>
        _cache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve request handlers.</param>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Sends a request to be handled asynchronously and returns the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response returned by the handler.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation and returns the response.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="request"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the handler for the request type is not found.</exception>
    public async Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var requestType = request.GetType();

        var (handlerType, method) = _cache.GetOrAdd(requestType, t =>
        {
            var hType = typeof(IRequestHandler<,>)
                .MakeGenericType(t, typeof(TResponse));

            var m = hType.GetMethod("HandleAsync")
                ?? throw new InvalidOperationException("HandleAsync not found");

            return (hType, m);
        });

        var handler = _serviceProvider.GetRequiredService(handlerType);

        var result = method.Invoke(handler, [request, cancellationToken]);

        return await (Task<TResponse>)result!;
    }
}