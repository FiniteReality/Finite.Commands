using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Wumpus.Commands
{
    internal class ClassBuilder<TContext>
        where TContext : class, ICommandContext
    {
        private static readonly TypeInfo _VoidTypeInfo =
            typeof(void).GetTypeInfo();
        private static readonly TypeInfo _NonGenericTaskTypeInfo =
            typeof(Task).GetTypeInfo();
        private static readonly TypeInfo _GenericTaskTypeInfo =
            typeof(Task<>).GetTypeInfo();
        private static readonly TypeInfo _ICommandResultTypeInfo =
            typeof(IResult).GetTypeInfo();
        private static readonly TypeInfo _ModuleBaseTypeInfo =
            typeof(ModuleBase<TContext>).GetTypeInfo();

        private static readonly ConcurrentDictionary<Type,
            Func<Task, IResult>> _compiledResultGetters =
                new ConcurrentDictionary<Type, Func<Task, IResult>>
                {
                    [typeof(Task)] = (x) => SuccessResult.Instance
                };

        public static bool IsValidModuleDefinition(TypeInfo type)
        {
            return _ModuleBaseTypeInfo.IsAssignableFrom(type) &&
                type.DeclaredMethods.Any(x => IsValidCommandDefinition(x));
        }

        public static bool IsValidCommandDefinition(MethodInfo method)
        {
            bool IsValidReturnType(Type returnType)
            {
                return _VoidTypeInfo == returnType
                    || _NonGenericTaskTypeInfo == returnType
                    || (returnType.IsConstructedGenericType
                        && _ICommandResultTypeInfo.IsAssignableFrom(
                            returnType.GetGenericArguments().First()));
            }

            return method.IsPublic &&
                method.GetCustomAttribute<CommandAttribute>() != null &&
                IsValidReturnType(method.ReturnType);
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

            foreach (var parameter in method.GetParameters())
                builder.AddParameter(BuildParameter(parameter));

            return builder;
        }

        private static ParameterBuilder BuildParameter(
            System.Reflection.ParameterInfo parameter)
        {
            var builder = new ParameterBuilder(parameter.Name);
            var attributes = parameter.GetCustomAttributes();

            builder.WithType(parameter.ParameterType);

            foreach (var attribute in attributes)
            {
                switch (attribute)
                {
                    case AliasAttribute aliases:
                        builder.AddAliases(aliases.Aliases);
                        break;
                    default:
                        builder.AddAttribute(attribute);
                        break;
                }
            }

            return builder;
        }

        private static CommandCallback CreateCallback(MethodInfo method)
        {
            var factory = ActivatorUtilities
                .CreateFactory(method.DeclaringType, Array.Empty<Type>());

            var onExecuting = GetOnExecutingCallback(method.DeclaringType);

            return async (command, context, services, arguments) =>
            {
                var module = factory(services, Array.Empty<object>())
                    as ModuleBase<TContext>;

                module.SetContext(context);

                if (onExecuting != null)
                    onExecuting(module, command);

                try
                {
                    var boxedResult = method.Invoke(module, arguments);

                    if (!(boxedResult is Task task))
                        return SuccessResult.Instance;

                    await task;

                    var getter = _compiledResultGetters.GetOrAdd(
                        task.GetType(), CreateResultGetter);

                    return getter(task);
                }
                finally
                {
                    if (module is IDisposable disposable)
                        disposable.Dispose();
                }
            };
        }

        private static Func<Task, IResult> CreateResultGetter(Type type)
        {
            // Creates a lambda function which looks similar to this:
            // (x) => ((Task<TResult>)x).Result

            var parameter = Expression.Parameter(typeof(Task));

            return Expression.Lambda<Func<Task, IResult>>(
                Expression.Property(
                    Expression.Convert(parameter, type),
                    "Result"),
                parameter)
                    .Compile();
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
