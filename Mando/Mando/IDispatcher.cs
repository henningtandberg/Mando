namespace Mando;

public interface IDispatcher
{
    public Task Dispatch(ICommand command);
}