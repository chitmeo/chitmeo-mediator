# ChitMeo.Mediator

**ChitMeo.Mediator** - Simple mediator for modular .NET applications.
ChitMeo.Mediator is a lightweight mediator implementation for .NET designed to be simple, dependency-free, and easy to extend.
It implements the **Mediator pattern** commonly used in CQRS architectures while keeping the codebase minimal and transparent.

## Features

* Lightweight and dependency-free
* Automatic request and notification handler discovery
* Seamless integration with `Microsoft.Extensions.DependencyInjection`
* Designed for modular monolith architectures
* Minimal reflection usage
* Easy to extend with pipeline behaviors
* Publish/subscribe notifications via `PublishAsync`

## Installation

Install from NuGet:

```
dotnet add package ChitMeo.Mediator
```

## Quick Start

### 1. Register ChitMeo.Mediator

```csharp
builder.Services.AddMediator();
```

`AddMediator()` automatically scans assemblies containing `.Module.` in their name and registers both request handlers and notification handlers.

You can customize the module prefix:

```csharp
builder.Services.AddMediator(opt =>
{
    opt.ModulePrefix = ".Feature";
});
```

---

### 2. Create a Request

```csharp
public class Ping : IRequest<string>
{
}
```

---

### 3. Create a Handler

```csharp
public class PingHandler : IRequestHandler<Ping, string>
{
    public Task<string> HandleAsync(Ping request, CancellationToken cancellationToken)
    {
        return Task.FromResult("Pong");
    }
}
```

---

### 4. Send Request

```csharp
var result = await mediator.SendAsync(new Ping());

Console.WriteLine(result); // Pong
```

---

### 5. Create a Notification

```csharp
public class PingPublished : INotification
{
    public string? Message { get; init; }
}
```

---

### 6. Create a Notification Handler

```csharp
public class PingPublishedHandler : INotificationHandler<PingPublished>
{
    public Task HandleAsync(PingPublished notification, CancellationToken cancellationToken)
    {
        Console.WriteLine($"{notification.Message} published");
        return Task.CompletedTask;
    }
}
```

---

### 7. Publish a Notification

```csharp
await mediator.PublishAsync(new PingPublished { Message = "Ping" });
```

All registered handlers for the notification type are resolved and invoked when `PublishAsync` is called.

---

## Pipeline Behaviors

Pipeline behaviors allow you to intercept requests before and after the handler executes — useful for cross-cutting concerns such as logging, validation, caching, or transactions.

### How it works

Behaviors wrap around the handler in the order they are registered. The first registered behavior runs outermost (first in, last out):

```
Request → Behavior 1 → Behavior 2 → Handler → Behavior 2 → Behavior 1 → Response
```

---

### 1. Implement IPipelineBehavior

```csharp
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"Handling {typeof(TRequest).Name}");

        var response = await next(); // invoke the next behavior or handler

        Console.WriteLine($"Handled {typeof(TRequest).Name}");

        return response;
    }
}
```

---

### 2. Register the Behavior

Register using the open generic to apply the behavior to **all requests**:

```csharp
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
```

Or register using the closed generic to apply only to a **specific request**:

```csharp
builder.Services.AddTransient<IPipelineBehavior<Ping, string>, LoggingBehavior<Ping, string>>();
```

---

### 3. Multiple Behaviors

Multiple behaviors are executed in registration order:

```csharp
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
```

Execution order:

```
Request → LoggingBehavior → ValidationBehavior → Handler
                                                     ↓
Response ← LoggingBehavior ← ValidationBehavior ←───┘
```

---

### Example: Validation Behavior

```csharp
public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // Pre-handler: validate the request
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        // Continue down the pipeline
        return await next();
    }
}
```

---

## Interfaces

### IRequest

Represents a request expecting a response.

```csharp
public interface IRequest<TResponse>
{
}
```

---

### IRequestHandler

Handles a specific request.

```csharp
public interface IRequestHandler<TRequest, TResponse>
{
    Task<TResponse> HandleAsync(TRequest request, CancellationToken cancellationToken);
}
```

---

### INotification

Represents a message that can be broadcast to multiple handlers.

```csharp
public interface INotification
{
}
```

---

### INotificationHandler

Handles a specific notification.

```csharp
public interface INotificationHandler<in TNotification>
    where TNotification : INotification
{
    Task HandleAsync(TNotification notification, CancellationToken cancellationToken);
}
```

---

### IMediator

Sends requests to their handlers and publishes notifications to all matching handlers.

```csharp
public interface IMediator
{
    Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);

    Task PublishAsync<TNotification>(
        TNotification notification,
        CancellationToken cancellationToken = default)
        where TNotification : INotification;
}
```

---

### IPipelineBehavior

Intercepts requests in the pipeline.

```csharp
public interface IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken);
}
```

---

## Assembly Scanning

ChitMeo.Mediator automatically discovers both request handlers and notification handlers in assemblies that match:

```
*.Module.*.dll
```

Example:

```
MyApp.Module.Users
MyApp.Module.Orders
```

This allows a **modular monolith architecture** where each module can contain its own requests, notifications, and handlers.

---

## Performance

ChitMeo.Mediator focuses on simplicity while maintaining excellent performance.

Example benchmark:

| Method            | Mean    |
| ----------------- | ------- |
| Direct call       | ~21 ns  |
| ChitMeo.Mediator  | ~135 ns |

The overhead is minimal and suitable for most applications.

---

## Roadmap

Possible future features:

* Source generator optimization
* Request caching
* Transaction pipeline

---

## License

MIT
