using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Finite.Commands
{
    internal static class ClassBuilder<TContext>
        where TContext : class, ICommandContext
    {
        private static readonly TypeInfo NonGenericTaskTypeInfo =
            typeof(Task).GetTypeInfo();
        private static readonly TypeInfo GenericTaskTypeInfo =
            typeof(Task<>).GetTypeInfo();
        private static readonly TypeInfo IResultTypeInfo =
            typeof(IResult).GetTypeInfo();
        private static readonly TypeInfo ModuleBaseTypeInfo =
            typeof(ModuleBase<TContext>).GetTypeInfo();
        private static readonly MethodInfo OnBuildingCallbackMethodInfo =
            typeof(OnBuildingCallback).GetMethod("Invoke");

        private static readonly ConcurrentDictionary<Type,
            Func<Task, IResult>> _compiledResultGetters =
                new ConcurrentDictionary<Type, Func<Task, IResult>>
                {
                    [typeof(Task)] = (x) => SuccessResult.Instance
                };

        public static bool IsValidModuleDefinition(TypeInfo type)
        {
            return !type.IsAbstract
                && ModuleBaseTypeInfo.IsAssignableFrom(type)
                && type.DeclaredMethods.Any(IsValidCommandDefinition)
                && type
                    .GetMethods(BindingFlags.Public | BindingFlags.Static)
                    .Where(x =>
                        x.GetCustomAttribute<OnBuildingAttribute>() != null)
                    .Select(IsValidOnBuildingDefinition)
                    .All(x => x);
        }

        public static bool IsValidCommandDefinition(MethodInfo method)
        {
            bool IsValidReturnType(Type returnType)
            {
                return returnType == NonGenericTaskTypeInfo
                    || IResultTypeInfo.IsAssignableFrom(returnType)
                    || (returnType.IsConstructedGenericType
                        && returnType
                            .GetGenericTypeDefinition() == GenericTaskTypeInfo
                        && IResultTypeInfo.IsAssignableFrom(
                            returnType.GetGenericArguments().First())
                        );
            }

            return method.IsPublic
                && method.GetCustomAttribute<CommandAttribute>() != null
                && IsValidReturnType(method.ReturnType);
        }

        public static bool IsValidOnBuildingDefinition(MethodInfo method)
        {
            return method.GetCustomAttribute<OnBuildingAttribute>() != null
                && method.ReturnType == OnBuildingCallbackMethodInfo.ReturnType
                && GetMatchingArgumentTypes(
                    OnBuildingCallbackMethodInfo, method)
                    .All(x => x);
        }

        public static ModuleInfo Build(TypeInfo type)
            => BuildType(type).Build<TContext>();

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

            if (parameter.IsOptional || parameter.HasDefaultValue)
                builder.WithDefaultValue(parameter.DefaultValue);

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

            var invoker = CreateMethodInvoker(method);

            Func<Task, IResult> resultGetter = null;
            if (NonGenericTaskTypeInfo.IsAssignableFrom(method.ReturnType))
                resultGetter = _compiledResultGetters.GetOrAdd(
                    method.ReturnType, CreateResultGetter);

            return async (command, context, commands, services, arguments) =>
            {
                var module = factory(services, Array.Empty<object>())
                    as ModuleBase<TContext>;

                module.SetCommands(commands);
                module.SetContext(context);

                module.CallOnExecuting(command);

                try
                {
                    var boxedResult = invoker(module, arguments);

                    if (boxedResult is null)
                        return SuccessResult.Instance;

                    if (boxedResult is IResult result)
                        return result;

                    if (!(boxedResult is Task task))
                        return SuccessResult.Instance;

                    await task;

                    return resultGetter(task);
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
            // (task) => ((Task<TResult>)task).Result

            var parameter = Expression.Parameter(typeof(Task), "task");

            return Expression.Lambda<Func<Task, IResult>>(
                Expression.Property(
                    Expression.Convert(parameter, type),
                    "Result"),
                tailCall: true,
                parameter)
                    .Compile();
        }

        private static Func<ModuleBase<TContext>, object[], object>
            CreateMethodInvoker(MethodInfo method)
        {
            // Creates a lambda function which looks similar to this:
            // (target, @params) => (object)((TModule)target.<Method>(
            //     (T1)@params[0], (T2)@params[1], ..., (TN)@params[N]))

            var target = Expression.Parameter(typeof(ModuleBase<TContext>),
                "target");
            var parameters = Expression.Parameter(typeof(object[]), "params");

            var parameterCasts = method.GetParameters()
                .Select((p, i) => Expression.Convert(
                    Expression.ArrayIndex(parameters, Expression.Constant(i)),
                    p.ParameterType));

            return Expression
                .Lambda<Func<ModuleBase<TContext>, object[], object>>(
                    Expression.Convert(
                        Expression.Call(
                            Expression.Convert(target, method.DeclaringType),
                            method, parameterCasts),
                        typeof(object)),
                    target, parameters)
                .Compile();
        }

        private static OnBuildingCallback
            GetOnBuildingCallback(TypeInfo type)
        {
            var methods = type.GetMethods(
                BindingFlags.Public | BindingFlags.Static)
                .Where(IsValidOnBuildingDefinition)
                .Select(method => method.CreateDelegate(
                    typeof(OnBuildingCallback)) as OnBuildingCallback)
                .ToArray();

            if (methods.Length > 0)
                return (module) =>
                {
                    foreach (var method in methods)
                        method(module);
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

        private static IEnumerable<bool> GetMatchingArgumentTypes(
            MethodInfo expected, MethodInfo actual)
            => expected.GetParameters()
                .Join(actual.GetParameters(),
                x => x.Position, y => y.Position,
                (expectedParam, actualParam) =>
                    expectedParam.ParameterType == actualParam.ParameterType);
    }
}
