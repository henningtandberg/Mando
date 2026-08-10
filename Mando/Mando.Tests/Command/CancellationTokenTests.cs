using System.Collections.Concurrent;
using System.Reflection;
using Mando.Command;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests.Command;

public class CancellationTokenTests
{
    private readonly IDispatcher _dispatcher;

    public CancellationTokenTests()
    {
        _dispatcher = new ServiceCollection()
            .AddSingleton<IStd, FakeStd>()
            .AddMando(Assembly.GetExecutingAssembly())
            .BuildServiceProvider()
            .GetRequiredService<IDispatcher>();
    }

    [Fact]
    public async Task Dispatch_ForwardsSameToken_ToVoidHandler()
    {
        using var cancellation = new CancellationTokenSource();
        CaptureTokenHandler.Captured = null;

        await _dispatcher.Dispatch(new CaptureTokenCommand(), cancellation.Token);

        Assert.Equal(cancellation.Token, CaptureTokenHandler.Captured);
    }

    [Fact]
    public async Task Dispatch_WithoutToken_PassesNoneToVoidHandler()
    {
        CaptureTokenHandler.Captured = null;

        await _dispatcher.Dispatch(new CaptureTokenCommand());

        Assert.Equal(CancellationToken.None, CaptureTokenHandler.Captured);
    }

    [Fact]
    public async Task Dispatch_ForwardsToken_ToResultHandler()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        bool cancellationRequested = await _dispatcher.Dispatch(new CancelAwareCommand(), cancellation.Token);

        Assert.True(cancellationRequested);
    }

    [Fact]
    public async Task Dispatch_PreCancelledToken_SurfacesOperationCanceledException()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _dispatcher.Dispatch(new ThrowIfCancelledCommand(), cancellation.Token));
    }

    [Fact]
    public async Task Dispatch_ForwardsSameToken_ToEveryHandler()
    {
        using var cancellation = new CancellationTokenSource();
        FanOutSpy.Captured.Clear();

        await _dispatcher.Dispatch(new FanOutCommand(), cancellation.Token);

        Assert.Equal(2, FanOutSpy.Captured.Count);
        Assert.All(FanOutSpy.Captured, token => Assert.Equal(cancellation.Token, token));
    }

    [Fact]
    public async Task Dispatch_WithoutToken_PassesNoneToResultHandler()
    {
        bool cancellationRequested = await _dispatcher.Dispatch(new CancelAwareCommand());

        Assert.False(cancellationRequested);
    }

    [Fact]
    public async Task Dispatch_ResultHandler_PreCancelledToken_SurfacesOperationCanceledException()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => _dispatcher.Dispatch(new ThrowIfCancelledResultCommand(), cancellation.Token));
    }

    [Fact]
    public async Task Dispatch_VoidHandlerThrows_SurfacesOriginalException()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _dispatcher.Dispatch(new FaultingCommand()));

        Assert.Equal("boom", exception.Message);
    }

    [Fact]
    public async Task Dispatch_ResultHandlerThrows_SurfacesOriginalException()
    {
        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _dispatcher.Dispatch(new FaultingResultCommand()));

        Assert.Equal("boom result", exception.Message);
    }

    [Fact]
    public async Task Dispatch_TokenCancelledMidFlight_SurfacesOperationCanceledException()
    {
        using var cancellation = new CancellationTokenSource();

        var dispatch = _dispatcher.Dispatch(new NeverCompletesCommand(), cancellation.Token);
        await cancellation.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => dispatch);
    }
}

public sealed record CaptureTokenCommand : ICommand;

public sealed class CaptureTokenHandler : ICommandHandler<CaptureTokenCommand>
{
    // Set by the handler on invocation, read by the test. Only this test class dispatches
    // CaptureTokenCommand, and its methods run sequentially, so the shared field is safe here.
    public static CancellationToken? Captured;

    public Task Execute(CaptureTokenCommand command, CancellationToken cancellationToken)
    {
        Captured = cancellationToken;
        return Task.CompletedTask;
    }
}

public sealed record CancelAwareCommand : ICommand<bool>;

public sealed class CancelAwareHandler : ICommandHandler<CancelAwareCommand, bool>
{
    public Task<bool> Execute(CancelAwareCommand command, CancellationToken cancellationToken)
        => Task.FromResult(cancellationToken.IsCancellationRequested);
}

public sealed record ThrowIfCancelledCommand : ICommand;

public sealed class ThrowIfCancelledHandler : ICommandHandler<ThrowIfCancelledCommand>
{
    public Task Execute(ThrowIfCancelledCommand command, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.CompletedTask;
    }
}

public sealed record ThrowIfCancelledResultCommand : ICommand<int>;

public sealed class ThrowIfCancelledResultHandler : ICommandHandler<ThrowIfCancelledResultCommand, int>
{
    public Task<int> Execute(ThrowIfCancelledResultCommand command, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(0);
    }
}

public static class FanOutSpy
{
    // Every FanOut handler records the token it received. Only this test class dispatches
    // FanOutCommand, so the shared collection is safe here.
    public static readonly ConcurrentBag<CancellationToken> Captured = [];
}

public sealed record FanOutCommand : ICommand;

public sealed class FanOutHandlerA : ICommandHandler<FanOutCommand>
{
    public Task Execute(FanOutCommand command, CancellationToken cancellationToken)
    {
        FanOutSpy.Captured.Add(cancellationToken);
        return Task.CompletedTask;
    }
}

public sealed class FanOutHandlerB : ICommandHandler<FanOutCommand>
{
    public Task Execute(FanOutCommand command, CancellationToken cancellationToken)
    {
        FanOutSpy.Captured.Add(cancellationToken);
        return Task.CompletedTask;
    }
}

public sealed record FaultingCommand : ICommand;

public sealed class FaultingHandler : ICommandHandler<FaultingCommand>
{
    public Task Execute(FaultingCommand command, CancellationToken cancellationToken)
        => throw new InvalidOperationException("boom");
}

public sealed record FaultingResultCommand : ICommand<int>;

public sealed class FaultingResultHandler : ICommandHandler<FaultingResultCommand, int>
{
    public Task<int> Execute(FaultingResultCommand command, CancellationToken cancellationToken)
        => throw new InvalidOperationException("boom result");
}

public sealed record NeverCompletesCommand : ICommand;

public sealed class NeverCompletesHandler : ICommandHandler<NeverCompletesCommand>
{
    public Task Execute(NeverCompletesCommand command, CancellationToken cancellationToken)
        => Task.Delay(Timeout.Infinite, cancellationToken);
}
