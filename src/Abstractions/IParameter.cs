namespace Finite.Commands
{
    /// <summary>
    /// Defines an interface which stores information for a given command
    /// parameter.
    /// </summary>
    public interface IParameter
    {
        /// <summary>
        /// Gets the name which uniquely identifies this command parameter.
        /// </summary>
        string Name { get; }
    }
}
