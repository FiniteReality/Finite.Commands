using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Finite.Commands
{
    /// <summary>
    /// A builder class for creating <see cref="CommandInfo"/> instances.
    /// </summary>
    public sealed class CommandBuilder
    {
        // Aliases of the command
        private readonly List<string> _aliases;
        // Attributes of the command
        private readonly List<Attribute> _attributes;
        private readonly List<ParameterBuilder> _parameters;

        /// <summary>
        /// A collection of aliases applied to the
        /// <see cref="CommandInfo"/>.
        /// </summary>
        public IReadOnlyCollection<string> Aliases
            => _aliases.AsReadOnly();

        /// <summary>
        /// A collection of attributes applied to the
        /// <see cref="CommandInfo"/>.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes
            => _attributes.AsReadOnly();

        /// <summary>
        /// A collection of parameters passed to this command.
        /// </summary>
        public IReadOnlyCollection<ParameterBuilder> Parameters
            => _parameters.AsReadOnly();

        /// <summary>
        /// The callback of the created <see cref="CommandInfo"/>.
        /// </summary>
        public CommandCallback Callback { get; set; }

        /// <summary>
        /// Creates a new <see cref="CommandBuilder"/> with the specified
        /// callback.
        /// </summary>
        /// <param name="callback">
        /// The callback which is executed when the command is invoked.
        /// </param>
        public CommandBuilder(CommandCallback callback)
        {
            _aliases = new List<string>();
            _attributes = new List<Attribute>();
            _parameters = new List<ParameterBuilder>();

            Callback = callback;
        }

        /// <summary>
        /// Adds aliases to the created <see cref="CommandInfo"/>.
        /// </summary>
        /// <param name="aliases">
        /// The new aliases to add.
        /// </param>
        /// <returns>
        /// The current instance, for chaining calls.</returns>
        public CommandBuilder AddAliases(params string[] aliases)
        {
            _aliases.AddRange(aliases);
            return this;
        }

        /// <summary>
        /// Adds an attribute to the created <see cref="CommandInfo"/>.
        /// </summary>
        /// <param name="attribute">
        /// The attribute to add.
        /// </param>
        /// <returns>
        /// The current instance, for chaining calls.
        /// </returns>
        public CommandBuilder AddAttribute(Attribute attribute)
        {
            _attributes.Add(attribute);
            return this;
        }

        /// <summary>
        /// Adds a parameter to the created <see cref="CommandInfo"/>.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to add.
        /// </param>
        /// <returns>
        /// The current instance, for chaining calls.</returns>
        public CommandBuilder AddParameter(ParameterBuilder parameter)
        {
            _parameters.Add(parameter);
            return this;
        }

        /// <summary>
        /// Sets the callback of the created <see cref="CommandInfo"/>.
        /// </summary>
        /// <param name="callback">
        /// The new callback to use.
        /// </param>
        /// <returns>
        /// The current instance, for chaining calls.
        /// </returns>
        public CommandBuilder WithCallback(CommandCallback callback)
        {
            Callback = callback;
            return this;
        }

        /// <summary>
        /// Builds a <see cref="CommandInfo"/> object with the given
        /// properties.
        /// </summary>
        /// <returns>
        /// The built command.
        /// </returns>
        public CommandInfo Build()
            => Build(null);

        internal CommandInfo Build(ModuleInfo module)
        {
            return new CommandInfo(module, Callback,
                Aliases, Attributes, Parameters);
        }
    }
}
