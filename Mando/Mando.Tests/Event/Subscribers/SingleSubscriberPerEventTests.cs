using System.Reflection;
using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests.Event.Subscribers;

public class SingleSubscriberPerEventTests
{
    private readonly FakeStd _std = new();
    private readonly IEventBus _eventBus;

    public SingleSubscriberPerEventTests()
    {
        _eventBus = new ServiceCollection()
            .AddSingleton<IStd>(_std)
            .AddMando(Assembly.GetExecutingAssembly())
            .BuildServiceProvider()
            .GetRequiredService<IEventBus>();
    }

    [Fact]
    public async Task EventOne_Publish_OnlyEventSubscriberOneHandles()
    {
        await _eventBus.Publish(new EventOne());

        Assert.Equal("EventSubscriberOne handled EventOne!", _std.Out.First());
    }

    [Fact]
    public async Task EventTwo_Publish_OnlyEventSubscriberTwoHandles()
    {
        await _eventBus.Publish(new EventTwo());

        Assert.Equal("EventSubscriberTwo handled EventTwo!", _std.Out.First());
    }

    [Fact]
    public async Task EventThree_Publish_OnlyEventSubscriberThreeHandles()
    {
        await _eventBus.Publish(new EventThree());

        Assert.Equal("EventSubscriberThree handled EventThree!", _std.Out.First());
    }

    [Fact]
    public async Task NoSubscriberEvent_Publish_CompletesWithoutException()
    {
        await _eventBus.Publish(new NoSubscriberEvent());

        Assert.Empty(_std.Out);
    }
}
