namespace Mando.Event;

/// <summary>
/// Defines the procedure to execute when a given event is published.
/// </summary>
/// <typeparam name="TEvent">Type of event to subscribe to</typeparam>
public interface IEventSubscriber<in TEvent> : IEventSubscriber where TEvent : IEvent
{
    /// <summary>
    /// The procedure to execute when the event is published.
    /// </summary>
    /// <param name="event">The published event</param>
    /// <returns>Task</returns>
    public Task Handle(TEvent @event);
}

/// <summary>
/// Marker interface used during Dependency Injection setup to enumerate all subscribers
/// without needing to know their specific event types.
/// </summary>
public interface IEventSubscriber;
