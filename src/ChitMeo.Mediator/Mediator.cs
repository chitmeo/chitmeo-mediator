using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace ChitMeo.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        if (request == null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        Type handlerType = typeof(IRequestHandler<,>)
            .MakeGenericType(request.GetType(), typeof(TResponse));

        var handler = _serviceProvider.GetService(handlerType);
        if (handler == null)
            throw new InvalidOperationException($"No handler registered for {request.GetType().Name}");

        MethodInfo? method = handlerType.GetMethod("HandleAsync");
        if (method == null)
            throw new InvalidOperationException($"Method 'HandleAsync' not found on {handlerType.Name}");

        var result = method.Invoke(handler, [request, cancellationToken]);
        if (result is Task<TResponse> task)
        {
            return await task;
        }
        throw new InvalidOperationException($"Handler method did not return Task<{typeof(TResponse).Name}>");
    }
}