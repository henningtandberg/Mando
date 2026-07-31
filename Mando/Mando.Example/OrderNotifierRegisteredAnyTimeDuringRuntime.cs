using Mando.Event;

namespace Mando.Example;

// Registers a runtime event subscription on construction, and removes it on dispose.
// Contrast with the DI-registered IEventSubscriber<TEvent> subscribers, whose lifecycle
// is owned by the container. Here the owner holds the subscription handle and controls it.
internal sealed class OrderNotifierRegisteredAnyTimeDuringRuntime(IEventBus eventBus) : IDisposable
{
    private readonly IEventSubscription _subscription = eventBus.Subscribe<OrderPlaced>(OnOrderPlaced);

    private static Task OnOrderPlaced(OrderPlaced @event)
    {
        Console.WriteLine($"Notification: Order \"{@event.OrderId}\", has been placed!");
        return Task.CompletedTask;
    }

    // Removes the subscription so OnOrderPlaced is no longer invoked on publish.
    public void Dispose() => _subscription.Dispose();
}
