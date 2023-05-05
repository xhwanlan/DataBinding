namespace DataBinding
{
    /// <summary>
    /// Only Class
    /// </summary>
    public interface IDataBindingObject
    {
        bool SetPropertyValue<T>(ref T target, T value, string propertyName)
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
