using System.Reflection;
using Mando.Event;
using Mando.Tests.Event.Events;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests.Event.Subscriptions;

public class RuntimeSubscriptionConcurrencyTests
{
    private readonly FakeStd _std = new();
    private readonly IEventBus _eventBus;

    public RuntimeSubscriptionConcurrencyTests()
    {
        _eventBus = new ServiceCollection()
            .AddSingleton<IStd>(_std)
            .AddMando(Assembly.GetExecutingAssembly())
            .BuildServiceProvider()
            .GetRequiredService<IEventBus>();
    }

    [Fact]
    public async Task ConcurrentSubscribe_NoRegistrationsLost()
    {
        const int subscriberCount = 500;
        var handled = 0;

        await Parallel.ForEachAsync(
            Enumerable.Range(0, subscriberCount),
            (_, _) =>
            {
                _eventBus.Subscribe<EventOne>(_ =>
                {
                    Interlocked.Increment(ref handled);
                    return Task.CompletedTask;
                });
                return ValueTask.CompletedTask;
            });

        await _eventBus.Publish(new EventOne());

        Assert.Equal(subscriberCount, handled);
    }

    [Fact]
    public async Task ConcurrentDispose_AllRemoved()
    {
        const int subscriberCount = 500;
        var handled = 0;

        var subscriptions = Enumerable.Range(0, subscriberCount)
            .Select(_ => _eventBus.Subscribe<EventOne>(_ =>
            {
                Interlocked.Increment(ref handled);
                return Task.CompletedTask;
            }))
            .ToList();

        await Parallel.ForEachAsync(
            subscriptions,
            (subscription, _) =>
            {
                subscription.Dispose();
                return ValueTask.CompletedTask;
            });

        await _eventBus.Publish(new EventOne());

        Assert.Equal(0, handled);
    }

    [Fact]
    public async Task ConcurrentSubscribeDisposeAndPublish_DoesNotThrow()
    {
        using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(2));
        var token = cancellation.Token;

        // Churn: continuously subscribe then dispose.
        var churn = Enumerable.Range(0, 8).Select(_ => Task.Run(() =>
        {
            while (!token.IsCancellationRequested)
            {
                var subscription = _eventBus.Subscribe<EventOne>(_ => Task.CompletedTask);
                subscription.Dispose();
            }
        })).ToList();

        // Publishers: continuously publish while the set of subscriptions mutates underneath.
        var publishers = Enumerable.Range(0, 8).Select(_ => Task.Run(async () =>
        {
            while (!token.IsCancellationRequested)
                await _eventBus.Publish(new EventOne());
        })).ToList();

        // Should complete without any exception (torn snapshots, lost updates, etc.).
        await Task.WhenAll(churn.Concat(publishers));
    }
}
