using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Setup;

namespace Mando.Tests.Event.EventSubscribers;

public sealed class AlertSubscriberA(IStd std) : IEventSubscriber<AlertEvent>
{
    public Task Handle(AlertEvent @event)
    {
        std.Write($"AlertSubscriberA: {@event.Message}");
        return Task.CompletedTask;
    }
}
