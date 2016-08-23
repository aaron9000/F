using UnityEngine;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Collections;
using System.Linq;


public static class F {

	private const string PROP = "prop";
	private const string FIELD = "field";

	private class CachedTypeInfo {

		public Type type;
		public string name;
		public Dictionary<string, string> keyMap = new Dictionary<string, string>();
		public Dictionary<string, PropertyInfo> propertyTypeMap = new Dictionary<string, PropertyInfo>();
		public Dictionary<string, FieldInfo> fieldTypeMap = new Dictionary<string, FieldInfo>();

		public CachedTypeInfo(object obj){			
			PropertyInfo[] props = this.type.GetProperties ();
			foreach (var prop in props){
				if (prop.CanRead) {
					this.propertyTypeMap.Add(prop.Name, prop);
					this.keyMap.Add(prop.Name, PROP);
				}
			}

			FieldInfo[] fields = this.type.GetFields(BindingFlags.Public | BindingFlags.Instance);
			foreach (var field in fields){
				this.fieldTypeMap.Add(field.Name, field);
				this.keyMap.Add(field.Name, FIELD);
			}

			this.type = obj.GetType();
			this.name = type.Name;
		}
	}

	#region reflection & type helpers

	private static Dictionary<string, CachedTypeInfo> _cachedTypeInfo = new Dictionary<string, CachedTypeInfo>();

	private static CachedTypeInfo getTypeInfo(object obj){
		string name = obj.GetType().Name;
		if (_cachedTypeInfo.ContainsKey(name) == false){			
			_cachedTypeInfo.Add(name, new CachedTypeInfo(obj));
		}
		return _cachedTypeInfo[name];
	}

	private static T getValueForObjectKeyFast<T>(string key, CachedTypeInfo info, object obj) {
		if (obj == null)
			return default(T);
		if (info.keyMap.ContainsKey(key)){
			string kind = info.keyMap[key];
			return kind == FIELD ? (T)info.fieldTypeMap[key].GetValue(obj) : (T)info.propertyTypeMap[key].GetValue(obj, null);		
		}
		return default(T);
	}
	private static bool isCollection(Type t){
		return (t.GetInterface ("ICollection") != null);
	}

	private static bool isDictionary(Type t){
		return (t.GetInterface ("IDictionary") != null);
	}
	#endregion

	#region type & value by key
	public static Type getTypeForObjectKey(string key, object obj) {
		if (obj == null)
			return default(Type);
		var info = getTypeInfo(obj);
		if (info.keyMap.ContainsKey(key)){
			string kind = info.keyMap[key];
			return kind == FIELD ? info.fieldTypeMap[key].GetType() : info.propertyTypeMap[key].GetType();		
		}
		return default(Type);
	}
		
	public static T getValueForObjectKey<T>(string key, object obj) {
		if (obj == null)
			return default(T);
		var info = getTypeInfo(obj);
		return getValueForObjectKeyFast<T>(key, info, obj);
	}
	#endregion


	public static string[] getKeys(object obj){
		return getTypeInfo(obj).keyMap.Keys.ToArray();
	}

	public static object[] getValues(object obj){
		var info = getTypeInfo(obj);
		object[] values = new object[info.keyMap.Keys.Count];
		int index = 0;
		foreach (string key in info.keyMap.Keys){
			index++;
			values[index] = getValueForObjectKeyFast<object>(key, info, obj);
		}
		return values;
	}
		
	public static T identity<T>(T value){
		return value;
	}

	// TODO: want shallow clone of: collection, object, struct, dictionary
	public static TCollection shallowClone<TElement, TCollection>(TCollection source) where TCollection: ICollection<TElement>, new(){
		TCollection clone = new TCollection ();
		// TODO: may need to do something different for dictionaries...
		foreach(var value in source){
			clone.Add (value);
		}
		return clone;
	}
		
	#region Map
	public static TOutputElement[] mapDictionary<TInputKey, TInputValue, TOutputElement>(Func<TInputKey, TInputValue, TOutputElement> mappingFunction, IDictionary<TInputKey, TInputValue> dictionary){
		var newArray = new TOutputElement[dictionary.Keys.Count];
		int index = 0;
		foreach (KeyValuePair<TInputKey, TInputValue> entry in dictionary){
			newArray[index] = mappingFunction(entry.Key, entry.Value);
			index++;
		}
		return newArray;
	}

	public static TOutputElement[] map<TInputElement, TOutputElement>(Func<TInputElement, TOutputElement> mappingFunction, IEnumerable<TInputElement> collection){
		var newList = new List<TOutputElement>();
		foreach(TInputElement value in collection){
			newList.Add(mappingFunction (value));		
		}
		return newList.ToArray();
	}
	public static TOutputElement[] mapRectangularArray<TInputElement, TOutputElement>(Func<TInputElement[], TOutputElement> mappingFunction, TInputElement[,] rectArray){
		var newArray = new TOutputElement[rectArray.GetLength(0)];
		int innerLength = 0;
		TInputElement[] row = null;
		for (int i = 0; i < rectArray.GetLength(0); i++) {
			innerLength = rectArray.GetLength(1);
			row = new TInputElement[innerLength];
			for (int j = 0; j < innerLength; j++){
				row[j] = rectArray[i, j];
			}
			newArray[i] = mappingFunction (row);
		}
		return newArray;
	}
	#endregion

	#region FromPairs
	public static Dictionary<TKey, TValue> fromPairs<TKey, TValue>(object[][] pairs){
		var newDict = new Dictionary<TKey, TValue> ();
		foreach(var pair in pairs){
			newDict.Add ((TKey)pair[0], (TValue)pair[1]);
		}
		return newDict;
	}
	public static Dictionary<TKey, TValue> fromPairs<TKey, TValue>(List<List<object>> pairs){
		var newDict = new Dictionary<TKey, TValue> ();
		foreach(var pair in pairs){
			newDict.Add ((TKey)pair[0], (TValue)pair[1]);
		}
		return newDict;
	}
	public static Dictionary<TKey, TValue> fromPairs<TKey, TValue>(object[,] pairs){
		var newDict = new Dictionary<TKey, TValue> ();
		for (int i = 0; i < pairs.GetLength(0); i++) {
			newDict.Add ((TKey)pairs[i,0], (TValue)pairs[i,1]);
		}
		return newDict;
	}
	#endregion

	#region ToPairs
	// TODO: probably dont want lists of lists
//	public static object[,] toPairs<TKey, TValue>(object obj){		
//		object[,] pairs = new object[0, 2];
//		foreach(TKey key in []){
//			pairs.Add (new List<object> (new object[] { key, dict [key] }));
//		}
//		return pairs;
//	}
	public static object[,] toPairs<TKey, TValue>(Dictionary<TKey, TValue> dict){
		object[,] pairs = new object[dict.Keys.Count, 2];
		int index = 0;
		foreach(TKey key in dict.Keys){
			pairs[index,0] = key;
			pairs[index,1] = dict[key];
			index++;
		}
		return pairs;
	}
	#endregion

	#region reduce
	public static TAccum reduce<TAccum, TElement> (Func<TAccum, TElement, TAccum> reducingFunction, TAccum startValue, IEnumerable<TElement> list) {
		// TODO: shallowClone if not a value type
		TAccum accum = startValue;
		foreach(TElement value in list){
			accum = reducingFunction (accum, value);
		}
		return accum;
	}
	#endregion

	// Filter
	public static List<TElement> filter<TElement>(Func<TElement, bool> filteringFunction, IEnumerable<TElement> collection){
		return null;
	}


	// Zip
	public static Dictionary<TKey, TValue> zip<TKey, TValue>(IEnumerable<TKey> keys, IEnumerable<TValue> values){
		return null;
	}


	// Pluck
	public static List<TPluckedValue> pluck<TElement, TPluckedValue>(string key, IEnumerable<TElement> collection){
//		if (isDictionary())
		return null;
	}

	// Uniq

	// Merge

	// AssignToObject
		
	#region ShallowFlatten
	public static TElement[] shallowFlatten<TElement>(TElement[,] array){
		TElement[] newList = new TElement[array.Length];
		int index = 0;
		foreach (TElement element in array) {
			newList[index] = element;
			index++;
		}
		return newList;
	}
	public static TElement[] shallowFlatten<TElement>(TElement[][] lists){
		return lists.SelectMany(s => s).ToArray();
	}

	public static TElement[] shallowFlatten<TElement>(List<List<TElement>> lists){
		return lists.SelectMany(s => s).ToArray();
	}
	#endregion

}
