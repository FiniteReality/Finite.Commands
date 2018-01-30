using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    /// <summary>
    /// A callback for commands executed through a <see cref="CommandService"/>.
    /// </summary>
    /// <param name="command">The command executed</param>
    /// <param name="services">The service container for requesting required services</param>
    /// <param name="arguments">The pre-parsed arguments to the command</param>
    /// <returns>An object indicating the command's success</returns>
    public delegate Task<ICommandResult> CommandCallback(CommandInfo command,
        IServiceProvider services, object[] arguments);
}
