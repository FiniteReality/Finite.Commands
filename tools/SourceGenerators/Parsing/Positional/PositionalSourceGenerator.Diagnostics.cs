using Microsoft.CodeAnalysis;

namespace Finite.Commands.Parsing.Positional.SourceGenerator
{
    public partial class PositionalSourceGenerator
    {
        private static readonly DiagnosticDescriptor RemainderParameterMustBeLastRule
            = new(
                id: "FCPP0001",
                title: "Remainder parameter must be last",
                messageFormat: "Remainder parameter '{0}' must be the last " +
                    "parameter in the method",
                category: "FiniteCommandsCorrectness",
                defaultSeverity: DiagnosticSeverity.Warning,
                isEnabledByDefault: true,
                description: "Parameters marked with RemainderAttribute " +
                    "should be the last parameter."
            );
    }
}
