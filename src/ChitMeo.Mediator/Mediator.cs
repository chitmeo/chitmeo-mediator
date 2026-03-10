using System.Collections.Concurrent;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace ChitMeo.Mediator;

public class Mediator : IMediator
{
    private readonly IServiceProvider _serviceProvider;

    private static readonly ConcurrentDictionary<Type, (Type handlerType, MethodInfo method)>
        _cache = new();

    public Mediator(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

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

        var result = method.Invoke(handler, new object[] { request, cancellationToken });

        return await (Task<TResponse>)result!;
    }
}