using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Finite.Commands.AttributedModel.SourceGenerator
{
    [Generator]
    public partial class AttributedModelSourceGenerator : ISourceGenerator
    {
        private static readonly string[] AlwaysActiveNamespaces
            = new[]
            {
                "System",
                "System.Collections.Generic",
                "System.Reflection",
                "System.Threading",
                "System.Threading.Tasks",
                "Microsoft.Extensions.DependencyInjection"
            };

        private static IEnumerable<string> GetDataFactoryAttributes(
            Compilation compilation, string attributeName)
            => compilation.Assembly.Modules
                .SelectMany(x => x.ReferencedAssemblySymbols)
                .SelectMany(x => x.GetAttributes())
                .Where(x => x.AttributeClass?.Name == attributeName)
                .Select(x => (x.ConstructorArguments.FirstOrDefault().Value as string)!)
                .Where(x => x != null);

        public void Execute(GeneratorExecutionContext context)
        {
            var receiver = (SyntaxReceiver)context.SyntaxContextReceiver!;

            var commandResultSymbol = context.Compilation
                .GetTypeByMetadataName(
                    "Finite.Commands.ICommandResult");
            var groupAttributeSymbol = context.Compilation
                .GetTypeByMetadataName(
                    "Finite.Commands.AttributedModel.GroupAttribute");
            var commandAttributeSymbol = context.Compilation
                .GetTypeByMetadataName(
                    "Finite.Commands.AttributedModel.CommandAttribute");

            Debug.Assert(commandResultSymbol != null);
            Debug.Assert(groupAttributeSymbol != null);
            Debug.Assert(commandAttributeSymbol != null);

            context.AddSource(
                "DataProvider",
                GenerateDataProviderSource(
                    GetDataFactoryAttributes(context.Compilation,
                        "AdditionalDataProviderFactoryAttribute")));

            foreach (var module in receiver.GetCommands())
            {
                var semanticModel = context.Compilation.GetSemanticModel(
                    module.Key.SyntaxTree);

                if (semanticModel.GetDeclaredSymbol(module.Key) is
                    not INamedTypeSymbol classSymbol)
                    throw new InvalidOperationException(
                        "Could not find named type symbol for " +
                        $"{module.Key.Identifier}");

                foreach (var method in module)
                {
                    if (semanticModel.GetDeclaredSymbol(method) is
                        not IMethodSymbol methodSymbol)
                        throw new InvalidOperationException(
                            "Could not find method symbol for " +
                            $"{method.Identifier}");

                    if (
                        !IsValidSyncReturnType(methodSymbol.ReturnType,
                            commandResultSymbol!)
                        && !IsValidAsyncReturnType(semanticModel,
                            methodSymbol.ReturnType, commandResultSymbol!))
                    {
                        context.ReportDiagnostic(
                            Diagnostic.Create(
                                InvalidCommandReturnTypeRule,
                                method.GetLocation(),
                                methodSymbol.ToDisplayString(),
                                methodSymbol.ReturnType.ToDisplayString()));

                        continue;
                    }


                    foreach (var parameterSymbol in methodSymbol.Parameters)
                    {
                        context.AddSource(
                            $"CommandFactory.{classSymbol.Name}.{methodSymbol.Name}.{parameterSymbol.Name}",
                            GenerateParameterSource(classSymbol, methodSymbol,
                                parameterSymbol));
                    }

                    context.AddSource(
                        $"CommandFactory.{classSymbol.Name}.{methodSymbol.Name}",
                        GenerateCommandSource(classSymbol, methodSymbol,
                            groupAttributeSymbol!, commandAttributeSymbol!));
                }
            }

            static bool IsValidAsyncReturnType(SemanticModel model,
                ITypeSymbol returnType, INamedTypeSymbol commandResultType)
            {
                return returnType.GetMembers()
                        .OfType<IMethodSymbol>()
                        .FirstOrDefault(x => x.Name == "GetAwaiter")
                        is IMethodSymbol getAwaiterMethod

                    && getAwaiterMethod.ReturnType.GetMembers()
                        .OfType<IMethodSymbol>()
                        .FirstOrDefault(x => x.Name == "GetResult")
                        is IMethodSymbol getResultMethod

                    && (
                        SymbolEqualityComparer.Default.Equals(
                            getResultMethod.ReturnType, commandResultType)
                        || getResultMethod.ReturnType.AllInterfaces
                            .Contains(commandResultType,
                                SymbolEqualityComparer.Default));
            }

            static bool IsValidSyncReturnType(ITypeSymbol returnType,
                INamedTypeSymbol commandResultType)
            {
                return returnType.AllInterfaces
                    .Contains(commandResultType,
                        SymbolEqualityComparer.Default);
            }
        }

        public void Initialize(GeneratorInitializationContext context)
            => context.RegisterForSyntaxNotifications(
                () => new SyntaxReceiver());

        private class SyntaxReceiver : ISyntaxContextReceiver
        {
            private readonly List<ClassDeclarationSyntax> _classes;
            private readonly List<MethodDeclarationSyntax> _methods;

            private INamedTypeSymbol? _moduleClassSymbol;
            private INamedTypeSymbol? _groupAttributeSymbol;
            private INamedTypeSymbol? _commandAttributeSymbol;

            public ILookup<ClassDeclarationSyntax, MethodDeclarationSyntax> GetCommands()
                => GetGroups()
                    .ToLookup(x => x.Item1, x => x.Item2);

            public int NumberOfClasses => _classes.Count;
            public int NumberOfMethods => _methods.Count;

            private IEnumerable<(ClassDeclarationSyntax, MethodDeclarationSyntax)> GetGroups()
            {
                foreach (var method in _methods)
                {
                    foreach (var @class in _classes)
                    {
                        if (@class.Contains(method))
                        {
                            yield return (@class, method);
                        }
                    }
                }
            }

            public SyntaxReceiver()
            {
                _classes = new();
                _methods = new();
            }

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                _moduleClassSymbol ??= context.SemanticModel.Compilation
                    .GetTypeByMetadataName(
                        "Finite.Commands.AttributedModel.Module");
                _groupAttributeSymbol ??= context.SemanticModel.Compilation
                    .GetTypeByMetadataName(
                        "Finite.Commands.AttributedModel.GroupAttribute");
                _commandAttributeSymbol ??= context.SemanticModel.Compilation
                    .GetTypeByMetadataName(
                        "Finite.Commands.AttributedModel.CommandAttribute");

                Debug.Assert(_moduleClassSymbol != null);
                Debug.Assert(_groupAttributeSymbol != null);
                Debug.Assert(_commandAttributeSymbol != null);

                if (
                    context.Node is ClassDeclarationSyntax classDeclaration
                    && classDeclaration.BaseList is BaseListSyntax baseList
                    && baseList.Types
                        .Any(n => IsModuleClass(n, context.SemanticModel,
                            _moduleClassSymbol!))
                    && classDeclaration.AttributeLists is
                        SyntaxList<AttributeListSyntax> attributeLists
                    && attributeLists.Any(
                        list => list.Attributes
                            .Any(
                                n => IsAttribute(n, context.SemanticModel,
                                    _groupAttributeSymbol!)))
                    )
                {
                    _classes.Add(classDeclaration);
                }
                else if (
                    context.Node is
                        MethodDeclarationSyntax methodDeclaration
                    && _classes.Any(x => x.Contains(methodDeclaration))
                    && methodDeclaration.AttributeLists.Any(
                        list => list.Attributes
                            .Any(
                                n => IsAttribute(n,
                                    context.SemanticModel,
                                    _commandAttributeSymbol!)))
                    )
                {
                    _methods.Add(methodDeclaration);
                }
            }

            private static bool IsModuleClass(BaseTypeSyntax baseType,
                SemanticModel model, INamedTypeSymbol moduleType)
            {
                var typeInfo = model.GetTypeInfo(baseType.Type);

                return SymbolEqualityComparer.Default
                    .Equals(typeInfo.Type, moduleType);
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
