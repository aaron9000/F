#F

Functional programming utilities for Unity3D. Inspiration taken from `ramda.js`.

##Features
- Side-effect free*
- Declarative syntax
- Encourages the use of lambdas and LINQ
- Shallow immutability
- Write more dynamic code without inheritance

##Tradeoffs
- Less type safety
- Performance (reflection)


##Functions
- `Map`
- `MapObject`
- `MapDictionary`
- `MapRectangularArray`
- `FromPairs`
- `ToPairs`
- `Reduce`
- `Merge`
- `Zip`
- `PluckFromDictionaries`
- `PluckFromObjects`
- `PickAll`
- `Shuffle`
- `EmptyDictionary`
- `Range`
- `CoerceDictionary`
- `GetValue`
- `SetValue`
- `GetKeys`
- `GetValues`
- `ShallowObjectFromDictionary`
- `ShallowDictionaryFromObject`
- `ShallowCloneObject`
- `ShallowCloneDictionary`
- `ShallowCloneCollection`
- `ShallowFlatten`
- `DeepCloneObjectCollection`

##Works with Familiar Types
- `object`
- `IDictionary<K, V>`
- `ICollection<T>`
- `ICollection<List<T>>`
- `T[][]`
- `T[,]`

##Samples
```c#
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

            // Boom!
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

```
