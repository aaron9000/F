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

		#region 1D Colections

		// Uniform 1D
		private HashSet<int> INT_SET() {
			return new HashSet<int> {1, 2};
		}
		private int[] INT_ARRAY() { 
			return new int[] {1, 2}; 
		}
		private List<int> INT_LIST() {
			return new List<int> (new int[] {1, 2});
		}

		// Mixed 1D
		private HashSet<object> MIXED_OBJECT_SET(){ 
			return new HashSet<object> {1, "2", 2, "3"}; 
		}
		private List<object> MIXED_OBJECT_LIST(){ 
			return new List<object> (new object[]{1, "2", 3, "3"}); 
		}
		private object[] MIXED_OBJECT_ARRAY(){
			return new object[] {1, "2", 3, "3"}; 
		}

		// Mixed 2D 
		private object[,] MIXED_OBJECT_RECT_ARRAY(){
			return new object[,] { { 1, "2" }, { 2, "3" } }; 
		}
		private object[][] MIXED_OBJECT_JAGGED_ARRAY(){ 
			return new object[][] {new object[]{ 1, "2" }, new object[]{ 2, "3" }}; 
		}
		private List<List<object>> MIXED_OBJECT_NESTED_LISTS(){ 
			var x = new List<List<object>> ();
			x.Add(new List<object>(new object[] {1 ,"2"}));
			x.Add(new List<object>(new object[] {2 ,"3"}));
			return x;
		}

		// Dict
		private Dictionary<string, int> STRING_INT_DICTIONARY() { 
			return new Dictionary<string, int> {{"a", 1}, {"b", 2}}; 
		}
		private Dictionary<string, object> STRING_OBJECT_DICTIONARY() { 
			return new Dictionary<string, object> {{"a", 1}, {"b", "2"}};
		}
		#endregion

		#region ShallowClone
		[Test]
		public void ShallowCloneTest()
		{
			var list = new List<int> (new int[] {1 ,2});
			var clonedList = F.shallowClone<int, List<int>>(list);
			list.Clear ();
			Assert.AreEqual (list.Count, 0);
			Assert.AreEqual (clonedList.Count, 2);
			Assert.AreEqual (clonedList[0], 1);
		}
		#endregion

		#region Map
		[Test]
		public void MapDictionaryTest()
		{
			var mappedList = F.mapDictionary<string, int, string>((k, v) => String.Format("{0}{1}", k, v.ToString()), STRING_INT_DICTIONARY());
			Assert.AreEqual (mappedList[0], "a1");
			Assert.AreEqual (mappedList[1], "b2");
		}

		[Test]
		public void MapHashSetTest()
		{
			var mappedList = F.map<int, string>(x => (x * x).ToString(), INT_SET());
			Assert.AreEqual (mappedList[0], "1");
			Assert.AreEqual (mappedList[1], "4");
		}

		[Test]
		public void MapListTest()
		{
					var mappedList = F.map<int, string>(x => (x * x).ToString(), INT_LIST());
			Assert.AreEqual (mappedList[0], "1");
			Assert.AreEqual (mappedList[1], "4");
		}
			
		[Test]
		public void MapRectArrayTest()
		{
					var mappedList = F.mapRectangularArray<object, string>(x => x[0].ToString() + x[1].ToString(), MIXED_OBJECT_RECT_ARRAY());
			Assert.AreEqual (mappedList[0], "12");
			Assert.AreEqual (mappedList[1], "23");
		}

		[Test]
		public void MapJaggedArrayTest()
		{
			var mappedList = F.map<object[], string>(x => x[0].ToString() + x[1].ToString(), MIXED_OBJECT_JAGGED_ARRAY());
			Assert.AreEqual (mappedList[0], "12");
			Assert.AreEqual (mappedList[1], "23");
		}
			
		[Test]
		public void MapNestedArrayTest()
		{
			var mappedList = F.map<List<object>, string>(x => x[0].ToString() + x[1].ToString(), MIXED_OBJECT_NESTED_LISTS());
			Assert.AreEqual (mappedList[0], "12");
			Assert.AreEqual (mappedList[1], "23");
		}
		#endregion


		#region ToPairs
		[Test]
		public void ToPairsTest()
		{
			var dictionary = new Dictionary<string, int> {{"a", 1}, {"b", 2}};
//			dictionary.Add ("a", 1);
//			dictionary.Add ("b", 2);

			var list = F.toPairs<string, int>(dictionary);
			Assert.AreEqual (list[0, 0], "a");
			Assert.AreEqual (list[0, 1], 1);
			Assert.AreEqual (list[1, 0], "b");
			Assert.AreEqual (list[1, 1], 2);
		}
		#endregion

		#region FromPairs
		[Test]
		public void FromPairsJaggedArrayTest()
		{
			object[,] pairs = new object[,] { { 1, "2" }, { 2, "3" } };
			Dictionary<int, string> dict = F.fromPairs<int, string>(pairs);
			Assert.AreEqual (dict[1], "2");
			Assert.AreEqual (dict[2], "3");
		}


		[Test]
		public void FromPairsRectArrayTest()
		{
			object[,] pairs = new object[,] { { 1, "2" }, { 2, "3" } };
			Dictionary<int, string> dict = F.fromPairs<int, string>(pairs);
			Assert.AreEqual (dict[1], "2");
			Assert.AreEqual (dict[2], "3");
		}

		public void FromPairsArrayTest()
		{
			//			object[,] pairs = new object[,] { { 1, "2" }, { 2, "3" } };
			var pairs = new List<List<object>> ();
			pairs.Add(new List<object>(new object[] {1 ,"2"}));
			pairs.Add(new List<object>(new object[] {2 ,"3"}));

			Dictionary<int, string> dict = F.fromPairs<int, string>(pairs);
			Assert.AreEqual (dict[1], "2");
			Assert.AreEqual (dict[2], "3");
		}
		#endregion

		#region ShallowFlatten
		[Test]
		public void ShallowFlattenListTest()
		{
			var pairs = new List<List<int>> ();
			pairs.Add(new List<int>(new int[] {1, 2}));
			pairs.Add(new List<int>(new int[] {3, 4}));

			var list = F.shallowFlatten<int>(pairs);
			Assert.AreEqual (list[0], 1);
			Assert.AreEqual (list[1], 2);
			Assert.AreEqual (list[2], 3);
			Assert.AreEqual (list[3], 4);
		}

		[Test]
		public void ShallowFlattenJaggedArrayTest()
		{
			int[][] pairs = new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5 } };
	
			var list = F.shallowFlatten<int>(pairs);
			Assert.AreEqual (list[0], 1);
			Assert.AreEqual (list[1], 2);
			Assert.AreEqual (list[2], 3);
			Assert.AreEqual (list[3], 4);
			Assert.AreEqual (list[4], 5);
		}

		[Test]
		public void ShallowFlattenRectArrayTest()
		{
			int[,] pairs = new int[,] { { 1, 2 }, { 3, 4 } };

			var list = F.shallowFlatten<int>(pairs);
			Assert.AreEqual (list[0], 1);
			Assert.AreEqual (list[1], 2);
			Assert.AreEqual (list[2], 3);
			Assert.AreEqual (list[3], 4);
		}
		#endregion


		#region Reduce
		[Test]
		public void ReduceValueTest()
		{
			var list = new List<int> (new int[] {1 ,2});
			var reducedValue = F.reduce<int, int> ((accum, value) => {
				return accum + value;
			}, 0, list);
			Assert.AreEqual (reducedValue, 3);
		}

		[Test]
		public void ReduceCollectionTest()
		{
			var list = new List<bool> (new bool[] {false, true, true});
			var reducedCollection = F.reduce<List<int>, bool> ((accum, value) => {
				accum.Add(value ? 1 : 0);
				return accum;
			}, new List<int>(), list);
			Assert.AreEqual (reducedCollection.Count, 3);
			Assert.AreEqual (reducedCollection[0], 0);
			Assert.AreEqual (reducedCollection[1], 1);
			Assert.AreEqual (reducedCollection[2], 1);
		}
		#endregion
	}
}
