using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
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

        public IEnumerable<CommandMatch> FindCommands(string[] path)
        {
            return _commandMap.GetCommands(path);
        }

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

        Task<IResult> ICommandService.ExecuteAsync(ICommandContext context,
            IServiceProvider services)
            => ExecuteAsync(context as TContext, services);
    }
}
