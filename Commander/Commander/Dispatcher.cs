namespace Commander;

internal sealed class Dispatcher(IEnumerable<ICommandHandler> handlers) : IDispatcher
{
    public Task Dispatch(ICommand command)
    {
        var handler = handlers
            .FirstOrDefault(h => h.GetType()
                .GetInterfaces()
                .Any(i => i.IsGenericType &&
                          i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) &&
                          i.GetGenericArguments()[0] == command.GetType()));

        if (handler is null)
            throw new InvalidOperationException($"No handler for {command.GetType()}");

        return InvokeHandler(handler, command);
    }

    private static Task InvokeHandler(ICommandHandler handler, ICommand command)
    {
        var method = handler.GetType()
            .GetMethod("Execute", [command.GetType()])!;

        return (Task)method.Invoke(handler, [command])!;
    } 
}