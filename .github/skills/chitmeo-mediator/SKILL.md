---
name: chitmeo-mediator
description: 'Assist with ChitMeo.Mediator library usage. Use for generating requests, notifications, handlers, and integrating the mediator in .NET projects.'
argument-hint: 'Describe the mediator component to generate or task to perform'
---

# ChitMeo.Mediator Skill

## When to Use

- Generating new requests, notifications, and handlers for ChitMeo.Mediator
- Setting up mediator integration in a .NET project
- Adding pipeline behaviors or extending the mediator
- Publishing notifications to multiple subscribers

## Procedures

### 1. Generate a Request

To create a new request:

1. Define a class that implements `IRequest<TResponse>`

Example:

```csharp
public class Ping : IRequest<string>
{
    public string Message { get; set; }
}
```

### 2. Generate a Request Handler

To create a handler for the request:

1. Implement `IRequestHandler<TRequest, TResponse>`

Example:

```csharp
public class PingHandler : IRequestHandler<Ping, string>
{
    public async Task<string> HandleAsync(Ping request, CancellationToken cancellationToken)
    {
        return $"Pong: {request.Message}";
    }
}
```

### 3. Generate a Notification

To create a notification for fan-out handling:

1. Define a class that implements `INotification`

Example:

```csharp
public class PingPublished : INotification
{
    public string Message { get; init; }
}
```

### 4. Generate a Notification Handler

To create a handler for the notification:

1. Implement `INotificationHandler<TNotification>`

Example:

```csharp
public class PingPublishedHandler : INotificationHandler<PingPublished>
{
    public Task HandleAsync(PingPublished notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"Received: {notification.Message}");
        return Task.CompletedTask;
    }
}
```

### 5. Register the Mediator

In your DI container setup:

```csharp
builder.Services.AddMediator();
```

This automatically scans assemblies containing `.Module.` in their name for request and notification handlers.

### 6. Send a Request or Publish a Notification

Use the mediator to send requests:

```csharp
var mediator = serviceProvider.GetRequiredService<IMediator>();
var response = await mediator.SendAsync(new Ping { Message = "Hello" });
```

Use the mediator to publish notifications to all matching handlers:

```csharp
await mediator.PublishAsync(new PingPublished { Message = "Hello" });
```

## References

- [ChitMeo.Mediator README](../../../README.md)
- [IMediator Interface](../../../src/ChitMeo.Mediator/IMediator.cs)
- [Notification Interface](../../../src/ChitMeo.Mediator/INotification.cs)
- [Notification Handler Interface](../../../src/ChitMeo.Mediator/INotificationHandler.cs)
- [Mediator Implementation](../../../src/ChitMeo.Mediator/Mediator.cs)