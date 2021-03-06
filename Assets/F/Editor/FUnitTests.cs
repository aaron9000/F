﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace UnityTest
{
    [TestFixture]
    [Category("F Unit Tests")]
    internal class FUnitTests
    {
        #region Test Classes & Data

        private class ObjectA
        {
            public string FirstName;
            public string MiddleName;
            public string LastName;

            public ObjectA()
            {

            }

            public ObjectA(string first, string middle, string last)
            {
                FirstName = first;
                MiddleName = middle;
                LastName = last;
            }
        }

        private class ObjectB
        {
            public string FirstName;

            public ObjectB(string first)
            {
                FirstName = first;
            }
        }

        private class ObjectC
        {
            public int A { get; set; }
            public int B;

            public ObjectC()
            {
                A = 1;
                B = 2;
            }
        }

        private static ObjectC GetObject()
        {
            return new ObjectC();
        }

        private static HashSet<int> GetIntSet()
        {
            return new HashSet<int> {1, 2};
        }

        private static List<int> GetIntList()
        {
            return new List<int>(new[] {1, 2});
        }

        private static List<int> GetLongIntList()
        {
            return new List<int>(new[] {1, 2, 3, 4, 5, 6, 7, 9, 10});
        }

        private static object[,] GetMixedObjectRectArray()
        {
            return new object[,] {{"1", 2}, {"2", 3}};
        }

        private static object[][] GetMixedObjectJaggedArray()
        {
            return new[] {new object[] {"1", 2}, new object[] {"2", 3}};
        }

        private static List<List<object>> GetMixedObjectNestedLists()
        {
            var x = new List<List<object>>();
            x.Add(new List<object>(new object[] {"1", 2}));
            x.Add(new List<object>(new object[] {"2", 3}));
            return x;
        }

        private static Dictionary<string, int> GetStringIntDictionary()
        {
            return new Dictionary<string, int> {{"A", 1}, {"B", 2}};
        }

        private static Dictionary<string, object> GetStringObjectDictionary()
        {
            return new Dictionary<string, object> {{"A", 1}, {"B", "2"}};
        }

        private static Dictionary<string, object> GetUniformStringObjectDictionary()
        {
            return new Dictionary<string, object> {{"A", 1}, {"B", 2}};
        }

        #endregion

        #region Misc

        [Test]
        public void RangeTest()
        {
            var r = F.Range(4, 6);
            Assert.AreEqual(r.Length, 2);
            Assert.AreEqual(r[0], 4);
            Assert.AreEqual(r[1], 5);
        }

        [Test]
        public void TupleTest()
        {
            var t = new F.Tuple<int, string>(5, "five");
            Assert.AreEqual(t.First, 5);
            Assert.AreEqual(t.Second, "five");
        }

        #endregion

        #region Cloning and Type Conversions

        [Test]
        public void CoerceDictionaryTest()
        {
            var dict = GetUniformStringObjectDictionary();
            var coercedDict = F.CoerceDictionary<int>(dict);
            Assert.AreEqual(coercedDict["A"], 1);
            Assert.AreEqual(coercedDict["B"], 2);
        }

        [Test]
        public void ObjectFromDictionaryTest()
        {
            var dict = GetUniformStringObjectDictionary();
            dict["A"] = 33;
            var obj = F.ShallowObjectFromDictionary<ObjectC>(dict);
            Assert.AreEqual(obj.A, 33);
            Assert.AreEqual(obj.B, 2);
        }

        [Test]
        public void ObjectFromDictionaryMismatchedTypesTest()
        {
            var dict = GetStringObjectDictionary();
            dict["A"] = 33;
            var obj = F.ShallowObjectFromDictionary<ObjectC>(dict);
            Assert.AreEqual(obj.A, 33);
            Assert.AreEqual(obj.B, 2);
        }

        [Test]
        public void DictionaryFromObjectTest()
        {
            var obj = GetObject();
            var dict = F.ShallowDictionaryFromObject(obj);
            Assert.AreEqual(dict["A"], 1);
            Assert.AreEqual(dict["B"], 2);
        }

        [Test]
        public void ShallowCloneObjectTest()
        {
            var obj = GetObject();
            obj.B = 33;
            var clonedObj = F.ShallowCloneObject(obj);
            obj.A = 100;
            obj.B = 10;
            Assert.AreEqual(clonedObj.B, 33);
            Assert.AreEqual(obj.A, 100);
        }

        [Test]
        public void ShallowCloneCollectionTest()
        {
            var list = GetIntList();
            var clonedList = F.ShallowCloneCollection<int, List<int>>(list);
            list.Clear();
            Assert.AreEqual(list.Count, 0);
            Assert.AreEqual(clonedList.Count, 2);
            Assert.AreEqual(clonedList[0], 1);
        }

        [Test]
        public void DeepCloneCollectionTest()
        {
            var a = new ObjectA("a", "a", "a");
            var b = new ObjectA("b", "b", "b");
            var c = new ObjectA("c", "c", "c");
            var list = new List<ObjectA>(new[] {a, b, c});
            var clonedList = F.DeepCloneObjectCollection<ObjectA, List<ObjectA>>(list);
            clonedList[0].FirstName = "d";
            Assert.AreEqual(a.FirstName, "a");
            Assert.AreEqual(clonedList[1].LastName, "b");
            Assert.AreEqual(clonedList.Count, 3);
        }

        [Test]
        public void ShallowCloneDictionaryTest()
        {
            var dict = GetStringObjectDictionary();
            var clonedDict = F.ShallowCloneDictionary<string, object, Dictionary<string, object>>(dict);
            dict.Clear();
            Assert.AreEqual(dict.Count, 0);
            Assert.AreEqual(clonedDict.Count, 2);
            Assert.AreEqual(clonedDict["A"], 1);
        }

        #endregion

        #region Key / Value

        [Test]
        public void GetObjectValueTest()
        {
            var obj = GetObject();
            var a = F.GetValue<int>("A", obj);
            var b = F.GetValue<int>("B", obj);
            var c = F.GetValue<int>("C", obj);
            var x = F.GetValue<string>("A", obj);
            Assert.AreEqual(a, 1);
            Assert.AreEqual(b, 2);
            Assert.AreEqual(c, 0);
            Assert.AreEqual(x, null);
        }

        [Test]
        public void GetDictionaryValueTest()
        {
            var dict = GetStringObjectDictionary();
            var a = F.GetValue<int>("A", dict);
            Assert.AreEqual(a, 1);
        }

        [Test]
        public void GetDictionaryKeysTest()
        {
            var dict = GetStringObjectDictionary();
            var a = F.GetKeys(dict);
            Assert.AreEqual(a[0], "A");
            Assert.AreEqual(a[1], "B");
        }

        [Test]
        public void GetObjectKeysTest()
        {
            var a = F.GetKeys(GetObject());
            Assert.AreEqual(a[0], "A");
            Assert.AreEqual(a[1], "B");
        }

        [Test]
        public void GetDictionaryValuesTest()
        {
            var dict = GetStringObjectDictionary();
            var a = F.GetValues(dict);
            Assert.AreEqual(a[0], 1);
            Assert.AreEqual(a[1], "2");
        }

        [Test]
        public void GetObjectValuesTest()
        {
            var a = F.GetValues(GetObject());
            Assert.AreEqual(a[0], 1);
            Assert.AreEqual(a[1], 2);
        }

        [Test]
        public void SetObjectValueTest()
        {
            var obj = GetObject();
            F.SetValue("A", 22, obj);
            F.SetValue("B", 33, obj);
            Assert.AreEqual(obj.A, 22);
            Assert.AreEqual(obj.B, 33);
        }

        #endregion

        #region Map

        [Test]
        public void MapObjectTest()
        {
            var obj = GetObject();
            var mappedList = F.MapObject((k, v) => String.Format("{0}{1}", k, v.ToString()), obj);
            Assert.AreEqual(mappedList[0], "A1");
            Assert.AreEqual(mappedList[1], "B2");
        }

        [Test]
        public void MapDictionaryTest()
        {
            var dict = GetStringObjectDictionary();
            var mappedList = F.MapDictionary((k, v) => String.Format("{0}{1}", k, v.ToString()), dict);
            Assert.AreEqual(mappedList[0], "A1");
            Assert.AreEqual(mappedList[1], "B2");
        }

        [Test]
        public void MapHashSetTest()
        {
            var list = GetIntSet();
            var mappedList = F.Map(x => (x * x).ToString(), list);
            Assert.AreEqual(mappedList[0], "1");
            Assert.AreEqual(mappedList[1], "4");
        }

        [Test]
        public void MapListTest()
        {
            var list = GetIntList();
            var mappedList = F.Map(x => (x * x).ToString(), list);
            Assert.AreEqual(mappedList[0], "1");
            Assert.AreEqual(mappedList[1], "4");
        }

        [Test]
        public void MapRectArrayTest()
        {
            var rectArray = GetMixedObjectRectArray();
            var mappedList = F.MapRectangularArray(x => x[0].ToString() + x[1].ToString(), rectArray);
            Assert.AreEqual(mappedList[0], "12");
            Assert.AreEqual(mappedList[1], "23");
        }

        [Test]
        public void MapJaggedArrayTest()
        {
            var jaggedArray = GetMixedObjectJaggedArray();
            var mappedList = F.Map(x => x[0].ToString() + x[1].ToString(), jaggedArray);
            Assert.AreEqual(mappedList[0], "12");
            Assert.AreEqual(mappedList[1], "23");
        }

        [Test]
        public void MapNestedArrayTest()
        {
            var list = GetMixedObjectNestedLists();
            var mappedList = F.Map(x => x[0].ToString() + x[1].ToString(), list);
            Assert.AreEqual(mappedList[0], "12");
            Assert.AreEqual(mappedList[1], "23");
        }

        #endregion

        #region ToPairs

        [Test]
        public void ToPairsDictionaryTest()
        {
            var dict = GetStringIntDictionary();
            var list = F.ToPairs(dict);
            Assert.AreEqual(list[0][0], "A");
            Assert.AreEqual(list[0][1], 1);
            Assert.AreEqual(list[1][0], "B");
            Assert.AreEqual(list[1][1], 2);
        }

        [Test]
        public void ToPairsObjectWithMixedTest()
        {
            var list = F.ToPairs(GetObject());
            Assert.AreEqual(list[0][0], "A");
            Assert.AreEqual(list[0][1], 1);
            Assert.AreEqual(list[1][0], "B");
            Assert.AreEqual(list[1][1], 2);
        }

        #endregion

        #region FromPairs

        [Test]
        public void FromPairsJaggedArrayTest()
        {
            var jaggedArray = GetMixedObjectJaggedArray();
            var dict = F.FromPairs(jaggedArray);
            Assert.AreEqual(dict["1"], 2);
            Assert.AreEqual(dict["2"], 3);
        }

        [Test]
        public void FromPairsRectArrayTest()
        {
            var rectArray = GetMixedObjectRectArray();
            var dict = F.FromPairs(rectArray);
            Assert.AreEqual(dict["1"], 2);
            Assert.AreEqual(dict["2"], 3);
        }

        public void FromPairsArrayTest()
        {
            var nestedLists = GetMixedObjectNestedLists();
            var dict = F.FromPairs(nestedLists);
            Assert.AreEqual(dict["1"], 2);
            Assert.AreEqual(dict["2"], 3);
        }

        #endregion

        #region ShallowFlatten

        [Test]
        public void ShallowFlattenListTest()
        {
            var nestedLists = GetMixedObjectNestedLists();
            var list = F.ShallowFlatten(nestedLists);
            Assert.AreEqual(list[0], "1");
            Assert.AreEqual(list[1], 2);
            Assert.AreEqual(list[2], "2");
            Assert.AreEqual(list[3], 3);
        }

        [Test]
        public void ShallowFlattenJaggedArrayTest()
        {
            var jaggedArray = GetMixedObjectJaggedArray();
            var list = F.ShallowFlatten(jaggedArray);
            Assert.AreEqual(list[0], "1");
            Assert.AreEqual(list[1], 2);
            Assert.AreEqual(list[2], "2");
            Assert.AreEqual(list[3], 3);
        }

        [Test]
        public void ShallowFlattenRectArrayTest()
        {
            var rectArray = GetMixedObjectRectArray();
            var list = F.ShallowFlatten(rectArray);
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

        #region Zip and Merge

        [Test]
        public void MergeTest()
        {
            var a = new Dictionary<string, object> {{"a", 1}, {"b", 2}};
            var b = new Dictionary<string, object> {{"c", 1}, {"b", 3}};
            var c = F.Merge(a, b);

            Assert.AreEqual(c["a"], 1);
            Assert.AreEqual(c["b"], 3);
            Assert.AreEqual(c["c"], 1);
        }

        [Test]
        public void ZipListTest()
        {
            var keys = new[] {"a", "b", "c", "d"};
            var values = new object[] {1, "2", 3, "4"};
            var dict = F.Zip(keys, values);
            Assert.AreEqual(dict["a"], 1);
            Assert.AreEqual(dict["b"], "2");
            Assert.AreEqual(dict["c"], 3);
            Assert.AreEqual(dict["d"], "4");
        }

        [Test]
        public void ZipRectangularArrayTest()
        {
            var rectArray = GetMixedObjectRectArray();
            var dict = F.Zip(rectArray);
            Assert.AreEqual(dict["1"], 2);
            Assert.AreEqual(dict["2"], 3);
        }

        #endregion

        [Test]
        public void ShuffleTest()
        {
            var a = GetLongIntList();
            var b = F.Shuffle<int, List<int>>(a);
            Assert.AreEqual(a[0], 1);
            Assert.AreEqual(a[1], 2);
            Assert.AreEqual(a.SequenceEqual(b), false);
        }

        #region Pick and Pluck

        [Test]
        public void PluckObjectTest()
        {
            var a = new object[]
            {
                new ObjectA("samuel", "l", "jackson"),
                new ObjectA("albert", "d", "einstein"),
                new ObjectA("george", "k", "bush"),
                new ObjectB("aaron"),
                new Dictionary<string, object> {{"FirstName", "frank"}}
            };

            var b = F.PluckFromObjects<string>("FirstName", a);
            Assert.AreEqual(b[0], "samuel");
            Assert.AreEqual(b[1], "albert");
            Assert.AreEqual(b[2], "george");
            Assert.AreEqual(b[3], "aaron");
            Assert.AreEqual(b[4], null);
        }

        [Test]
        public void PluckDictionaryTest()
        {
            var a = new[]
            {
                new Dictionary<string, object> {{"FirstName", "frank"}},
                new Dictionary<string, object> {{"FirstName", "dennis"}},
                new Dictionary<string, object> {{"FirstName", "dee"}},
                new Dictionary<string, object> {{"FirstName", "mac"}}
            };

            var b = F.PluckFromDictionaries<string>("FirstName", a);
            Assert.AreEqual(b[0], "frank");
            Assert.AreEqual(b[1], "dennis");
            Assert.AreEqual(b[2], "dee");
            Assert.AreEqual(b[3], "mac");
        }

        [Test]
        public void PickAllObjectTest()
        {
            var obj = GetObject();
            var picked = F.PickAll(new[] {"A", "b"}, obj);
            Assert.AreEqual(picked["A"], 1);
            Assert.AreEqual(picked.ContainsKey("B"), false);
            Assert.AreEqual(picked.ContainsKey("b"), true);
        }

        [Test]
        public void PickAllDictionaryTest()
        {
            var dict = GetStringObjectDictionary();
            var picked = F.PickAll(new[] {"A", "b"}, dict);
            Assert.AreEqual(picked["A"], 1);
            Assert.AreEqual(picked.ContainsKey("B"), false);
            Assert.AreEqual(picked.ContainsKey("b"), true);
        }

        #endregion
    }
}