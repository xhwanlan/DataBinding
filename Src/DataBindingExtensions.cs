using System;
namespace DataBinding
{
    public static class DataBindingExtensions
    {
        public static void Binding(this IDataBindingObject source, String sourcePropertyName, IDataBindingObject target, String targetProperty, BindingMode mode = BindingMode.OneWay)
        {
            DataBindingCenter.Binding(source, sourcePropertyName, target, targetProperty, mode);
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