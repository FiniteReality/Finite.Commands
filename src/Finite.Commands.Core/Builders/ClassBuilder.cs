using System;
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
            => Build<TContext>(typeof(TModule));

        /// <summary>
        /// Builds a <see cref="ModuleInfo"/> for the given
        /// <paramref name="moduleType"/>.
        /// </summary>
        /// <param name="moduleType">
        /// The module type to build.
        /// </param>
        /// <typeparam name="TContext">
        /// The context type used by <paramref name="moduleType"/>.
        /// </typeparam>
        /// <returns>
        /// A <see cref="ModuleInfo"/> instance representing the module and any
        /// submodules and commands it contains.
        /// </returns>
        public static ModuleInfo Build<TContext>(Type moduleType)
            where TContext : class, ICommandContext
            => IsValidModule<TContext>(moduleType)
                ? ClassBuilder<TContext>.Build(moduleType.GetTypeInfo())
                : throw new ArgumentException(
                    $"{moduleType.FullName} is not a valid module",
                    nameof(moduleType));

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
        /// Returns <code>true</code> when the given module type is a valid
        /// module.
        /// </returns>
        public static bool IsValidModule<TModule, TContext>()
            where TModule : ModuleBase<TContext>
            where TContext : class, ICommandContext
            => IsValidModule<TContext>(typeof(TModule));

        /// <summary>
        /// Checks whether the given <paramref name="moduleType"/> is a valid
        /// module definition when used with the
        /// <typeparamref name="TContext"/> context.
        /// </summary>
        /// <param name="moduleType">
        /// The module type to check.
        /// </param>
        /// <typeparam name="TContext">
        /// The context type to check.
        /// </typeparam>
        /// <returns>
        /// Returns <code>true</code> when the given module is a valid module.
        /// </returns>
        public static bool IsValidModule<TContext>(Type moduleType)
            where TContext : class, ICommandContext
            => ClassBuilder<TContext>.IsValidModuleDefinition(
                moduleType.GetTypeInfo());
    }
}
