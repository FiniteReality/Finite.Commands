namespace System.Collections.Generic
{
    internal static class DictionaryExtensions
    {
        /// <summary>
        /// Attempts to add a given key/value pair into a Dictionary.
        /// </summary>
        /// <param name="this">
        /// The dictionary to add into.
        /// </param>
        /// <param name="key">
        /// The key of the key/value pair to add.
        /// </param>
        /// <param name="value">
        /// The value of the key/value pair to add.
        /// </param>
        /// <typeparam name="TKey">
        /// The type of <paramref name="key"/>.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of <paramref name="value"/>.
        /// </typeparam>
        /// <returns>
        /// <code>true</code> when the insertion is successful.
        /// </returns>
        public static bool TryAdd<TKey, TValue>(
            this Dictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (@this.ContainsKey(key))
                return false;

            @this.Add(key, value);
            return true;
        }

        /// <summary>
        /// Attempts to retrieve or create a key/value pair in a dictionary.
        /// </summary>
        /// <param name="this">
        /// The dictionary to retrieve from, or add a key/value pair into.
        /// </param>
        /// <param name="key">
        /// The key to search for.
        /// </param>
        /// <param name="valueFactory">
        /// A callback to create a value to insert if the key is not present.
        /// </param>
        /// <typeparam name="TKey">
        /// The type of <paramref name="key"/>.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of value to insert.
        /// </typeparam>
        /// <returns>
        /// An instance of type <typeparamref name="TValue"/> which was
        /// retrieved, or inserted.
        /// </returns>
        public static TValue GetOrAdd<TKey, TValue>(
            this Dictionary<TKey, TValue> @this,
            TKey key, Func<TKey, TValue> valueFactory)
        {
            TValue value;
            if (@this.TryGetValue(key, out value))
                return value;

            value = valueFactory(key);

            @this.Add(key, value);

            return value;
        }

        /// <summary>
        /// Attempts to remove a key/value pair from a dictionary.
        /// </summary>
        /// <param name="this">
        /// The dictionary to remove from.
        /// </param>
        /// <param name="key">
        /// The key to remove.
        /// </param>
        /// <param name="value">
        /// Updated with the value the key/value pair contained, or
        /// initialized to <code>default(TValue)</code> when unsuccessful.
        /// </param>
        /// <typeparam name="TKey">
        /// The type of <paramref name="key"/>.
        /// </typeparam>
        /// <typeparam name="TValue">
        /// The type of <paramref name="value"/>.
        /// </typeparam>
        /// <returns>
        /// <code>true</code> when the removal was successful.
        /// </returns>
        public static bool TryRemove<TKey, TValue>(
            this Dictionary<TKey, TValue> @this,
            TKey key, out TValue value)
        {
            if (@this.TryGetValue(key, out value))
            {
                return @this.Remove(key);
            }

            return false;
        }
    }
}
