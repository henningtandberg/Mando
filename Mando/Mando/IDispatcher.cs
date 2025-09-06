namespace Mando;

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
    /// <returns>Task</returns>
    public Task Dispatch(ICommand command);
}