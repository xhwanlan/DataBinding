using System;

namespace DataBinding
{
    public interface IDataBindingCollectionKeyValueConverter : IDataBindingValueConverter
    {
        public Object ConvertKey(Object key);

        public Object ConvertBackKey(Object key);
    }
}