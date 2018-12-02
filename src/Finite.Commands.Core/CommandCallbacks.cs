using System;
using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// A callback for commands executed through a
    /// <see cref="CommandService{TContext}"/>.
    /// </summary>
    /// <param name="command">
    /// The command which has been executed.
    /// </param>
    /// <param name="context">
    /// The context for the executed command.
    /// </param>
    /// <param name="commands">
    /// The command service this command is being executed under.
    /// </param>
    /// <param name="services">
    /// The service container for requesting required services.
    /// </param>
    /// <param name="arguments">
    /// The pre-parsed arguments to the command.
    /// </param>
    /// <returns>
    /// An object indicating the command's success.
    /// </returns>
    public delegate Task<IResult> CommandCallback(CommandInfo command,
        ICommandContext context, ICommandService commands,
        IServiceProvider services, object[] arguments);

    /// <summary>
    /// A callback invoked when a module is being built.
    /// </summary>
    /// <param name="module">
    /// The module being built.
    /// </param>
    public delegate void OnBuildingCallback(ModuleBuilder module);

    /// <summary>
    /// A callback invoked when a command is being executed.
    /// </summary>
    /// <param name="command">
    /// The command being executed.
    /// </param>
    public delegate void OnExecutingCallback(CommandInfo command);

    /// <summary>
    /// A callback invoked during a pipeline.
    /// </summary>
    /// <param name="context">
    /// The execution context for the command.
    /// </param>
    /// <param name="next">
    /// A callback to instruct that execution of the pipeline may continue.
    /// The result value should be returned, e.g.:
    /// <code>
    /// if (someCondition)
    ///     return await next();
    /// else
    ///     return SomeErrorResult.Instance;
    /// </code>
    /// </param>
    /// <returns>
    /// An <see cref="IResult"/> instance representing the result of this
    /// pipeline.
    /// </returns>
    public delegate Task<IResult> PipelineCallback(
        CommandExecutionContext context, Func<Task<IResult>> next);
}
