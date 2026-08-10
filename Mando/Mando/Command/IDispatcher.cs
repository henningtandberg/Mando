namespace Mando.Command;

/// <summary>
/// Responsible for selecting the appropriate command handlers given the specified command
/// keeping the caller decoupled from the callee.
/// </summary>
public interface IDispatcher
{
    /// <summary>
    /// Finds one or more command handlers registered for the given command and passes the command on for handling
    /// </summary>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">Token forwarded to the handler(s) to observe cancellation requests</param>
    /// <returns>Task</returns>
    public Task Dispatch(ICommand command, CancellationToken cancellationToken = default);

    /// <summary>
    /// Finds the command handler registered for the given command and returns the result
    /// </summary>
    /// <typeparam name="TResult">The result type</typeparam>
    /// <param name="command">The command to execute</param>
    /// <param name="cancellationToken">Token forwarded to the handler to observe cancellation requests</param>
    /// <returns>Task<TResult></returns>
    public Task<TResult> Dispatch<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default);
}