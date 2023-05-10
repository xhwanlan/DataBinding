using System;

namespace DataBinding.Collections
{
    /// <summary>
    /// Only Class
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public interface IDataBindingCollectionObject<TKey, TValue> : IDataBindingCollectionObject
    {
        Boolean AddItem(TKey key, TValue value)
        {
            var operaion = new DataBindingCollectionOperation<TKey, TValue>()
            {
                Sender = this,
                Operaion = DataBindingCollectionOperationTypes.Add,
                Key = key,
                Value = value
            };

            return DataBindingCenter.OnCollectionChanged(this, ref operaion);
        }

        Boolean RemoveItem(TKey key, TValue value)
        {
            var operaion = new DataBindingCollectionOperation<TKey, TValue>()
            {
                Sender = this,
                Operaion = DataBindingCollectionOperationTypes.Remove,
                Key = key,
                Value = value
            };

            return DataBindingCenter.OnCollectionChanged(this, ref operaion);
        }

        Boolean ClearItems()
        {
            var operaion = new DataBindingCollectionOperation<TKey, TValue>()
            {
                Sender = this,
                Operaion = DataBindingCollectionOperationTypes.Clear,
                Key = default,
                Value = default
            };

            return DataBindingCenter.OnCollectionChanged(this, ref operaion);
        }

        Boolean AssignmentItem(TKey key, TValue value)
        {
            var operaion = new DataBindingCollectionOperation<TKey, TValue>()
            {
                Sender = this,
                Operaion = DataBindingCollectionOperationTypes.Assignment,
                Key = key,
                Value = value
            };

            return DataBindingCenter.OnCollectionChanged(this, ref operaion);
        }
    }

    public interface IDataBindingCollectionObject : IDataBindingObject
    {
        String PropertyName { get; }
        Boolean OnAdd(Object key, Object value);
        Boolean OnRemove(Object key, Object value);
        Boolean OnSetValue(Object key, Object value);
        Boolean OnClear();
    }
}