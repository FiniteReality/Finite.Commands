using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Finite.Commands
{
    internal sealed class DefaultCommandContextFactory : ICommandContextFactory
    {
        private readonly IServiceScopeFactory _scopeFactory;

        private readonly ConcurrentBag<DefaultCommandContext> _contexts;

        public DefaultCommandContextFactory(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            _contexts = new();
        }

        public CommandContext CreateContext()
            => Init(_contexts.TryTake(out var result)
                ? result
                : new DefaultCommandContext());

        private CommandContext Init(DefaultCommandContext context)
        {
            context.ServiceScope = _scopeFactory.CreateScope();

            return context;
        }

        public void ReleaseContext(CommandContext context)
        {
            Debug.Assert(context is DefaultCommandContext);

            var realContext = (DefaultCommandContext)context;
            realContext.Reset();

            _contexts.Add(realContext);
        }
    }
}
