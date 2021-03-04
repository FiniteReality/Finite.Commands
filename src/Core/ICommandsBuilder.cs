using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Finite.Commands
{
    /// <summary>
    /// An interface for configuring commands services.
    /// </summary>
    public interface ICommandsBuilder
    {
        /// <summary>
        /// Gets the <see cref="IServiceCollection"/> where commands services
        /// are configured.
        /// </summary>
        IServiceCollection Services { get; }

        /// <summary>
        /// Adds the middleware to the command pipeline.
        /// </summary>
        /// <param name="middleware">
        /// The middleware.
        /// </param>
        /// <returns>
        /// The builder.
        /// </returns>
        ICommandsBuilder Use(
            Func<CommandMiddleware, CommandContext, CancellationToken, ValueTask<ICommandResult>> middleware);
    }
}
