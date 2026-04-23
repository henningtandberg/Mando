using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Setup;

namespace Mando.Tests.Event.EventSubscribers;

public sealed class EventSubscriberThree(IStd std) : IEventSubscriber<EventThree>
{
    public Task Handle(EventThree @event)
    {
        std.Write("EventSubscriberThree handled EventThree!");
        return Task.CompletedTask;
    }
}
