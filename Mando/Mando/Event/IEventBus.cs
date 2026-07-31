namespace Mando.Event;

/// <summary>
/// Responsible for publishing events to all registered subscribers,
/// keeping publishers decoupled from subscribers.
/// </summary>
public interface IEventBus
{
    /// <summary>
    /// Publishes an event to all subscribers registered for the given event type.
    /// If no subscribers are registered, this is a no-op.
    /// </summary>
    /// <typeparam name="TEvent">The event type</typeparam>
    /// <param name="event">The event to publish</param>
    /// <returns>Task</returns>
    public Task Publish<TEvent>(TEvent @event) where TEvent : IEvent;

    /// <summary>
    /// Subscribes a handler to an event type at runtime. The handler is invoked, alongside any
    /// dependency-injection registered <see cref="IEventSubscriber{TEvent}"/>, whenever an event
    /// of the given type is published.
    /// </summary>
    /// <typeparam name="TEvent">The event type to subscribe to</typeparam>
    /// <param name="handler">The handler to invoke when the event is published</param>
    /// <returns>
    /// A subscription handle. Dispose it to remove the subscription. Safe to call concurrently
    /// with <see cref="Publish{TEvent}"/> and other subscribe/dispose calls.
    /// </returns>
    public IEventSubscription Subscribe<TEvent>(Func<TEvent, Task> handler) where TEvent : IEvent;
}
