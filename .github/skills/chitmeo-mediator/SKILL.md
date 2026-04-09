---
name: chitmeo-mediator
description: 'Assist with ChitMeo.Mediator library usage. Use for generating requests, handlers, and integrating the mediator in .NET projects.'
argument-hint: 'Describe the mediator component to generate or task to perform'
---

# ChitMeo.Mediator Skill

## When to Use

- Generating new requests and handlers for ChitMeo.Mediator
- Setting up mediator integration in a .NET project
- Adding pipeline behaviors or extending the mediator

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

### 2. Generate a Handler

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

### 3. Register the Mediator

In your DI container setup:

```csharp
builder.Services.AddMediator();
```

This automatically scans assemblies containing `.Module.` in their name for handlers.

### 4. Send a Request

Use the mediator to send requests:

```csharp
var mediator = serviceProvider.GetRequiredService<IMediator>();
var response = await mediator.SendAsync(new Ping { Message = "Hello" });
```

## References

- [ChitMeo.Mediator README](../../../README.md)
- [IMediator Interface](../../../src/ChitMeo.Mediator/IMediator.cs)
- [Mediator Implementation](../../../src/ChitMeo.Mediator/Mediator.cs)