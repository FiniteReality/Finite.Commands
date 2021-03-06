using System;
using System.Collections.Generic;

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

        /// <summary>
        /// Gets the type of values this parameter accepts.
        /// </summary>
        Type Type { get; }

        /// <summary>
        /// Gets the extra data which has been stored with this parameter.
        /// </summary>
        IReadOnlyDictionary<object, object?> Data { get; }
    }
}
