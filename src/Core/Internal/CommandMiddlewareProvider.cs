using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Middleware = System.Func<Finite.Commands.CommandMiddleware,
    Finite.Commands.CommandContext,
    System.Threading.CancellationToken,
    System.Threading.Tasks.ValueTask<
        Finite.Commands.ICommandResult>>;

namespace Finite.Commands
{
    internal class CommandMiddlewareProvider
    {
        internal List<Middleware> _middleware;

        public CommandMiddlewareProvider()
        {
            _middleware = new();
        }

        public void Add(
            Middleware middleware)
                => _middleware.Add(middleware);

        public ValueTask<ICommandResult> ExecuteCallbacksAsync(
            CommandMiddleware final,
            CommandContext context,
            CancellationToken cancellationToken)
        {
            return ExecuteNextAsync(_middleware, 0, context, cancellationToken,
                final);

            static async ValueTask<ICommandResult> ExecuteNextAsync(
                List<Middleware> middleware, int pos,
                CommandContext context, CancellationToken token,
                CommandMiddleware final)
            {
                return pos >= middleware.Count
                    ? await final()
                    : await middleware[pos](
                    () => ExecuteNextAsync(middleware, pos + 1, context, token, final),
                    context,
                    token);
            }
        }
    }
}
