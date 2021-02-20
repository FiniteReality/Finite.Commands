namespace Finite.Commands
{
    /// <summary>
    /// Defines convenience methods for <see cref="IParameter"/>.
    /// </summary>
    public static class IParameterExtensions
    {
        /// <summary>
        /// Gets the <paramref name="value"/> associated with the specified
        /// <paramref name="key"/>, converted to the type
        /// <typeparamref name="T"/>.
        /// </summary>
        /// <param name="parameter">
        /// The <see cref="IParameter"/> to retrieve data values on.
        /// </param>
        /// <param name="key">
        /// The key to access.
        /// </param>
        /// <param name="value">
        /// Used to contain the retrieved value.
        /// </param>
        /// <typeparam name="T">
        /// The type of value to retrieve.
        /// </typeparam>
        /// <returns>
        /// <c>true</c> if the data could be
        /// </returns>
        public static bool TryGetData<T>(
            this IParameter parameter, object key,
            out T? value)
        {
            if (!parameter.Data.TryGetValue(key, out var boxedValue))
            {
                value = default;
                return false;
            }

            if (boxedValue is not T realValue)
            {
                value = default;
                return false;
            }

            value = realValue;
            return true;
        }
    }
}
