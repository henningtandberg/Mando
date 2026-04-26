using System.Reflection;
using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Event.EventSubscribers;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests.Event;

public class SubscriberStatePersistsTests
{
    private readonly IEventBus _eventBus;
    private readonly ServiceProvider _serviceProvider;

    public SubscriberStatePersistsTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddSingleton<IStd, FakeStd>()
            .AddMando(Assembly.GetExecutingAssembly())
            .AddSingleton<ICounter>(sp => sp.GetRequiredService<MultiEventSubscriber>())
            .BuildServiceProvider();

        _eventBus = _serviceProvider.GetRequiredService<IEventBus>();
    }

    [Fact]
    public async Task PublishMultipleTimes_SubscriberStateAccumulates()
    {
        await _eventBus.Publish(new MultiEvent1());
        await _eventBus.Publish(new MultiEvent2());
        await _eventBus.Publish(new MultiEvent3());

        var counter = _serviceProvider.GetRequiredService<ICounter>();
        Assert.Equal(3, counter.GetCount());
    }
}
