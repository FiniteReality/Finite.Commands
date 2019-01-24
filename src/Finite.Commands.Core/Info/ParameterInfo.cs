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
        /// <summary>
        /// A collection of aliases of the parameter.
        /// </summary>
        public IReadOnlyCollection<string> Aliases { get; }

        /// <summary>
        /// A collection of attributes applied to the parameter.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        /// The type of this parameter.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Indicates whether this parameter is optional or not.
        /// </summary>
        public bool Optional { get; }

        /// <summary>
        /// The default value for this parameter if <see cref="Optional"/> is
        /// set.
        /// </summary>
        public object DefaultValue { get; }

        /// <summary>
        /// The parent command of this parameter.
        /// </summary>
        public CommandInfo Command { get; }

        internal ParameterInfo(CommandInfo command,
            IReadOnlyCollection<string> aliases,
            IReadOnlyCollection<Attribute> attributes,
            Type type, bool hasDefault, object defaultValue)
        {
            Attributes = attributes;
            Aliases = aliases;
            Command = command;
            Type = type;
            Optional = hasDefault;
            DefaultValue = defaultValue;
        }
    }
}
