using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    public class CommandService<TContext>
        where TContext : class, ICommandContext<TContext>
    {
        private readonly HashSet<ModuleInfo> _modules;
        private readonly List<PipelineCallback> _pipelines;
        private readonly CommandMap _commandMap;
        private readonly ICommandParser _parser;

        public CommandService()
        {
            _modules = new HashSet<ModuleInfo>();
            _pipelines = new List<PipelineCallback>();
            _commandMap = new CommandMap();

            _parser = new DefaultCommandParser();
        }


        public ModuleInfo LoadModule<TModule>()
            where TModule : ModuleBase<TContext>
        {
            void AddCommandsFrom(ModuleInfo module, Stack<string> root)
            {
                foreach (var moduleAlias in module.Aliases)
                {
                    root.Push(moduleAlias);

                    foreach (var command in module.Commands)
                    {
                        foreach (var commandAlias in command.Aliases)
                        {
                            root.Push(commandAlias);

                            _commandMap.AddCommand(root.ToArray(), command);

                            root.Pop();
                        }
                    }

                    foreach (var submodule in module.Submodules)
                        AddCommandsFrom(submodule, root);

                    root.Pop();
                }
            }

            if (!ClassBuilder.IsValidModule<TModule, TContext>())
                throw new ArgumentException(
                    $"Cannot build {typeof(TModule).Name} " +
                    "as it is not a valid module.");

            var builtModule = ClassBuilder.Build<TModule, TContext>();

            lock (_modules)
                _modules.Add(builtModule);

            AddCommandsFrom(builtModule, new Stack<string>());

            return builtModule;
        }

        /// <summary>
        /// Adds a pipeline to the command service
        /// </summary>
        /// <param name="pipeline">The callback of the pipeline</param>
        public void AddPipeline(PipelineCallback pipeline)
        {
            lock (_pipelines)
                _pipelines.Add(pipeline);
        }


        public IEnumerable<CommandMatch> FindCommands(string[] commandSegments)
            => _commandMap.GetCommands(commandSegments);

        public Task<ParseResult> ParseMessageAsync(string message)
            => _parser.ParseAsync<TContext>(this, message);

        
        public async Task<IResult> ExecuteAsync(
            CommandInfo command,
            TContext context,
            object[] arguments,
            IServiceProvider services = null)
        {
            var execContext = new CommandExecutionContext(
                command, context, services, arguments);

            Func<Task<IResult>> GetPipelineFunc(
                CommandExecutionContext ctx, int pos)
            {
                if (pos >= _pipelines.Count)
                    return () => ctx.Command.ExecuteAsync(ctx);

                return () => _pipelines[pos](ctx, GetPipelineFunc(ctx, pos+1));
            }

            return await GetPipelineFunc(execContext, 0)();
        }
    }
}
