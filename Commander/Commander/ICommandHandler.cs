namespace Commander;

public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : ICommand
{
    public Task Execute(TCommand command);
}

public interface ICommandHandler;