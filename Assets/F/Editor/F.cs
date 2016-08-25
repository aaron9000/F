using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Linq;


public static class F
{
  private class KeyInfo
  {
    public FieldInfo fieldInfo;
    public PropertyInfo propertyInfo;

    public KeyInfo(PropertyInfo p)
    {
      fieldInfo = null;
      propertyInfo = p;
    }

    public KeyInfo(FieldInfo f)
    {
      fieldInfo = f;
      propertyInfo = null;
    }

    public object getValue(object obj)
    {
      return fieldInfo != null ? fieldInfo.GetValue(obj) : propertyInfo.GetValue(obj, null);
    }

    public void setValueHelper(object obj, object value)
    {
      if (fieldInfo != null)
      {
        fieldInfo.SetValue(obj, value);
      }
      else
      {
        propertyInfo.SetValue(obj, value, null);
      }
      ;
    }
  }

  private class CachedReflectionInfo
  {
    public readonly Type type;
    public readonly Dictionary<string, KeyInfo> keyInfo = new Dictionary<string, KeyInfo>();
    public readonly HashSet<string> keySet;

    public CachedReflectionInfo(object obj)
    {
      type = obj.GetType();
      var props = type.GetProperties();
      foreach (var prop in props)
      {
        if (prop.CanRead)
        {
          keyInfo.Add(prop.Name, new KeyInfo(prop));
        }
      }
      var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
      foreach (var field in fields)
      {
        keyInfo.Add(field.Name, new KeyInfo(field));
      }
      keySet = new HashSet<string>(keyInfo.Keys.ToArray());
    }
  }

  #region Reflection & Type Helpers

  private static Dictionary<string, CachedReflectionInfo> _cachedTypeInfo =
    new Dictionary<string, CachedReflectionInfo>();

  private static CachedReflectionInfo getReflectionInfo(object obj)
  {
    var name = obj.GetType().Name;
    if (_cachedTypeInfo.ContainsKey(name) == false)
    {
      _cachedTypeInfo.Add(name, new CachedReflectionInfo(obj));
    }
    return _cachedTypeInfo[name];
  }

  private static T getObjectValueFast<T>(string key, object subject, CachedReflectionInfo info)
  {
    if (subject == null)
      return default(T);
    if (info.keyInfo.ContainsKey(key))
    {
      return (T) info.keyInfo[key].getValue(subject);
    }
    return default(T);
  }

  private static void setObjectValueFast(string key, object value, object subject, CachedReflectionInfo info)
  {
    if (subject == null)
      return;
    if (info.keyInfo.ContainsKey(key))
    {
      info.keyInfo[key].setValueHelper(subject, value);
    }
  }

  // TODO: do we want this?
//	private static Type getTypeForObjectKey(string key, object obj) {
//		if (obj == null)
//			return default(Type);
//		var info = getReflectionInfo(obj);
//		if (info.keySet.Contains(key)){
//			return info.keyInfo[key].type;
//		}
//		return default(Type);
//	}
  //	private static bool isCollection(object obj){
  //		return (obj.GetType().GetInterface ("ICollection") != null);
  //	}
  //
  //	private static bool isDictionary(object obj){
  //		return (obj.GetType().GetInterface ("IDictionary") != null);
  //	}

  #endregion

  #region Key / Value (maybe get rid of dict stuff?)

  public static T getValue<T>(string key, IDictionary<string, object> dictionary)
  {
    if (dictionary == null || !dictionary.ContainsKey(key))
      return default(T);
    return (T) dictionary[key];
  }

  public static T getValue<T>(string key, object subject)
  {
    if (subject == null)
      return default(T);
    var info = getReflectionInfo(subject);
    return getObjectValueFast<T>(key, subject, info);
  }

  public static void setValue(string key, object value, object subject)
  {
    if (subject == null)
      return;
    var info = getReflectionInfo(subject);
    setObjectValueFast(key, value, subject, info);
  }

  public static void setValue(string key, object value, IDictionary<string, object> subject)
  {
    if (subject == null)
      return;
    subject.Add(key, value);
  }

  public static string[] getKeys(IDictionary<string, object> dict)
  {
    return dict.Keys.ToArray();
  }

  public static string[] getKeys(object obj)
  {
    return getReflectionInfo(obj).keySet.ToArray();
  }

  public static object[] getValues(IDictionary<string, object> dict)
  {
    return dict.Values.ToArray();
  }

  public static object[] getValues(object obj)
  {
    var info = getReflectionInfo(obj);
    var values = new object[info.keySet.Count];
    var index = 0;
    foreach (var key in info.keySet)
    {
      values[index] = getObjectValueFast<object>(key, obj, info);
      index++;
    }
    return values;
  }

  public static T identity<T>(T value)
  {
    return value;
  }

  #endregion

  #region Merge

  #endregion

  #region shallowDictionary

  #endregion

  #region Cloning Collections and Objects

  // TODO: want shallow clone of: collection, object, struct, dictionary, value type
  public static T shallowClone<T>(T source) where T : new()
  {
    var clone = new T();

    return clone;
  }

  public static TDictionary shallowClone<TKey, TValue, TDictionary>(TDictionary source)
    where TDictionary : IDictionary<TKey, TValue>, new()
  {
    var clone = new TDictionary();
    foreach (var value in source)
    {
      clone.Add(value);
    }
    return clone;
  }

  public static TCollection shallowClone<TElement, TCollection>(TCollection source)
    where TCollection : ICollection<TElement>, new()
  {
    var clone = new TCollection();
    foreach (var value in source)
    {
      clone.Add(value);
    }
    return clone;
  }

  #endregion

  #region Map

  public static TOutputElement[] mapObject<TOutputElement>(Func<string, object, TOutputElement> mappingFunction,
    object obj)
  {
    var info = getReflectionInfo(obj);
    var newArray = new TOutputElement[info.keySet.Count];
    var index = 0;
    foreach (var key in info.keySet)
    {
      newArray[index] = mappingFunction(key, info.keyInfo[key].getValue(obj));
      index++;
    }
    return newArray;
  }

  public static TOutputElement[] mapDictionary<TOutputElement>(Func<string, object, TOutputElement> mappingFunction,
    IDictionary<string, object> dictionary)
  {
    var newArray = new TOutputElement[dictionary.Keys.Count];
    var index = 0;
    foreach (var entry in dictionary)
    {
      newArray[index] = mappingFunction(entry.Key, entry.Value);
      index++;
    }
    return newArray;
  }

  public static TOutputElement[] map<TInputElement, TOutputElement>(Func<TInputElement, TOutputElement> mappingFunction,
    IEnumerable<TInputElement> collection)
  {
    var newList = new List<TOutputElement>();
    foreach (var value in collection)
    {
      newList.Add(mappingFunction(value));
    }
    return newList.ToArray();
  }

  public static TOutputElement[] mapRectangularArray<TInputElement, TOutputElement>(
    Func<TInputElement[], TOutputElement> mappingFunction, TInputElement[,] rectArray)
  {
    var newArray = new TOutputElement[rectArray.GetLength(0)];
    var innerLength = rectArray.GetLength(1);
    var row = new TInputElement[innerLength];
    for (var i = 0; i < rectArray.GetLength(0); i++)
    {
      for (var j = 0; j < innerLength; j++)
      {
        row[j] = rectArray[i, j];
      }
      newArray[i] = mappingFunction(row);
    }
    return newArray;
  }

  #endregion

  #region toDictionary

  #endregion

  #region FromPairs

  // TODO: string, object dict
  public static Dictionary<TKey, TValue> fromPairs<TKey, TValue>(object[][] pairs)
  {
    var newDict = new Dictionary<TKey, TValue>();
    foreach (var pair in pairs)
    {
      newDict.Add((TKey) pair[0], (TValue) pair[1]);
    }
    return newDict;
  }

  public static Dictionary<TKey, TValue> fromPairs<TKey, TValue>(ICollection<List<object>> pairs)
  {
    var newDict = new Dictionary<TKey, TValue>();
    foreach (var pair in pairs)
    {
      newDict.Add((TKey) pair[0], (TValue) pair[1]);
    }
    return newDict;
  }

  public static Dictionary<TKey, TValue> fromPairs<TKey, TValue>(object[,] pairs)
  {
    var newDict = new Dictionary<TKey, TValue>();
    for (var i = 0; i < pairs.GetLength(0); i++)
    {
      newDict.Add((TKey) pairs[i, 0], (TValue) pairs[i, 1]);
    }
    return newDict;
  }

  #endregion

  #region ToPairs

  public static object[,] toPairs(object obj)
  {
    var info = getReflectionInfo(obj);
    var pairs = new object[info.keySet.Count, 2];
    var index = 0;
    foreach (var key in info.keySet)
    {
      pairs[index, 0] = key;
      pairs[index, 1] = getObjectValueFast<object>(key, obj, info);
      index++;
    }
    return pairs;
  }

  public static object[,] toPairs<TKey, TValue>(IDictionary<TKey, TValue> dict)
  {
    var pairs = new object[dict.Keys.Count, 2];
    var index = 0;
    foreach (var key in dict.Keys)
    {
      pairs[index, 0] = key;
      pairs[index, 1] = dict[key];
      index++;
    }
    return pairs;
  }

  #endregion

  #region Reduce

  public static TAccum reduce<TAccum, TElement>(Func<TAccum, TElement, TAccum> reducingFunction, TAccum startValue,
    IEnumerable<TElement> list)
  {
    // TODO: shallowClone if not a value type
    var accum = startValue;
    foreach (var value in list)
    {
      accum = reducingFunction(accum, value);
    }
    return accum;
  }

  #endregion

  // Filter
  public static List<TElement> filter<TElement>(Func<TElement, bool> filteringFunction, IEnumerable<TElement> collection)
  {
    return null;
  }


  // Zip
  public static Dictionary<TKey, TValue> zip<TKey, TValue>(IEnumerable<TKey> keys, IEnumerable<TValue> values)
  {
    return null;
  }


  // Pluck
  public static List<TPluckedValue> pluck<TElement, TPluckedValue>(string key, IEnumerable<TElement> collection)
  {
//		if (isDictionary())
    return null;
  }

  // Unique

  // Merge

  #region PickAll

  #endregion

  #region ShallowFlatten

  public static TElement[] shallowFlatten<TElement>(TElement[,] array)
  {
    var newList = new TElement[array.Length];
    var index = 0;
    foreach (var element in array)
    {
      newList[index] = element;
      index++;
    }
    return newList;
  }

  public static TElement[] shallowFlatten<TElement>(TElement[][] lists)
  {
    return lists.SelectMany(s => s).ToArray();
  }

  public static TElement[] shallowFlatten<TElement>(ICollection<List<TElement>> lists)
  {
    return lists.SelectMany(s => s).ToArray();
  }

  #endregion
}