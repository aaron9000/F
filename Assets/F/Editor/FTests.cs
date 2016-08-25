using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace UnityTest
{
    [TestFixture]
    [Category("F Tests")]
    internal class FTests
    {
        #region Test Classes

        private class ObjWithFields
        {
            public int a;
            public int b;

            public ObjWithFields()
            {
                a = 1;
                b = 2;
            }
        }

        private class ObjWithProperties
        {
            public int a { get; set; }
            public int b { get; set; }

            public ObjWithProperties()
            {
                a = 1;
                b = 2;
            }
        }

        private class ObjWithMixed
        {
            public int a { get; set; }
            public int b;

            public ObjWithMixed()
            {
                a = 1;
                b = 2;
            }
        }

        #endregion

        #region Test Data

        // Objects
        private static ObjWithFields OBJECT_WITH_FIELDS()
        {
            return new ObjWithFields();
        }

        private static ObjWithProperties OBJECT_WITH_PROPERTIES()
        {
            return new ObjWithProperties();
        }

        private static ObjWithMixed OBJECT_WITH_MIXED()
        {
            return new ObjWithMixed();
        }

        // Uniform 1D
        private static HashSet<int> INT_SET()
        {
            return new HashSet<int> {1, 2};
        }

        private static int[] INT_ARRAY()
        {
            return new [] {1, 2};
        }

        private static List<int> INT_LIST()
        {
            return new List<int>(new int[] {1, 2});
        }

        // Mixed 1D
        private static HashSet<object> MIXED_OBJECT_SET()
        {
            return new HashSet<object> {1, "2", 2, "3"};
        }

        private static List<object> MIXED_OBJECT_LIST()
        {
            return new List<object>(new object[] {1, "2", 3, "3"});
        }

        private static object[] MIXED_OBJECT_ARRAY()
        {
            return new object[] {1, "2", 3, "3"};
        }

        // Mixed 2D
        private static object[,] MIXED_OBJECT_RECT_ARRAY()
        {
            return new object[,] {{"1", 2}, {"2", 3}};
        }

        private static object[][] MIXED_OBJECT_JAGGED_ARRAY()
        {
            return new object[][] {new object[] {"1", 2}, new object[] {"2", 3}};
        }

        private static List<List<object>> MIXED_OBJECT_NESTED_LISTS()
        {
            var x = new List<List<object>>();
            x.Add(new List<object>(new object[] {"1", 2}));
            x.Add(new List<object>(new object[] {"2", 3}));
            return x;
        }

        // Dict
        private static Dictionary<string, int> STRING_INT_DICTIONARY()
        {
            return new Dictionary<string, int> {{"a", 1}, {"b", 2}};
        }

        private static Dictionary<string, object> STRING_OBJECT_DICTIONARY()
        {
            return new Dictionary<string, object> {{"a", 1}, {"b", "2"}};
        }

        #endregion

        #region ShallowClone

        [Test]
        public void ShallowCloneListTest()
        {
            var list = INT_LIST();
            var clonedList = F.shallowCloneCollection<int, List<int>>(list);
            list.Clear();
            Assert.AreEqual(list.Count, 0);
            Assert.AreEqual(clonedList.Count, 2);
            Assert.AreEqual(clonedList[0], 1);
        }

        [Test]
        public void ShallowCloneDictionaryTest()
        {
            var dict = STRING_OBJECT_DICTIONARY();
            var clonedDict = F.shallowCloneDictionary<string, object, Dictionary<string, object>>(dict);
            dict.Clear();
            Assert.AreEqual(dict.Count, 0);
            Assert.AreEqual(clonedDict.Count, 2);
            Assert.AreEqual(clonedDict["a"], 1);
        }

        #endregion

        #region Key / Value

        [Test]
        public void GetObjectValueTest()
        {
            var obj = OBJECT_WITH_MIXED();
            var a = F.getValue<int>("a", obj);
            var b = F.getValue<int>("b", obj);
            Assert.AreEqual(a, 1);
            Assert.AreEqual(b, 2);
        }

        [Test]
        public void GetDictionaryValueTest()
        {
            var a = F.getValue<int>("a", STRING_OBJECT_DICTIONARY());
            Assert.AreEqual(a, 1);
        }

        [Test]
        public void GetDictionaryKeys()
        {
            var a = F.getKeys(STRING_OBJECT_DICTIONARY());
            Assert.AreEqual(a[0], "a");
            Assert.AreEqual(a[1], "b");
        }

        [Test]
        public void GetObjectKeys()
        {
            var a = F.getKeys(OBJECT_WITH_MIXED());
            Assert.AreEqual(a[0], "a");
            Assert.AreEqual(a[1], "b");
        }

        [Test]
        public void GetDictionaryValues()
        {
            var a = F.getValues(STRING_OBJECT_DICTIONARY());
            Assert.AreEqual(a[0], 1);
            Assert.AreEqual(a[1], "2");
        }

        [Test]
        public void GetObjectValues()
        {
            var a = F.getValues(OBJECT_WITH_MIXED());
            Assert.AreEqual(a[0], 1);
            Assert.AreEqual(a[1], 2);
        }

        [Test]
        public void SetObjectValue()
        {
            var obj = OBJECT_WITH_MIXED();
            F.setValue("a", 22, obj);
            F.setValue("b", 33, obj);
            Assert.AreEqual(obj.a, 22);
            Assert.AreEqual(obj.b, 33);
        }

        #endregion

        #region Map

        [Test]
        public void MapObjectTest()
        {
            var mappedList = F.mapObject((k, v) => String.Format("{0}{1}", k, v.ToString()),
                OBJECT_WITH_MIXED());
            Assert.AreEqual(mappedList[0], "a1");
            Assert.AreEqual(mappedList[1], "b2");
        }

        [Test]
        public void MapDictionaryTest()
        {
            var mappedList = F.mapDictionary((k, v) => String.Format("{0}{1}", k, v.ToString()),
                STRING_OBJECT_DICTIONARY());
            Assert.AreEqual(mappedList[0], "a1");
            Assert.AreEqual(mappedList[1], "b2");
        }

        [Test]
        public void MapHashSetTest()
        {
            var mappedList = F.map(x => (x * x).ToString(), INT_SET());
            Assert.AreEqual(mappedList[0], "1");
            Assert.AreEqual(mappedList[1], "4");
        }

        [Test]
        public void MapListTest()
        {
            var mappedList = F.map(x => (x * x).ToString(), INT_LIST());
            Assert.AreEqual(mappedList[0], "1");
            Assert.AreEqual(mappedList[1], "4");
        }

        [Test]
        public void MapRectArrayTest()
        {
            var mappedList = F.mapRectangularArray(x => x[0].ToString() + x[1].ToString(),
                MIXED_OBJECT_RECT_ARRAY());
            Assert.AreEqual(mappedList[0], "12");
            Assert.AreEqual(mappedList[1], "23");
        }

        [Test]
        public void MapJaggedArrayTest()
        {
            var mappedList = F.map(x => x[0].ToString() + x[1].ToString(), MIXED_OBJECT_JAGGED_ARRAY());
            Assert.AreEqual(mappedList[0], "12");
            Assert.AreEqual(mappedList[1], "23");
        }

        [Test]
        public void MapNestedArrayTest()
        {
            var mappedList = F.map(x => x[0].ToString() + x[1].ToString(), MIXED_OBJECT_NESTED_LISTS());
            Assert.AreEqual(mappedList[0], "12");
            Assert.AreEqual(mappedList[1], "23");
        }

        #endregion

        #region ToPairs

        [Test]
        public void ToPairsDictionaryTest()
        {
            var list = F.toPairs(STRING_INT_DICTIONARY());
            Assert.AreEqual(list[0, 0], "a");
            Assert.AreEqual(list[0, 1], 1);
            Assert.AreEqual(list[1, 0], "b");
            Assert.AreEqual(list[1, 1], 2);
        }

        [Test]
        public void ToPairsObjectWithFieldsTest()
        {
            var list = F.toPairs(OBJECT_WITH_FIELDS());
            Assert.AreEqual(list[0, 0], "a");
            Assert.AreEqual(list[0, 1], 1);
            Assert.AreEqual(list[1, 0], "b");
            Assert.AreEqual(list[1, 1], 2);
        }

        [Test]
        public void ToPairsObjectWithPropertiesTest()
        {
            var list = F.toPairs(OBJECT_WITH_PROPERTIES());
            Assert.AreEqual(list[0, 0], "a");
            Assert.AreEqual(list[0, 1], 1);
            Assert.AreEqual(list[1, 0], "b");
            Assert.AreEqual(list[1, 1], 2);
        }

        [Test]
        public void ToPairsObjectWithMixedTest()
        {
            var list = F.toPairs(OBJECT_WITH_MIXED());
            Assert.AreEqual(list[0, 0], "a");
            Assert.AreEqual(list[0, 1], 1);
            Assert.AreEqual(list[1, 0], "b");
            Assert.AreEqual(list[1, 1], 2);
        }

        #endregion

        #region FromPairs

        [Test]
        public void FromPairsJaggedArrayTest()
        {
            var dict = F.fromPairs(MIXED_OBJECT_JAGGED_ARRAY());
            Assert.AreEqual(dict["1"], 2);
            Assert.AreEqual(dict["2"], 3);
        }

        [Test]
        public void FromPairsRectArrayTest()
        {
            var dict = F.fromPairs(MIXED_OBJECT_RECT_ARRAY());
            Assert.AreEqual(dict["1"], 2);
            Assert.AreEqual(dict["2"], 3);
        }

        public void FromPairsArrayTest()
        {
            var dict = F.fromPairs(MIXED_OBJECT_NESTED_LISTS());
            Assert.AreEqual(dict["1"], 2);
            Assert.AreEqual(dict["2"], 3);
        }

        #endregion

        #region ShallowFlatten

        [Test]
        public void ShallowFlattenListTest()
        {
            var list = F.shallowFlatten(MIXED_OBJECT_NESTED_LISTS());
            Assert.AreEqual(list[0], "1");
            Assert.AreEqual(list[1], 2);
            Assert.AreEqual(list[2], "2");
            Assert.AreEqual(list[3], 3);
        }

        [Test]
        public void ShallowFlattenJaggedArrayTest()
        {
            var list = F.shallowFlatten(MIXED_OBJECT_JAGGED_ARRAY());
            Assert.AreEqual(list[0], "1");
            Assert.AreEqual(list[1], 2);
            Assert.AreEqual(list[2], "2");
            Assert.AreEqual(list[3], 3);
        }

        [Test]
        public void ShallowFlattenRectArrayTest()
        {
            var list = F.shallowFlatten(MIXED_OBJECT_RECT_ARRAY());
            Assert.AreEqual(list[0], "1");
            Assert.AreEqual(list[1], 2);
            Assert.AreEqual(list[2], "2");
            Assert.AreEqual(list[3], 3);
        }

        #endregion

        #region Reduce

        [Test]
        public void ReduceValueTest()
        {
            var list = new List<int>(new[] {1, 2});
            var reducedValue = F.reduce((accum, value) => { return accum + value; }, 0, list);
            Assert.AreEqual(reducedValue, 3);
        }

        [Test]
        public void ReduceCollectionTest()
        {
            var list = new List<bool>(new[] {false, true, true});
            var reducedCollection = F.reduce((accum, value) =>
            {
                accum.Add(value ? 1 : 0);
                return accum;
            }, new List<int>(), list);
            Assert.AreEqual(reducedCollection.Count, 3);
            Assert.AreEqual(reducedCollection[0], 0);
            Assert.AreEqual(reducedCollection[1], 1);
            Assert.AreEqual(reducedCollection[2], 1);
        }

        #endregion

        #region Examples

        private class ObjectA
        {
            public string first_name_old = "dave";
            public string last_name_old = "davidson";
            public string first_name = "aaron";
            public string last_name = "geisler";

            public ObjectA()
            {
            }
        }

        private class ObjectB
        {
            public int first_name_digits = 5;
            public int last_name_digits = 7;
            public string capitalized_first_name = "AARON";
            public string capitalized_last_name = "GEISLER";

            public ObjectB()
            {
            }
        }

        [Test]
        public void ExampleTest()
        {
//      var a = F.toPairs(new ObjectA());
//		  var a = F.toPairs(new ObjectA());
//      var b = F.mapRectangularArray<object, object>(pair => pair, a);
//      var c = F.filter(pair => ((string) pair[0]).EndsWith("old"));
//		  var d = F.instantiateWithDictionary();
//			var list = new List<bool> (new bool[] {false, true, true});
//			var reducedCollection = F.reduce<List<int>, bool> ((accum, value) => {
//				accum.Add(value ? 1 : 0);
//				return accum;
//			}, new List<int>(), list);
//			Assert.AreEqual (reducedCollection.Count, 3);
//			Assert.AreEqual (reducedCollection[0], 0);
//			Assert.AreEqual (reducedCollection[1], 1);
//			Assert.AreEqual (reducedCollection[2], 1);
        }

        #endregion
    }
}