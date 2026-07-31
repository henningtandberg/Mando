using System.Reflection;
using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests.Event.Subscribers;

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
            .AddSingleton<IEventCounter>(sp => sp.GetRequiredService<MultiEventSubscriber>())
            .BuildServiceProvider();

        _eventBus = _serviceProvider.GetRequiredService<IEventBus>();
    }

    [Fact]
    public async Task MultipleEvents_Publish_MultiEventSubscriberHandlesAll()
    {
        await _eventBus.Publish(new EventOne());
        await _eventBus.Publish(new EventTwo());
        await _eventBus.Publish(new EventThree());

        Assert.Contains("MultiEventSubscriber handled EventOne, EventCount incremented", _std.Out);
        Assert.Contains("MultiEventSubscriber handled EventTwo, EventCount incremented", _std.Out);
        Assert.Contains("MultiEventSubscriber handled EventThree, EventCount incremented", _std.Out);
    }
    
    [Fact]
    public async Task MultipleEvents_Publish_MultiEventSubscriberHasSingleAccumulatedState()
    {
        await _eventBus.Publish(new EventOne());
        await _eventBus.Publish(new EventTwo());
        await _eventBus.Publish(new EventThree());

        var multiEventSubscriber = _serviceProvider.GetRequiredService<IEventCounter>();
        Assert.Equal(3, multiEventSubscriber.GetEventCount());
    }

    [Fact]
    public void MultipleEventInterfaces_SameInstanceServedForAll()
    {
        var multiEventSubscribers = _serviceProvider
            .GetServices<MultiEventSubscriber>()
            .ToList();

        Assert.Single(multiEventSubscribers);
    }
}
