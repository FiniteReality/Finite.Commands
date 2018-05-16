using System.Threading.Tasks;

namespace Wumpus.Commands
{
    public interface ICommandParser
    {
        Task ParseAsync<TContext>(
            CommandExecutionContext executionContext)
            where TContext : class, ICommandContext<TContext>;
    }
}
