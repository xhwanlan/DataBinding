using System;
using System.Collections;
using System.Collections.Generic;

namespace DataBinding.Collections
{
    public class ObservableList<T> : ICollection<T>, IEnumerable<T>, IList<T>, IReadOnlyCollection<T>, IReadOnlyList<T>, ICollection, IEnumerable, IList, IDataBindingCollectionObject<Int32, T>
    {
        private readonly List<T> m_list;

        #region ctor
        public ObservableList()
        {
            m_list = new List<T>();
        }
        public ObservableList(Int32 capacity)
        {
            m_list = new List<T>(capacity);
        }
        #endregion

        #region Properties
        public T this[Int32 index] { get => Get(index); set => Set(index, value); }
        public Int32 Count => m_list.Count;

        #endregion

        #region Methods
        public Boolean Contains(T item)
        {
            return m_list.Contains(item);
        }

        public Int32 IndexOf(T item)
        {
            return m_list.IndexOf(item);
        }

        public Boolean Insert(Int32 index, T item)
        {
            m_list.Insert(index, item);
            return ((IDataBindingCollectionObject<Int32, T>)this).AddItem(index, item);
        }

        public Boolean Add(T item)
        {
            return Insert(Count, item);
        }

        public Boolean Clear()
        {
            m_list.Clear();
            return ((IDataBindingCollectionObject<Int32, T>)this).ClearItems();
        }

        public Boolean Remove(T item)
        {
            var index = IndexOf(item);
            return RemoveAt(index);
        }

        public Boolean RemoveAt(Int32 index)
        {
            m_list.RemoveAt(index);
            return ((IDataBindingCollectionObject<Int32, T>)this).RemoveItem(index, default);
        }
        #endregion

        #region Internal
        private T Get(Int32 index)
        {
            return m_list[index];
        }

        private Boolean Set(Int32 index, T value)
        {
            m_list[index] = value;
            return ((IDataBindingCollectionObject<Int32, T>)this).AssignmentItem(index, value);
        }
        #endregion

        #region Interface
        T IList<T>.this[Int32 index] { get => this[index]; set => this[index] = value; }

        T IReadOnlyList<T>.this[Int32 index] => this[index];

        Object IList.this[Int32 index] { get => this[index]; set => this[index] = (T)value; }

        Int32 ICollection<T>.Count => Count;

        Int32 IReadOnlyCollection<T>.Count => Count;

        Int32 ICollection.Count => Count;

        Boolean ICollection<T>.IsReadOnly => ((ICollection<T>)m_list).IsReadOnly;

        Boolean IList.IsReadOnly => ((IList)m_list).IsReadOnly;

        Boolean ICollection.IsSynchronized => ((ICollection)m_list).IsSynchronized;

        Object ICollection.SyncRoot => this;

        Boolean IList.IsFixedSize => ((IList)m_list).IsFixedSize;

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        Int32 IList.Add(Object value)
        {
            this.Add((T)value);
            return this.Count - 1;
        }

        void ICollection<T>.Clear()
        {
            this.Clear();
        }

        void IList.Clear()
        {
            this.Clear();
        }

        Boolean ICollection<T>.Contains(T item)
        {
            return this.Contains(item);
        }

        Boolean IList.Contains(Object value)
        {
            return this.Contains((T)value);
        }

        void ICollection<T>.CopyTo(T[] array, Int32 arrayIndex)
        {
            ((ICollection<T>)m_list).CopyTo(array, arrayIndex);
        }

        void ICollection.CopyTo(Array array, Int32 index)
        {
            ((ICollection)m_list).CopyTo(array, index);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return ((IEnumerable<T>)m_list).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)m_list).GetEnumerator();
        }

        Int32 IList<T>.IndexOf(T item)
        {
            return this.IndexOf(item);
        }

        Int32 IList.IndexOf(Object value)
        {
            return this.IndexOf((T)value);
        }

        void IList<T>.Insert(Int32 index, T item)
        {
            this.Insert(index, item);
        }

        void IList.Insert(Int32 index, Object value)
        {
            this.Insert(index, (T)value);
        }

        Boolean ICollection<T>.Remove(T item)
        {
            return this.Remove(item);
        }

        void IList.Remove(Object value)
        {
            this.Remove((T)value);
        }

        void IList<T>.RemoveAt(Int32 index)
        {
            this.RemoveAt(index);
        }

        void IList.RemoveAt(Int32 index)
        {
            this.RemoveAt(index);
        }
        #endregion

        #region Data Binding Collection Interface
        String IDataBindingCollectionObject.PropertyName => nameof(ObservableList<T>);

        Boolean IDataBindingCollectionObject.OnAdd(Object key, Object value)
        {
            if (key is Int32 index && value is T item)
            {
                return Insert(index, item);
            }
            return false;
        }

        Boolean IDataBindingCollectionObject.OnRemove(Object key, Object value)
        {
            if (key is Int32 index)
            {
                return RemoveAt(index);
            }
            return false;
        }

        Boolean IDataBindingCollectionObject.OnSetValue(Object key, Object value)
        {
            if (key is Int32 index && value is T item)
            {
                return Set(index, item);
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