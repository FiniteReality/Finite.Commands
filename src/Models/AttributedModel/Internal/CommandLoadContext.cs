using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Loader;

namespace Finite.Commands.AttributedModel
{
    internal class CommandLoadContext : AssemblyLoadContext, IDisposable
    {
        private readonly AssemblyDependencyResolver _resolver;

        public Assembly MainAssembly { get; }

        public CommandLoadContext(string mainAssembly)
            : base(true)
        {
            _resolver = new AssemblyDependencyResolver(mainAssembly);

            MainAssembly = LoadFromAssemblyPath(mainAssembly);
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
