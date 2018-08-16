using System.Reflection;

namespace Finite.Commands
{
    /// <summary>
    /// Utility functions for building <see cref="ModuleInfo"/> from types
    /// defined in user code.
    /// </summary>
    public static class ClassBuilder
    {
        /// <summary>
        /// Builds a <see cref="ModuleInfo"/> for the given
        /// <typeparamref name="TModule"/>.
        /// </summary>
        /// <typeparam name="TModule">
        /// The module type to build.
        /// </typeparam>
        /// <typeparam name="TContext">
        /// The context type used by <typeparamref name="TModule"/>.
        /// </typeparam>
        /// <returns>
        /// A <see cref="ModuleInfo"/> instance representing the module and any
        /// submodules and commands it contains.
        /// </returns>
        public static ModuleInfo Build<TModule, TContext>()
            where TModule : ModuleBase<TContext>
            where TContext : class, ICommandContext
            => ClassBuilder<TContext>.Build(
                typeof(TModule).GetTypeInfo());

        /// <summary>
        /// Checks whether the given <typeparamref name="TModule"/> is a valid
        /// module definition when used with the
        /// <typeparamref name="TContext"/> context.
        /// </summary>
        /// <typeparam name="TModule">
        /// The module type to check.
        /// </typeparam>
        /// <typeparam name="TContext">
        /// The context type to check.
        /// </typeparam>
        /// <returns>
        /// Returns <code>true</code> when the given type parameters pass
        /// validation.
        /// </returns>
        public static bool IsValidModule<TModule, TContext>()
            where TModule : ModuleBase<TContext>
            where TContext : class, ICommandContext
            => ClassBuilder<TContext>.IsValidModuleDefinition(
                typeof(TModule).GetTypeInfo());
    }
}
