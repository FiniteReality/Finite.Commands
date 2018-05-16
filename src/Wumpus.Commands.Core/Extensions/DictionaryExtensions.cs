namespace System.Collections.Generic
{
    internal static class DictionaryExtensions
    {
        public static bool TryAdd<TKey, TValue>(
            this Dictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (@this.ContainsKey(key))
                return false;

            @this.Add(key, value);
            return true;
        }

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
