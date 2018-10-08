using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ClassLibraryORM.Collections
{
    internal class Map<T1, T2> : IDictionary<T1, T2>
    {
        protected readonly IDictionary<T1, T2> forward;
        protected readonly IDictionary<T2, T1> reverse;
        protected readonly Indexer<T1, T2> forwardIndexer;
        protected readonly Indexer<T2, T1> reverseIndexer;

        public Map()
        {
            forward = new Dictionary<T1, T2>();
            reverse = new Dictionary<T2, T1>();
            forwardIndexer = new Indexer<T1, T2>(ref forward);
            reverseIndexer = new Indexer<T2, T1>(ref reverse);
        }

        public int Count => forward.Count;
        public bool IsReadOnly => false;
        public ICollection<T1> Keys => forward.Keys;
        public ICollection<T2> Values => forward.Values;
        public Indexer<T1, T2> KeysIndexer => forwardIndexer;
        public Indexer<T2, T1> ValuesIndexer => reverseIndexer;

        public T2 this[T1 key]
        {
            get => forward[key];
            set => Insert(key, value, false);
        }

        public void Add(T1 key, T2 value) => Insert(key, value, true);

        public void Add(KeyValuePair<T1, T2> item) => Add(item.Key, item.Value);

        public IDictionary<T1, T2> AsForwardDictionary() => forward.ToDictionary(q => q.Key, q => q.Value);

        public IDictionary<T2, T1> AsReverseDictionary() => reverse.ToDictionary(q => q.Key, q => q.Value);

        public void Clear()
        {
            forward.Clear();
            reverse.Clear();
        }

        public bool Contains(KeyValuePair<T1, T2> item) => forward.ContainsKey(item.Key) && reverse.ContainsKey(item.Value);

        public bool ContainsKey(T1 key) => forward.ContainsKey(key);

        public bool ContainsValue(T2 value) => reverse.ContainsKey(value);

        public void CopyTo(KeyValuePair<T1, T2>[] array, int arrayIndex) => throw new NotImplementedException();

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator() => forward.GetEnumerator();

        public bool Remove(T1 key)
        {
            if (!forward.ContainsKey(key))
            {
                return false;
            }

            var kv = new KeyValuePair<T1, T2>(key, forward[key]);
            return Remove(kv);
        }

        public bool Remove(KeyValuePair<T1, T2> item) => throw new NotImplementedException();

        public bool TryGetValue(T1 key, out T2 value) => forward.TryGetValue(key, out value);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private void Insert(T1 key, T2 value, bool add)
        {
            if (!key.GetType().IsValueType && key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            if (!value.GetType().IsValueType && value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!add)
            {
                if (!forward.ContainsKey(key) && !reverse.ContainsKey(value))
                {
                    add = true;
                }
                else if (!forward.ContainsKey(key))
                {
                    throw new ArgumentException($"Argument '{nameof(key)}' is not assigned.");
                }
                else if (!reverse.ContainsKey(value))
                {
                    throw new ArgumentException($"Argument '{nameof(value)}' is not assigned.");
                }
                else
                {
                    var oldvalue = forward[key];
                    reverse.Remove(oldvalue);
                    reverse.Add(value, key);

                    forward[key] = value;
                    return;
                }
            }

            if (add)
            {
                if (forward.ContainsKey(key))
                {
                    throw new ArgumentException($"Argument '{nameof(key)}' is duplicated.");
                }
                if (reverse.ContainsKey(value))
                {
                    throw new ArgumentException($"Argument '{nameof(value)}' is duplicated.");
                }

                forward.Add(key, value);
                reverse.Add(value, key);
            }
        }

        public sealed class Indexer<TIn, TOut>
        {
            private readonly IDictionary<TIn, TOut> dictionary;

            public Indexer(ref IDictionary<TIn, TOut> dictionary)
            {
                this.dictionary = dictionary ?? throw new ArgumentNullException(nameof(dictionary));
            }

            public TOut this[TIn key] => dictionary[key];
        }
    }
}
