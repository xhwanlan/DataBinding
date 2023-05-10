using DataBinding.Collections;
using System;
using System.Reflection;

namespace DataBinding
{
    internal sealed class DataBindingRecord
    {
        private Boolean m_twoWayChanging;
        private BindingMode m_bindingMode;
        private IDataBindingValueConverter m_converter;

        public IDataBindingObject Source { get; private set; }
        public PropertyInfo SourcePropertyInfo { get; private set; }

        public IDataBindingObject Target { get; private set; }
        public PropertyInfo TargetPropertyInfo { get; private set; }
        public Boolean IsCollection { get; private set; }

        private DataBindingRecord(Boolean isCollection, BindingMode bindingMode, IDataBindingValueConverter converter)
        {
            IsCollection = isCollection;
            m_bindingMode = bindingMode;
            m_converter = converter;
        }

        public DataBindingRecord(IDataBindingObject source, String sourcePropertyName, IDataBindingObject target, String targetPropertyName, BindingMode bindingMode, IDataBindingValueConverter valueConverter) : this(false, bindingMode, valueConverter)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (String.IsNullOrWhiteSpace(sourcePropertyName))
            {
                throw new ArgumentException("The property name is invalid!", nameof(sourcePropertyName));
            }

            var propertyInfo = source.GetType().GetProperty(sourcePropertyName);

            if (propertyInfo is null)
            {
                throw new ArgumentException($"There is no property named {sourcePropertyName}", nameof(sourcePropertyName));
            }

            Source = source;
            SourcePropertyInfo = propertyInfo;

            if ((bindingMode is BindingMode.TwoWay or BindingMode.OneWayToSource) && !SourcePropertyInfo.CanWrite)
            {
                throw new ArgumentException($"The property named {sourcePropertyName} can't set value!", nameof(sourcePropertyName));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (String.IsNullOrWhiteSpace(targetPropertyName))
            {
                throw new ArgumentException("The property name is invalid!", nameof(targetPropertyName));
            }

            propertyInfo = target.GetType().GetProperty(targetPropertyName);

            if (propertyInfo is null)
            {
                throw new ArgumentException($"There is no property named {targetPropertyName}", nameof(targetPropertyName));
            }

            Target = target;
            TargetPropertyInfo = propertyInfo;

            if ((bindingMode is BindingMode.TwoWay or BindingMode.OneWay) && !TargetPropertyInfo.CanWrite)
            {
                throw new ArgumentException($"The property named {targetPropertyName} can't set value!", nameof(targetPropertyName));
            }
        }

        public DataBindingRecord(BindingMode bindingMode, IDataBindingCollectionKeyValueConverter keyValueConverter = null) : this(true, bindingMode, keyValueConverter)
        {
            SourcePropertyInfo = null;
            TargetPropertyInfo = null;
        }

        public void BindingCollection<TSourceKey, TSourceValue, TTargetKey, TTargetValue>(IDataBindingCollectionObject<TSourceKey, TSourceValue> source, IDataBindingCollectionObject<TTargetKey, TTargetValue> target)
        {
            if (!IsCollection)
            {
                throw new ArgumentException("This record is not a record of a collection!");
            }

            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Source = source;
            Target = target;
        }

        public Boolean NotifyValueChanged(IDataBindingObject sender, Object value)
        {
            switch (m_bindingMode)
            {
                case BindingMode.OneWay:
                    if (ReferenceEquals(sender, Source))
                    {
                        TargetPropertyInfo.SetValue(Target, m_converter?.Convert(value) ?? value);
                        return true;
                    }
                    return false;
                case BindingMode.TwoWay:
                    if (m_twoWayChanging)
                    {
                        return true;
                    }
                    m_twoWayChanging = true;
                    if (ReferenceEquals(sender, Source))
                    {
                        TargetPropertyInfo.SetValue(Target, m_converter?.Convert(value) ?? value);
                    }
                    else if (ReferenceEquals(sender, Target))
                    {
                        SourcePropertyInfo.SetValue(Source, m_converter?.ConvertBack(value) ?? value);
                    }
                    else
                    {
                        m_twoWayChanging = false;
                        return false;
                    }
                    m_twoWayChanging = false;
                    return true;
                case BindingMode.OneWayToSource:
                    if (ReferenceEquals(sender, Target))
                    {
                        SourcePropertyInfo.SetValue(Source, m_converter?.ConvertBack(value) ?? value);
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        public Boolean NotifyCollectionChanged<TSourceKey, TSourceValue>(IDataBindingCollectionObject<TSourceKey, TSourceValue> sender, ref DataBindingCollectionOperation<TSourceKey, TSourceValue> operation)
        {
            if (!IsCollection)
            {
                return false;
            }

            Func<Object, Object> CastKeyFun = CastDefault;
            Func<Object, Object> CastValueFun = CastDefault;

            switch (m_bindingMode)
            {
                case BindingMode.OneWay:
                    if (ReferenceEquals(sender, Source))
                    {
                        GetConverter(true);
                        return NotifyTarget((IDataBindingCollectionObject)Target, ref operation, CastKeyFun, CastValueFun);
                    }
                    return false;
                case BindingMode.TwoWay:
                    {
                        if (m_twoWayChanging)
                        {
                            return true;
                        }
                        var result = false;
                        m_twoWayChanging = true;
                        if (ReferenceEquals(sender, Source))
                        {
                            GetConverter(true);
                            result = NotifyTarget((IDataBindingCollectionObject)Target, ref operation, CastKeyFun, CastValueFun);
                        }
                        else if (ReferenceEquals(sender, Target))
                        {
                            GetConverter(false);
                            result = NotifyTarget((IDataBindingCollectionObject)Source, ref operation, CastKeyFun, CastValueFun);
                        }
                        else
                        {
                            m_twoWayChanging = false;
                            return false;
                        }
                        m_twoWayChanging = false;
                        return result;
                    }
                case BindingMode.OneWayToSource:
                    if (ReferenceEquals(sender, Target))
                    {
                        GetConverter(false);
                        return NotifyTarget((IDataBindingCollectionObject)Source, ref operation, CastKeyFun, CastValueFun);
                    }
                    return false;
                default:
                    return false;
            }

            void GetConverter(bool senderIsSource)
            {
                if (m_converter is IDataBindingCollectionKeyValueConverter converter)
                {
                    if (senderIsSource)
                    {
                        CastKeyFun = converter.ConvertKey;
                        CastValueFun = converter.Convert;
                    }
                    else
                    {
                        CastKeyFun = converter.ConvertBackKey;
                        CastValueFun = converter.ConvertBack;
                    }
                }
            }

            static Boolean NotifyTarget(IDataBindingCollectionObject target, ref DataBindingCollectionOperation<TSourceKey, TSourceValue> operation, Func<Object, Object> castKey, Func<Object, Object> castValue)
            {
                switch (operation.Operaion)
                {
                    case DataBindingCollectionOperationTypes.Add:
                        return target?.OnAdd(castKey(operation.Key), castValue(operation.Value)) ?? false;
                    case DataBindingCollectionOperationTypes.Remove:
                        return target?.OnRemove(castKey(operation.Key), castValue(operation.Value)) ?? false;
                    case DataBindingCollectionOperationTypes.Clear:
                        return target?.OnClear() ?? false;
                    case DataBindingCollectionOperationTypes.Assignment:
                        return target?.OnSetValue(castKey(operation.Key), castValue(operation.Value)) ?? false;
                    default:
                        return false;
                }
            }

            static Object CastDefault(Object item)
            {
                return item;
            }
        }

    }
}
