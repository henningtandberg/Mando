using Commander.Tests.Commands;
using Commander.Tests.Setup;

namespace Commander.Tests.CommandHandlers;

public sealed class CommandHandlerOne(IStd std) : ICommandHandler<CommandOne>
{
    public Task Execute(CommandOne command)
    {
        std.Write("Command one executed!");
        return Task.CompletedTask;
    }
}