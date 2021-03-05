using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Finite.Commands.AttributedModel.SourceGenerator
{
    public partial class AttributedModelSourceGenerator
    {
        private static string GenerateDataProviderSource(
            IEnumerable<string> providers)
        {
            var namespaces = string.Join("\n",
                AlwaysActiveNamespaces
                    .Distinct()
                    .OrderBy(x => x)
                    .Select(x => $"using {x};"));

            var providerFactoryTypes = string.Join(", ",
                providers.Select(x => $"new {x}()"));

            return
$@"{namespaces}

namespace Finite.Commands.AttributedModel.Internal.Commands
{{
    internal static class DataProvider
    {{
        private static readonly IAdditionalDataProviderFactory[] Factories
            = new IAdditionalDataProviderFactory[]
            {{
                {providerFactoryTypes}
            }};

        public static IEnumerable<KeyValuePair<object, object?>> GetData(
            MethodInfo method)
        {{
            foreach (var factory in Factories)
                foreach (var provider in factory.GetDataProvider(method))
                    foreach (var kvp in provider.GetData())
                        yield return kvp;
        }}

        public static IEnumerable<KeyValuePair<object, object?>> GetData(
            ParameterInfo parameter)
        {{
            foreach (var factory in Factories)
                foreach (var provider in factory.GetDataProvider(parameter))
                    foreach (var kvp in provider.GetData())
                        yield return kvp;
        }}
    }}
}}";
        }
    }
}
