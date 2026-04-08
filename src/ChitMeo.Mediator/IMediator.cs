namespace ChitMeo.Mediator;

/// <summary>
/// Defines the interface for a mediator that sends requests and returns responses.
/// </summary>
public interface IMediator
{
    /// <summary>
    /// Sends an asynchronous request and returns the response.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response.</typeparam>
    /// <param name="request">The request to send.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A task representing the asynchronous operation with the response.</returns>
    Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default);
}
