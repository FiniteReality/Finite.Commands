using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Wumpus.Commands
{
    /// <summary>
    /// A builder class for creating <see cref="CommandInfo"/> instances.
    /// </summary>
    public class CommandBuilder
    {
        // Aliases of the command
        private readonly List<string> _aliases;
        // Attributes of the command
        private readonly List<Attribute> _attributes;

        /// <summary>
        /// A collection of aliases applied to the <see cref="CommandInfo"/>.
        /// </summary>
        public IReadOnlyCollection<string> Aliases
            => _aliases.AsReadOnly();

        /// <summary>
        /// A collection of attributes applied to the <see cref="CommandInfo"/>.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes
            => _attributes.AsReadOnly();

        /// <summary>
        /// The callback of the created <see cref="CommandInfo"/>.
        /// </summary>
        public CommandCallback Callback { get; set; }

        /// <summary>
        /// The module which the command belongs to.
        /// </summary>
        public ModuleBuilder Module { get; internal set; }

        /// <summary>
        /// Creates a new CommandBuilder with the specified callback.
        /// </summary>
        /// <param name="callback">The callback which is executed whe command is invoked.</param>
        public CommandBuilder(CommandCallback callback)
        {
            _aliases = new List<string>();
            _attributes = new List<Attribute>();

            Callback = callback;
        }

        /// <summary>
        /// Adds aliases to the created <see cref="CommandInfo"/>.
        /// </summary>
        /// <param name="aliases">The new aliases to add</param>
        /// <returns>The current instance, for chaining calls</returns>
        public CommandBuilder WithAliases(params string[] aliases)
        {
            _aliases.AddRange(aliases);
            return this;
        }

        /// <summary>
        /// Adds an attribute to the created <see cref="CommandInfo"/>.
        /// </summary>
        /// <param name="attribute">The attribute to add</param>
        /// <returns>The current instance, for chaining calls</returns>
        public CommandBuilder AddAttribute(Attribute attribute)
        {
            _attributes.Add(attribute);
            return this;
        }

        /// <summary>
        /// Sets the callback of the created <see cref="CommandInfo"/>.
        /// </summary>
        /// <param name="callback">The new callback to use</param>
        /// <returns>The current instance, for chaining calls</returns>
        public CommandBuilder WithCallback(CommandCallback callback)
        {
            Callback = callback;
            return this;
        }

        /// <summary>
        /// Builds a <see cref="CommandInfo"/> object with the given properties.
        /// </summary>
        /// <returns>The built command.</returns>
        public CommandInfo Build()
            => Build(null);

        internal CommandInfo Build(ModuleInfo module)
        {
            return new CommandInfo(module, Callback, Attributes, Aliases);
        }
    }
}
