using System;
using System.Collections.Generic;

namespace Finite.Commands
{
    /// <summary>
    /// A builder which represents a parameter passed to a command.
    /// <seealso cref="CommandBuilder"/>
    /// </summary>
    public class ParameterBuilder
    {
        // Aliases of the parameter
        private readonly List<string> _aliases;
        // Attributes of this module
        private readonly List<Attribute> _attributes;
        private Type _type;

        /// <summary>
        /// A collection of aliases of the parameter.
        /// </summary>
        public IReadOnlyCollection<string> Aliases
            => _aliases.AsReadOnly();
        /// <summary>
        /// A collection of attributes applied to the parameter.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes
            => _attributes.AsReadOnly();

        /// <summary>
        /// The type of this parameter.
        /// </summary>
        public Type Type => _type;

        /// <summary>
        /// Creates a new <see cref="ParameterBuilder"/> with the given name.
        /// </summary>
        /// <param name="name">
        /// The name of the parameter.
        /// </param>
        public ParameterBuilder(string name)
        {
            _aliases = new List<string>();
            _attributes = new List<Attribute>();

            _aliases.Add(name);
        }

        /// <summary>
        /// Adds aliases to the created <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="aliases">
        /// The new aliases to add
        /// </param>
        /// <returns>
        /// The current instance, for chaining calls.
        /// </returns>
        public ParameterBuilder AddAliases(params string[] aliases)
        {
            _aliases.AddRange(aliases);
            return this;
        }

        /// <summary>
        /// Adds an attribute to the created <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to add.
        /// </param>
        /// <returns>
        /// The current instance, for chaining calls.
        /// </returns>
        public ParameterBuilder AddAttribute(Attribute attribute)
        {
            _attributes.Add(attribute);
            return this;
        }

        /// <summary>
        /// Sets the type of the created <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="type">
        /// The type of the parameter.
        /// </param>
        /// <returns>
        /// The current instance, for chaining calls.
        /// </returns>
        public ParameterBuilder WithType(Type type)
        {
            _type = type;
            return this;
        }

        internal ParameterInfo Build(CommandInfo command)
        {
            return new ParameterInfo(command, Aliases, Attributes, Type);
        }
    }
}
