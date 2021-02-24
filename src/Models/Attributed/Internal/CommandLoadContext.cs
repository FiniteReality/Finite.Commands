using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;

namespace Finite.Commands.AttributedModel
{
    internal class CommandLoadContext : AssemblyLoadContext, IDisposable
    {
        private readonly AssemblyDependencyResolver _resolver;

        public string EntryPointPath { get; }
        public Assembly EntryPoint { get; }

        public CommandLoadContext(string mainAssembly)
            : base(true)
        {
            EntryPointPath = mainAssembly;

            _resolver = new AssemblyDependencyResolver(mainAssembly);
            EntryPoint = LoadFromAssemblyPath(mainAssembly);
        }

        /// <summary>
        /// Defines a dynamic assembly with the given display name
        /// <paramref name="name"/>.
        /// </summary>
        /// <param name="name">
        /// The display name of the dynamic assembly.
        /// </param>
        /// <returns>
        /// An <see cref="AssemblyBuilder"/> which can be used to populate the
        /// assembly.
        /// </returns>
#pragma warning disable CA1822 // Instance method to force access through scope
        public AssemblyBuilder DefineDynamicAssembly(string name)
#pragma warning restore CA1822
        {
            var assemblyName = new AssemblyName(
                $"Finite.Commands.Models.AttributedModel.Internal.{name}");

            return AssemblyBuilder.DefineDynamicAssembly(assemblyName,
                AssemblyBuilderAccess.RunAndCollect);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            foreach (var ignoredAssembly in Default.Assemblies)
            {
                var ignoredName = ignoredAssembly.GetName();

                if (AssemblyName.ReferenceMatchesDefinition(
                    ignoredName, assemblyName))
                    return null;
            }

            var path = _resolver.ResolveAssemblyToPath(assemblyName);

            return path != null
                ? LoadFromAssemblyPath(path)
                : null;
        }

        protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
        {
            var path = _resolver.ResolveUnmanagedDllToPath(unmanagedDllName);

            return path != null
                ? LoadUnmanagedDllFromPath(path)
                : default;
        }

        public void Dispose()
            => Unload();
    }
}
