using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Wumpus.Commands
{
    /// <summary>
    /// Contains information about a module.
    /// </summary>
    public class ModuleInfo
    {
        private readonly IReadOnlyCollection<string> _aliases;
        private readonly IReadOnlyCollection<Attribute> _attributes;
        private readonly IReadOnlyCollection<ModuleInfo> _submodules;
        private readonly IReadOnlyCollection<CommandInfo> _commands;
        private readonly ModuleInfo _module;

        /// <summary>
        /// A collection of aliases used to invoke this module.
        /// </summary>
        /// <value>
        /// The Aliases property gets a collection of type <see cref="string"/>
        /// of aliases added to the module.
        /// </value>
        public IReadOnlyCollection<string> Aliases => _aliases;

        /// <summary>
        /// A collection of attributes applied to this module.
        /// </summary>
        /// <value>
        /// The Attributes property gets a collection of type
        /// <see cref="Attribute"/> of attributes added to the module.
        /// </value>
        public IReadOnlyCollection<Attribute> Attributes => _attributes;

        /// <summary>
        /// A collection of submodules of this module.
        /// </summary>
        /// <value>
        /// the Submodules property gets a collection of type
        /// <see cref="ModuleInfo"/> of submodules added to the module.
        /// </value>
        public IReadOnlyCollection<ModuleInfo> Submodules => _submodules;

        /// <summary>
        /// A collection of commands registered in this module.
        /// </summary>
        /// <value>
        /// the Commands property gets a collection of type
        /// <see cref="CommandInfo"/> of commands added to the module.
        /// </value>
        public IReadOnlyCollection<CommandInfo> Commands => _commands;

        /// <summary>
        /// The parent module of this module.
        /// </summary>
        /// <value>
        /// The Module property gets the <see cref="ModuleInfo"/> which
        /// contains this module.
        /// </value>
        public ModuleInfo Module => _module;

        internal ModuleInfo(ModuleInfo parent,
            IReadOnlyCollection<string> aliases,
            IReadOnlyCollection<Attribute> attributes,
            IReadOnlyCollection<ModuleBuilder> submodules,
            IReadOnlyCollection<CommandBuilder> commands)
        {
            _aliases = aliases;
            _attributes = attributes;

            _module = Module;

            var builtSubmodules = ImmutableArray
                .CreateBuilder<ModuleInfo>(submodules.Count);
            foreach (var module in submodules)
            {
                builtSubmodules.Add(module.Build(this));
            }

            var builtCommands = ImmutableArray
                .CreateBuilder<CommandInfo>(commands.Count);
            foreach (var command in commands)
            {
                builtCommands.Add(command.Build(this));
            }

            _submodules = builtSubmodules.ToImmutable();
            _commands = builtCommands.ToImmutable();
        }
    }
}
