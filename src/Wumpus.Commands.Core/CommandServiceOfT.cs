using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    /// <summary>
    /// A service for parsing and executing commands in chat messages.
    /// </summary>
    /// <typeparam name="TContext">
    /// The command context to use.
    /// </typeparam>
    public class CommandService<TContext> : ICommandService
        where TContext : class, ICommandContext<TContext>
    {
        private readonly IReadOnlyList<ModuleInfo> _modules;
        private readonly IReadOnlyList<PipelineCallback> _pipelines;
        private readonly CommandMap _commandMap;
        private readonly ICommandParser _parser;

        internal CommandService(
            IReadOnlyList<PipelineCallback> pipelines,
            IReadOnlyList<ModuleInfo> modules)
        {
            _modules = modules;
            _pipelines = pipelines;
            _commandMap = new CommandMap(modules);

            _parser = new DefaultCommandParser();
        }

        /// <inheritdoc/>
        public IEnumerable<CommandMatch> FindCommands(string[] fullPath)
        {
            return _commandMap.GetCommands(fullPath);
        }

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
        public async Task<IResult> ExecuteAsync(
            TContext context, IServiceProvider services)
        {
            var execContext = new CommandExecutionContext(this, context,
                services);

            async Task<IResult> ExecuteCommand(CommandExecutionContext ctx)
            {
                if (ctx.Command == null)
                {
                    return CommandNotFoundResult.Instance;
                }

                return await ctx.Command.ExecuteAsync(ctx)
                    .ConfigureAwait(false);
            }

            Func<Task<IResult>> GetPipelineFunc(
                CommandExecutionContext ctx, int pos)
            {
                if (pos >= _pipelines.Count)
                    return () => ExecuteCommand(ctx);

                return () => _pipelines[pos](ctx, GetPipelineFunc(ctx, pos+1));
            }

            return await GetPipelineFunc(execContext, 0)()
                .ConfigureAwait(false);
        }

        /// <inheritdoc/>
        Task<IResult> ICommandService.ExecuteAsync(ICommandContext context,
            IServiceProvider services)
            => ExecuteAsync(context as TContext, services);
    }
}
