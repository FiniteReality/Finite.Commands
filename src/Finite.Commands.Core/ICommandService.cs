using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// The base interface for all command services.
    /// </summary>
    public interface ICommandService
    {
        /// <summary>
        /// Executes any stored pipelines on a context, returning any result
        /// they produce.
        /// </summary>
        /// <param name="context">
        /// The contextual message data to execute pipelines on.
        /// </param>
        /// <param name="services">
        /// A provider for services used to create modules based on their
        /// dependencies.
        /// </param>
        /// <returns>
        /// A <see cref="IResult"/> produced somewhere in the pipeline chain.
        /// </returns>
        Task<IResult> ExecuteAsync(ICommandContext context,
            IServiceProvider services);

        /// <summary>
        /// Finds commands matching the given path.
        /// </summary>
        /// <param name="fullPath">
        /// The path to search for commands in.
        /// </param>
        /// <returns>
        /// Any commands matched by <paramref name="fullPath"/>. Any extr
        /// members of <paramref name="fullPath"/> are passed as parameters to
        /// the command.
        /// </returns>
        IEnumerable<CommandMatch> FindCommands(string[] fullPath);
    }
}
