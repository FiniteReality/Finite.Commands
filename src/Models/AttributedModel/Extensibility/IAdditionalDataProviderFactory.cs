using System.Collections.Generic;
using System.Reflection;

namespace Finite.Commands.AttributedModel
{
    /// <summary>
    /// Defines an interface which can be used to create instances of
    /// <see cref="IAdditionalDataProviderFactory"/>.
    ///
    /// This interface is designed for infrastructure use and should not be
    /// directly used by your code.
    /// </summary>
    public interface IAdditionalDataProviderFactory
    {
        /// <summary>
        /// Gets the data provided for the given method.
        /// </summary>
        /// <param name="method">
        /// The method to return data for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> containing key-value pairs to add
        /// to <see cref="ICommand.Data"/>.
        /// </returns>
        IEnumerable<IAdditionalDataProvider> GetDataProvider(
            MethodInfo method);

        /// <summary>
        /// Gets the data provided for the given parameter.
        /// </summary>
        /// <param name="parameter">
        /// The parameter to return data for.
        /// </param>
        /// <returns>
        /// An <see cref="IEnumerable{T}"/> containing key-value pairs to add
        /// to <see cref="IParameter.Data"/>.
        /// </returns>
        IEnumerable<IAdditionalDataProvider> GetDataProvider(
            ParameterInfo parameter);
    }
}
