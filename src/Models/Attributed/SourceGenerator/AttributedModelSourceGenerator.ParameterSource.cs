using System.Linq;
using Microsoft.CodeAnalysis;

namespace Finite.Commands.AttributedModel.SourceGenerator
{
    public partial class AttributedModelSourceGenerator
    {
        private static string GenerateParameterSource(
            INamedTypeSymbol @class,
            IMethodSymbol method,
            IParameterSymbol parameter,
            bool isRemainder)
        {
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
        : IParameter{(isRemainder ? ", IRemainderParameter" : "")}
    {{
        public string Name {{ get; }} = ""{parameter.Name}"";
        public Type Type {{ get; }} = typeof({parameter.Type.ToDisplayString()});

        public IReadOnlyDictionary<object, object?> Data
            => new Dictionary<object, object?>();
    }}
}}";
        }
    }
}
