using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// Public interface for implementing a command parser
    /// </summary>
    public interface ICommandParser
    {
        /// <summary>
        /// Parses a command message into a token stream
        /// </summary>
        /// <param name="executionContext">
        /// The execution context of the command to parse.
        /// </param>
        /// <typeparam name="TContext">
        /// The context type of the command.
        /// </typeparam>
        /// <returns>
        /// A Task which completes when the command has finished parsing.
        /// </returns>
        Task ParseAsync<TContext>(
            CommandExecutionContext executionContext)
            where TContext : class, ICommandContext<TContext>;
    }
}
