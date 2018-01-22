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

        /// <summary>
        /// The callback to invoke when executing the command
        /// </summary>
        public CommandCallback Callback => _callback;
        /// <summary>
        /// A collection of attributes applied to the command
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes => _attributes;
        /// <summary>
        /// A collection of aliases used to invoke the command
        /// </summary>
        public IReadOnlyCollection<string> Aliases => _aliases;

        internal CommandInfo(CommandCallback callback,
            IReadOnlyCollection<Attribute> attributes,
            IReadOnlyCollection<string> aliases)
        {
            _callback = callback;
            _attributes = attributes;
            _aliases = aliases;
        }

        /// <summary>
        /// Executes the command.
        /// </summary>
        /// <param name="services">The service container to use for instanciating services</param>
        /// <param name="args">Any required arguments to <see cref="Callback"/>.</param>
        /// <returns>Any useful information after executing the command.</returns>
        public async Task<CommandResult> ExecuteAsync(IServiceProvider services, params object[] args)
        {
            return await Callback(this, services, args)
                .ConfigureAwait(false);
        }
    }
}
