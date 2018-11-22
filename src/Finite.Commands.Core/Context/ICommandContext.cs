namespace Finite.Commands
{
    /// <summary>
    /// The base context for all commands. This is not intended to be used
    /// publicly, instead you should use
    /// <see cref="ICommandContext{TContext}"/>.
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        /// The contents of the message which triggered a command.
        /// </summary>
        string Message { get; }

        /// <summary>
        /// The sender of the message which triggered a command.
        /// </summary>
        string Author { get; }
    }
}
