using System.Collections.Generic;

namespace Finite.Commands
{
    /// <summary>
    /// Defines an interface which stores the information for a given command
    /// overload.
    /// </summary>
    public interface ICommand
    {
        /// <summary>
        /// Gets the <see cref="CommandPath"/>. which can be used to invoke
        /// this command overload.
        /// </summary>
        CommandPath Name { get; }

        /// <summary>
        /// Gets the parameters which can be used to pass information to this
        /// command.
        /// </summary>
        IReadOnlyCollection<IParameter> Parameters { get; }
    }
}
