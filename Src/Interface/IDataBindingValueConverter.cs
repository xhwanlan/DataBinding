using System;

namespace DataBinding
{
    public interface IDataBindingValueConverter
    {
        public Object Convert(Object value);

        public Object ConvertBack(Object value);
    }
}