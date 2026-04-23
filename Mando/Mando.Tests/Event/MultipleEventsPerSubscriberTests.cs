using System.Reflection;
using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Event.EventSubscribers;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests.Event;

public class MultipleEventsPerSubscriberTests
{
    private readonly FakeStd _std = new();
    private readonly IEventBus _eventBus;
    private readonly ServiceProvider _serviceProvider;

    public MultipleEventsPerSubscriberTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddSingleton<IStd>(_std)
            .AddMando(Assembly.GetExecutingAssembly())
            .BuildServiceProvider();

        _eventBus = _serviceProvider.GetRequiredService<IEventBus>();
    }

    [Fact]
    public async Task MultipleEvents_Publish_MultiEventSubscriberHandlesAll()
    {
        await _eventBus.Publish(new MultiEvent1());
        await _eventBus.Publish(new MultiEvent2());
        await _eventBus.Publish(new MultiEvent3());

        Assert.Equal(3, _std.Out.Count);
    }

    [Fact]
    public void MultipleEventInterfaces_SameInstanceServedForAll()
    {
        var sub1 = _serviceProvider.GetRequiredService<IEventSubscriber<MultiEvent1>>();
        var sub2 = _serviceProvider.GetRequiredService<IEventSubscriber<MultiEvent2>>();
        var sub3 = _serviceProvider.GetRequiredService<IEventSubscriber<MultiEvent3>>();

        Assert.Same(sub1, sub2);
        Assert.Same(sub1, sub3);
        Assert.IsType<MultiEventSubscriber>(sub1);
    }
}
