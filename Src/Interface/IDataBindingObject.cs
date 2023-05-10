using System;

namespace DataBinding
{
    /// <summary>
    /// Only Class
    /// </summary>
    public interface IDataBindingObject
    {
        Boolean SetPropertyValue<T>(ref T target, T value, string propertyName)
        {
            if (DataBindingCenter.OnPropertyChanged(this, propertyName, value))
            {
                target = value;
                return true;
            }
            return false;
        }
    }
}
