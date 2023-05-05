using NUnit.Framework;
using System;

namespace DataBinding.UnitTests
{
    internal class ValueConverterTests
    {
        [Test]
        public void StringInt32Converte()
        {
            var button = new Button();
            var data = new Data();
            var converter = new ButtonDataConverter();

            data.Binding(nameof(data.Value), button, nameof(button.Title), BindingMode.TwoWay, converter);

            data.Value = 10;
            Assert.IsTrue(data.Value == 10);
            Assert.IsTrue(String.CompareOrdinal(button.Title, "10") == 0);

            button.Title = "11";
            Assert.IsTrue(data.Value == 11);
        }

        private class Button : IDataBindingObject
        {
            private String m_title;
            public String Title
            {
                get => m_title;
                set => ((IDataBindingObject)this).SetPropertyValue(ref m_title, value, nameof(Title));
            }

        }

        private class Data : IDataBindingObject
        {
            private Int32 m_value;
            public Int32 Value
            {
                get => m_value;
                set => ((IDataBindingObject)this).SetPropertyValue(ref m_value, value, nameof(Value));
            }
        }

        private class ButtonDataConverter : IDataBindingValueConverter
        {
            public object Convert(object value)
            {
                if (value is Int32 num)
                {
                    return $"{num}";
                }
                return value;
            }

            public object ConvertBack(object value)
            {
                if (value is String str)
                {
                    return Int32.Parse(str);
                }
                return value;
            }
        }
    }
}