using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Setup;

namespace Mando.Tests.Event.Subscribers;

public sealed class EventSubscriberOne(IStd std) : IEventSubscriber<EventOne>
{
    public Task Handle(EventOne @event)
    {
        std.Write("EventSubscriberOne handled EventOne!");
        return Task.CompletedTask;
    }
}
