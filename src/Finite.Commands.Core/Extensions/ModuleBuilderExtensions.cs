using System;

namespace Finite.Commands.Extensions
{
    /// <summary>
    /// Common utility methods for <see cref="ModuleBuilder"/>.
    /// </summary>
    public static class ModuleBuilderExtensions
    {
        /// <summary>
        /// Adds a submodule to a module.
        /// </summary>
        /// <param name="this">
        /// The module to add a submodule to.
        /// </param>
        /// <param name="builderFunc">
        /// A builder function, which is called to populate the submodule.
        /// </param>
        /// <returns>
        /// The return value of
        /// <see cref="ModuleBuilder.AddSubmodule(ModuleBuilder)"/>.
        /// </returns>
        public static ModuleBuilder AddSubmodule(this ModuleBuilder @this,
            Action<ModuleBuilder> builderFunc)
        {
            var module = new ModuleBuilder();
            builderFunc(module);

            if (module.Commands.Count == 0 || module.Submodules.Count == 0)
                throw new InvalidOperationException(
                    "Cannot add a module with no commands or submodules");

            return @this.AddSubmodule(module);
        }

        /// <summary>
        /// Adds a command to a module.
        /// </summary>
        /// <param name="this">
        /// The module to add a command to.
        /// </param>
        /// <param name="builderFunc">
        /// A builder function, which is called to populate the command.
        /// </param>
        /// <returns>
        /// The return value of
        /// <see cref="ModuleBuilder.AddCommand(CommandBuilder)"/>.
        /// </returns>
        public static ModuleBuilder AddCommand(this ModuleBuilder @this,
            Action<CommandBuilder> builderFunc)
        {
            var command = new CommandBuilder(null);
            builderFunc(command);

            if (command.Callback == null)
                throw new InvalidOperationException(
                    "Cannot add a command with no callback");

            return @this.AddCommand(command);
        }
    }
}
