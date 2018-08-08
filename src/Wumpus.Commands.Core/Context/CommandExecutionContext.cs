using System;
using System.Collections.Generic;

namespace Wumpus.Commands
{
    /// <summary>
    /// An execution context associated with a command
    /// </summary>
    public sealed class CommandExecutionContext
    {
        /// <summary>
        /// The command service which started this execution
        /// </summary>
        /// <value>
        /// The CommandService property gets the
        /// <see cref="ICommandService"/> which began this current execution.
        /// </value>
        public ICommandService CommandService { get; }

        /// <summary>
        /// The command which is to be executed by this execution.
        /// </summary>
        /// <value>
        /// The Command property gets or sets the <see cref="CommandInfo"/>
        /// which should be executed at the end of the pipeline.
        /// </value>
        public CommandInfo Command { get; set; }

        /// <summary>
        /// The context passed to the command when executed.
        /// </summary>
        /// <value>
        /// The Context property gets the <see cref="ICommandContext"/> which
        /// should be passed to the command when executed.
        /// </value>
        public ICommandContext Context { get; }

        /// <summary>
        /// The service provider used by the command for dependency injection.
        /// </summary>
        /// <value>
        /// The ServiceProvider property gets the
        /// <see cref="IServiceProvider"/> which is used by the command during
        /// execution for dependencies.
        /// </value>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// The arguments passed to the command when executed.
        /// </summary>
        /// <value>
        /// The Arguments property gets or sets the arguments passed to the
        /// command method when invoked. They should be in-order as the command
        /// expects them, and the correct type as the command expects them.
        /// </value>
        public object[] Arguments { get; set; }

        internal CommandExecutionContext(
            ICommandService commands,
            ICommandContext context,
            IServiceProvider services)
        {
            CommandService = commands;
            Context = context;
            ServiceProvider = services;
        }
    }
}
