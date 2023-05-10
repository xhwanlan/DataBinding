using DataBinding.Collections;
using NUnit.Framework;
using System;
using System.Linq;

namespace DataBinding.UnitTests
{
    internal class ObservableDictionaryTests
    {
        [Test]
        public void ObservableDictionaryTest()
        {
            var dict1 = new ObservableDictionary<Int32, Int32>();
            var dict2 = new ObservableDictionary<Int32, Int32>(8);

            dict1.BindingCollection(dict2);

            var random = new Random();
            for (var i = 0; i < 8; i++)
            {
                dict1[i] = random.Next(0, 100);
            }

            Int32 dictKey = 0;
            foreach (var key in dict1.Keys.Reverse())
            {
                Assert.IsTrue(dict1[key] == dict2[key]);
                dictKey = key;
            }

            dict2.Remove(dictKey);

            Assert.IsTrue(dict1.Count == dict2.Count);

            dict2.Clear();
            Assert.IsTrue(dict1.Count == dict2.Count);
            Assert.IsTrue(dict1.Count == 0);
        }
    }
}