using System.Collections.Generic;

namespace Finite.Commands
{
    /// <summary>
    /// Defines an interface which can be used to store and retrieve commands.
    /// </summary>
    public interface ICommandStore
    {
        /// <summary>
        /// Gets the command group with the given <paramref name="prefix"/>.
        /// </summary>
        /// <param name="prefix">
        /// The <see cref="CommandString"/> to search for command groups.
        /// </param>
        /// <returns>
        /// A <see cref="ICommandStore"/> which can be used to search for
        /// nested commands, or <code>null</code> if none exists.
        /// </returns>
        ICommandStoreSection? GetCommandGroup(CommandString prefix);

        /// <summary>
        /// Gets all of the commands with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The <see cref="CommandString"/> to search for commands.
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> containing all possible overloads of
        /// commands with the given <paramref name="name"/>.
        /// </returns>
        IEnumerable<ICommand> GetCommands(CommandString name);
    }
}
