using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Finite.Commands.AttributedModel
{
    internal static class CommandBuilder
    {
        private static readonly Type ModuleType = typeof(Module);

        private static readonly MethodInfo CommandContextServicesGetterMethod
            = typeof(CommandContext).GetProperty("Services")!.GetMethod!;

        private static readonly MethodInfo CreateFactoryMethod
            = typeof(ActivatorUtilities).GetMethod("CreateFactory")!;

        private static readonly MethodInfo ArrayEmptyMethod
            = typeof(Array).GetMethod("Empty", 1, Array.Empty<Type>())!;

        private static readonly MethodInfo ExecuteAsyncMethod
            = typeof(ICommand).GetMethod("ExecuteAsync")!;

        private static readonly MethodInfo GetTypeFromHandleMethod
            = typeof(Type).GetMethod("GetTypeFromHandle")!;

        private static readonly ConstructorInfo ObjectConstructor
            = typeof(object).GetConstructor(Array.Empty<Type>())!;

        private static readonly MethodInfo ObjectFactoryInvokeMethod
            = typeof(ObjectFactory).GetMethod("Invoke")!;

        private static readonly MethodInfo UtilityExecuteAsyncMethod
            = typeof(CommandUtility).GetMethod("ExecuteAsync")!;

        public static void BuildCommandsFor(CommandLoadContext context)
        {
            var module = context.CommandsAssembly
                .DefineDynamicModule(
                    $"{context.CommandsAssembly.GetName().Name}.dll");

            foreach (var type in context.OriginalAssembly.ExportedTypes)
                if (IsValidModule(type))
                    BuildFactory(module, type);
        }

        private static void BuildFactory(
            ModuleBuilder module, Type original)
        {
            //var commands = new List<TypeBuilder>();

            foreach (var method in original.GetMethods())
            {
                if (IsValidCommand(method))
                {
                    var factory = module.DefineType(
                        $"{method.Name}Factory",
                        TypeAttributes.NotPublic | TypeAttributes.Sealed
                        | TypeAttributes.Class);
                    factory.AddInterfaceImplementation(typeof(ICommand));

                    var factoryField = PopulateConstructor(factory, original);
                    PopulateExecuteAsyncMethod(factory, method, factoryField);
                    //commands.Add(factory);

                    var type = factory.CreateType();
                    Debug.Assert(type != null);
                }
            }
        }

        private static FieldInfo PopulateConstructor(TypeBuilder builder, Type type)
        {
            var field = builder.DefineField("CommandInstanceFactory",
                typeof(ObjectFactory),
                FieldAttributes.Private | FieldAttributes.Static);

            var ctor = builder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                Array.Empty<Type>());

            var generator = ctor.GetILGenerator();

            // Call base constructor
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, ObjectConstructor);

            // Call ActivatorUtilities.CreateFactory(Type, Type[])
            generator.Emit(OpCodes.Ldtoken, type);
            generator.Emit(OpCodes.Call, GetTypeFromHandleMethod);
            generator.Emit(OpCodes.Call,
                ArrayEmptyMethod.MakeGenericMethod(typeof(Type)));
            generator.Emit(OpCodes.Call, CreateFactoryMethod);

            // CommandInstanceFactory = ActivatorUtilities.CreateFactory(
            //     typeof(type), Array.Empty<Type>());
            generator.Emit(OpCodes.Stsfld, field);

            generator.Emit(OpCodes.Ret);

            return field;
        }

        private static void PopulateExecuteAsyncMethod(TypeBuilder type,
            MethodInfo method, FieldInfo factoryField)
        {
            var builder = type.DefineMethod("ExecuteAsync",
                MethodAttributes.Public
                | MethodAttributes.Final
                | MethodAttributes.Virtual);

            builder.SetReturnType(typeof(ValueTask<ICommandResult>));
            builder.SetParameters(
                typeof(CommandContext),
                typeof(CancellationToken));

            type.DefineMethodOverride(builder, ExecuteAsyncMethod);

            var generator = builder.GetILGenerator();

            // Create instance of method.DefiningType
            // Pass to CommandUtility.ExecuteAsync
            // return ValueTask<ICommandResult> from ExecuteAsync

            // Create instance of method.DefiningType
            generator.Emit(OpCodes.Ldsfld, factoryField);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Callvirt,
                CommandContextServicesGetterMethod);
            generator.Emit(OpCodes.Call,
                ArrayEmptyMethod.MakeGenericMethod(typeof(object)));
            generator.Emit(OpCodes.Callvirt, ObjectFactoryInvokeMethod);

            // TODO: push args
            Debug.Assert(method.GetParameters().Length == 0);

            // Call command method
            generator.Emit(OpCodes.Callvirt, method);

            // Call utility ExecuteAsync method on the result
            generator.Emit(OpCodes.Call, UtilityExecuteAsyncMethod);

            // Return the result
            generator.Emit(OpCodes.Ret);
        }

        public static bool IsValidModule(Type type)
            => ModuleType.IsAssignableFrom(type)
                && type.GetMethods()
                    .Any(x => x.GetCustomAttribute<CommandAttribute>() is {});

        // TODO: validate the command
        public static bool IsValidCommand(MethodInfo method)
            => method.GetCustomAttribute<CommandAttribute>() is {}
                && method.ReturnType == typeof(ValueTask<ICommandResult>);
    }
}
