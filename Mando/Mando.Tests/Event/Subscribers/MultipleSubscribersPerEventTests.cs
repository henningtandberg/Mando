using System.Reflection;
using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests.Event.Subscribers;

public class MultipleSubscribersPerEventTests
{
    private readonly FakeStd _std = new();
    private readonly IEventBus _eventBus;

    public MultipleSubscribersPerEventTests()
    {
        _eventBus = new ServiceCollection()
            .AddSingleton<IStd>(_std)
            .AddMando(Assembly.GetExecutingAssembly())
            .BuildServiceProvider()
            .GetRequiredService<IEventBus>();
    }

    [Fact]
    public async Task AlertEvent_Publish_AllThreeSubscribersHandle()
    {
        await _eventBus.Publish(new AlertEvent("red alert"));

        Assert.Equal(3, _std.Out.Count);
        Assert.Equivalent(
            new[]
            {
                "AlertSubscriberA: red alert",
                "AlertSubscriberB: red alert",
                "AlertSubscriberC: red alert"
            },
            _std.Out,
            strict: true);
    }
}
