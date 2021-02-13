namespace Finite.Commands.Parsing
{
    /// <summary>
    /// Defines an interface which can be used to populate command contexts
    /// based on input strings.
    /// </summary>
    public interface ICommandParser
    {
        /// <summary>
        /// Parses <paramref name="message"/> and populates
        /// <paramref name="context"/> with the result.
        /// </summary>
        /// <param name="context">
        /// The context to populate.
        /// </param>
        /// <param name="message">
        /// The raw text for the message.
        /// </param>
        void Parse(CommandContext context, string message);
    }
}
