namespace Commander;

public interface IDispatcher
{
    public Task Dispatch(ICommand command);
}