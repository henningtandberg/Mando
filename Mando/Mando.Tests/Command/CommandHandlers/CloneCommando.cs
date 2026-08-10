using Mando.Command;
using Mando.Tests.Command.Commands;
using Mando.Tests.Setup;

namespace Mando.Tests.Command.CommandHandlers;

public sealed class CloneCommando(IStd std) : ICommandHandler<Order66>
{
    public Task Execute(Order66 command, CancellationToken cancellationToken)
    {
        std.Write("Clone commando executed order 66!");
        return Task.CompletedTask;
    }
}