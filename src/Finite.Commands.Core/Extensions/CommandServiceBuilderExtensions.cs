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
        /// <summary>
        /// Looks up all modules in the current assembly and loads them into the CommandServiceBuilder
        /// </summary>
        /// <typeparam name="TContext">The <see cref="ICommandContext{TContext}"/> of the CommandServiceBuilder</typeparam>
        /// <param name="assembly">Use <see cref="Assembly.GetEntryAssembly"/>.</param>
        /// <returns>The modified <see cref="CommandServiceBuilder{TContext}"/> for chaining.</returns>
        public static CommandServiceBuilder<TContext> AddModules<TContext>(this CommandServiceBuilder<TContext> builder, Assembly assembly)
            where TContext : class, ICommandContext
        {
            foreach (TypeInfo type in assembly.DefinedTypes)
            {
                if (type.IsPublic &&
                    ClassBuilder.IsValidModule<TContext>(type))
                {
                    builder.AddModule(type);
                }
            }

            return builder;
        }
    }
}
