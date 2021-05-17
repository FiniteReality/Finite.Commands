using System;
using Microsoft.CodeAnalysis;

namespace Finite.Commands.AttributedModel.SourceGenerator
{
    public partial class AttributedModelSourceGenerator
    {
        private static readonly DiagnosticDescriptor InvalidCommandReturnTypeRule
            = new(
                id: "FCAM0001",
                title: "Command has invalid return type",
                messageFormat: "Command '{0}' has invalid " +
                    "return type '{1}'",
                category: "FiniteCommandsCorrectness",
                defaultSeverity: DiagnosticSeverity.Error,
                isEnabledByDefault: true,
                description: "Commands should return ICommandResult, or an " +
                    "awaitable type returning ICommandResult."
            );
    }
}
