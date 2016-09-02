using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using UnityEngine;

namespace UnityTest
{
    [TestFixture]
    [Category("F Example Tests")]
    internal class FExampleTests
    {
        private class Name
        {
            public string FirstName;
            public string MiddleName;
            public string LastName;

            public Name()
            {
            }
        }

        private class NameMetrics
        {
            public int FirstNameLetterCount;
            public int MiddleNameLetterCount;
            public int LastNameLetterCount;
            public string FirstNameNormalized;
            public string MiddleNameNormalized;
            public string LastNameNormalized;
            public string FirstNameInitial;
            public string MiddleNameInitial;
            public string LastNameInitial;

            public NameMetrics()
            {
            }
        }

        [Test]
        public void ConvertNameToMetricsTest()
        {
            // Converts an object of one class to another without writing repetitive code
            var a = new Name {FirstName = "Samuel", MiddleName = "Leroy", LastName = "Jackson"};
            var b = F.ToPairs(a);
            var c = F.Reduce((accum, pair) =>
            {
                var key = (string) pair[0];
                var val = (string) pair[1];
                accum.Add(key + "Normalized", val.ToLower());
                accum.Add(key + "LetterCount", val.Length);
                accum.Add(key + "Initial", val.Substring(0, 1));
                return accum;
            }, F.EmptyDictionary(), b);
            var d = F.ShallowObjectFromDictionary<NameMetrics>(c);

            Assert.AreEqual(d.FirstNameNormalized, "samuel");
            Assert.AreEqual(d.MiddleNameNormalized, "leroy");
            Assert.AreEqual(d.LastNameNormalized, "jackson");
            Assert.AreEqual(d.FirstNameLetterCount, 6);
            Assert.AreEqual(d.MiddleNameLetterCount, 5);
            Assert.AreEqual(d.LastNameLetterCount, 7);
            Assert.AreEqual(d.FirstNameInitial, "S");
            Assert.AreEqual(d.MiddleNameInitial, "L");
            Assert.AreEqual(d.LastNameInitial, "J");
        }

        private class Rabbit
        {
            public float Weight = 2.0f;
            public int Carrots = 0;

            public Rabbit(){}
        }

        private class Squirrel
        {
            public float Weight = 3.0f;
            public int Nuts = 0;

            public Squirrel(){}
        }

        [Test]
        public void SumWeightsOfAnimalsTest()
        {
            // Sum values of objects with a common "Weight" field
            var a = new object[] {new Rabbit(), new Squirrel()};
            var b = F.PluckFromObjects<float>("Weight", a);
            var c = b.Sum();

            Assert.AreEqual(c, 5.0f);
        }


        private class PartOne
        {
            public int A = 1;
            public int B = 2;

            public PartOne(){}
        }

        private class PartTwo
        {
            public int C = 3;
            public int D = 4;

            public PartTwo()
            {
            }
        }

        private class CombinedParts
        {
            public int A = 0;
            public int B = 0;
            public int C = 0;
            public int D = 0;

            public CombinedParts(){}
        }

        [Test]
        public void CombinePartsTest()
        {
            // Combines two typed objects into a third type
            var a = F.ShallowDictionaryFromObject(new PartOne());
            var b = F.ShallowDictionaryFromObject(new PartTwo());
            var c = F.Merge(a, b);
            var d = F.ShallowObjectFromDictionary<CombinedParts>(c);

            Assert.AreEqual(d.A, 1);
            Assert.AreEqual(d.B, 2);
            Assert.AreEqual(d.C, 3);
            Assert.AreEqual(d.D, 4);
        }

    }
}