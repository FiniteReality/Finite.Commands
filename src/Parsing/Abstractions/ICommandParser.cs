using System.Threading;
using System.Threading.Tasks;

namespace Finite.Commands.Parsing
{
    /// <summary>
    /// Defines an interface which can be used to populate command contexts
    /// based on input strings.
    /// </summary>
    public interface ICommandParser
    {
        /// <summary>
        /// Parses <paramref name="message"/> and populates
        /// <paramref name="context"/> with the result.
        /// </summary>
        /// <param name="context">
        /// The context to populate.
        /// </param>
        /// <param name="message">
        /// The raw text for the message.
        /// </param>
        /// <param name="cancellationToken">
        /// A cancellation token which may be used to cancel the parsing.
        /// </param>
        /// <returns>
        /// A ValueTask which represents the completion of the parsing.
        /// </returns>
        ValueTask ParseAsync(CommandContext context, string message,
            CancellationToken cancellationToken = default);
    }
}
