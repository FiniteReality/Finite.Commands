using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Finite.Commands.AttributedModel.SourceGenerator
{
    [Generator]
    public class AttributedModelSourceGenerator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            // TODO: generate source
            var receiver = (SyntaxReceiver)context.SyntaxContextReceiver!;

            foreach (var module in receiver.GetCommands())
            {
                var semanticModel = context.Compilation.GetSemanticModel(
                    module.Key.SyntaxTree);

                var groupAttributeSymbol = context.Compilation
                    .GetTypeByMetadataName(
                        "Finite.Commands.AttributedModel.GroupAttribute");
                var commandAttributeSymbol = context.Compilation
                    .GetTypeByMetadataName(
                        "Finite.Commands.AttributedModel.CommandAttribute");

                Debug.Assert(groupAttributeSymbol != null);
                Debug.Assert(commandAttributeSymbol != null);

                var _classSymbol = semanticModel.GetDeclaredSymbol(module.Key);
                if (_classSymbol is null)
                    throw new InvalidOperationException(
                        $"Could not find symbol for {module.Key.Identifier}");
                if (_classSymbol is not INamedTypeSymbol classSymbol)
                    throw new InvalidOperationException(
                        $"Could not find named type symbol for {module.Key.Identifier}");


                foreach (var method in module)
                {
                    var _methodSymbol = semanticModel.GetDeclaredSymbol(method);
                    if (_methodSymbol is null)
                        throw new InvalidOperationException(
                            $"Could not find symbol for {method.Identifier}");
                    if (_methodSymbol is not IMethodSymbol methodSymbol)
                        throw new InvalidOperationException(
                            $"Could not find method symbol for {method.Identifier}");

                    context.AddSource(
                        $"CommandFactory__{module.Key.Identifier}__{method.Identifier}",
                        GenerateSourceString(classSymbol, methodSymbol,
                            groupAttributeSymbol!, commandAttributeSymbol!));

                    /*foreach (var parameter)
                    {
                        context.AddSource(
                            $"CommandFactory__{module.Key.Identifier}__{method.Identifier}__{parameter.Identifier}",
                            GenerateSourceString(classSymbol, methodSymbol,
                                groupAttributeSymbol!, commandAttributeSymbol!));
                    }*/
                }
            }

            static string GenerateSourceString(
                INamedTypeSymbol @class,
                IMethodSymbol method,
                INamedTypeSymbol groupAttributeSymbol,
                INamedTypeSymbol commandAttributeSymbol)
            {
                var commandPath = string.Empty;
                {
                    var segment = GetStringFromAttribute(method,
                        commandAttributeSymbol);

                    commandPath = $"new CommandString(\"{segment}\")";

                    var currentClass = @class;
                    do
                    {
                        segment = GetStringFromAttribute(currentClass,
                            groupAttributeSymbol);

                        commandPath =
                            "CommandPath.Combine(" +
                            $"new CommandString(\"{segment}\"), " +
                            $"{commandPath})";

                        currentClass = @class.ContainingType;
                    }
                    while (currentClass != null);
                }

                return
$@"using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

using {@class.ContainingNamespace.MetadataName};

namespace Finite.Commands.AttributedModel.Internal.Commands
{{
    class CommandFactory__{@class.Name}__{method.Name} : ICommand
    {{
        private static ObjectFactory CommandClassFactory
            = ActivatorUtilities.CreateFactory(
                typeof({@class.Name}),
                Array.Empty<Type>());

        public CommandString Name {{ get; }} = {commandPath};

        public IReadOnlyList<IParameter> Parameters
            => Array.Empty<IParameter>();

        public IReadOnlyDictionary<object, object?> Data
            => new Dictionary<object, object?>();

        public async ValueTask<ICommandResult> ExecuteAsync(
            CommandContext context, CancellationToken cancellationToken)
        {{
            var commandClass = ({@class.Name})CommandClassFactory(
                context.Services, Array.Empty<object?>());

            Module.SetCommandContext(commandClass, context);

            try
            {{
                return await commandClass.{method.Name}(cancellationToken);
            }}
            finally
            {{
                if (commandClass is IDisposable disposable)
                    disposable.Dispose();
            }}
        }}
    }}
}}";
            }

            static string GetStringFromAttribute(ISymbol symbol,
                INamedTypeSymbol attributeType)
            {
                var attribute = symbol.GetAttributes()
                    .First(x => SymbolEqualityComparer.Default.Equals(
                        x.AttributeClass, attributeType));
                var firstArgument = attribute.ConstructorArguments.First();

                return firstArgument.Value is not string result
                    ? throw new InvalidOperationException(
                        $"First argument to attribute {attributeType.Name} "+
                        "was not a string")
                    : result;
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
                    && baseList.ChildNodes()
                        .Any(n => IsModuleClass(n, context.SemanticModel,
                            _moduleClassSymbol!))
                    && classDeclaration.AttributeLists is
                        SyntaxList<AttributeListSyntax> attributeLists
                    && attributeLists.Any(
                        list => list.ChildNodes()
                            .Any(
                                n => IsGroupAttribute(n, context.SemanticModel,
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
                        list => list.ChildNodes()
                            .Any(
                                n => IsCommandAttribute(n,
                                    context.SemanticModel,
                                    _commandAttributeSymbol!)))
                    )
                {
                    _methods.Add(methodDeclaration);
                }
            }

            private static bool IsModuleClass(SyntaxNode node,
                SemanticModel model, INamedTypeSymbol moduleType)
            {
                if (node is not BaseTypeSyntax baseType)
                    return false;

                var typeInfo = model.GetTypeInfo(baseType.Type);

                return SymbolEqualityComparer.Default
                    .Equals(typeInfo.Type, moduleType);
            }

            private static bool IsGroupAttribute(SyntaxNode node,
                SemanticModel model, INamedTypeSymbol groupType)
            {
                if (node is not AttributeSyntax attr)
                    return false;

                var typeInfo = model.GetTypeInfo(attr.Name);

                return SymbolEqualityComparer.Default
                    .Equals(typeInfo.Type, groupType);
            }

            private static bool IsCommandAttribute(SyntaxNode node,
                SemanticModel model, INamedTypeSymbol commandType)
            {
                if (node is not AttributeSyntax attr)
                    return false;

                var typeInfo = model.GetTypeInfo(attr.Name);

                return SymbolEqualityComparer.Default
                    .Equals(typeInfo.Type, commandType);
            }
        }
    }
}
