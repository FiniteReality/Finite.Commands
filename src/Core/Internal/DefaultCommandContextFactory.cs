using System.Collections.Concurrent;
using System.Diagnostics;
using Finite.Commands;

namespace Finite.Commands
{
    internal sealed class DefaultCommandContextFactory : ICommandContextFactory
    {
        private readonly ConcurrentBag<DefaultCommandContext> _contexts;

        public DefaultCommandContextFactory()
        {
            _contexts = new();
        }

        public CommandContext CreateContext()
            => _contexts.TryTake(out var result)
                ? result
                : new DefaultCommandContext();

        public void ReleaseContext(CommandContext context)
        {
            Debug.Assert(context is DefaultCommandContext);

            var realContext = (DefaultCommandContext)context;
            realContext.Reset();

            _contexts.Add(realContext);
        }
    }
}
