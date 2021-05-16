using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Finite.Commands
{
    /// <summary>
    /// Extension methods for adding middleware to
    /// an <see cref="ICommandsBuilder"/>.
    /// </summary>
    public static class CommandsBuilderMiddlewareExtensions
    {
        /// <summary>
        /// Adds a middleware object to the command pipeline.
        /// </summary>
        /// <param name="builder">
        /// The commands builder to add middleware to.
        /// </param>
        /// <typeparam name="TMiddleware">
        /// The type of middleware to add to the command pipeline.
        /// </typeparam>
        /// <returns>
        /// The builder.
        /// </returns>
        public static ICommandsBuilder Use<TMiddleware>(
            this ICommandsBuilder builder)
            where TMiddleware : ICommandMiddleware
        {

            return builder.Use(
                static (n, c, ct) => ExecuteMiddleware(n, c, ct));

            static ValueTask<ICommandResult> ExecuteMiddleware(
                CommandMiddleware next, CommandContext context,
                CancellationToken cancellationToken)
            {
                var middleware = context.Services
                    .GetRequiredService<TMiddleware>();

                return middleware.ExecuteAsync(next, context, cancellationToken);
            }
        }
    }
}
