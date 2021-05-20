using System.Threading;
using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// Defines an interface which can be used to define middleware using a
    /// type.
    /// </summary>
    public interface ICommandMiddleware
    {
        /// <summary>
        /// Executes the middleware.
        /// </summary>
        /// <param name="next">
        /// A callback used to transfer control in the middleware pipeline.
        /// </param>
        /// <param name="context">
        /// The command context to execute.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token, indicating cancellation of processing.
        /// </param>
        /// <returns>
        /// A task that represents the completion of command processing.
        /// </returns>
        public ValueTask<ICommandResult> ExecuteAsync(
            CommandMiddleware next, CommandContext context,
            CancellationToken cancellationToken);
    }
}
