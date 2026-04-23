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
}
