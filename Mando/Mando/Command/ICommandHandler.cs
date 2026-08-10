namespace Mando.Command;

/// <summary>
/// Defines the procedure to be executed by a command
/// </summary>
/// <typeparam name="TCommand">Type of command to handle</typeparam>
public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : ICommand
{
    /// <summary>
    /// The procedure to be executed
    /// </summary>
    /// <param name="command">The command</param>
    /// <param name="cancellationToken">Token used to observe cancellation requests</param>
    /// <returns>void</returns>
    public Task Execute(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Defines the procedure to be executed by a command that returns a value
/// </summary>
/// <typeparam name="TCommand">Type of command to handle</typeparam>
/// <typeparam name="TResult">Type of result</typeparam>
public interface ICommandHandler<in TCommand, TResult> : ICommandHandler where TCommand : ICommand<TResult>
{
    /// <summary>
    /// The procedure to be executed
    /// </summary>
    /// <param name="command">The command</param>
    /// <param name="cancellationToken">Token used to observe cancellation requests</param>
    /// <returns>Result</returns>
    public Task<TResult> Execute(TCommand command, CancellationToken cancellationToken);
}

/// <summary>
/// Used during the Dependency Injection setup to avoid having to register all specific handlers seperatly.
/// </summary>
public interface ICommandHandler;