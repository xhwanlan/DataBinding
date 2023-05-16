using DataBinding.Collections;
using NUnit.Framework;
using System;

namespace DataBinding.UnitTests
{
    internal class ObservableCollectionKeyValueConverterTests
    {
        [Test]
        public void DictionaryListKeyValueConverterTest()
        {
            var list = new ObservableList<Int32>();
            var dict = new ObservableDictionary<String, String>();
            var converter = new DictionaryListKeyValueConverter();

            list.BindingCollection(dict, converter);

            for (var i = 0; i < 8; i++)
            {
                list.Add(i);
            }

            Assert.IsTrue(list.Count == 8);
            Assert.IsTrue(list.Count == dict.Count);
            foreach (var pair in dict)
            {
                var index = Int32.Parse(pair.Key);
                Assert.IsTrue(String.CompareOrdinal($"{list[index]}", pair.Value) == 0);
            }

            dict.Clear();
            Assert.IsTrue(list.Count == 0);
            Assert.IsTrue(list.Count == dict.Count);

            for (var i = 0; i < 8; i++)
            {
                dict.Add($"{i}", $"{i}");
            }
            Assert.IsTrue(list.Count == 8);
            Assert.IsTrue(list.Count == dict.Count);
            foreach (var pair in dict)
            {
                var index = Int32.Parse(pair.Key);
                Assert.IsTrue(String.CompareOrdinal($"{list[index]}", pair.Value) == 0);
            }
        }

        private class DictionaryListKeyValueConverter : IDataBindingCollectionKeyValueConverter
        {
            Object IDataBindingValueConverter.Convert(Object value)
            {
                if (value is Int32 val)
                {
                    return $"{val}";
                }
                return value;
            }

            Object IDataBindingValueConverter.ConvertBack(Object value)
            {
                if (value is String val)
                {
                    if (Int32.TryParse(val, out Int32 item))
                    {
                        return item;
                    }
                }
                return value;
            }

            Object IDataBindingCollectionKeyValueConverter.ConvertKey(Object key)
            {
                if (key is Int32 k)
                {
                    return $"{k}";
                }
                return key;
            }

            Object IDataBindingCollectionKeyValueConverter.ConvertBackKey(Object key)
            {
                if (key is String k)
                {
                    if (Int32.TryParse(k, out Int32 index))
                    {
                        return index;
                    }
                }
                return key;
            }
        }

    }
}