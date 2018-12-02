using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// A service for parsing and executing commands in chat messages.
    /// </summary>
    /// <typeparam name="TContext">
    /// The command context to use.
    /// </typeparam>
    public sealed class CommandService<TContext> : ICommandService
        where TContext : class, ICommandContext
    {
        private readonly IReadOnlyList<ModuleInfo> _modules;
        private readonly IReadOnlyList<PipelineCallback> _pipelines;
        private readonly CommandMap _commandMap;

        private readonly ITypeReaderFactory _readerFactory;

        /// <inheritdoc/>
        public IReadOnlyCollection<ModuleInfo> Modules
            => _modules;

        /// <inheritdoc/>
        public ITypeReaderFactory TypeReaderFactory
            => _readerFactory;

        internal CommandService(
            IReadOnlyList<PipelineCallback> pipelines,
            IReadOnlyList<ModuleInfo> modules,
            ITypeReaderFactory factory)
        {
            _modules = modules;
            _pipelines = pipelines;
            _commandMap = new CommandMap(modules);
            _readerFactory = factory;
        }

        /// <summary>
        /// Gets all of the commands stored by this command service.
        /// </summary>
        /// <remarks>
        /// This is a potentially expensive operation
        /// </remarks>
        /// <returns>
        /// An un-ordered enumerable of commands.
        /// </returns>
        public IEnumerable<CommandInfo> GetAllCommands()
        {
            IEnumerable<CommandInfo> IterateModule(ModuleInfo module)
            {
                foreach (var submodule in module.Submodules)
                    foreach (var command in IterateModule(submodule))
                        yield return command;

                foreach (var command in module.Commands)
                    yield return command;
            }

            foreach (var module in _modules)
                foreach (var command in IterateModule(module))
                    yield return command;
        }

        /// <inheritdoc/>
        public IEnumerable<CommandMatch> FindCommands(string[] fullPath)
        {
            return _commandMap.GetCommands(fullPath)
                .OrderByDescending(x => x.CommandPath.Length)
                .ThenByDescending(x => x.Command.Parameters.Count)
                .ThenByDescending(x => x.Arguments.Length);
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

            Task<IResult> ExecuteCommand(CommandExecutionContext ctx)
                => ctx.Command.ExecuteAsync(ctx);

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
