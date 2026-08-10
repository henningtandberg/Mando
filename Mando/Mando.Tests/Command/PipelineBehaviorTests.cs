using System.Collections.Concurrent;
using System.Reflection;
using Mando.Command;
using Mando.Tests.Setup;
using Microsoft.Extensions.DependencyInjection;

namespace Mando.Tests.Command;

public class PipelineBehaviorTests
{
    public PipelineBehaviorTests()
    {
        PipelineSpy.Reset();
    }

    private static IDispatcher BuildDispatcher(params Type[] behaviors)
    {
        var services = new ServiceCollection()
            .AddSingleton<IStd, FakeStd>()
            .AddMando(Assembly.GetExecutingAssembly());

        foreach (var behavior in behaviors)
            services.AddPipelineBehavior(behavior);

        return services.BuildServiceProvider().GetRequiredService<IDispatcher>();
    }

    [Fact]
    public async Task Behaviors_RunInRegistrationOrder_AroundHandler()
    {
        var dispatcher = BuildDispatcher(typeof(LogBehaviorA<>), typeof(LogBehaviorB<>));

        await dispatcher.Dispatch(new PipelineCommand());

        Assert.Equal(
            ["A:before", "B:before", "handler", "B:after", "A:after"],
            PipelineSpy.Log.ToArray());
    }

    [Fact]
    public async Task Behavior_ThatSkipsNext_ShortCircuitsHandler()
    {
        var dispatcher = BuildDispatcher(typeof(ShortCircuitBehavior<>));

        await dispatcher.Dispatch(new PipelineCommand());

        Assert.Equal(["short-circuit"], PipelineSpy.Log.ToArray());
        Assert.DoesNotContain("handler", PipelineSpy.Log);
    }

    [Fact]
    public async Task ResultBehavior_CanShortCircuit_WithCachedResult()
    {
        var dispatcher = BuildDispatcher(typeof(CachedResultBehavior));

        int result = await dispatcher.Dispatch(new PipelineResultCommand());

        Assert.Equal(99, result);
        Assert.DoesNotContain("result-handler", PipelineSpy.Log);
    }

    [Fact]
    public async Task ResultBehavior_CanTransform_HandlerResult()
    {
        var dispatcher = BuildDispatcher(typeof(AddFortyOneResultBehavior));

        int result = await dispatcher.Dispatch(new PipelineResultCommand());

        Assert.Equal(42, result);
        Assert.Contains("result-handler", PipelineSpy.Log);
    }

    [Fact]
    public async Task OpenGenericBehavior_AppliesToEveryCommand()
    {
        var dispatcher = BuildDispatcher(typeof(LogBehaviorA<>));

        await dispatcher.Dispatch(new PipelineCommand());
        await dispatcher.Dispatch(new OtherPipelineCommand());

        Assert.Equal(2, PipelineSpy.Log.Count(entry => entry == "A:before"));
    }

    [Fact]
    public async Task Behavior_ReceivesCancellationToken()
    {
        using var cancellation = new CancellationTokenSource();
        var dispatcher = BuildDispatcher(typeof(CaptureTokenBehavior<>));

        await dispatcher.Dispatch(new PipelineCommand(), cancellation.Token);

        Assert.Equal(cancellation.Token, PipelineSpy.CapturedToken);
    }

    [Fact]
    public async Task Behavior_WrapsMultiHandlerFanOut_ExactlyOnce()
    {
        var dispatcher = BuildDispatcher(typeof(LogBehaviorA<>));

        await dispatcher.Dispatch(new MultiHandlerCommand());

        Assert.Single(PipelineSpy.Log, entry => entry == "A:before");
        Assert.Single(PipelineSpy.Log, entry => entry == "A:after");
        Assert.Contains("handlerA", PipelineSpy.Log);
        Assert.Contains("handlerB", PipelineSpy.Log);
    }

    [Fact]
    public async Task Dispatch_WithoutBehaviors_StillInvokesHandler()
    {
        var dispatcher = BuildDispatcher();

        await dispatcher.Dispatch(new PipelineCommand());

        Assert.Equal(["handler"], PipelineSpy.Log.ToArray());
    }

    [Fact]
    public async Task HandlerException_PropagatesThroughBehaviors()
    {
        var dispatcher = BuildDispatcher(typeof(LogBehaviorA<>));

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => dispatcher.Dispatch(new FaultingPipelineCommand()));
    }

    [Fact]
    public async Task Behavior_CanCatch_HandlerException()
    {
        var dispatcher = BuildDispatcher(typeof(CatchingBehavior<>));

        await dispatcher.Dispatch(new FaultingPipelineCommand());

        Assert.Contains("caught:kaboom", PipelineSpy.Log);
    }
}

public static class PipelineSpy
{
    public static readonly ConcurrentQueue<string> Log = new();
    public static CancellationToken? CapturedToken;

    public static void Reset()
    {
        Log.Clear();
        CapturedToken = null;
    }
}

public sealed record PipelineCommand : ICommand;

public sealed class PipelineCommandHandler : ICommandHandler<PipelineCommand>
{
    public Task Execute(PipelineCommand command, CancellationToken cancellationToken)
    {
        PipelineSpy.Log.Enqueue("handler");
        return Task.CompletedTask;
    }
}

public sealed record OtherPipelineCommand : ICommand;

public sealed class OtherPipelineCommandHandler : ICommandHandler<OtherPipelineCommand>
{
    public Task Execute(OtherPipelineCommand command, CancellationToken cancellationToken)
    {
        PipelineSpy.Log.Enqueue("other-handler");
        return Task.CompletedTask;
    }
}

public sealed record MultiHandlerCommand : ICommand;

public sealed class MultiHandlerCommandHandlerA : ICommandHandler<MultiHandlerCommand>
{
    public Task Execute(MultiHandlerCommand command, CancellationToken cancellationToken)
    {
        PipelineSpy.Log.Enqueue("handlerA");
        return Task.CompletedTask;
    }
}

public sealed class MultiHandlerCommandHandlerB : ICommandHandler<MultiHandlerCommand>
{
    public Task Execute(MultiHandlerCommand command, CancellationToken cancellationToken)
    {
        PipelineSpy.Log.Enqueue("handlerB");
        return Task.CompletedTask;
    }
}

public sealed record FaultingPipelineCommand : ICommand;

public sealed class FaultingPipelineCommandHandler : ICommandHandler<FaultingPipelineCommand>
{
    public Task Execute(FaultingPipelineCommand command, CancellationToken cancellationToken)
        => throw new InvalidOperationException("kaboom");
}

public sealed record PipelineResultCommand : ICommand<int>;

public sealed class PipelineResultCommandHandler : ICommandHandler<PipelineResultCommand, int>
{
    public Task<int> Execute(PipelineResultCommand command, CancellationToken cancellationToken)
    {
        PipelineSpy.Log.Enqueue("result-handler");
        return Task.FromResult(1);
    }
}

public sealed class LogBehaviorA<TCommand> : IPipelineBehavior<TCommand> where TCommand : ICommand
{
    public async Task Handle(TCommand command, CommandHandlerDelegate next, CancellationToken cancellationToken)
    {
        PipelineSpy.Log.Enqueue("A:before");
        await next();
        PipelineSpy.Log.Enqueue("A:after");
    }
}

public sealed class LogBehaviorB<TCommand> : IPipelineBehavior<TCommand> where TCommand : ICommand
{
    public async Task Handle(TCommand command, CommandHandlerDelegate next, CancellationToken cancellationToken)
    {
        PipelineSpy.Log.Enqueue("B:before");
        await next();
        PipelineSpy.Log.Enqueue("B:after");
    }
}

public sealed class ShortCircuitBehavior<TCommand> : IPipelineBehavior<TCommand> where TCommand : ICommand
{
    public Task Handle(TCommand command, CommandHandlerDelegate next, CancellationToken cancellationToken)
    {
        PipelineSpy.Log.Enqueue("short-circuit");
        return Task.CompletedTask;
    }
}

public sealed class CaptureTokenBehavior<TCommand> : IPipelineBehavior<TCommand> where TCommand : ICommand
{
    public Task Handle(TCommand command, CommandHandlerDelegate next, CancellationToken cancellationToken)
    {
        PipelineSpy.CapturedToken = cancellationToken;
        return next();
    }
}

public sealed class CatchingBehavior<TCommand> : IPipelineBehavior<TCommand> where TCommand : ICommand
{
    public async Task Handle(TCommand command, CommandHandlerDelegate next, CancellationToken cancellationToken)
    {
        try
        {
            await next();
        }
        catch (Exception exception)
        {
            PipelineSpy.Log.Enqueue($"caught:{exception.Message}");
        }
    }
}

public sealed class CachedResultBehavior : IPipelineBehavior<PipelineResultCommand, int>
{
    public Task<int> Handle(PipelineResultCommand command, CommandHandlerDelegate<int> next, CancellationToken cancellationToken)
        => Task.FromResult(99);
}

public sealed class AddFortyOneResultBehavior : IPipelineBehavior<PipelineResultCommand, int>
{
    public async Task<int> Handle(PipelineResultCommand command, CommandHandlerDelegate<int> next, CancellationToken cancellationToken)
        => await next() + 41;
}
