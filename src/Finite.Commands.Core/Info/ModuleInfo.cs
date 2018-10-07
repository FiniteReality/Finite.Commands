using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Finite.Commands
{
    /// <summary>
    /// Contains information about a module.
    /// </summary>
    public sealed class ModuleInfo
    {
        /// <summary>
        /// A collection of aliases used to invoke this module.
        /// </summary>
        public IReadOnlyCollection<string> Aliases { get; }

        /// <summary>
        /// A collection of attributes applied to this module.
        /// </summary>
        public IReadOnlyCollection<Attribute> Attributes { get; }

        /// <summary>
        /// A collection of submodules of this module.
        /// </summary>
        public IReadOnlyCollection<ModuleInfo> Submodules { get; }

        /// <summary>
        /// A collection of commands registered in this module.
        /// </summary>
        public IReadOnlyCollection<CommandInfo> Commands { get; }

        /// <summary>
        /// The parent module of this module.
        /// </summary>
        public ModuleInfo Module { get; }

        /// <summary>
        /// The context type this module supports.
        /// </summary>
        public Type ContextType { get; }

        internal ModuleInfo(ModuleInfo parent,
            Type contextType,
            IReadOnlyCollection<string> aliases,
            IReadOnlyCollection<Attribute> attributes,
            IReadOnlyCollection<ModuleBuilder> submodules,
            IReadOnlyCollection<CommandBuilder> commands)
        {
            Aliases = aliases;
            Attributes = attributes;

            Module = parent;
            ContextType = contextType;

            var builtSubmodules = ImmutableArray
                .CreateBuilder<ModuleInfo>(submodules.Count);
            foreach (var module in submodules)
            {
                builtSubmodules.Add(module.Build(this, contextType));
            }

            var builtCommands = ImmutableArray
                .CreateBuilder<CommandInfo>(commands.Count);
            foreach (var command in commands)
            {
                builtCommands.Add(command.Build(this, contextType));
            }

            Submodules = builtSubmodules.ToImmutable();
            Commands = builtCommands.ToImmutable();
        }
    }
}
