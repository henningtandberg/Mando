namespace Mando;

/// <summary>
/// A command triggers one or more ICommandHandler&lt;TCommand&gt;, where TCommand implements ICommand
/// </summary>
public interface ICommand;

/// <summary>
/// A command that returns a value
/// </summary>
/// <typeparam name="TResult">The result type</typeparam>
public interface ICommand<TResult> : ICommand;
