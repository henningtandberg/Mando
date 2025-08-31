namespace Mando;

internal sealed class Dispatcher(IEnumerable<ICommandHandler> handlers) : IDispatcher
{
    public async Task Dispatch(ICommand command)
    {
        var matchingHandlers = handlers
            .Where(h => CanHandle(h, command))
            .ToList();

        if (matchingHandlers.Count == 0)
            throw new InvalidOperationException($"No handlers for {command.GetType()}");

        var tasks = matchingHandlers
            .Select(h => InvokeHandler(h, command));

        await Task.WhenAll(tasks); 
    }
    
    private static bool CanHandle(ICommandHandler handler, ICommand command)
    {
        return handler.GetType().GetInterfaces()
            .Any(i => i.IsGenericType &&
                      i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) &&
                      i.GetGenericArguments()[0].IsInstanceOfType(command));
    }

    private static Task InvokeHandler(ICommandHandler handler, ICommand command)
    {
        var method = handler.GetType()
            .GetMethod("Execute", [command.GetType()])!;

        return (Task)method.Invoke(handler, [command])!;
    } 
}