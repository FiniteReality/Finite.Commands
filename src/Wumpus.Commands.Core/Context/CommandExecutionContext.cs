using System;
using System.Collections.Generic;

namespace Wumpus.Commands
{
    public sealed class CommandExecutionContext
    {
        public ICommandService CommandService { get; }
        public CommandInfo Command { get; set; }
        public ICommandContext Context { get; }
        public IServiceProvider ServiceProvider { get; }
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
