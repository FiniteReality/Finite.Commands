using System;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method,
        Inherited = false, AllowMultiple = true)]
    public abstract class PreconditionAttribute : Attribute
    {
        protected abstract Task<IPreconditionResult> ExecuteAsync(
            CommandService commands, CommandInfo command,
            IServiceProvider services, ICommandContext context);
    }
}
