namespace ChitMeo.Mediator;

/// <summary>
/// Defines a behavior that can intercept and modify the handling of requests in the pipeline.
/// </summary>
/// <typeparam name="TRequest">The type of the request.</typeparam>
/// <typeparam name="TResponse">The type of the response.</typeparam>
public interface IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Handles the request by executing the next handler in the pipeline or performing additional processing.
    /// </summary>
    /// <param name="request">The request to handle.</param>
    /// <param name="next">The delegate to invoke the next handler in the pipeline.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The response from the handler.</returns>
    Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}

/// <summary>
/// Represents a delegate that handles a request in the pipeline and returns a response.
/// </summary>
/// <typeparam name="TResponse">The type of the response.</typeparam>
/// <returns>A task that represents the asynchronous operation and returns the response.</returns>
public delegate Task<TResponse> RequestHandlerDelegate<TResponse>();