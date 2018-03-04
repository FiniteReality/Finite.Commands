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
    /// <param name="context">The context for the executed command</param>
    /// <param name="services">The service container for requesting required services</param>
    /// <param name="arguments">The pre-parsed arguments to the command</param>
    /// <returns>An object indicating the command's success</returns>
    public delegate Task<IResult> CommandCallback(CommandInfo command,
        ICommandContext context, IServiceProvider services, object[] arguments);

    public delegate void OnBuildingCallback(ModuleBuilder module);

    public delegate void OnExecutingCallback(CommandInfo command);

    public delegate Task<IResult> PipelineCallback(
        CommandInfo command, ICommandContext context,
        IServiceProvider services, object[] arguments,
        Func<Task<IResult>> next);

    internal delegate Task<IResult> PipelineFunc(
        CommandInfo command, ICommandContext context,
        IServiceProvider services, object[] arguments);
}
