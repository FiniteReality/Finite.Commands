using System.Linq;
using Microsoft.CodeAnalysis;

namespace Finite.Commands.AttributedModel.SourceGenerator
{
    public partial class AttributedModelSourceGenerator
    {
        private static string GenerateParameterSource(
            INamedTypeSymbol @class,
            IMethodSymbol method,
            IParameterSymbol parameter)
        {
            var namespaces = string.Join("\n",
                AlwaysActiveNamespaces
                    .Append(@class.ContainingNamespace.ToDisplayString())
                    .Append(@parameter.Type.ContainingNamespace.ToDisplayString())
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
    internal class CommandFactory__{@class.Name}__{method.Name}__{parameter.Name}
        : IParameter
    {{
        private static readonly ParameterInfo Parameter
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
                    modifiers: null)!
                .GetParameters()[{parameter.Ordinal}]!;

        private IReadOnlyDictionary<object, object?>? _data;

        public string Name {{ get; }} = ""{parameter.Name}"";
        public Type Type {{ get; }} = typeof({parameter.Type.ToDisplayString()});

        public IReadOnlyDictionary<object, object?> Data
        {{
            get
            {{
                return _data ??= new Dictionary<object, object?>(
                    GetData());
            }}
        }}

        private static IEnumerable<KeyValuePair<object, object?>> GetData()
            => DataProvider.GetData(Parameter);
    }}
}}";
        }
    }
}
