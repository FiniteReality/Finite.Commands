using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Finite.Commands.Parsing.Positional.SourceGenerator
{
    public partial class PositionalSourceGenerator
    {
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

        private static string GetReflectionSnippet(INamedTypeSymbol @class,
            IMethodSymbol method, IParameterSymbol parameter)
        {
            var reflectionTypes = string.Join(", ",
                method.Parameters
                    .Select(x => $"typeof({x.ToDisplayString()})"));

            return
$@"        private static readonly ParameterInfo Parameter__{@class.Name}__{method.Name}__{parameter.Name}
            = typeof({@class.ToDisplayString()})
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
                .GetParameters()[{parameter.Ordinal}]!;";
        }

        private static string GenerateDataFactorySource(
            List<(string provider, INamedTypeSymbol @class,
                IMethodSymbol method, IParameterSymbol parameter)> providers)
        {
            var reflectionSnippets = string.Join("\n",
                providers.Select(x =>
                    GetReflectionSnippet(x.@class, x.method,
                        x.parameter)));

            var providersWithFields = providers.Select(x =>
                (
                    field: $"Parameter__{x.@class.Name}__{x.method.Name}__{x.parameter.Name}",
                    x.provider
                ));

            var providerSnippets = string.Join(", ",
                providersWithFields
                    .Select(x => $"[{x.field}] = new {x.provider}()"));

            return
$@"using Finite.Commands.AttributedModel;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Finite.Commands.Parsing.Internal
{{
    internal class PositionalDataProviderFactory
        : IAdditionalDataProviderFactory
    {{
{reflectionSnippets}

        private static readonly Dictionary<ParameterInfo, IAdditionalDataProvider>
            DataProviders = new()
            {{
                {providerSnippets}
            }};

        public IEnumerable<IAdditionalDataProvider> GetDataProvider(
            MethodInfo method)
            => Array.Empty<IAdditionalDataProvider>();

        public IEnumerable<IAdditionalDataProvider> GetDataProvider(
            ParameterInfo parameter)
        {{
            if (DataProviders.TryGetValue(parameter, out var provider))
                yield return provider;
        }}
    }}
}}";
        }
    }
}
