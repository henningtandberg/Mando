using Commander.Tests.Commands;
using Commander.Tests.Setup;

namespace Commander.Tests.CommandHandlers;

public sealed class CommandHandlerThree(IStd std) : ICommandHandler<CommandThree>
{
    public Task Execute(CommandThree command)
    {
        std.Write("Command three executed!");
        return Task.CompletedTask;
    }
}