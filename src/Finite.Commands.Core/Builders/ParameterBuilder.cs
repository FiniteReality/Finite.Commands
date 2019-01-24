using System;
using System.Collections.Generic;

namespace Finite.Commands
{
    /// <summary>
    /// A builder which represents a parameter passed to a command.
    /// <seealso cref="CommandBuilder"/>
    /// </summary>
    public sealed class ParameterBuilder
    {
        // Aliases of the parameter
        private readonly List<string> _aliases;
        // Attributes of this module
        private readonly List<Attribute> _attributes;

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
        /// The type of the parameter.
        /// </summary>
        public Type Type { get; private set; }

        /// <summary>
        /// Indicates whether the parameter is optional or not.
        /// </summary>
        public bool Optional { get; private set; }

        /// <summary>
        /// Specifies the default value when <see cref="Optional"/> is set.
        /// </summary>
        public object DefaultValue { get; private set; }

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
            Type = type;
            return this;
        }

        /// <summary>
        /// Sets the default value of the created <see cref="ParameterInfo"/>.
        /// </summary>
        /// <param name="value">
        /// The default value of the parameter.
        /// </param>
        /// <returns>
        /// The current instance, for chaining calls.
        /// </returns>
        public ParameterBuilder WithDefaultValue(object value)
        {
            Optional = true;
            DefaultValue = value;
            return this;
        }

        internal ParameterInfo Build(CommandInfo command)
        {
            return new ParameterInfo(command, Aliases, Attributes, Type,
                Optional, DefaultValue);
        }
    }
}
