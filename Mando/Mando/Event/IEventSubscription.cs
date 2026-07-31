namespace Mando.Event;

/// <summary>
/// A handle to a runtime event subscription created via <see cref="IEventBus.Subscribe{TEvent}"/>.
/// Dispose the handle to remove the subscription so its handler is no longer invoked on publish.
/// Disposing is idempotent and thread-safe.
/// </summary>
public interface IEventSubscription : IDisposable;
