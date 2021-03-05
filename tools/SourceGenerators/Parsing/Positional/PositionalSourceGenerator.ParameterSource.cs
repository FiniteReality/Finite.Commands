using Microsoft.CodeAnalysis;

namespace Finite.Commands.Parsing.Positional.SourceGenerator
{
    public partial class PositionalSourceGenerator
    {
        private static string GenerateParameterDataProviderSource(
            INamedTypeSymbol @class, IMethodSymbol method,
            IParameterSymbol parameter)
        {
            return
$@"using Finite.Commands.AttributedModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Finite.Commands.Parsing.Internal
{{
    internal class ParameterDataProvider__{@class.Name}__{method.Name}__{parameter.Name}
        : IAdditionalDataProvider
    {{
        public IEnumerable<KeyValuePair<object, object?>> GetData()
        {{
            yield return KeyValuePair.Create<object,object?>(
                PositionalCommandParserConstants.RemainderParameter,
                true);
        }}
    }}
}}";
        }
    }
}
