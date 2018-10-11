using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// Contains information about a command
    /// </summary>
    public sealed class CommandInfo
    {
        private readonly CommandCallback _callback;

        /// <summary>
        /// A collection of aliases used to invoke the command.
        /// </summary>
        public IReadOnlyCollection<string> Aliases { get; }
        /// <summary>
        /// A collection of attributes applied to the command.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes { get; }
        /// <summary>
        /// A collection of parameters passed to the command.
        /// </summary>
        public IReadOnlyCollection<ParameterInfo> Parameters { get; }
        /// <summary>
        /// The parent module of this command.
        /// </summary>
        public ModuleInfo Module { get; }

        /// <summary>
        /// The type of context this command supports.
        /// </summary>
        public Type ContextType { get; }

        internal CommandInfo(ModuleInfo module,
            Type contextType,
            CommandCallback callback,
            IReadOnlyCollection<string> aliases,
            IReadOnlyCollection<Attribute> attributes,
            IReadOnlyCollection<ParameterBuilder> parameters)
        {
            _callback = callback;
            Aliases = aliases;
            Attributes = attributes;

            Module = module;
            ContextType = contextType;

            var builtParameters = ImmutableArray
                .CreateBuilder<ParameterInfo>(parameters.Count);
            foreach (var parameter in parameters)
            {
                builtParameters.Add(parameter.Build(this));
            }

            Parameters = builtParameters.ToImmutable();
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="services">
        /// The service container to use for instanciating services
        /// </param>
        /// <param name="context">
        /// The context to use for this command.
        /// </param>
        /// <param name="args">
        /// Any required arguments to the command's callback.
        /// </param>
        /// <returns>
        /// Any useful information after executing the command.
        /// </returns>
        internal async Task<IResult> ExecuteAsync(
            ICommandContext context, IServiceProvider services, object[] args)
        {
            return await _callback(this, context, services, args)
                .ConfigureAwait(false);
        }

        internal Task<IResult> ExecuteAsync(
            CommandExecutionContext context)
            => ExecuteAsync(context.Context, context.ServiceProvider,
                context.Arguments);
    }
}
