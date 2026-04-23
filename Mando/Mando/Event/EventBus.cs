using Microsoft.Extensions.DependencyInjection;

namespace Mando.Event;

internal sealed class EventBus(IServiceProvider serviceProvider) : IEventBus
{
    public async Task Publish<TEvent>(TEvent @event) where TEvent : IEvent
    {
        var subscribers = serviceProvider.GetServices<IEventSubscriber<TEvent>>().ToList();

        if (subscribers.Count == 0)
            return;

        await Task.WhenAll(subscribers.Select(s => s.Handle(@event)));
    }
}
