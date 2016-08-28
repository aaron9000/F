#F

Functional programming utilities for Unity3D. Inspiration taken from `ramda.js`.

##Features
- Consistent parameter ordering
- Side-effect free
- Declarative syntax
- Encourages the use of lambdas and LINQ

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

##Supported Data Types
- `object`
- `IDictionary<K, V>`
- `ICollection<T>`
- `ICollection<List<T>>`
- `T[][]`
- `T[,]`

##Samples
`c#

private class ObjectA
{
    public string first_name;
    public string middle_name;
    public string last_name;

    public ObjectA(string first, string middle, string last)
    {
        first_name = first;
        middle_name = middle;
        last_name = last;
    }
}

private class ObjectB
{
    public int first_name_letter_count;
    public int last_name_letter_count;
    public string first_name_capitalized;
    public string last_name_capitalized;

    public ObjectB()
    {
    }
}

private class ObjectC
{
    public string first_name;

    public ObjectC(string first)
    {
        first_name = first;
    }
}

[Test]
public void ExampleATest()
{
    var a = new ObjectA("samuel", "l", "jackson");
    var b = F.ToPairs(a);
    var c = F.Reduce((accum, pair) =>
    {
        var key = (string) pair[0];
        var val = (string) pair[1];
        accum.Add(key + "_letter_count", val.Length);
        accum.Add(key + "_capitalized", val.ToUpper());
        return accum;
    }, F.EmptyDictionary(), b);
    var d = F.ShallowObjectFromDictionary<ObjectB>(c);

    Assert.AreEqual(d.first_name_capitalized, "SAMUEL");
    Assert.AreEqual(d.last_name_capitalized, "JACKSON");
    Assert.AreEqual(d.first_name_letter_count, 6);
    Assert.AreEqual(d.last_name_letter_count, 7);
}

`
