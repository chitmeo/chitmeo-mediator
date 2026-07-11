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

    private static readonly ConcurrentDictionary<Type, (Type handlerType, MethodInfo handlerMethod, Type behaviorType, MethodInfo behaviorMethod)> _requestHandlerCache = new();
    private static readonly ConcurrentDictionary<Type, (Type handlerType, MethodInfo handlerMethod)> _notificationCache = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Mediator"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider used to resolve request handlers.</param>
    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Sends a request through the behavior pipeline and then to its handler, returning the response.
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

        var (handlerType, handlerMethod, behaviorType, behaviorMethod) = _requestHandlerCache.GetOrAdd(requestType, t =>
        {
            var hType = typeof(IRequestHandler<,>)
                .MakeGenericType(t, typeof(TResponse));

            var hMethod = hType.GetMethod("HandleAsync")
                ?? throw new InvalidOperationException($"HandleAsync not found on {hType}");

            var bType = typeof(IPipelineBehavior<,>)
                .MakeGenericType(t, typeof(TResponse));

            var bMethod = bType.GetMethod("HandleAsync")
                ?? throw new InvalidOperationException($"HandleAsync not found on {bType}");

            return (hType, hMethod, bType, bMethod);
        });

        var handler = _serviceProvider.GetRequiredService(handlerType);

        // Build the innermost delegate: the actual handler invocation.
        RequestHandlerDelegate<TResponse> handlerDelegate = () => (Task<TResponse>)handlerMethod.Invoke(handler, [request, cancellationToken])!;

        // Resolve all registered behaviors and wrap them around the handler delegate,
        // last-registered behavior executes outermost (first in the chain).
        var behaviors = _serviceProvider
            .GetServices(behaviorType)
            .Reverse()
            .ToList();

        var pipeline = behaviors.Aggregate(
            handlerDelegate,
            (next, behavior) =>
            {
                var capturedNext = next;
                return () => (Task<TResponse>)behaviorMethod.Invoke(
                    behavior, [request, capturedNext, cancellationToken])!;
            });

        return await pipeline();
    }

    /// <summary>
    /// Asynchronously sends a notification to all registered handlers for the notification type.
    /// </summary>
    /// <typeparam name="TNotification"></typeparam>
    /// <param name="notification"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public Task PublishAsync<TNotification>(TNotification notification, CancellationToken cancellationToken = default) where TNotification : INotification
    {
        if (notification == null)
            throw new ArgumentNullException(nameof(notification));

        var notificationType = notification.GetType();
        var (handlerType, handlerMethod) = _notificationCache.GetOrAdd(notificationType, t =>
        {
            var hType = typeof(INotificationHandler<>).MakeGenericType(t);
            var hMethod = hType.GetMethod("HandleAsync") ?? throw new InvalidOperationException($"HandleAsync not found on {hType}");

            return (hType, hMethod);
        });

        var handlers = _serviceProvider.GetServices(handlerType).ToList();

        var tasks = handlers.Select(handler => (Task)handlerMethod.Invoke(handler, [notification, cancellationToken])!);

        return Task.WhenAll(tasks);
    }
}