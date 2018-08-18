using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Finite.Commands
{
    internal sealed class MultiMap<TKey, TValue>
        : ILookup<TKey, TValue>
    {
        private readonly Dictionary<TKey, List<TValue>> _members =
            new Dictionary<TKey, List<TValue>>();

        public IEnumerable<TValue> this[TKey key]
            => _members[key];

        public int Count
            => _members.Sum(x => x.Value.Count);

        public bool Contains(TKey key)
            => _members.ContainsKey(key);

        public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
            => _members.Select(x => new Grouping(x))
                    .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public bool TryGetValues(TKey key, out ICollection<TValue> values)
        {
            List<TValue> temp;
            if (_members.TryGetValue(key, out temp))
            {
                values = temp;
                return true;
            }

            values = null;
            return false;
        }

        public bool TryAddValue(TKey key, TValue value)
        {
            var values = _members.GetOrAdd(key, (_) => new List<TValue>());

            values.Add(value);

            return true;
        }

        public bool TryRemoveValue(TKey key, TValue value)
        {
            if (_members.TryGetValue(key, out var values))
                return values.Remove(value);

            return false;
        }

        private class Grouping : IGrouping<TKey, TValue>
        {
            private readonly List<TValue> _values;

            public Grouping(KeyValuePair<TKey, List<TValue>> pair)
            {
                Key = pair.Key;
                _values = pair.Value;
            }

            public TKey Key { get; }

            public IEnumerator<TValue> GetEnumerator()
                => _values.GetEnumerator();

            IEnumerator IEnumerable.GetEnumerator()
                => GetEnumerator();
        }
    }
}
