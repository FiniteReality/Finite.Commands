using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Wumpus.Commands
{
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
        public IReadOnlyCollection<string> Aliases => _aliases;

        /// <summary>
        /// A collection of attributes applied to this module.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes => _attributes;

        /// <summary>
        /// A collection of submodules of this module.
        /// </summary>
        public IReadOnlyCollection<ModuleInfo> Submodules => _submodules;

        /// <summary>
        /// A collection of commands registered in this module.
        /// </summary>
        public IReadOnlyCollection<CommandInfo> Commands => _commands;

        /// <summary>
        /// The parent module of this module.
        /// </summary>
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

            var builtSubmodules = ImmutableArray.CreateBuilder<ModuleInfo>(submodules.Count);
            foreach (var module in submodules)
            {
                builtSubmodules.Add(module.Build(this));
            }

            var builtCommands = ImmutableArray.CreateBuilder<CommandInfo>(commands.Count);
            foreach (var command in commands)
            {
                builtCommands.Add(command.Build(this));
            }

            _submodules = builtSubmodules.ToImmutable();
            _commands = builtCommands.ToImmutable();
        }
    }
}
