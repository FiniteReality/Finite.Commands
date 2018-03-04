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

        /// <summary>
        /// Loads a module with the same context as this command service
        /// </summary>
        /// <returns>The loaded module, if successful</returns>
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

        /// <summary>
        /// Finds all commands with the given prefix
        /// </summary>
        /// <param name="commandSegments">The tokenized command prefix to search for</param>
        /// <returns>All possible commands that could be selected along with their arguments</returns>
        public IEnumerable<CommandMatch> FindCommands(string[] commandSegments)
            => _commandMap.GetCommands(commandSegments);

        public Task<ParseResult> ParseMessage(string message)
            => _parser.ParseAsync<TContext>(this, message);

        /// <summary>
        /// Executes a command
        /// </summary>
        /// <param name="command">The command to execute</param>
        /// <param name="context">The context to provide to the command</param>
        /// <param name="arguments">Arguments to the command</param>
        /// <param name="services">The service provider used for dependency injection</param>
        /// <returns>A task representing the command's execution</returns>
        public async Task<IResult> ExecuteAsync(
            CommandInfo command,
            TContext context,
            object[] arguments,
            IServiceProvider services = null)
        {
            PipelineFunc GetPipelineFunc(int pos)
            {
                if (pos >= _pipelines.Count)
                    return (cmd, ctx, s, a) => cmd.ExecuteAsync(ctx, s, a);

                return (cmd, ctx, s, a) => _pipelines[pos](cmd, ctx, s, a,
                    () => GetPipelineFunc(pos + 1)(cmd, ctx, s, a));
            }

            return await GetPipelineFunc(0)(command, context, services,
                arguments);
        }
    }
}
