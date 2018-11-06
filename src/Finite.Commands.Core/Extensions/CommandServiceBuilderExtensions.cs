using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Finite.Commands.Extensions
{
    /// <summary>
    /// Utility methods for <see cref="CommandServiceBuilder{TContext}"/>
    /// </summary>
    public static class CommandServiceBuilderExtensions
    {
        // TODO: Add overloads for AddModules to accept params Type[], params ModuleInfo[], and IEnumerables of such?
        
        /// <summary>
        /// Looks up all modules in the current assembly and loads them into the CommandServiceBuilder
        /// </summary>
        /// <typeparam name="TContext">The <see cref="ICommandContext{TContext}"/> of the CommandServiceBuilder</typeparam>
        /// <param name="assembly">Use <see cref="Assembly.GetEntryAssembly"/>.</param>
        /// <param name="services">A service collection, for dependency injection. Currently unused.</param>
        /// <returns>The modified <see cref="CommandServiceBuilder{TContext}"/> for chaining.</returns>
        public static CommandServiceBuilder<TContext> AddModules<TContext>(this CommandServiceBuilder<TContext> builder, Assembly assembly, IServiceProvider services)
            where TContext : class, ICommandContext<TContext>
        {
            IReadOnlyList<TypeInfo> types = Search<TContext>(assembly);

            foreach (TypeInfo type in types)
            {
                builder.AddModule(type);
            }

            return builder;
        }

        private static IReadOnlyList<TypeInfo> Search<TContext>(Assembly assembly)
            where TContext : class, ICommandContext<TContext>
        {
            var result = new List<TypeInfo>();

            foreach (TypeInfo typeInfo in assembly.DefinedTypes)
            {
                if (typeInfo.IsPublic || typeInfo.IsNestedPublic)
                {
                    if (ClassBuilder.IsValidModule<TContext>(typeInfo))
                    {
                        result.Add(typeInfo);
                    }
                }
            }

            return result;
        }
    }
}
