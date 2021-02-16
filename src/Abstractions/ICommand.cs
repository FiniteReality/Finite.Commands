using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// Defines an interface which stores the information for a given command
    /// overload.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the <see cref="CommandPath"/>. which can be used to invoke
        /// this command overload.
        /// </summary>
        CommandPath Name { get; }

        /// <summary>
        /// Gets the parameters which can be used to pass information to this
        /// command.
        /// </summary>
        IReadOnlyList<IParameter> Parameters { get; }

        /// <summary>
        /// Gets the extra data which has been stored with this command.
        /// </summary>
        IReadOnlyDictionary<object, object?> Data { get; }

        /// <summary>
        /// Executes the command with the given <paramref name="context"/>,
        /// potentially asynchronously.
        /// </summary>
        /// <param name="context">
        /// The context to execute the command with.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token which may be used to cancel the command.
        /// </param>
        /// <returns>
        /// A <see cref="ValueTask"/> which represents the completion of the
        /// command.
        /// </returns>
        ValueTask ExecuteAsync(CommandContext context,
            CancellationToken cancellationToken);
    }
}
