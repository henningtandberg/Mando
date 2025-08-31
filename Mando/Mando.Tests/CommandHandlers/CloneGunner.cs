using Mando.Tests.Commands;
using Mando.Tests.Setup;

namespace Mando.Tests.CommandHandlers;

public sealed class CloneGunner(IStd std) : ICommandHandler<Order66>
{
    public Task Execute(Order66 command)
    {
        std.Write("Clone gunner executed order 66!");
        return Task.CompletedTask;
    }
}