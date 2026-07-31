using Mando.Event;

namespace Mando.Example;

internal sealed class OrderNotifierRegisteredInDI : IEventSubscriber<OrderPlaced>
{
    public Task Handle(OrderPlaced @event)
    {
        Console.WriteLine($"Notification: Order \"{@event.OrderId}\", has been placed!");
        return Task.CompletedTask;
    }
}
