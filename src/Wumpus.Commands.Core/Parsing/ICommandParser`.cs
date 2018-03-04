using System.Threading.Tasks;

namespace Wumpus.Commands
{
    public interface ICommandParser
    {
        Task<ParseResult> ParseAsync<TContext>(
            CommandService<TContext> commands,
            string commandText)
            where TContext : class, ICommandContext<TContext>;
    }
}
