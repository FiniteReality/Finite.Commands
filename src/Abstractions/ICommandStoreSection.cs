namespace Finite.Commands
{
    /// <summary>
    /// Defines an interface which represents a named section of a
    /// <see cref="ICommandStore"/>.
    /// </summary>
    public interface ICommandStoreSection : ICommandStore
    {
        /// <summary>
        /// Gets the name of this section.
        /// </summary>
        CommandString Name { get; }
    }
}
