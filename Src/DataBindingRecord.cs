using System;
using System.Reflection;

namespace DataBinding
{
    internal sealed class DataBindingRecord
    {
        private bool m_twoWayChanging;
        private BindingMode m_bindingMode;
        private IDataBindingValueConverter m_converter;

        public IDataBindingObject Source { get; private set; }
        public PropertyInfo SourcePropertyInfo { get; private set; }

        public IDataBindingObject Target { get; private set; }
        public PropertyInfo TargetPropertyInfo { get; private set; }

        private DataBindingRecord(BindingMode bindingMode, IDataBindingValueConverter converter)
        {
            m_bindingMode = bindingMode;
            m_converter = converter;
        }

        public DataBindingRecord(IDataBindingObject source, String sourcePropertyName, IDataBindingObject target, String targetPropertyName, BindingMode mode, IDataBindingValueConverter valueConverter) : this(mode, valueConverter)
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

            if ((mode is BindingMode.TwoWay or BindingMode.OneWayToSource) && !SourcePropertyInfo.CanWrite)
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

            if ((mode is BindingMode.TwoWay or BindingMode.OneWay) && !TargetPropertyInfo.CanWrite)
            {
                throw new ArgumentException($"The property named {targetPropertyName} can't set value!", nameof(targetPropertyName));
            }
        }

        public bool NotifyValueChanged(IDataBindingObject sender, Object value)
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
                    return true; ;
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
    }
}
