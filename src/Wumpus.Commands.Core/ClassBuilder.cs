using System.Reflection;

namespace Wumpus.Commands
{
    public static class ClassBuilder
    {
        public static ModuleInfo Build<TModule, TContext>()
            where TModule : ModuleBase<TContext>
            where TContext : class, ICommandContext
            => ClassBuilder<TContext>.Build(
                typeof(TModule).GetTypeInfo());

        public static bool IsValidModule<TModule, TContext>()
            where TModule : ModuleBase<TContext>
            where TContext : class, ICommandContext
            => ClassBuilder<TContext>.IsValidModuleDefinition(
                typeof(TModule).GetTypeInfo());
    }
}
