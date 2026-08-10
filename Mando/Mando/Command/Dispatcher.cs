using System.Reflection;

namespace Mando.Command;

internal sealed class Dispatcher(IEnumerable<ICommandHandler> handlers) : IDispatcher
{
    public async Task Dispatch(ICommand command, CancellationToken cancellationToken = default)
    {
        var matchingHandlers = handlers
            .Where(h => CanHandle(h, command))
            .ToList();

        if (matchingHandlers.Count == 0)
            throw new InvalidOperationException($"No handlers for {command.GetType()}");

        var tasks = matchingHandlers
            .Select(h => InvokeHandler(h, command, cancellationToken));

        await Task.WhenAll(tasks); 
    }

    public async Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default)
    {
        var handler = handlers
            .FirstOrDefault(h => CanHandleWithResult<TResult>(h, command));

        if (handler is null)
            throw new InvalidOperationException($"No handler for {command.GetType()} with result {typeof(TResult)}");

        return await InvokeHandlerWithResult<TResult>(handler, command, cancellationToken);
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

    private static Task InvokeHandler(ICommandHandler handler, ICommand command, CancellationToken cancellationToken)
    {
        var method = handler.GetType()
            .GetMethod("Execute", [command.GetType(), typeof(CancellationToken)])!;

        return (Task)Invoke(method, handler, [command, cancellationToken]);
    }

    private static Task<TResult> InvokeHandlerWithResult<TResult>(ICommandHandler handler, ICommand<TResult> command, CancellationToken cancellationToken)
    {
        var method = handler.GetType()
            .GetMethod("Execute", [command.GetType(), typeof(CancellationToken)])!;

        return (Task<TResult>)Invoke(method, handler, [command, cancellationToken]);
    }

    // DoNotWrapExceptions makes reflection rethrow an exception thrown synchronously inside a
    // handler (e.g. CancellationToken.ThrowIfCancellationRequested) as itself, with its stack
    // trace intact, instead of wrapping it in a TargetInvocationException.
    // If you ever need to inspect or transform the original exception, drop the flag and wrap
    // the call in try/catch (TargetInvocationException ex): the handler's exception is ex.InnerException.
    private static object Invoke(MethodInfo method, ICommandHandler handler, object[] arguments)
        => method.Invoke(handler, BindingFlags.DoNotWrapExceptions, binder: null, parameters: arguments, culture: null)!;
}