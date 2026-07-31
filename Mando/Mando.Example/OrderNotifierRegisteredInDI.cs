using Mando.Event;

namespace Mando.Example;

// Registers a runtime event subscription on construction, and removes it on dispose.
// Contrast with the DI-registered IEventSubscriber<TEvent> subscribers, whose lifecycle
// is owned by the container. Here the owner holds the subscription handle and controls it.
internal sealed class OrderNotifierRegisteredInDI : IEventSubscriber<OrderPlaced>
{
    public Task Handle(OrderPlaced @event)
    {
        Console.WriteLine($"Notification: Order \"{@event.OrderId}\", has been placed!");
        return Task.CompletedTask;
    }
}
