using Mando.Command;
using Mando.Tests.Command.Commands;
using Mando.Tests.Setup;

namespace Mando.Tests.Command.CommandHandlers;

public sealed class CloneParatrooper(IStd std) : ICommandHandler<Order66>
{
    public Task Execute(Order66 command)
    {
        std.Write("Clone paratrooper executed order 66!");
        return Task.CompletedTask;
    }
}