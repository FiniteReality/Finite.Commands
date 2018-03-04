using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    /// <summary>
    /// Contains information about a command
    /// </summary>
    public class CommandInfo
    {
        private readonly CommandCallback _callback;
        private readonly IReadOnlyCollection<Attribute> _attributes;
        private readonly IReadOnlyCollection<string> _aliases;
        private readonly ModuleInfo _module;

        /// <summary>
        /// A collection of attributes applied to the command
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes => _attributes;
        /// <summary>
        /// A collection of aliases used to invoke the command
        /// </summary>
        public IReadOnlyCollection<string> Aliases => _aliases;
        /// <summary>
        /// The parent module of this command.
        /// </summary>
        public ModuleInfo Module => _module;

        internal CommandInfo(ModuleInfo module,
            CommandCallback callback,
            IReadOnlyCollection<Attribute> attributes,
            IReadOnlyCollection<string> aliases)
        {
            _callback = callback;
            _attributes = attributes;
            _aliases = aliases;
            _module = module;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="services">The service container to use for instanciating services</param>
        /// <param name="context">The context to use for this command.</param>
        /// <param name="args">Any required arguments to <see cref="Callback"/>.</param>
        /// <returns>Any useful information after executing the command.</returns>
        internal async Task<IResult> ExecuteAsync(
            ICommandContext context, IServiceProvider services, object[] args)
        {
            return await _callback(this, context, services, args)
                .ConfigureAwait(false);
        }
    }
}
