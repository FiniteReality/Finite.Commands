using System;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    public interface IPipeline
    {
        Task<IResult> ExecuteAsync(CommandExecutionContext context,
            Func<Task<IResult>> next);
    }
}
