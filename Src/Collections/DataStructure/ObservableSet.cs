using System;
using System.Collections;
using System.Collections.Generic;

namespace DataBinding.Collections
{
    public class ObservableSet<T> : ICollection<T>, IEnumerable<T>, IReadOnlyCollection<T>, ISet<T>, IEnumerable, IDataBindingCollectionObject<T, T>
    {
        private readonly HashSet<T> m_set;

        #region ctor
        public ObservableSet()
        {
            m_set = new HashSet<T>();
        }
        public ObservableSet(Int32 capacity)
        {
            m_set = new HashSet<T>(capacity);
        }
        #endregion

        #region Properties
        public Int32 Count => m_set.Count;
        #endregion

        #region Methods
        public Boolean Contains(T item)
        {
            return m_set.Contains(item);
        }

        public Boolean Add(T item)
        {
            m_set.Add(item);
            return ((IDataBindingCollectionObject<T, T>)this).AddItem(item, default);
        }

        public Boolean Clear()
        {
            m_set.Clear();
            return ((IDataBindingCollectionObject<T, T>)this).ClearItems();
        }

        public Boolean Remove(T item)
        {
            if (m_set.Remove(item))
            {
                return ((IDataBindingCollectionObject<T, T>)this).RemoveItem(item, default);
            }
            return false;
        }

        public void ExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                this.Remove(item);
            }
        }

        public void IntersectWith(IEnumerable<T> other)
        {
            var set = new HashSet<T>(m_set);
            set.IntersectWith(other);

            this.Clear();
            foreach (var item in set)
            {
                this.Add(item);
            }
        }

        public void SymmetricExceptWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                if (this.Contains(item))
                {
                    this.Remove(item);
                }
                else
                {
                    this.Add(item);
                }
            }
        }

        public void UnionWith(IEnumerable<T> other)
        {
            foreach (var item in other)
            {
                this.Add(item);
            }
        }

        public Boolean SetEquals(IEnumerable<T> other)
        {
            return m_set.SetEquals(other);
        }
        #endregion

        #region Interface
        Boolean ICollection<T>.IsReadOnly => ((ICollection<T>)this).IsReadOnly;

        void ICollection<T>.Add(T item)
        {
            this.Add(item);
        }

        void ICollection<T>.Clear()
        {
            this.Clear();
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            ((ICollection<T>)m_set).CopyTo(array, arrayIndex);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return m_set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_set.GetEnumerator();
        }

        void ISet<T>.ExceptWith(IEnumerable<T> other)
        {
            this.ExceptWith(other);
        }

        void ISet<T>.IntersectWith(IEnumerable<T> other)
        {
            this.IntersectWith(other);
        }

        Boolean ISet<T>.IsProperSubsetOf(IEnumerable<T> other)
        {
            return m_set.IsProperSubsetOf(other);
        }

        Boolean ISet<T>.IsProperSupersetOf(IEnumerable<T> other)
        {
            return m_set.IsProperSupersetOf(other);
        }

        Boolean ISet<T>.IsSubsetOf(IEnumerable<T> other)
        {
            return m_set.IsSubsetOf(other);
        }

        Boolean ISet<T>.IsSupersetOf(IEnumerable<T> other)
        {
            return m_set.IsSupersetOf(other);
        }

        Boolean ISet<T>.Overlaps(IEnumerable<T> other)
        {
            return m_set.Overlaps(other);
        }

        Boolean ISet<T>.SetEquals(IEnumerable<T> other)
        {
            return this.SetEquals(other);
        }

        void ISet<T>.SymmetricExceptWith(IEnumerable<T> other)
        {
            this.SymmetricExceptWith(other);
        }

        void ISet<T>.UnionWith(IEnumerable<T> other)
        {
            this.UnionWith(other);
        }
        #endregion

        #region Data Binding Collection Interface
        String IDataBindingCollectionObject.PropertyName => nameof(ObservableSet<T>);


        Boolean IDataBindingCollectionObject.OnAdd(Object key, Object value)
        {
            if (key is T item)
            {
                return Add(item);
            }
            return false;
        }

        Boolean IDataBindingCollectionObject.OnRemove(Object key, Object value)
        {
            if (key is T item)
            {
                return Remove(item);
            }
            return false;
        }

        Boolean IDataBindingCollectionObject.OnSetValue(Object key, Object value)
        {
            return false;
        }

        Boolean IDataBindingCollectionObject.OnClear()
        {
            return Clear();
        }
        #endregion
    }
}