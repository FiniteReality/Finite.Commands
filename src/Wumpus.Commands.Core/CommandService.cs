using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    public class CommandService
    {
        private readonly HashSet<ModuleInfo> _modules;

        public async Task<ModuleInfo> LoadModuleAsync<TModule, TContext>()
            where TModule : ModuleBase<TContext>
            where TContext : class, ICommandContext
        {
            var type = typeof(TModule).GetTypeInfo();
            var module = ClassBuilder.BuildClass(type);

            lock (_modules)
                _modules.Add(module);

            return module;
        }
    }
}
