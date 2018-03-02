using System;
using System.Collections.Generic;
using System.Text;

namespace Wumpus.Commands.Extensions
{
    public static class ModuleBuilderExtensions
    {
        public static ModuleBuilder AddSubmodule(this ModuleBuilder @this, Action<ModuleBuilder> builderFunc)
        {
            var module = new ModuleBuilder();
            builderFunc(module);

            return @this.AddSubmodule(module);
        }

        public static ModuleBuilder AddCommand(this ModuleBuilder @this, Action<CommandBuilder> builderFunc)
        {
            var command = new CommandBuilder(null);
            builderFunc(command);

            if (command.Callback == null)
                throw new InvalidOperationException("Cannot add a command with no callback");

            return @this.AddCommand(command);
        }
    }
}
