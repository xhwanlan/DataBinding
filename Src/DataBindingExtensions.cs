using System;
namespace DataBinding
{
    public static class DataBindingExtensions
    {
        public static void Binding(this IDataBindingObject source, String sourcePropertyName, IDataBindingObject target, String targetProperty, BindingMode mode = BindingMode.OneWay, IDataBindingValueConverter converter = null)
        {
            DataBindingCenter.Binding(source, sourcePropertyName, target, targetProperty, mode, converter);
        }

        public static bool ClearBinding(this IDataBindingObject target, String targetPropertyName)
        {
            return DataBindingCenter.ClearBinding(target, targetPropertyName);
        }

        public static bool ClearAllBinding(this IDataBindingObject target)
        {
            return DataBindingCenter.ClearAllBindings(target);
        }
    }
}