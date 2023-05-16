using DataBinding.Collections;
using System;

namespace DataBinding
{
    public static class DataBindingExtensions
    {
        public static void Binding(this IDataBindingObject source, String sourcePropertyName, IDataBindingObject target, String targetProperty, BindingMode mode = BindingMode.OneWay, IDataBindingValueConverter converter = null)
        {
            DataBindingCenter.Binding(source, sourcePropertyName, target, targetProperty, mode, converter);
        }

        public static Boolean ClearBinding(this IDataBindingObject target, String targetPropertyName)
        {
            return DataBindingCenter.ClearBinding(target, targetPropertyName);
        }

        public static Boolean ClearAllBinding(this IDataBindingObject target)
        {
            return DataBindingCenter.ClearAllBindings(target);
        }

        public static void BindingCollection<TSourceKey, TSourceValue, TTargetKey, TTargetValue>(this IDataBindingCollectionObject<TSourceKey, TSourceValue> source, IDataBindingCollectionObject<TTargetKey, TTargetValue> target, IDataBindingCollectionKeyValueConverter keyValueConverter = null, BindingMode bindingMode = BindingMode.TwoWay)
        {
            DataBindingCenter.BindingCollection(source, target, bindingMode, keyValueConverter);
        }

        public static Boolean ClearCollectionBinding(this IDataBindingCollectionObject target)
        {
            return DataBindingCenter.ClearCollectionBinding(target);
        }
    }
}