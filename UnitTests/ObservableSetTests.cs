using DataBinding.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DataBinding.UnitTests
{
    internal class ObservableSetTests
    {
        [Test]
        public void ObservableSetTest()
        {
            var set1 = new ObservableSet<Int32>();
            var set2 = new ObservableSet<Int32>(8);
            set1.BindingCollection(set2);

            var random = new Random();
            var set = new HashSet<Int32>();
            var list = new List<Int32>();
            for (var i = 0; i < 10; i++)
            {
                set.Add(i);
                list.Add(i);
            }
            set1.UnionWith(set);

            Assert.IsTrue(set.SetEquals(set1));
            Assert.IsTrue(set1.SetEquals(set2));

            while (list.Count > 5)
            {
                var index = random.Next(0, list.Count - 1);
                set.Remove(list[index]);
                list.RemoveAt(index);
            }
            set1.ExceptWith(set);

            Assert.IsTrue(set1.SetEquals(set2));

            while (list.Count > 2)
            {
                var index = random.Next(0, list.Count - 1);
                set.Remove(list[index]);
                list.RemoveAt(index);
            }
            set2.IntersectWith(set1);

            Assert.IsTrue(set2.SetEquals(set1));

            var value = random.Next(10, 100);
            set.Clear();
            foreach (var item in set1)
            {
                set.Add(item);
            }
            while (set.Contains(value))
            {
                value = random.Next();
            }
            set.Add(value);

            set1.SymmetricExceptWith(set);
            Assert.IsTrue(set1.SetEquals(set2));
            Assert.IsTrue(set2.Count == 1);
            foreach (var i in set2)
            {
                Assert.IsTrue(i == value);
            }

            set2.Clear();
            Assert.IsTrue(set1.SetEquals(set2));
            Assert.IsTrue(set1.Count == 0);
        }
    }
}