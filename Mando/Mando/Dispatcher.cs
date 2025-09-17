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

    public async Task<TResult> Dispatch<TResult>(ICommand<TResult> command)
    {
        var handler = handlers
            .FirstOrDefault(h => CanHandleWithResult<TResult>(h, command));

        if (handler is null)
            throw new InvalidOperationException($"No handler for {command.GetType()} with result {typeof(TResult)}");

        return await InvokeHandlerWithResult<TResult>(handler, command);
    }

    private static bool CanHandle(ICommandHandler handler, ICommand command)
    {
        return handler.GetType().GetInterfaces()
            .Any(i => i.IsGenericType &&
                      i.GetGenericTypeDefinition() == typeof(ICommandHandler<>) &&
                      i.GetGenericArguments()[0].IsInstanceOfType(command));
    }

    private static bool CanHandleWithResult<TResult>(ICommandHandler handler, ICommand<TResult> command)
    {
        return handler.GetType().GetInterfaces()
            .Any(i => i.IsGenericType &&
                      i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>) &&
                      i.GetGenericArguments()[0].IsInstanceOfType(command) &&
                      i.GetGenericArguments()[1] == typeof(TResult));
    }

    private static Task InvokeHandler(ICommandHandler handler, ICommand command)
    {
        var method = handler.GetType()
            .GetMethod("Execute", [command.GetType()])!;

        return (Task)method.Invoke(handler, [command])!;
    }

    private static Task<TResult> InvokeHandlerWithResult<TResult>(ICommandHandler handler, ICommand<TResult> command)
    {
        var method = handler.GetType()
            .GetMethod("Execute", [command.GetType()])!;

        return (Task<TResult>)method.Invoke(handler, [command])!;
    }
}