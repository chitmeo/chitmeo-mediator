# ChitMeo.Mediator

ChitMeo.Mediator is a lightweight implementation of the Mediator design pattern for .NET.

It helps decouple components in your application by allowing objects to communicate
through a mediator instead of referencing each other directly.

This library is framework-agnostic and can be used with:

- ASP.NET Core Web API
- ASP.NET MVC
- Blazor
- Console applications
- Background services

Inspired by the Mediator pattern described at refactoring.guru.

---

## Installation

Install via NuGet:
```
dotnet add package ChitMeo.Mediator
```

---

## Basic Idea

Instead of components talking directly to each other:
```
Controller → Service → Repository
```

They communicate through a mediator:
```
Controller → Mediator → Handler
```

This reduces coupling and improves maintainability.

---

## Example

### Request

```csharp
public record GetUserQuery(int Id);
//Handler
public class GetUserHandler : IRequestHandler<GetUserQuery, User>
{
    public Task<User> Handle(GetUserQuery request)
    {
        // handle logic
    }
}
```
Usage
```
var result = await mediator.Send(new GetUserQuery(1));
```

# Why ChitMeo.Mediator?

1. Lightweight

2. No heavy dependencies

3. Simple API

4. Works with any .NET project

5. Easy to understand and extend

# Use Cases

1. API request handling

2. CQRS style architecture

3. Decoupling services

4. Blazor component communication

5. Clean architecture

# Roadmap
TBD
