using System;
using System.Threading;
using System.Threading.Tasks;

namespace ChitMeo.Mediator
{
    public class Mediator : IMediator
    {
        private readonly Func<Type, object> _resolver;

        public Mediator(Func<Type, object> resolver)
        {
            _resolver = resolver;
        }

        public Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
        {
            var requestType = request.GetType();

            var handlerType = typeof(IRequestHandler<,>)
                .MakeGenericType(requestType, typeof(TResponse));

            var handler = _resolver(handlerType);

            if (handler == null)
            {
                throw new InvalidOperationException(
                    $"Handler not found for {requestType.Name}");
            }

            var method = handlerType.GetMethod("Handle");

            return (Task<TResponse>)method.Invoke(
                handler,
                new object[] { request, cancellationToken });
        }
    }
}
