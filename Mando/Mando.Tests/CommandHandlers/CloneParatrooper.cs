using Mando.Tests.Commands;
using Mando.Tests.Setup;

namespace Mando.Tests.CommandHandlers;

public sealed class CloneParatrooper(IStd std) : ICommandHandler<Order66>
{
    public Task Execute(Order66 command)
    {
        std.Write("Clone paratrooper executed order 66!");
        return Task.CompletedTask;
    }
}