using System;

namespace Wumpus.Commands
{
    public sealed class CommandExecutionContext
    {
        public CommandInfo Command { get; }
        public ICommandContext Context { get; }
        public IServiceProvider ServiceProvider { get; }
        public object[] Arguments { get; }

        internal CommandExecutionContext(
            CommandInfo command,
            ICommandContext context,
            IServiceProvider services,
            object[] arguments)
        {
            Command = command;
            Context = context;
            ServiceProvider = services;
            Arguments = arguments;
        }
    }
}
