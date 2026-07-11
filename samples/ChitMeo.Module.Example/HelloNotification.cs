using ChitMeo.Mediator;

namespace ChitMeo.Module.Example;

public class HelloNotification : INotification
{
    public string? Message { get; init; }
}
