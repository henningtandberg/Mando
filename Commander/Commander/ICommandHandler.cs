namespace Commander;

public interface ICommandHandler<in TCommand> where TCommand : ICommand, ICommandHandler
{
    public Task Execute(TCommand command);
}

public interface ICommandHandler
{
    public Task Execute(ICommand command);
}