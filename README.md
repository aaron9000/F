#F

##Functional Programming Utilities for Unity3D
F makes working with collections, objects, and dictionaries more pleasant. Transform data easily and spend less time converting data between types. Additionally, it will encourage you to write fewer loops and more lambdas. F is inspired by `ramda.js`.

##Features
- Declarative syntax
- Encourages the use of lambdas and LINQ
- Shallow immutability
- Makes repetitive data transformations easier

##Add to Your Project
Place `F.cs` in your Scripts folder.

##Running the Tests
F's tests are written using Unity's testing framework. You will need to open the project in a Unity3D editor.

`Window -> Editor Tests Runner -> Run All`

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

##Examples
```c#
private class Name
{
    public string FirstName;
    public string MiddleName;
    public string LastName;

    public Name() {}
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

    public NameMetrics() {}
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

```

```c#
 private class Rabbit
{
    public float Weight = 2.0f;
    public int Carrots = 0;

    public Rabbit() {}
}

private class Squirrel
{
    public float Weight = 3.0f;
    public int Nuts = 0;

    public Squirrel() {}
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

```
        
```c#
private class PartOne
{
    public int A = 1;
    public int B = 2;

    public PartOne() {}
}

private class PartTwo   
{
    public int C = 3;
    public int D = 4;

    public PartTwo() {}
}

private class CombinedParts
{
    public int A = 0;
    public int B = 0;
    public int C = 0;
    public int D = 0;

    public CombinedParts() {}
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
```
