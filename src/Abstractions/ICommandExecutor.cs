using System.Threading;
using System.Threading.Tasks;

namespace Finite.Commands.Abstractions
{
    /// <summary>
    /// Defines an interface which can be used to execute a command pipeline.
    /// </summary>
    public interface ICommandExecutor
    {
        /// <summary>
        /// Executes the command pipeline over the given
        /// <paramref name="context"/>, potentially asynchronously.
        /// </summary>
        /// <param name="context">
        /// The context to execute the command pipeline over.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token which may be used to cancel the command.
        /// </param>
        /// <returns>
        /// A ValueTask which represents the completion of the command.
        /// </returns>
        ValueTask ExecuteAsync(CommandContext context,
            CancellationToken cancellationToken = default);
    }
}
