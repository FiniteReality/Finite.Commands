using System.Threading;
using System.Threading.Tasks;

namespace Finite.Commands.Abstractions
{
    /// <summary>
    /// Defines an interface which can be used to populate command contexts
    /// based on input strings.
    /// </summary>
    public interface ICommandParser
    {
        /// <summary>
        /// Parses the given message into a command context.
        /// </summary>
        /// <param name="message">
        /// The raw text for the message.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token which may be used to cancel the parsing.
        /// </param>
        /// <returns>
        /// A ValueTask which represents the completion of the parsing.
        /// </returns>
        ValueTask<CommandContext> ParseAsync(string message,
            CancellationToken cancellationToken = default);
    }
}
