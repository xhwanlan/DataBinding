using DataBinding.Collections;
using NUnit.Framework;
using System;

namespace DataBinding.UnitTests
{
    internal class ObservableListTests
    {
        [Test]
        public void ObservableListTest()
        {
            var list1 = new ObservableList<Person>();
            var list2 = new ObservableList<Person>(8);

            Assert.IsTrue(list1.Count == list2.Count);
            Assert.IsTrue(list1.Count == 0);

            var person1 = new Person();
            var person2 = new Person();
            person1.Binding(nameof(person1.Value), person2, nameof(person2.Value), BindingMode.TwoWay);

            list1.Add(person1);
            list2.Insert(0, person2);

            Assert.IsTrue(list1.Count == list2.Count);
            Assert.IsTrue(list1.Count == 1);

            for (var i = 1; i < 8; i++)
            {
                list1.Add(new Person());
                list2.Add(new Person());

                person1 = list1[^1];
                person2 = list2[^1];

                person1.Binding(nameof(person1.Value), person2, nameof(person2.Value), BindingMode.TwoWay);
            }

            list1.BindingCollection(list2);

            var random = new Random();
            foreach (var item in list1)
            {
                item.Value = random.Next();
            }

            for (var i = 0; i < list2.Count; i++)
            {
                Assert.IsTrue(list2[i].Value == list1[i].Value);
            }

            list1.RemoveAt(random.Next(0, list1.Count - 2));

            Assert.IsTrue(list1.Count == list2.Count);

            for (var i = 0; i < list1.Count; i++)
            {
                list1[i].ClearAllBinding();
            }

            list1.Remove(person1);
            Assert.IsTrue(list1.Count == list2.Count);

            for (var i = 0; i < list1.Count; i++)
            {
                Assert.IsTrue(list1[i].Value == list2[i].Value);
            }

            person1 = list1[0];

            list2[0] = list2[0];
            Assert.IsTrue(ReferenceEquals(list1[0], list2[0]));

            list2.Clear();
            Assert.IsTrue(list2.Count == list1.Count);
            Assert.IsTrue(list1.Count == 0);

            list2.ClearCollectionBinding();
            list1.Add(person1);

            Assert.IsTrue(list1.Count != list2.Count);
        }

        private class Person : IDataBindingObject
        {
            private Int32 m_value;
            public Int32 Value
            {
                get => m_value;
                set => ((IDataBindingObject)this).SetPropertyValue(ref m_value, value, nameof(Value));
            }

        }
    }
}