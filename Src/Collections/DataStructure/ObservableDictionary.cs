using System;
using System.Collections;
using System.Collections.Generic;

namespace DataBinding.Collections
{
    public class ObservableDictionary<TKey, TValue> : ICollection<KeyValuePair<TKey, TValue>>, IDictionary<TKey, TValue>, IEnumerable<KeyValuePair<TKey, TValue>>, IReadOnlyCollection<KeyValuePair<TKey, TValue>>, IReadOnlyDictionary<TKey, TValue>, ICollection, IDictionary, IEnumerable, IDataBindingCollectionObject<TKey, TValue>
    {
        private readonly Dictionary<TKey, TValue> m_dict;

        #region ctor
        public ObservableDictionary()
        {
            m_dict = new Dictionary<TKey, TValue>();
        }
        public ObservableDictionary(Int32 capacity)
        {
            m_dict = new Dictionary<TKey, TValue>(capacity);
        }
        #endregion

        #region Properties
        public TValue this[TKey key] { get => Get(key); set => Set(key, value); }
        public Int32 Count => m_dict.Count;
        public Dictionary<TKey, TValue>.KeyCollection Keys => m_dict.Keys;
        public Dictionary<TKey, TValue>.ValueCollection Values => m_dict.Values;
        #endregion

        #region Methods
        public Boolean ContainsKey(TKey key)
        {
            return m_dict.ContainsKey(key);
        }

        public Boolean ContainsValue(TValue item)
        {
            return m_dict.ContainsValue(item);
        }

        public Boolean Add(TKey key, TValue value)
        {
            m_dict.Add(key, value);
            return ((IDataBindingCollectionObject<TKey, TValue>)this).AddItem(key, value);
        }

        public Boolean Clear()
        {
            m_dict.Clear();
            return ((IDataBindingCollectionObject<TKey, TValue>)this).ClearItems();
        }

        public Boolean Remove(TKey key)
        {
            if (m_dict.Remove(key))
            {
                return ((IDataBindingCollectionObject<TKey, TValue>)this).RemoveItem(key, default);
            }
            return false;
        }

        public Boolean Remove(TKey key, out TValue value)
        {
            if (m_dict.Remove(key, out value))
            {
                return ((IDataBindingCollectionObject<TKey, TValue>)this).RemoveItem(key, default);
            }
            return false;
        }
        #endregion

        #region Internal
        private TValue Get(TKey key)
        {
            return m_dict[key];
        }

        private Boolean Set(TKey key, TValue value)
        {
            m_dict[key] = value;
            return ((IDataBindingCollectionObject<TKey, TValue>)this).AssignmentItem(key, value);
        }
        #endregion

        #region Interface
        Boolean ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly => ((ICollection<KeyValuePair<TKey, TValue>>)m_dict).IsReadOnly;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => m_dict.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => m_dict.Values;

        IEnumerable<TKey> IReadOnlyDictionary<TKey, TValue>.Keys => m_dict.Keys;

        IEnumerable<TValue> IReadOnlyDictionary<TKey, TValue>.Values => m_dict.Values;

        Boolean ICollection.IsSynchronized => ((ICollection)m_dict).IsSynchronized;

        Object ICollection.SyncRoot => this;

        Boolean IDictionary.IsFixedSize => ((IDictionary)m_dict).IsFixedSize;

        Boolean IDictionary.IsReadOnly => ((IDictionary)m_dict).IsReadOnly;

        ICollection IDictionary.Keys => ((IDictionary)m_dict).Keys;

        ICollection IDictionary.Values => ((IDictionary)m_dict).Values;

        Object IDictionary.this[Object key] { get => this[(TKey)key]; set => this[(TKey)key] = (TValue)value; }

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.Clear()
        {
            this.Clear();
        }

        Boolean ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
        {
            return ((ICollection<KeyValuePair<TKey, TValue>>)m_dict).Contains(item);
        }

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, Int32 arrayIndex)
        {
            ((ICollection<KeyValuePair<TKey, TValue>>)m_dict).CopyTo(array, arrayIndex);
        }

        Boolean ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
        {
            return this.Remove(item.Key);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return m_dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_dict.GetEnumerator();
        }

        void IDictionary<TKey, TValue>.Add(TKey key, TValue value)
        {
            this.Add(key, value);
        }

        Boolean IDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return m_dict.TryGetValue(key, out value);
        }

        Boolean IReadOnlyDictionary<TKey, TValue>.TryGetValue(TKey key, out TValue value)
        {
            return m_dict.TryGetValue(key, out value);
        }

        void ICollection.CopyTo(Array array, Int32 index)
        {
            ((ICollection)m_dict).CopyTo(array, index);
        }

        void IDictionary.Add(Object key, Object value)
        {
            this.Add((TKey)key, (TValue)value);
        }

        void IDictionary.Clear()
        {
            this.Clear();
        }

        Boolean IDictionary.Contains(Object key)
        {
            return this.ContainsKey((TKey)key);
        }

        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            return m_dict.GetEnumerator();
        }

        void IDictionary.Remove(Object key)
        {
            this.Remove((TKey)key);
        }

        #endregion

        #region Data Binding Collection Interface
        String IDataBindingCollectionObject.PropertyName => nameof(ObservableDictionary<TKey, TValue>);

        Boolean IDataBindingCollectionObject.OnAdd(Object key, Object value)
        {
            if (key is TKey k && value is TValue v)
            {
                return Add(k, v);
            }
            return false;
        }

        Boolean IDataBindingCollectionObject.OnRemove(Object key, Object value)
        {
            if (key is TKey k)
            {
                return Remove(k);
            }
            return false;
        }

        Boolean IDataBindingCollectionObject.OnSetValue(Object key, Object value)
        {
            if (key is TKey k && value is TValue v)
            {
                return Set(k, v);
            }
            return false;
        }

        Boolean IDataBindingCollectionObject.OnClear()
        {
            return Clear();
        }
        #endregion
    }
}