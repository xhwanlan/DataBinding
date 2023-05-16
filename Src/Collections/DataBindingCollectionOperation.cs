using System;

namespace DataBinding.Collections
{
    internal ref struct DataBindingCollectionOperation<TKey, TValue>
    {
        public IDataBindingCollectionObject<TKey, TValue> Sender;
        public DataBindingCollectionOperationTypes Operaion;

        public TKey Key;
        public TValue Value;
    }

    internal enum DataBindingCollectionOperationTypes : Byte
    {
        Add,
        Remove,
        Clear,
        Assignment
    }
}