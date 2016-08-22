using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;

public static class F {

	// TODO: can cache stuff that relied on reflection...
	private static Dictionary<string, Dictionary<string, Type>> _cachedTypes = new Dictionary<string, Dictionary<string, Type>>();


	//public static bool isEnumerable(Type t){
	//	return (t.GetInterface ("IEnumerable") != null);
	//}

	public static bool isCollection(Type t){
		return (t.GetInterface ("ICollection") != null);
	}

	public static bool isDictionary(Type t){
		return (t.GetInterface ("IDictionary") != null);
	}

	public static T getValueForStructKey<T>(string key, object strct) {
		return default(T);
	}

	public static T getValueForObjectKey<T>(string key, object obj) {
		if (obj == null)
			return default(T);
		Type t = obj.GetType ();
		PropertyInfo[] props = t.GetProperties ();
		foreach (var info in props){
			if (info.Name == key && info.GetType () == typeof(T) && info.CanRead) {
				return (T)info.GetValue (obj, null);
			}
		}
		return default(T);
	}

	public static T identity<T>(T value){
		return value;
	}

	// TODO: want shallow clone of: collection, object, struct, dictionary
	public static TCollection shallowClone<TCollection, TElement>(TCollection source) where TCollection: ICollection<TElement>, new(){
		TCollection clone = new TCollection ();
		// TODO: may need to do something different for dictionaries...
		foreach(var value in source){
			clone.Add (value);
		}
		return clone;
	}

	public static List<TOutput> map<TInput, TOutput>(Func<TInput, TOutput> mappingFunction, IEnumerable<TInput> collection){
		List<TOutput> newList = new List<TOutput> ();
		foreach(TInput value in collection){
			newList.Add (mappingFunction (value));
		}
		return newList;
	}

	public static Dictionary<TKey, TValue> fromPairs<TKey, TValue>(List<List<object>> pairs){
		Dictionary<TKey, TValue> newDict = new Dictionary<TKey, TValue> ();
		foreach(List<object> pair in pairs){
			newDict.Add ((TKey)pair [0], (TValue)pair [1]);
		}
		return newDict;
	}

	public static List<List<object>> toPairs<TKey, TValue>(Dictionary<TKey, TValue> dict){
		List<List<object>> pairs = new List<List<object>>();
		foreach(TKey key in dict.Keys){
			pairs.Add (new List<object> (new object[] { key, dict [key] }));
		}
		return pairs;
	}

	public static TAccum reduce<TAccum, TElement> (Func<TAccum, TElement, TAccum> reducingFunction, TAccum startValue, IEnumerable<TElement> list) {
		TAccum accum = startValue;
		foreach(TElement value in list){
			accum = reducingFunction (accum, value);
		}
		return accum;
	}

	public static List<TElement> filter<TElement>(Func<TElement, bool> filteringFunction,IEnumerable<TElement> collection){
		return null;
	}

	public static Dictionary<TKey, TValue> zip<TKey, TValue>(IEnumerable<TKey> keys, IEnumerable<TValue> values){
		return null;
	}

	public static List<TPluckedValue> pluck<TElement, TPluckedValue>(string key, IEnumerable<TElement> collection){
		return null;
	}



}
