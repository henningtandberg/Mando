using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Setup;

namespace Mando.Tests.Event.Subscribers;

public sealed class MultiEventSubscriber(IStd std) :
    IEventSubscriber<EventOne>,
    IEventSubscriber<EventTwo>,
    IEventSubscriber<EventThree>,
    IEventCounter
{
    private int _eventCounter;

    public Task Handle(EventOne @event)
    {
        _eventCounter++;
        std.Write("MultiEventSubscriber handled EventOne, EventCount incremented");
        return Task.CompletedTask;
    }

    public Task Handle(EventTwo @event)
    {
        _eventCounter++;
        std.Write("MultiEventSubscriber handled EventTwo, EventCount incremented");
        return Task.CompletedTask;
    }

    public Task Handle(EventThree @event)
    {
        _eventCounter++;
        std.Write("MultiEventSubscriber handled EventThree, EventCount incremented");
        return Task.CompletedTask;
    }

    public int GetEventCount() => _eventCounter;
}

public interface IEventCounter
{
    public int GetEventCount();
}
