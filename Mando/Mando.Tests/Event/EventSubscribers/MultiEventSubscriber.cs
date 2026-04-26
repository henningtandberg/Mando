using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Setup;

namespace Mando.Tests.Event.EventSubscribers;

public sealed class MultiEventSubscriber(IStd std) :
    IEventSubscriber<MultiEvent1>,
    IEventSubscriber<MultiEvent2>,
    IEventSubscriber<MultiEvent3>,
    ICounter
{
    private int _counter;

    public Task Handle(MultiEvent1 @event)
    {
        _counter++;
        std.Write($"MultiEventSubscriber handled MultiEvent1 (count: {_counter})");
        return Task.CompletedTask;
    }

    public Task Handle(MultiEvent2 @event)
    {
        _counter++;
        std.Write($"MultiEventSubscriber handled MultiEvent2 (count: {_counter})");
        return Task.CompletedTask;
    }

    public Task Handle(MultiEvent3 @event)
    {
        _counter++;
        std.Write($"MultiEventSubscriber handled MultiEvent3 (count: {_counter})");
        return Task.CompletedTask;
    }

    public int GetCount() => _counter;
}

public interface ICounter
{
    public int GetCount();
}
