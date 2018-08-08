namespace Wumpus.Commands
{
    /// <summary>
    /// The base context for all commands. This is not intended to be used
    /// publicly, instead you should use
    /// <see cref="ICommandContext&lt;TContext&gt;"/>.
    /// </summary>
    public interface ICommandContext
    {
        /// <summary>
        /// The contents of the message which triggered a command.
        /// </summary>
        /// <value>
        /// The Message property gets a <see cref="string"/> containing the
        /// contents of the message which invoked a command.
        /// </value>
        string Message { get; }

        /// <summary>
        /// The sender of the message which triggered a command.
        /// </summary>
        /// <value>
        /// The Author property gets a <see cref="string"/> containing the name
        /// of the author which sent the message contained in
        /// <see cref="Message"/>.
        /// </value>
        string Author { get; }
    }
}
