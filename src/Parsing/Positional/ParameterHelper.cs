namespace Finite.Commands.Parsing
{
    internal static class ParameterHelper
    {
        public static string GetParameterName(int position)
            => $"{nameof(PositionalCommandParser)}_param_{position}";

        public static object TokenCount { get; } = new object();
    }
}
