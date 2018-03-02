using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Wumpus.Commands
{
    public class CommandService<TContext>
        where TContext : class, ICommandContext
    {
        private readonly HashSet<ModuleInfo> _modules;

        public async Task<ModuleInfo> LoadModuleAsync<TModule>()
            where TModule : ModuleBase<TContext>
        {
            var module = ClassBuilder.Build<TModule, TContext>();

            lock (_modules)
                _modules.Add(module);

            return module;
        }
    }
}
