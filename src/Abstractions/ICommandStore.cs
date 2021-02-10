namespace Finite.Commands
{
    /// <summary>
    /// Defines an interface which can be used to store and retrieve commands.
    /// </summary>
    public interface ICommandStore
    {
        /// <summary>
        /// Gets all the commands with the given <paramref name="prefix"/>.
        /// </summary>
        /// <param name="prefix">
        /// The <see cref="CommandPath"/> to search for commands.
        /// </param>
        /// <returns>
        /// A <see cref="ICommandStore"/> which can be used to search for
        /// nested commands.
        /// </returns>
        ICommandStore GetNestedCommands(CommandPath prefix);

        /// <summary>
        /// Gets a value indicating whether this store has nested values.
        /// </summary>
        bool HasNestedCommands { get; }
    }
}
