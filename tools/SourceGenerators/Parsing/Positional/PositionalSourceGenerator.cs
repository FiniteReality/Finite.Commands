using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Finite.Commands.Parsing.Positional.SourceGenerator
{
    [Generator]
    public partial class PositionalSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = (SyntaxReceiver)context.SyntaxContextReceiver!;

            var hasAttributedModel = context.Compilation
                .ReferencedAssemblyNames
                .Any(
                    x => x.Name == "Finite.Commands.Models.AttributedModel");

            if (!hasAttributedModel)
                return;

            SemanticModel? semanticModel = null;
            var parameterDataProviders = new List<(string, INamedTypeSymbol,
                IMethodSymbol, IParameterSymbol)>();

            foreach (var parameterSyntax in receiver.GetRemainderParameters())
            {
                if (semanticModel == null
                    || semanticModel.SyntaxTree != parameterSyntax.SyntaxTree)
                {
                    semanticModel = context.Compilation.GetSemanticModel(
                        parameterSyntax.SyntaxTree);
                }

                if (semanticModel.GetDeclaredSymbol(parameterSyntax) is
                    not IParameterSymbol parameter)
                    throw new InvalidOperationException(
                        "Could not find parameter symbol for " +
                        $"{parameterSyntax.Identifier}");

                if (parameter.ContainingSymbol is
                    not IMethodSymbol method)
                    throw new InvalidOperationException(
                        "Could not find method symbol for parameter " +
                        $"{parameter.Name}");

                var @class = method.ContainingType;

                parameterDataProviders.Add(
                    (
                        $"ParameterDataProvider__{@class.Name}__{method.Name}__{parameter.Name}",
                        @class,
                        method,
                        parameter
                    ));

                context.AddSource(
                    $"ParameterDataProvider.{@class.Name}.{method.Name}.{parameter.Name}",
                    GenerateParameterDataProviderSource(@class, method,
                        parameter));
            }

            context.AddSource(
                "DataProviderFactory",
                GenerateDataFactorySource(parameterDataProviders));
        }

        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(
                () => new SyntaxReceiver());

        private class SyntaxReceiver : ISyntaxContextReceiver
        {
            private readonly List<ParameterSyntax> _parameters;

            private INamedTypeSymbol? _remainderAttributeSymbol;

            public IEnumerable<ParameterSyntax> GetRemainderParameters()
                => _parameters;

            public SyntaxReceiver()
            {
                _parameters = new();
            }

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                _remainderAttributeSymbol ??= context.SemanticModel.Compilation
                    .GetTypeByMetadataName(
                        "Finite.Commands.AttributedModel.RemainderAttribute");

                Debug.Assert(_remainderAttributeSymbol != null);

                if (
                    context.Node is AttributeSyntax commandDataProviderAttr
                    && IsAttribute(commandDataProviderAttr,
                        context.SemanticModel, _remainderAttributeSymbol!))
                {
                    var node = context.Node;
                    while (node != null)
                    {
                        if (node.Parent is ParameterSyntax parameter)
                        {
                            _parameters.Add(parameter);
                            break;
                        }

                        node = node.Parent;
                    }
                }
            }

            private static bool IsAttribute(AttributeSyntax attr,
                SemanticModel model, INamedTypeSymbol commandType)
            {
                var typeInfo = model.GetTypeInfo(attr.Name);

                return SymbolEqualityComparer.Default
                    .Equals(typeInfo.Type, commandType);
            }
        }
    }
}
