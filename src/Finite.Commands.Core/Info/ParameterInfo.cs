using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Finite.Commands
{
    /// <summary>
    /// Contains information about a command
    /// </summary>
    public sealed class ParameterInfo
    {
        private readonly Type _type;
        private readonly IReadOnlyCollection<string> _aliases;
        private readonly IReadOnlyCollection<Attribute> _attributes;
        private readonly CommandInfo _command;

        /// <summary>
        /// A collection of aliases of the parameter.
        /// </summary>
        public IReadOnlyCollection<string> Aliases => _aliases;
        /// <summary>
        /// A collection of attributes applied to the parameter.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes => _attributes;
        /// <summary>
        /// The type of this parameter.
        /// </summary>
        public Type Type => _type;
        /// <summary>
        /// The parent command of this parameter.
        /// </summary>
        public CommandInfo Command => _command;

        internal ParameterInfo(CommandInfo command,
            IReadOnlyCollection<string> aliases,
            IReadOnlyCollection<Attribute> attributes,
            Type type)
        {
            _attributes = attributes;
            _aliases = aliases;
            _command = command;
            _type = type;
        }
    }
}
