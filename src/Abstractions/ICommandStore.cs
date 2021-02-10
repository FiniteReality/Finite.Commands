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
        /// The <see cref="CommandPath"/> to search for command groups.
        /// </param>
        /// <returns>
        /// A <see cref="ICommandStore"/> which can be used to search for
        /// nested commands.
        /// </returns>
        ICommandStoreSection GetCommandGroup(CommandPath prefix);

        /// <summary>
        /// Gets a value indicating whether this store has nested command
        /// groups.
        /// </summary>
        bool HasNestedCommandGroups { get; }

        /// <summary>
        /// Gets all of the commands with the given <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The <see cref="CommandPath"/> to search for commands.
        /// </param>
        /// <returns>
        /// A <see cref="IEnumerable{T}"/> containing all possible overloads of
        /// commands with the given <paramref name="name"/>.
        /// </returns>
        IEnumerable<ICommand> GetCommands(CommandPath name);
    }
}
