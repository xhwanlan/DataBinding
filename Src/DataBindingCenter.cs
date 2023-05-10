using DataBinding.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataBinding
{
    internal sealed class DataBindingCenter
    {
        #region ctor
        private static DataBindingCenter m_instance;
        private static DataBindingCenter Instance { get => m_instance ??= new DataBindingCenter(); }
        private DataBindingCenter() { }
        #endregion

        #region Proerpty
        private DataBindingDictionary<IDataBindingObject, String, DataBindingRecord> DataBindingRecordDict { get; } = new();
        #endregion

        #region Static Methods

        public static Boolean OnPropertyChanged(IDataBindingObject sender, String propertyName, Object value)
        {
            return Instance.PropertyChnagedd(sender, propertyName, value);
        }

        public static void Binding(IDataBindingObject source, String sourcePropertyName, IDataBindingObject target, String targetPropertyName, BindingMode bindingMode = BindingMode.OneWay, IDataBindingValueConverter converter = null)
        {
            Instance.BindingObject(source, sourcePropertyName, target, targetPropertyName, bindingMode, converter);
        }

        public static Boolean ClearBinding(IDataBindingObject target, String propertyName)
        {
            return Instance.Clear(target, propertyName);
        }

        public static Boolean ClearAllBindings(IDataBindingObject target)
        {
            return Instance.ClearAll(target);
        }

        public static Boolean OnCollectionChanged<TSourceKey, TSourceValue>(IDataBindingCollectionObject<TSourceKey, TSourceValue> sender, ref DataBindingCollectionOperation<TSourceKey, TSourceValue> operation)
        {
            return Instance.CollectionChanged(sender, ref operation);
        }

        public static void BindingCollection<TSourceKey, TSourceValue, TTargetKey, TTargetValue>(IDataBindingCollectionObject<TSourceKey, TSourceValue> source, IDataBindingCollectionObject<TTargetKey, TTargetValue> target, BindingMode bindingMode = BindingMode.TwoWay, IDataBindingCollectionKeyValueConverter keyValueConverter = null)
        {
            Instance.BindingCollectionObject(source, target, bindingMode, keyValueConverter);
        }

        public static Boolean ClearCollectionBinding(IDataBindingCollectionObject target)
        {
            return Instance.ClearCollection(target, target.PropertyName);
        }
        #endregion

        #region Methods
        private Boolean PropertyChnagedd(IDataBindingObject sender, String propertyName, Object value)
        {
            if (sender is null || String.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }

            if (DataBindingRecordDict.TryGetValue(sender, propertyName, out var record))
            {
                return record.NotifyValueChanged(sender, value);
            }

            return true;
        }

        private void BindingObject(IDataBindingObject source, String sourcePropertyName, IDataBindingObject target, String targetPropertyName, BindingMode bindingMode, IDataBindingValueConverter converter)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (String.IsNullOrWhiteSpace(sourcePropertyName))
            {
                throw new ArgumentException("The property name is invalid!", nameof(sourcePropertyName));
            }
            if (String.IsNullOrWhiteSpace(targetPropertyName))
            {
                throw new ArgumentException("The property name is invalid!", nameof(targetPropertyName));
            }

            if (DataBindingRecordDict.ContainsKey(source, sourcePropertyName))
            {
                throw new ArgumentException($"The property named {sourcePropertyName} of the source is already bound!");
            }

            if (DataBindingRecordDict.ContainsKey(target, targetPropertyName))
            {
                throw new ArgumentException($"The property named {targetPropertyName} of the target is already bound!");
            }

            var record = new DataBindingRecord(source, sourcePropertyName, target, targetPropertyName, bindingMode, converter);
            DataBindingRecordDict.Add(source, sourcePropertyName, record);
            DataBindingRecordDict.Add(target, targetPropertyName, record);
        }

        private Boolean Clear(IDataBindingObject target, String propertyName)
        {
            if (target is null || String.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }

            if (DataBindingRecordDict.TryGetValue(target, propertyName, out var record))
            {
                var recordSource = record.Source;
                var recordSourcePropertyName = record.SourcePropertyInfo.Name;

                var recordTarget = record.Target;
                var recordTargetPropertyName = record.TargetPropertyInfo.Name;

                if (!DataBindingRecordDict.Remove(recordSource, recordSourcePropertyName))
                {
                    return false;
                }

                if (!DataBindingRecordDict.Remove(recordTarget, recordTargetPropertyName))
                {
                    DataBindingRecordDict.Add(recordSource, recordTargetPropertyName, record);
                    return false;
                }

                return true;
            }

            return false;
        }

        private Boolean ClearAll(IDataBindingObject target)
        {
            if (target is null)
            {
                return false;
            }

            if (DataBindingRecordDict.TryGetValue(target, out var recordDict))
            {
                var records = recordDict.Values.ToArray();
                var failed = false;

                for (int i = 0; i < records.Length; i++)
                {
                    var record = records[i];
                    if (!Clear(record.Source, record.SourcePropertyInfo.Name))
                    {
                        for (int j = i - 1; j >= 0; j--)
                        {
                            record = records[j];
                            DataBindingRecordDict.Add(record.Source, record.SourcePropertyInfo.Name, record);
                            DataBindingRecordDict.Add(record.Target, record.TargetPropertyInfo.Name, record);
                        }

                        failed = true;
                        break;
                    }
                }
                return !failed;
            }

            return false;
        }

        private Boolean CollectionChanged<TSourceKey, TSourceValue>(IDataBindingCollectionObject<TSourceKey, TSourceValue> sender, ref DataBindingCollectionOperation<TSourceKey, TSourceValue> operation)
        {
            if (sender is null || String.IsNullOrWhiteSpace(sender.PropertyName))
            {
                return false;
            }

            if (DataBindingRecordDict.TryGetValue(sender, sender.PropertyName, out var record))
            {
                return record.NotifyCollectionChanged(sender, ref operation);
            }

            return true;
        }

        private void BindingCollectionObject<TSourceKey, TSourceValue, TTargetKey, TTargetValue>(IDataBindingCollectionObject<TSourceKey, TSourceValue> source, IDataBindingCollectionObject<TTargetKey, TTargetValue> target, BindingMode bindingMode = BindingMode.TwoWay, IDataBindingCollectionKeyValueConverter keyValueConverter = null)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (String.IsNullOrWhiteSpace(source.PropertyName))
            {
                throw new ArgumentException("The property name is invalid!", nameof(source));
            }
            if (String.IsNullOrWhiteSpace(target.PropertyName))
            {
                throw new ArgumentException("The property name is invalid!", nameof(target));
            }

            if (DataBindingRecordDict.ContainsKey(source, source.PropertyName))
            {
                throw new ArgumentException($"The property named {source.PropertyName} of the source is already bound!");
            }

            if (DataBindingRecordDict.ContainsKey(target, target.PropertyName))
            {
                throw new ArgumentException($"The property named {target.PropertyName} of the target is already bound!");
            }

            var record = new DataBindingRecord(bindingMode, keyValueConverter);
            record.BindingCollection(source, target);

            DataBindingRecordDict.Add(source, source.PropertyName, record);
            DataBindingRecordDict.Add(target, target.PropertyName, record);
        }

        private Boolean ClearCollection(IDataBindingObject target, String propertyName)
        {
            if (target is null || String.IsNullOrWhiteSpace(propertyName))
            {
                return false;
            }

            if (DataBindingRecordDict.TryGetValue(target, propertyName, out var record))
            {
                if (!record.IsCollection)
                {
                    return false;
                }

                var recordSource = (IDataBindingCollectionObject)record.Source;
                var recordSourcePropertyName = recordSource.PropertyName;

                var recordTarget = (IDataBindingCollectionObject)record.Target;
                var recordTargetPropertyName = recordTarget.PropertyName;

                if (!DataBindingRecordDict.Remove(recordSource, recordSourcePropertyName))
                {
                    return false;
                }

                if (!DataBindingRecordDict.Remove(recordTarget, recordTargetPropertyName))
                {
                    DataBindingRecordDict.Add(recordSource, recordTargetPropertyName, record);
                    return false;
                }

                return true;
            }

            return false;
        }
        #endregion

        #region Nested Class
        private sealed class DataBindingDictionary<TKey1, TKey2, TValue> : Dictionary<TKey1, Dictionary<TKey2, TValue>>
        {
            public TValue this[TKey1 key1, TKey2 key2]
            {
                get
                {
                    return base[key1][key2];
                }
                set
                {
                    Add(key1, key2, value);
                }
            }

            public void Add(TKey1 key1, TKey2 key2, TValue value)
            {
                if (!base.ContainsKey(key1))
                {
                    base.Add(key1, new Dictionary<TKey2, TValue>());
                }
                base[key1].Add(key2, value);
            }

            public Boolean Remove(TKey1 key1, TKey2 key2)
            {
                if (!base.ContainsKey(key1))
                {
                    return false;
                }
                return base[key1].Remove(key2);
            }

            public Boolean ContainsKey(TKey1 key1, TKey2 key2)
            {
                if (!base.ContainsKey(key1))
                {
                    return false;
                }
                return base[key1].ContainsKey(key2);
            }

            public Boolean TryGetValue(TKey1 key1, TKey2 key2, out TValue value)
            {
                value = default(TValue);
                if (ContainsKey(key1, key2))
                {
                    value = this[key1, key2];
                    return true;
                }
                return false;
            }
        }
        #endregion
    }
}
