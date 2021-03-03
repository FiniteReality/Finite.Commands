using System.Linq;
using Microsoft.CodeAnalysis;

namespace Finite.Commands.AttributedModel.SourceGenerator
{
    public partial class AttributedModelSourceGenerator
    {
        static string GenerateParameterSource(
            INamedTypeSymbol @class,
            IMethodSymbol method,
            IParameterSymbol parameter,
            INamedTypeSymbol remainderAttributeSymbol)
        {
            var hasRemainder = parameter.GetAttributes()
                .Any(x => SymbolEqualityComparer.Default.Equals(
                    x.AttributeClass, remainderAttributeSymbol));

            var namespaces = string.Join("\n",
                AlwaysActiveNamespaces
                    .Append(@class.ContainingNamespace.ToDisplayString())
                    .Append(@parameter.Type.ContainingNamespace.ToDisplayString())
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(x => $"using {x};"));

            return
$@"{namespaces}

namespace Finite.Commands.AttributedModel.Internal.Commands
{{
    class CommandFactory__{@class.Name}__{method.Name}__{parameter.Name}
        : IParameter{(hasRemainder ? ", IRemainderParameter" : "")}
    {{
        public string Name {{ get; }} = ""{parameter.Name}"";
        public Type Type {{ get; }} = typeof({parameter.Type.Name});

        public IReadOnlyDictionary<object, object?> Data
            => new Dictionary<object, object?>();
    }}
}}";
        }
    }
}
