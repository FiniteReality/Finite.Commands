namespace Finite.Commands.Parsing
{
    /// <summary>
    /// Defines a class which contains constants used in the positional command
    /// parser.
    /// </summary>
    public static class PositionalCommandParserConstants
    {
        /// <summary>
        /// Gets an object which can be used as a key in
        /// <see cref="IParameter.Data"/> to specify that a parameter consumes
        /// the entire input string.
        /// </summary>
        public static object RemainderParameter { get; } = new object();
    }
}
