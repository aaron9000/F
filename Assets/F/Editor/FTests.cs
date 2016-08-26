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
            return new [] {new object[] {"1", 2}, new object[] {"2", 3}};
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
            var clonedList = F.ShallowCloneCollection<int, List<int>>(list);
            list.Clear();
            Assert.AreEqual(list.Count, 0);
            Assert.AreEqual(clonedList.Count, 2);
            Assert.AreEqual(clonedList[0], 1);
        }

        [Test]
        public void ShallowCloneDictionaryTest()
        {
            var dict = STRING_OBJECT_DICTIONARY();
            var clonedDict = F.ShallowCloneDictionary<string, object, Dictionary<string, object>>(dict);
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
            var a = F.GetValue<int>("a", obj);
            var b = F.GetValue<int>("b", obj);
            Assert.AreEqual(a, 1);
            Assert.AreEqual(b, 2);
        }

        [Test]
        public void GetDictionaryValueTest()
        {
            var a = F.GetValue<int>("a", STRING_OBJECT_DICTIONARY());
            Assert.AreEqual(a, 1);
        }

        [Test]
        public void GetDictionaryKeysTest()
        {
            var a = F.GetKeys(STRING_OBJECT_DICTIONARY());
            Assert.AreEqual(a[0], "a");
            Assert.AreEqual(a[1], "b");
        }

        [Test]
        public void GetObjectKeysTest()
        {
            var a = F.GetKeys(OBJECT_WITH_MIXED());
            Assert.AreEqual(a[0], "a");
            Assert.AreEqual(a[1], "b");
        }

        [Test]
        public void GetDictionaryValuesTest()
        {
            var a = F.GetValues(STRING_OBJECT_DICTIONARY());
            Assert.AreEqual(a[0], 1);
            Assert.AreEqual(a[1], "2");
        }

        [Test]
        public void GetObjectValuesTest()
        {
            var a = F.GetValues(OBJECT_WITH_MIXED());
            Assert.AreEqual(a[0], 1);
            Assert.AreEqual(a[1], 2);
        }

        [Test]
        public void SetObjectValueTest()
        {
            var obj = OBJECT_WITH_MIXED();
            F.SetValue("a", 22, obj);
            F.SetValue("b", 33, obj);
            Assert.AreEqual(obj.a, 22);
            Assert.AreEqual(obj.b, 33);
        }

        #endregion

        #region Map

        [Test]
        public void MapObjectTest()
        {
            var mappedList = F.MapObject((k, v) => String.Format("{0}{1}", k, v.ToString()),
                OBJECT_WITH_MIXED());
            Assert.AreEqual(mappedList[0], "a1");
            Assert.AreEqual(mappedList[1], "b2");
        }

        [Test]
        public void MapDictionaryTest()
        {
            var mappedList = F.MapDictionary((k, v) => String.Format("{0}{1}", k, v.ToString()),
                STRING_OBJECT_DICTIONARY());
            Assert.AreEqual(mappedList[0], "a1");
            Assert.AreEqual(mappedList[1], "b2");
        }

        [Test]
        public void MapHashSetTest()
        {
            var mappedList = F.Map(x => (x * x).ToString(), INT_SET());
            Assert.AreEqual(mappedList[0], "1");
            Assert.AreEqual(mappedList[1], "4");
        }

        [Test]
        public void MapListTest()
        {
            var mappedList = F.Map(x => (x * x).ToString(), INT_LIST());
            Assert.AreEqual(mappedList[0], "1");
            Assert.AreEqual(mappedList[1], "4");
        }

        [Test]
        public void MapRectArrayTest()
        {
            var mappedList = F.MapRectangularArray(x => x[0].ToString() + x[1].ToString(),
                MIXED_OBJECT_RECT_ARRAY());
            Assert.AreEqual(mappedList[0], "12");
            Assert.AreEqual(mappedList[1], "23");
        }

        [Test]
        public void MapJaggedArrayTest()
        {
            var mappedList = F.Map(x => x[0].ToString() + x[1].ToString(), MIXED_OBJECT_JAGGED_ARRAY());
            Assert.AreEqual(mappedList[0], "12");
            Assert.AreEqual(mappedList[1], "23");
        }

        [Test]
        public void MapNestedArrayTest()
        {
            var mappedList = F.Map(x => x[0].ToString() + x[1].ToString(), MIXED_OBJECT_NESTED_LISTS());
            Assert.AreEqual(mappedList[0], "12");
            Assert.AreEqual(mappedList[1], "23");
        }

        #endregion

        #region ToPairs

        [Test]
        public void ToPairsDictionaryTest()
        {
            var list = F.ToPairs(STRING_INT_DICTIONARY());
            Assert.AreEqual(list[0, 0], "a");
            Assert.AreEqual(list[0, 1], 1);
            Assert.AreEqual(list[1, 0], "b");
            Assert.AreEqual(list[1, 1], 2);
        }

        [Test]
        public void ToPairsObjectWithFieldsTest()
        {
            var list = F.ToPairs(OBJECT_WITH_FIELDS());
            Assert.AreEqual(list[0, 0], "a");
            Assert.AreEqual(list[0, 1], 1);
            Assert.AreEqual(list[1, 0], "b");
            Assert.AreEqual(list[1, 1], 2);
        }

        [Test]
        public void ToPairsObjectWithPropertiesTest()
        {
            var list = F.ToPairs(OBJECT_WITH_PROPERTIES());
            Assert.AreEqual(list[0, 0], "a");
            Assert.AreEqual(list[0, 1], 1);
            Assert.AreEqual(list[1, 0], "b");
            Assert.AreEqual(list[1, 1], 2);
        }

        [Test]
        public void ToPairsObjectWithMixedTest()
        {
            var list = F.ToPairs(OBJECT_WITH_MIXED());
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
            var dict = F.FromPairs(MIXED_OBJECT_JAGGED_ARRAY());
            Assert.AreEqual(dict["1"], 2);
            Assert.AreEqual(dict["2"], 3);
        }

        [Test]
        public void FromPairsRectArrayTest()
        {
            var dict = F.FromPairs(MIXED_OBJECT_RECT_ARRAY());
            Assert.AreEqual(dict["1"], 2);
            Assert.AreEqual(dict["2"], 3);
        }

        public void FromPairsArrayTest()
        {
            var dict = F.FromPairs(MIXED_OBJECT_NESTED_LISTS());
            Assert.AreEqual(dict["1"], 2);
            Assert.AreEqual(dict["2"], 3);
        }

        #endregion

        #region ShallowFlatten

        [Test]
        public void ShallowFlattenListTest()
        {
            var list = F.ShallowFlatten(MIXED_OBJECT_NESTED_LISTS());
            Assert.AreEqual(list[0], "1");
            Assert.AreEqual(list[1], 2);
            Assert.AreEqual(list[2], "2");
            Assert.AreEqual(list[3], 3);
        }

        [Test]
        public void ShallowFlattenJaggedArrayTest()
        {
            var list = F.ShallowFlatten(MIXED_OBJECT_JAGGED_ARRAY());
            Assert.AreEqual(list[0], "1");
            Assert.AreEqual(list[1], 2);
            Assert.AreEqual(list[2], "2");
            Assert.AreEqual(list[3], 3);
        }

        [Test]
        public void ShallowFlattenRectArrayTest()
        {
            var list = F.ShallowFlatten(MIXED_OBJECT_RECT_ARRAY());
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
            var reducedValue = F.Reduce((accum, value) => accum + value, 0, list);
            Assert.AreEqual(reducedValue, 3);
        }

        [Test]
        public void ReduceCollectionTest()
        {
            var list = new List<bool>(new[] {false, true, true});
            var reducedCollection = F.Reduce((accum, value) =>
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
//            var objA = new ObjectA();
//            var a = F.ShallowDictionaryFromObject(objA);

//      var a = F.ToPairs(new ObjectA());
//		  var a = F.ToPairs(new ObjectA());
//      var b = F.MapRectangularArray<object, object>(pair => pair, a);
//      var c = F.filter(pair => ((string) pair[0]).EndsWith("old"));
//		  var d = F.instantiateWithDictionary();
//			var list = new List<bool> (new bool[] {false, true, true});
//			var reducedCollection = F.Reduce<List<int>, bool> ((accum, value) => {
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