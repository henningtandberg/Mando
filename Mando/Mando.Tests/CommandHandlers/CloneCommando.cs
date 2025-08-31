using Mando.Tests.Commands;
using Mando.Tests.Setup;

namespace Mando.Tests.CommandHandlers;

public sealed class CloneCommando(IStd std) : ICommandHandler<Order66>
{
    public Task Execute(Order66 command)
    {
        std.Write("Clone commando executed order 66!");
        return Task.CompletedTask;
    }
}