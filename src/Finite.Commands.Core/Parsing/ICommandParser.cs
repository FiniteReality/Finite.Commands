using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// Public interface for implementing a command parser
    /// </summary>
    public interface ICommandParser<TContext>
            where TContext : class, ICommandContext
    {
        /// <summary>
        /// Parses a command message into a token stream
        /// </summary>
        /// <param name="executionContext">
        /// The execution context of the command to parse.
        /// </param>
        /// <returns>
        /// An <see cref="IResult"/> indicating whether the parse succeeded or
        /// not.
        /// </returns>
        IResult Parse(CommandExecutionContext executionContext);
    }
}
