using System;
namespace DataBinding
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public sealed class DataBindingPropertyAttribute : Attribute
    {
        public String PropertyName { get; set; }
        public DataBindingPropertyAttribute(String propertyName)
        {
            PropertyName = propertyName;
        }
    }
}