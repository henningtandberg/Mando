namespace Mando.Command;

/// <summary>
/// Represents the next step in a command pipeline: either the next behavior or,
/// at the innermost layer, the command handler(s).
/// </summary>
/// <returns>Task</returns>
public delegate Task CommandHandlerDelegate();

/// <summary>
/// Represents the next step in a command pipeline for a command that returns a value.
/// </summary>
/// <typeparam name="TResult">The result type</typeparam>
/// <returns>Task<TResult></returns>
public delegate Task<TResult> CommandHandlerDelegate<TResult>();

/// <summary>
/// A behavior that wraps the handling of a command, running code before and/or after the
/// rest of the pipeline. Call <paramref name="next"/> to continue, or skip it to short-circuit.
/// </summary>
/// <typeparam name="TCommand">Type of command to wrap</typeparam>
public interface IPipelineBehavior<in TCommand> where TCommand : ICommand
{
    /// <summary>
    /// Handles the command, invoking <paramref name="next"/> to continue the pipeline.
    /// </summary>
    /// <param name="command">The command being dispatched</param>
    /// <param name="next">The next step in the pipeline (behavior or handler fan-out)</param>
    /// <param name="cancellationToken">Token used to observe cancellation requests</param>
    /// <returns>Task</returns>
    Task Handle(TCommand command, CommandHandlerDelegate next, CancellationToken cancellationToken);
}

/// <summary>
/// A behavior that wraps the handling of a command that returns a value, running code before
/// and/or after the rest of the pipeline. Call <paramref name="next"/> to continue and obtain
/// the result, or return a value directly to short-circuit.
/// </summary>
/// <typeparam name="TCommand">Type of command to wrap</typeparam>
/// <typeparam name="TResult">Type of result</typeparam>
public interface IPipelineBehavior<in TCommand, TResult> where TCommand : ICommand<TResult>
{
    /// <summary>
    /// Handles the command, invoking <paramref name="next"/> to continue the pipeline.
    /// </summary>
    /// <param name="command">The command being dispatched</param>
    /// <param name="next">The next step in the pipeline (behavior or handler)</param>
    /// <param name="cancellationToken">Token used to observe cancellation requests</param>
    /// <returns>Task<TResult></returns>
    Task<TResult> Handle(TCommand command, CommandHandlerDelegate<TResult> next, CancellationToken cancellationToken);
}
