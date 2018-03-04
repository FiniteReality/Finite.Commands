using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Wumpus.Commands
{
    internal class ClassBuilder<TContext>
        where TContext : class, ICommandContext
    {
        private static readonly object[] _emptyObjectArray = new object[]{};
        private static readonly Type[] _emptyTypeArray = new Type[]{};
        private static readonly TypeInfo _ICommandResultTypeInfo =
            typeof(IResult).GetTypeInfo();
        private static readonly TypeInfo _ModuleBaseTypeInfo =
            typeof(ModuleBase<TContext>).GetTypeInfo();

        private static readonly ConcurrentDictionary<Type,
            Func<Task, IResult>> _getResultMap =
                new ConcurrentDictionary<Type, Func<Task, IResult>>();

        public static bool IsValidModuleDefinition(TypeInfo type)
        {
            return _ModuleBaseTypeInfo.IsAssignableFrom(type) &&
                type.DeclaredMethods.Any(x => IsValidCommandDefinition(x));
        }

        public static bool IsValidCommandDefinition(MethodInfo method)
        {
            return method.IsPublic &&
                method.GetCustomAttribute<CommandAttribute>() != null;
        }

        public static ModuleInfo Build(TypeInfo type)
        {
            return BuildType(type).Build();
        }

        private static ModuleBuilder BuildType(TypeInfo type)
        {
            var builder = new ModuleBuilder();
            var attributes = type.GetCustomAttributes();
            var onBuildCallback = GetOnBuildingCallback(type);

            foreach (var attribute in attributes)
                switch (attribute)
                {
                    case AliasAttribute aliases:
                        builder.AddAliases(aliases.Aliases);
                        break;
                    default:
                        builder.AddAttribute(attribute);
                        break;
                }

            foreach (var method in GetValidCommands(type))
                builder.AddCommand(BuildCommand(method));

            foreach (var @class in GetValidModules(type))
                builder.AddSubmodule(BuildType(@class));

            if (onBuildCallback != null)
                onBuildCallback(builder);

            return builder;
        }

        private static CommandBuilder BuildCommand(MethodInfo method)
        {
            var builder = new CommandBuilder(CreateCallback(method));
            var attributes = method.GetCustomAttributes();

            foreach (var attribute in attributes)
                switch (attribute)
                {
                    case CommandAttribute command:
                        builder.AddAliases(command.Aliases);
                        break;
                    default:
                        builder.AddAttribute(attribute);
                        break;
                }

            return builder;
        }

        private static CommandCallback CreateCallback(MethodInfo method)
        {
            var factory = ActivatorUtilities
                .CreateFactory(method.DeclaringType, _emptyTypeArray);

            var onExecuting = GetOnExecutingCallback(method.DeclaringType);

            return async (command, context, services, arguments) =>
            {
                var module = factory(services, _emptyObjectArray)
                    as ModuleBase<TContext>;

                module.SetContext(context);

                if (onExecuting != null)
                    onExecuting(module, command);

                try
                {
                    var boxedResult = method.Invoke(module, arguments);
                    IResult result = SuccessResult.Instance;

                    if (boxedResult is Task task)
                    {
                        await task;
                        TryGetResult(task, ref result);
                    }

                    return result;
                }
                finally
                {
                    if (module is IDisposable disposable)
                        disposable.Dispose();
                }
            };
        }

        private static void TryGetResult(Task task, ref IResult result)
        {
            Func<Task, IResult> CreateGetterLambda(Type type)
            {
                var prop = type.GetProperty("Result")
                    .GetGetMethod();

                // TODO: find a delegate for this instead of relying on Invoke
                return x => (IResult)prop.Invoke(x, _emptyObjectArray);
            }

            bool IsTaskReturningICommandResult(Type type)
            {
                return type.IsGenericType &&
                    _ICommandResultTypeInfo.IsAssignableFrom(
                        type.GenericTypeArguments[0]);
            }

            var taskType = task.GetType();

            if (IsTaskReturningICommandResult(taskType))
            {
                var getter = _getResultMap.GetOrAdd(taskType,
                    CreateGetterLambda);

                result = getter(task) as IResult;
            }
        }

        private static OnBuildingCallback
            GetOnBuildingCallback(TypeInfo type)
        {
            var method = type.GetMethods(
                BindingFlags.Public | BindingFlags.Static)
                .SingleOrDefault(x =>
                    x.GetCustomAttribute<OnBuildingAttribute>() != null);

            if (method != null)
                return method.CreateDelegate(
                    typeof(OnBuildingCallback)) as OnBuildingCallback;

            return null;
        }

        private static Action<object, CommandInfo>
            GetOnExecutingCallback(Type type)
        {
            var method = type.GetMethods(
                BindingFlags.Public | BindingFlags.Instance)
                .SingleOrDefault(x =>
                    x.GetCustomAttribute<OnExecutingAttribute>() != null);

            if (method != null)
                return (obj, info) =>
                {
                    var cb = method.CreateDelegate(
                        typeof(OnExecutingCallback), obj)
                        as OnExecutingCallback;
                    cb(info);
                };

            return null;
        }

        private static IEnumerable<TypeInfo> GetValidModules(TypeInfo type)
        {
            foreach (var subType in type.DeclaredNestedTypes)
            {
                if (IsValidModuleDefinition(subType))
                    yield return subType;
            }
        }

        private static IEnumerable<MethodInfo> GetValidCommands(TypeInfo type)
        {
            foreach (var method in type.DeclaredMethods)
            {
                if (IsValidCommandDefinition(method))
                    yield return method;
            }
        }
    }
}
