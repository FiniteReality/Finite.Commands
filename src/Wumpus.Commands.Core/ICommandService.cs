using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    public interface ICommandService
    {
        Task<IResult> ExecuteAsync(ICommandContext context,
            IServiceProvider services);

        IEnumerable<CommandMatch> FindCommands(string[] fullPath);
    }
}
