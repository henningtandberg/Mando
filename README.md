
<div align="center">

[//]: # (You must have a lf before the markdown element when inside a block for it to work: https://stackoverflow.com/questions/29368902/how-can-i-wrap-my-markdown-in-an-html-div)
# Mando
A super lightweight and free alternative to libraries that provide
decoupled, in-process communication.

<img src="./mando.png" width="50%">

[![Build and Test](https://github.com/henningtandberg/Mando/actions/workflows/build-test.yml/badge.svg)](https://github.com/henningtandberg/Mando/actions/workflows/build-test.yml)
![GitHub License](https://img.shields.io/github/license/henningtandberg/Mando)
![GitHub Release](https://img.shields.io/github/v/release/henningtandberg/Mando)
![NuGet Version](https://img.shields.io/nuget/v/Mando)
![NuGet Downloads](https://img.shields.io/nuget/dt/Mando)

</div>

<hr>

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

### Dependency Injection
```csharp
services.AddMando(Assembly.GetExecutingAssembly()))
```

This registers
- `ICommandHandler<TCommand>` : All implementations are registered as Scoped
- `IDispatcher` as Scoped

### Usage
```csharp
internal sealed class Application(IDispatcher dispatcher) : IApplication
{
    public Task RunAsync() => dispatcher.Dispatch(new DoSomethingCommand());
}
```

Checkout [Mando.Example](https://github.com/henningtandberg/Mando/tree/main/Mando/Mando.Example) for a working example.
