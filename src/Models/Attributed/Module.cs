namespace Finite.Commands.AttributedModel
{
    /// <summary>
    /// Defines a class which can be used to define command modules.
    /// </summary>
    public abstract class Module
    {
        /// <summary>
        /// Gets the context which this command is executed under.
        /// </summary>
        public CommandContext Context { get; } = null!;
    }
}
