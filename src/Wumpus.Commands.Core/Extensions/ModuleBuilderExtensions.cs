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
    }
}
