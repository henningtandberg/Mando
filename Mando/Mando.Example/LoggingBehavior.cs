using Mando.Command;

namespace Mando.Example;

internal sealed class LoggingBehavior<TCommand> : IPipelineBehavior<TCommand> where TCommand : ICommand
{
    public async Task Handle(TCommand command, CommandHandlerDelegate next, CancellationToken cancellationToken)
    {
        Console.WriteLine($"[pipeline] handling {typeof(TCommand).Name}");
        await next();
        Console.WriteLine($"[pipeline] handled {typeof(TCommand).Name}");
    }
}
