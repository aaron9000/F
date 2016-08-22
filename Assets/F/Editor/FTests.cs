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

		[Test]
		public void ShallowCloneTest()
		{
			var list = new List<int> (new int[] {1 ,2});
			var clonedList = F.shallowClone<List<int>, int>(list);
			list.Clear ();
			Assert.AreEqual (list.Count, 0);
			Assert.AreEqual (clonedList.Count, 2);
			Assert.AreEqual (clonedList[0], 1);
		}

		[Test]
		public void MapTest()
		{
			var list = new List<int> (new int[] {1 ,2});
			var mappedList = F.map<int, string>(x => (x * x).ToString(), list);
			Assert.AreEqual (mappedList[0], "1");
			Assert.AreEqual (mappedList[1], "4");
		}


//		[Test]
//		public void FromPairsArrayTest()
//		{
////			object[,] pairs = new object[,] { { 1, "2" }, { 2, "3" } };
//			var pairs = new List<List<object>> ();
//			pairs.Add(new List<object>(new object[] {1 ,"2"}));
//			pairs.Add(new List<object>(new object[] {2 ,"3"}));
//
//			Dictionary<int, string> dict = F.fromPairs<int, string>(pairs);
//			Assert.AreEqual (dict[1], "2");
//			Assert.AreEqual (dict[2], "3");
//		}

//		[Test]
//		public void FromPairsListTest()
//		{
//			var pairs = new List<List<object>> ();
////			IEnumerable<IEnumerable<object>> pairs = new List<List<object>>();
//
//			pairs.Add(new List<object>(new object[] {1 ,"2"}));
//			pairs.Add(new List<object>(new object[] {2 ,"3"}));
//
//
//
//			Dictionary<int, string> dict = F.fromPairs<int, string>(pairs);
//			Assert.AreEqual (dict[1], "2");
//			Assert.AreEqual (dict[2], "3");
//		}


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



		[Test]
		public void ToPairsTest()
		{
			var dictionary = new Dictionary<string, int> ();
			dictionary.Add ("a", 1);
			dictionary.Add ("b", 2);

			var list = F.toPairs<string, int>(dictionary);
			Assert.AreEqual (list[0][0], "a");
			Assert.AreEqual (list[0][1], 1);
			Assert.AreEqual (list[1][0], "b");
			Assert.AreEqual (list[1][1], 2);
		}

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
	}
}
