using System;
using System.Collections.Generic;

namespace Finite.Commands
{
    /// <summary>
    /// An execution context associated with a command
    /// </summary>
    public sealed class CommandExecutionContext
    {
        /// <summary>
        /// The command service which started this execution
        /// </summary>
        public ICommandService CommandService { get; }

        /// <summary>
        /// The command which is to be executed by this execution.
        /// </summary>
        public CommandInfo Command { get; set; }

        /// <summary>
        /// The length of the prefix of this message. These many characters
        /// are ignored by the command tokenizer.
        /// </summary>
        public int PrefixLength { get; set; }

        /// <summary>
        /// The context passed to the command when executed.
        /// </summary>
        public ICommandContext Context { get; }

        /// <summary>
        /// The service provider used by the command for dependency injection.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// The arguments passed to the command when executed.
        /// </summary>
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
