# Mando
A super lightweight and free alternative to libraries that provide
decoupled, in-process communication.

![Mando Logo](mando.png)

[![Build and Test](https://github.com/henningtandberg/Mando/actions/workflows/build-test.yml/badge.svg)](https://github.com/henningtandberg/Mando/actions/workflows/build-test.yml)
![GitHub License](https://img.shields.io/github/license/henningtandberg/Mando)
![GitHub Release](https://img.shields.io/github/v/release/henningtandberg/Mando)
![NuGet Version](https://img.shields.io/nuget/v/Mando)
![NuGet Downloads](https://img.shields.io/nuget/dt/Mando)

---

## Usage
You can use the package directly or fork this repo and create your own,
custom implementation.

## Installation
The easiest way to add Mando to your project via [NuGet](https://www.nuget.org/packages/Mando).

## Features
The features of this library are limited by design, so that the library will
be easier to extend if you decide to fork the repo and create a more custom
implementation. The core features are, and will always be:

### Single command, single handler.
```csharp
internal sealed MyCustomCommand : ICommand;

internal sealed MyCustomCommandHandler : ICommandHandler<MyCustomCommand>
{
    public Task Execute(MyCustomCommand command)
    {
        // Your magic here!
    }
}
```

### Single command, multiple handlers.
```csharp
internal sealed MyCustomCommand : ICommand;

internal sealed MyCustomCommandHandlerOne : ICommandHandler<MyCustomCommand>
{
    public Task Execute(MyCustomCommand command)
    {
        // Your magic here!
    }
}

internal sealed MyCustomCommandHandlerTwo : ICommandHandler<MyCustomCommand>
{
    public Task Execute(MyCustomCommand command)
    {
        // And some other magic here!
    }
}
```

### Multiple commands, single handler

```csharp
internal sealed MyCustomCommandOne : ICommand;
internal sealed MyCustomCommandTwo : ICommand;

internal sealed MyCustomCommandHandler :
    ICommandHandler<MyCustomCommandOne>, ICommandHandler<MyCustomCommandTwo>
{
    public Task Execute(MyCustomCommandOne command)
    {
        // Your magic here!
    }
    
    public Task Execute(MyCustomCommandTwo command)
    {
        // Even more magic here!
    }
}
```

### Command with a result
```csharp
internal sealed record GetAnswerCommand : ICommand<int>;

internal sealed class GetAnswerCommandHandler : ICommandHandler<GetAnswerCommand, int>
{
    public Task<int> Execute(GetAnswerCommand command)
    {
        return Task.FromResult(42);
    }
}

// Dispatch and receive the result
int answer = await dispatcher.Dispatch(new GetAnswerCommand());
```

### Events (publish / subscribe)
Events are one-way notifications. A publisher fires an event and every subscriber
for that event type is notified, staying fully decoupled from each other.

Subscriber registered via Dependency Injection:
```csharp
internal sealed record OrderPlaced(int OrderId) : IEvent;

// Discovered and registered automatically by AddMando
internal sealed class OrderNotifier : IEventSubscriber<OrderPlaced>
{
    public Task Handle(OrderPlaced @event)
    {
        Console.WriteLine($"Order \"{@event.OrderId}\" has been placed!");
        return Task.CompletedTask;
    }
}

// Publish through IEventBus. All subscribers of OrderPlaced are notified.
await eventBus.Publish(new OrderPlaced(1));
```

Subscribe at runtime, anywhere:
```csharp
// Subscribe at any point during execution, not only during DI setup.
// Dispose the returned handle to remove the subscription.
IEventSubscription subscription = eventBus.Subscribe<OrderPlaced>(@event =>
{
    Console.WriteLine($"Order \"{@event.OrderId}\" has been placed!");
    return Task.CompletedTask;
});

await eventBus.Publish(new OrderPlaced(1)); // handler fires

subscription.Dispose(); // handler no longer fires
```

Runtime subscriptions run alongside DI-registered subscribers. Subscribing and
disposing are thread-safe, and safe to call concurrently with `Publish`.

### Dependency Injection
```csharp
services.AddMando(Assembly.GetExecutingAssembly()))
```

This registers
- `ICommandHandler<TCommand>` and `ICommandHandler<TCommand, TResult>` : All implementations are registered as Scoped
- `IDispatcher` as Scoped
- `IEventSubscriber<TEvent>` : All implementations are registered as Singleton (one instance shared across all subscribed events)
- `IEventBus` as Singleton

### Usage
```csharp
internal sealed class Application(IDispatcher dispatcher, IEventBus eventBus) : IApplication
{
    public async Task RunAsync()
    {
        await dispatcher.Dispatch(new DoSomethingCommand());
        await eventBus.Publish(new OrderPlaced(1));
    }
}
```

Checkout [Mando.Example](https://github.com/henningtandberg/Mando/tree/main/Mando/Mando.Example) for a working example.
