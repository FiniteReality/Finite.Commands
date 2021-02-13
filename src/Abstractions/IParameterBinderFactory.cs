using System;

namespace Finite.Commands
{
    /// <summary>
    /// Defines an interface which can be used to retrieve instances of
    /// <see cref="IParameterBinder{T}"/>.
    /// </summary>
    public interface IParameterBinderFactory
    {
        /// <summary>
        /// Gets the parameter binder for the given
        /// <paramref name="parameterType"/>.
        /// </summary>
        /// <param name="parameterType">
        /// The type of parameter binder to retrieve.
        /// </param>
        /// <returns>
        /// The <see cref="IParameterBinder{T}"/> which can be used to bind
        /// instances of <paramref name="parameterType"/>.
        /// </returns>
        IParameterBinder<object> GetBinder(Type parameterType);
    }
}
