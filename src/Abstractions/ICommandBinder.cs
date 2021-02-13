namespace Finite.Commands
{
    /// <summary>
    /// Defines an interface which can be used to bind parameters to commands.
    /// </summary>
    public interface ICommandBinder
    {
        /// <summary>
        /// Attempts to bind <paramref name="command"/> to the given
        /// <paramref name="context"/>.
        /// </summary>
        /// <param name="context">
        /// The context to bind to.
        /// </param>
        /// <param name="command">
        /// The command to bind.
        /// </param>
        /// <returns>
        /// <code>true</code> if the command binding succeeded,
        /// <code>false</code> otherwise.
        /// </returns>
        bool TryBind(CommandContext context, ICommand command);
    }
}
