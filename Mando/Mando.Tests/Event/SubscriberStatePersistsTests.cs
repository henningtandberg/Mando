using System.Reflection;
using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Event.EventSubscribers;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests.Event;

public class SubscriberStatePersistsTests
{
    private readonly FakeStd _std = new();
    private readonly IEventBus _eventBus;
    private readonly ServiceProvider _serviceProvider;

    public SubscriberStatePersistsTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddSingleton<IStd>(_std)
            .AddMando(Assembly.GetExecutingAssembly())
            .BuildServiceProvider();

        _eventBus = _serviceProvider.GetRequiredService<IEventBus>();
    }

    [Fact]
    public async Task PublishMultipleTimes_SubscriberStateAccumulates()
    {
        await _eventBus.Publish(new MultiEvent1());
        await _eventBus.Publish(new MultiEvent2());
        await _eventBus.Publish(new MultiEvent3());

        var subscriber = _serviceProvider.GetRequiredService<MultiEventSubscriber>();
        Assert.Equal(3, subscriber.HandleCount);
    }
}
