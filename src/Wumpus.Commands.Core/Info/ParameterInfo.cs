using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    /// <summary>
    /// Contains information about a command
    /// </summary>
    public class ParameterInfo
    {
        private readonly Type _type;
        private readonly IReadOnlyCollection<string> _aliases;
        private readonly IReadOnlyCollection<Attribute> _attributes;
        private readonly CommandInfo _command;

        /// <summary>
        /// A collection of aliases of the parameter.
        /// </summary>
        /// <value>
        /// The Aliases property gets a collection of type <see cref="string"/>
        /// of aliases added to the parameter.
        /// </value>
        public IReadOnlyCollection<string> Aliases => _aliases;
        /// <summary>
        /// A collection of attributes applied to the parameter.
        /// </summary>
        /// <value>
        /// The Attributes property gets a collection of type
        /// <see cref="Attribute"/> of attributes added to the parameter.
        /// </value>
        public IReadOnlyCollection<Attribute> Attributes => _attributes;
        /// <summary>
        /// The type of this parameter.
        /// </summary>
        /// <value>
        /// The Type property gets the <see cref="Type"/> of the parameter.
        /// </value>
        public Type Type => _type;
        /// <summary>
        /// The parent command of this parameter.
        /// </summary>
        /// <value>
        /// The Command property gets the <see cref="CommandInfo"/> which
        /// contains this parameter.
        /// </value>
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
