# DataBinding

## Summary

The repository can bind C# class proerptise and collections.

## Binding Property

1. The class that isn't nested extends  the interface `IDataBindingObject`.
2. The set accessor of the property that needs binding invokes the method of `IDataBindingObject`.
3. The instance of the class invokes an extension method to bind another instance.

```csharp
using DataBinding;
namespace Main
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var person1 = new Person();
            var person2 = new Person();

            person1.Binding(nameof(person1.Name), person2, nameof(person2.Name));
        }
    }

    internal class Person : IDataBindingObject
    {
        private String m_Name;
        public String Name
        {
            get => m_Name;
            set { ((IDataBindingObject)this).SetPropertyValue(ref m_Name, value, nameof(Name)); }
        }
    }
}
```

### Value Converter

Using a class that extends the interface `IDataBindingValueConverter` can bind properties of different types.

```csharp
using System;
using DataBinding;
namespace Main
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var person = new Person();
            var ui = new UI();
            var valueConverter = new ValueConverter();

            person.Binding(nameof(person.Age), ui,nameof(ui.Age),BindingMode.TwoWay,valueConverter);
        }
    }

    internal class Person : IDataBindingObject
    {
        private Int32 m_age;
        public Int32 Age
        {
            get => m_age;
            set =>((IDataBindingObject)this).SetPropertyValue(ref m_age,value, nameof(Age));
        }
    }

    internal class UI : IDataBindingObject
    {
        private String m_age;
        public String Age
        {
            get => m_age;
            set => ((IDataBindingObject)this).SetPropertyValue(ref m_age,value,nameof(Age));
        }
    }
    internal class ValueConverter : IDataBindingValueConverter
    {
	// source to target
        Object IDataBindingValueConverter.Convert(Object value)
        {
            if (value is Int32 age) {
                return $"age";
            }
            return value;
        }

	// target to source
        Object IDataBindingValueConverter.ConvertBack(Object value)
        {
            if(value is String age)
            {
                return Int32.Parse(age);
            }
            return value;
        }
    }
}

```

### Source Generator

1. Install the Microsoft.CodeAnalysis 3.8.
2. Property add `DataBindingPropertyAttribute`.

```csharp
using DataBinding;
namespace Main
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var person1 = new Person();
            var person2 = new Person();

            person1.Binding(nameof(person1.Name), person2, nameof(person2.Name));
        }
    }

    internal class Person : IDataBindingObject
    {
        [DataBindingProperty("Name")]
        private String m_Name;
    }
}
```

## Binding Collection

There are three classes that extend the interface `IDataBindingCollectionObject` and implement part of its methods. They are `ObservableList`, `ObservableDictionary`, and `ObservableSet`.

```csharp
using DataBinding;
using DataBinding.Collections;
namespace Main
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var list1 = new ObservableList<Int32>();
            var list2 = new ObservableList<Int32>();

            list1.BindingCollection(list2);
        }
    }
}
```

### Value Converter

```csharp
using System;
using DataBinding;
using DataBinding.Collections;
namespace Main
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dict = new ObservableDictionary<String, Int32>();
            var list = new ObservableList<Int32>();
            var keyValueConverter = new DictionaryListKeyValueConverter();

            list.BindingCollection(dict, keyValueConverter);
        }
    }

    internal class DictionaryListKeyValueConverter : IDataBindingCollectionKeyValueConverter
    {
	// source value to target value
        Object IDataBindingValueConverter.Convert(Object value)
        {
            if (value is Int32 val)
            {
                return $"{val}";
            }
            return value;
        }

	// target value to source value
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

	// source key to target key
        Object IDataBindingCollectionKeyValueConverter.ConvertKey(Object key)
        {
            if (key is Int32 k)
            {
                return $"{k}";
            }
            return key;
        }

	// target key to source key
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
```

### Custom collection

Custom observable collection only extends `IDataBindingCollectionObject`.

## Binding Mode

### OneWay

The target changes only when the source changes

### TwoWay

If the source and target are changed, they both notify the other.

### OneWayToSource

The source changes only when the target changes
