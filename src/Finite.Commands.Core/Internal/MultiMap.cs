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
            => _members[key].AsReadOnly();

        public int Count
            => _members.Sum(x => x.Value.Count);

        public bool Contains(TKey key)
            => _members.ContainsKey(key);

        public IEnumerator<IGrouping<TKey, TValue>> GetEnumerator()
            => _members.SelectMany(
                    x => x.Value,
                    (x, y) => new { x.Key, Value = y })
                .ToLookup(x => x.Key, x => x.Value)
                .GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        public bool TryGetValues(TKey key, out ICollection<TValue> values)
        {
            List<TValue> _values;
            if (_members.TryGetValue(key, out _values))
            {
                values = _values.AsReadOnly();
                return true;
            }
            else
            {
                values = null;
                return false;
            }
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
    }
}
