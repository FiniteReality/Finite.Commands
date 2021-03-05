using System;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;

namespace Finite.Commands.AttributedModel.SourceGenerator
{
    public partial class AttributedModelSourceGenerator
    {
        private static string GetStringFromAttribute(ISymbol symbol,
            INamedTypeSymbol attributeType)
        {
            var attribute = symbol.GetAttributes()
                .First(x => SymbolEqualityComparer.Default.Equals(
                    x.AttributeClass, attributeType));
            var firstArgument = attribute.ConstructorArguments.First();

            return firstArgument.Value is not string result
                ? throw new InvalidOperationException(
                    $"First argument to attribute {attributeType.Name} " +
                    "was not a string")
                : result;
        }

        private static string GetBindingFlags(IMethodSymbol method)
        {
            var publicOrNonPublic = method.DeclaredAccessibility switch
                {
                    Accessibility.Public => "BindingFlags.Public",
                    _ => "BindingFlags.NonPublic"
                };

            var staticOrInstance = method.IsStatic
                ? "BindingFlags.Static"
                : "BindingFlags.Instance";

            return $"{publicOrNonPublic} | {staticOrInstance}";
        }

        private static string GetCallConv(IMethodSymbol method)
            => !method.IsStatic
                ? "CallingConventions.HasThis"
                : "CallingConventions.Any";

        private static string GenerateCommandSource(
            INamedTypeSymbol @class,
            IMethodSymbol method,
            INamedTypeSymbol groupAttributeSymbol,
            INamedTypeSymbol commandAttributeSymbol)
        {
            string? commandPath;
            {
                var segment = GetStringFromAttribute(method,
                    commandAttributeSymbol);

                commandPath = $"new CommandString(\"{segment}\")";

                var currentClass = @class;
                do
                {
                    segment = GetStringFromAttribute(currentClass,
                        groupAttributeSymbol);

                    commandPath
                        = "CommandPath.Combine(" +
                        $"new CommandString(\"{segment}\"), " +
                        $"{commandPath})";

                    currentClass = @class.ContainingType;
                }
                while (currentClass != null);
            }

            var parameterNamespaces
                = method.Parameters
                    .Select(x => x.Type.ContainingNamespace.ToDisplayString());

            var parameterTypes = string.Join(", ",
                method.Parameters
                    .Select(
                        x => $"new CommandFactory__{@class.Name}__{method.Name}__{x.Name}()"));

            var parameterAccessors = string.Join(", ",
                method.Parameters
                    .Select(
                        x => $"({x.Type.ToDisplayString()})(context.Parameters[\"{x.Name}\"]!)"));

            var namespaces = string.Join("\n",
                AlwaysActiveNamespaces
                    .Concat(parameterNamespaces)
                    .Append(@class.ContainingNamespace.ToDisplayString())
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(x => $"using {x};"));

            var reflectionTypes = string.Join(", ",
                method.Parameters
                    .Select(x => $"typeof({x.ToDisplayString()})"));

            return
$@"{namespaces}

namespace Finite.Commands.AttributedModel.Internal.Commands
{{
    internal class CommandFactory__{@class.Name}__{method.Name}
        : ICommand
    {{
        private static readonly ObjectFactory CommandClassFactory
            = ActivatorUtilities.CreateFactory(
                typeof({@class.Name}),
                Array.Empty<Type>());

        private static readonly MethodInfo Method
            = typeof({@class.Name})
                .GetMethod(
                    name: ""{method.Name}"",
                    genericParameterCount: {method.Arity},
                    bindingAttr: {GetBindingFlags(method)},
                    binder: default,
                    callConvention: {GetCallConv(method)},
                    types: new[]
                    {{
                        {reflectionTypes}
                    }},
                    modifiers: null)!;

        private IReadOnlyDictionary<object, object?>? _data;

        public CommandString Name {{ get; }} = {commandPath};

        public IReadOnlyList<IParameter> Parameters {{ get; }}
            = new IParameter[]
            {{
                {parameterTypes}
            }};

        public IReadOnlyDictionary<object, object?> Data
        {{
            get
            {{
                return _data ??= new Dictionary<object, object?>(
                    GetData());
            }}
        }}

        public async ValueTask<ICommandResult> ExecuteAsync(
            CommandContext context, CancellationToken cancellationToken)
        {{
            var commandClass = ({@class.Name})CommandClassFactory(
                context.Services, Array.Empty<object?>());

            Module.SetCommandContext(commandClass, context);
            Module.SetCancellationToken(commandClass, cancellationToken);

            try
            {{
                return await commandClass.{method.Name}({parameterAccessors});
            }}
            finally
            {{
                if (commandClass is IDisposable disposable)
                    disposable.Dispose();
            }}
        }}

        private static IEnumerable<KeyValuePair<object, object?>> GetData()
            => DataProvider.GetData(Method);
    }}
}}";
        }
    }
}
