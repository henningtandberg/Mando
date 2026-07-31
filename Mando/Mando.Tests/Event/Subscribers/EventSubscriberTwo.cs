using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Setup;

namespace Mando.Tests.Event.Subscribers;

public sealed class EventSubscriberTwo(IStd std) : IEventSubscriber<EventTwo>
{
    public Task Handle(EventTwo @event)
    {
        std.Write("EventSubscriberTwo handled EventTwo!");
        return Task.CompletedTask;
    }
}
