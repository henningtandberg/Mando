using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Setup;

namespace Mando.Tests.Event.EventSubscribers;

public sealed class MultiEventSubscriber(IStd std) :
    IEventSubscriber<MultiEvent1>,
    IEventSubscriber<MultiEvent2>,
    IEventSubscriber<MultiEvent3>
{
    private int _handleCount;

    public int HandleCount => _handleCount;

    public Task Handle(MultiEvent1 @event)
    {
        _handleCount++;
        std.Write($"MultiEventSubscriber handled MultiEvent1 (count: {_handleCount})");
        return Task.CompletedTask;
    }

    public Task Handle(MultiEvent2 @event)
    {
        _handleCount++;
        std.Write($"MultiEventSubscriber handled MultiEvent2 (count: {_handleCount})");
        return Task.CompletedTask;
    }

    public Task Handle(MultiEvent3 @event)
    {
        _handleCount++;
        std.Write($"MultiEventSubscriber handled MultiEvent3 (count: {_handleCount})");
        return Task.CompletedTask;
    }
}
