using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// Contains information about a command
    /// </summary>
    public sealed class CommandInfo
    {
        private readonly CommandCallback _callback;
        private readonly IReadOnlyCollection<string> _aliases;
        private readonly IReadOnlyCollection<Attribute> _attributes;
        private readonly IReadOnlyCollection<ParameterInfo> _parameters;
        private readonly ModuleInfo _module;

        /// <summary>
        /// A collection of aliases used to invoke the command.
        /// </summary>
        public IReadOnlyCollection<string> Aliases => _aliases;
        /// <summary>
        /// A collection of attributes applied to the command.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes => _attributes;
        /// <summary>
        /// A collection of parameters passed to the command.
        /// </summary>
        public IReadOnlyCollection<ParameterInfo> Parameters => _parameters;
        /// <summary>
        /// The parent module of this command.
        /// </summary>
        public ModuleInfo Module => _module;

        internal CommandInfo(ModuleInfo module,
            CommandCallback callback,
            IReadOnlyCollection<string> aliases,
            IReadOnlyCollection<Attribute> attributes,
            IReadOnlyCollection<ParameterBuilder> parameters)
        {
            _callback = callback;
            _aliases = aliases;
            _attributes = attributes;

            _module = module;

            var builtParameters = ImmutableArray
                .CreateBuilder<ParameterInfo>(parameters.Count);
            foreach (var parameter in parameters)
            {
                builtParameters.Add(parameter.Build(this));
            }

            _parameters = builtParameters.ToImmutable();
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
