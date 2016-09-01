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
            public string MiddleNameIntial;
            public string LastNameInitial;

            public NameMetrics()
            {
            }
        }

        [Test]
        public void ObjectManipulationTest()
        {
            var a = new Name {FirstName = "Samuel", MiddleName = "Leroy", LastName = "Jackson"};
            var b = F.ToPairs(a);
            var c = F.Reduce((accum, pair) =>
            {
                var key = (string) pair[0];
                var val = (string) pair[1];
                accum.Add(key + "Normalized", val.ToLower());
                accum.Add(key + "LetterCount", val.Substring(0, 1));
                accum.Add(key + "Initial", val.First());
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
            Assert.AreEqual(d.MiddleNameIntial, "L");
            Assert.AreEqual(d.LastNameInitial, "J");
        }

        [Test]
        public void ExampleBTest()
        {
            var a = F.Range(1, 4);
            var b = F.Map(v => v * v, a);
            var c = b.Sum();
            Assert.AreEqual(c, 14);
        }

        [Test]
        public void ExampleCTest()
        {
            var a = F.Range(1, 4);
            var b = F.Map(v => v * v, a);
            var c = b.Sum();
            Assert.AreEqual(c, 14);
        }

    }
}