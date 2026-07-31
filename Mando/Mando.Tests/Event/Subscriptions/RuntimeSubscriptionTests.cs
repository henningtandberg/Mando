using System.Reflection;
using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests.Event.Subscriptions;

public class RuntimeSubscriptionTests
{
    private readonly FakeStd _std = new();
    private readonly IEventBus _eventBus;

    public RuntimeSubscriptionTests()
    {
        _eventBus = new ServiceCollection()
            .AddSingleton<IStd>(_std)
            .AddMando(Assembly.GetExecutingAssembly())
            .BuildServiceProvider()
            .GetRequiredService<IEventBus>();
    }

    [Fact]
    public async Task Subscribe_HandlerInvokedOnPublish()
    {
        var handled = 0;
        _eventBus.Subscribe<EventOne>(_ =>
        {
            handled++;
            return Task.CompletedTask;
        });

        await _eventBus.Publish(new EventOne());

        Assert.Equal(1, handled);
    }

    [Fact]
    public async Task Subscribe_NullHandler_Throws()
    {
        Assert.Throws<ArgumentNullException>(() =>
            _eventBus.Subscribe<EventOne>(null!));

        await Task.CompletedTask;
    }

    [Fact]
    public async Task Dispose_HandlerNotInvokedAfterwards()
    {
        var handled = 0;
        var subscription = _eventBus.Subscribe<EventOne>(_ =>
        {
            handled++;
            return Task.CompletedTask;
        });

        await _eventBus.Publish(new EventOne());
        subscription.Dispose();
        await _eventBus.Publish(new EventOne());

        Assert.Equal(1, handled);
    }

    [Fact]
    public async Task Dispose_IsIdempotent()
    {
        var handled = 0;
        var subscription = _eventBus.Subscribe<EventOne>(_ =>
        {
            handled++;
            return Task.CompletedTask;
        });

        subscription.Dispose();
        subscription.Dispose();

        await _eventBus.Publish(new EventOne());

        Assert.Equal(0, handled);
    }

    [Fact]
    public async Task DisposeOneSubscription_OthersStillInvoked()
    {
        var first = 0;
        var second = 0;
        var firstSubscription = _eventBus.Subscribe<EventOne>(_ =>
        {
            first++;
            return Task.CompletedTask;
        });
        _eventBus.Subscribe<EventOne>(_ =>
        {
            second++;
            return Task.CompletedTask;
        });

        firstSubscription.Dispose();
        await _eventBus.Publish(new EventOne());

        Assert.Equal(0, first);
        Assert.Equal(1, second);
    }

    [Fact]
    public async Task RuntimeAndDependencyInjectionSubscribers_BothInvoked()
    {
        var runtimeHandled = 0;
        _eventBus.Subscribe<EventOne>(_ =>
        {
            runtimeHandled++;
            return Task.CompletedTask;
        });

        await _eventBus.Publish(new EventOne());

        Assert.Equal(1, runtimeHandled);
        Assert.Contains("EventSubscriberOne handled EventOne!", _std.Out);
    }

    [Fact]
    public async Task Subscribe_IsScopedPerEventType()
    {
        var handled = 0;
        _eventBus.Subscribe<EventTwo>(_ =>
        {
            handled++;
            return Task.CompletedTask;
        });

        await _eventBus.Publish(new EventOne());

        Assert.Equal(0, handled);
    }
}
