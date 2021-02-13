using System;

namespace Finite.Commands
{
    /// <summary>
    /// Defines an interface for parameter binders.
    /// </summary>
    public interface IParameterBinder<out T>
    {
        /// <summary>
        /// Attempts to bind the given <paramref name="text"/> into an instance
        /// of <typeparamref name="T"/>.
        /// </summary>
        /// <param name="text">
        /// The text to bind.
        /// </param>
        /// <param name="success">
        /// Set to <code>true</code> to indicate successful binding.
        /// </param>
        /// <returns>
        /// An instance of <typeparamref name="T"/> if successful;
        /// <code>default</code> otherwise.
        /// </returns>
        public T? Bind(ReadOnlySpan<char> text, out bool success);
    }
}
