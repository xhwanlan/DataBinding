using System;
namespace DataBinding
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class DataBindingPropertyAttribute : Attribute
    {
        public String PropertyName { get; set; }
        public BindingMode BindingMode { get; set; }
        public Boolean IsSource { get; set; }
        public DataBindingPropertyAttribute(String propertyName, BindingMode bindingMode, Boolean isSource = true)
        {
            PropertyName = propertyName;
            BindingMode = bindingMode;
            IsSource = isSource;
        }
    }
}