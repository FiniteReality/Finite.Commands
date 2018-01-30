using System;
using System.Linq;
using System.Reflection;

namespace Wumpus.Commands
{
    internal class ClassBuilder
    {
        private static readonly TypeInfo _IModuleBaseTypeInfo =
            typeof(IModule<ICommandContext>).GetTypeInfo();

        public static bool IsValidModuleDefinition(TypeInfo type)
        {
            return _IModuleBaseTypeInfo.IsAssignableFrom(type) &&
                type.DeclaredMethods.Any(x => IsValidCommandDefinition(x));
        }

        public static bool IsValidCommandDefinition(MethodInfo method)
        {
            return method.GetCustomAttribute<CommandAttribute>() != null;
        }

        public static ModuleInfo BuildClass(TypeInfo type)
        {
            var builder = new ModuleBuilder();
            var onBuildCallback = GetOnBuildingCallback(type);


            if (onBuildCallback != null)
                onBuildCallback(null);

            return builder.Build();
        }

        private static Action<CommandService> GetOnBuildingCallback(TypeInfo type)
        {
            var method = type.GetMethod("OnBuilding",
                BindingFlags.Static | BindingFlags.NonPublic);

            if (method != null)
                return (x) => method.Invoke(null, new[]{ x });
            else
                return null;
        }
    }
}
