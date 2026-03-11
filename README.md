# ChitMeo.Mediator

**ChitMeo.Mediator** - Simple mediator for modular .NET applications.
ChitMeo.Mediator is a lightweight mediator implementation for .NET designed to be simple, dependency-free, and easy to extend.
It implements the **Mediator pattern** commonly used in CQRS architectures while keeping the codebase minimal and transparent.

## Features

* Lightweight and dependency-free
* Automatic handler discovery
* Seamless integration with `Microsoft.Extensions.DependencyInjection`
* Designed for modular monolith architectures
* Minimal reflection usage
* Easy to extend with pipelines and behaviors

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

`AddMediator()` automatically scans assemblies containing `.Module.` in their name and registers request handlers.

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

### IMediator

Sends requests to their handlers.

```csharp
public interface IMediator
{
    Task<TResponse> SendAsync<TResponse>(
        IRequest<TResponse> request,
        CancellationToken cancellationToken = default);
}
```

---

## Assembly Scanning

ChitMeo.Mediator automatically discovers handlers in assemblies that match:

```
*.Module.*.dll
```

Example:

```
MyApp.Module.Users
MyApp.Module.Orders
```

This allows a **modular monolith architecture** where each module contains its own handlers.

---

## Performance

ChitMeo.Mediator focuses on simplicity while maintaining excellent performance.

Example benchmark:

| Method          | Mean    |
| --------------- | ------- |
| Direct call     | ~21 ns  |
| ChitMeo.Mediator | ~135 ns |

The overhead is minimal and suitable for most applications.

---

## Roadmap

Possible future features:

* Pipeline behaviors
* Notification handlers
* Source generator optimization
* Request caching
* Transaction pipeline

---

## License

MIT
