using System.Collections.Generic;
using System.Reflection;

namespace Finite.Commands.AttributedModel
{
    /// <summary>
    /// Defines an interface which can be used to provide extra data to a
    /// command or parameter.
    ///
    /// This interface is designed for infrastructure use and should not be
    /// directly used by your code.
    /// </summary>
    public interface IAdditionalDataProvider
    {
        /// <summary>
        /// Gets the data provided for the given method.
        /// </summary>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> containing key-value pairs to add
        /// to <see cref="ICommand.Data"/> or <see cref="IParameter.Data"/>.
        /// </returns>
        IEnumerable<KeyValuePair<object, object?>> GetData();
    }
}
