using System.Collections.Concurrent;
using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Event;

internal sealed class EventBus(IServiceProvider serviceProvider) : IEventBus
{
    // Runtime subscriptions keyed by event type. Each value is an immutable snapshot swapped
    // atomically, so Publish can enumerate lock-free while Subscribe/Dispose mutate concurrently.
    private readonly ConcurrentDictionary<Type, ImmutableArray<RuntimeSubscription>> _runtimeSubscriptions = new();

    public async Task Publish<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var subscribers = serviceProvider.GetServices<IEventSubscriber<TEvent>>();

        var handleTasks = subscribers.Select(s => s.Handle(@event));

        if (_runtimeSubscriptions.TryGetValue(typeof(TEvent), out var runtime))
            handleTasks = handleTasks.Concat(runtime.Select(s => s.Invoke(@event)));

        await Task.WhenAll(handleTasks);
    }

    public IEventSubscription Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent
    {
        ArgumentNullException.ThrowIfNull(handler);

        var subscription = new RuntimeSubscription(this, typeof(TEvent), @event => handler((TEvent)@event));

        _runtimeSubscriptions.AddOrUpdate(
            typeof(TEvent),
            _ => [subscription],
            (_, existing) => existing.Add(subscription));

        return subscription;
    }

    private void Remove(Type eventType, RuntimeSubscription subscription)
    {
        // Atomic compare-and-swap loop (handled internally by AddOrUpdate). Removing a subscription
        // produces a new immutable snapshot, leaving any in-flight Publish enumeration untouched.
        _runtimeSubscriptions.AddOrUpdate(
            eventType,
            _ => [],
            (_, existing) => existing.Remove(subscription));
    }

    private sealed class RuntimeSubscription(EventBus bus, Type eventType, Func<IEvent, Task> handler)
        : IEventSubscription
    {
        private int _disposed;

        public Task Invoke(IEvent @event) => handler(@event);

        public void Dispose()
        {
            // Idempotent: only the first Dispose removes the subscription.
            if (Interlocked.Exchange(ref _disposed, 1) == 0)
                bus.Remove(eventType, this);
        }
    }
}
