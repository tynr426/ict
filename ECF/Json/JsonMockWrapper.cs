


using System;
using System.Collections;
using System.Collections.Specialized;


namespace ECF.Json
{
	/// <summary>
	/// FullName: <see cref="ECF.Json.JsonMockWrapper"/>
	/// Summary : Json mock wrapper
	/// JsonMockWrapper.cs
	///   Mock object implementing IJsonWrapper, to facilitate actions like
	///   skipping data more efficiently.
	/// The authors disclaim copyright to this source code. For more details, see
	/// the COPYING file included with this distribution.
	/// Version: 2.1
	/// DateTime: 2015/5/14 
	/// CopyRight (c) Shaipe
	/// </summary>
	public class JsonMockWrapper : IJsonWrapper
    {
		/// <summary>
		/// Gets a value indicating whether this instance is array.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is array; otherwise, <c>false</c>.
		/// </value>
		public bool IsArray   { get { return false; } }
		/// <summary>
		/// Gets a value indicating whether this instance is boolean.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is boolean; otherwise, <c>false</c>.
		/// </value>
		public bool IsBoolean { get { return false; } }
		/// <summary>
		/// Gets a value indicating whether this instance is double.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is double; otherwise, <c>false</c>.
		/// </value>
		public bool IsDouble  { get { return false; } }

        /// <summary>
        /// 
        /// </summary>
        public bool IsDecimal { get { return false; } }
		/// <summary>
		/// Gets a value indicating whether this instance is int.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is int; otherwise, <c>false</c>.
		/// </value>
		public bool IsInt     { get { return false; } }
		/// <summary>
		/// Gets a value indicating whether this instance is long.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is long; otherwise, <c>false</c>.
		/// </value>
		public bool IsLong    { get { return false; } }
		/// <summary>
		/// Gets a value indicating whether this instance is object.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is object; otherwise, <c>false</c>.
		/// </value>
		public bool IsObject  { get { return false; } }
		/// <summary>
		/// Gets a value indicating whether this instance is string.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is string; otherwise, <c>false</c>.
		/// </value>
		public bool IsString  { get { return false; } }

		/// <summary>
		/// Gets the boolean.
		/// </summary>
		/// <returns></returns>
		public bool     GetBoolean ()  { return false; }
		/// <summary>
		/// Gets the double.
		/// </summary>
		/// <returns></returns>
		public double   GetDouble ()   { return 0.0; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public decimal GetDecimal() { return 0.0M; }
		/// <summary>
		/// Gets the int.
		/// </summary>
		/// <returns></returns>
		public int      GetInt ()      { return 0; }
		/// <summary>
		/// Gets the type of the json.
		/// </summary>
		/// <returns></returns>
		public JsonType GetJsonType () { return JsonType.None; }
		/// <summary>
		/// Gets the long.
		/// </summary>
		/// <returns></returns>
		public long     GetLong ()     { return 0L; }
		/// <summary>
		/// Gets the string.
		/// </summary>
		/// <returns></returns>
		public string   GetString ()   { return ""; }

		/// <summary>
		/// Sets the boolean.
		/// </summary>
		/// <param name="val">if set to <c>true</c> [value].</param>
		public void SetBoolean  (bool val)      {}
		/// <summary>
		/// Sets the double.
		/// </summary>
		/// <param name="val">The value.</param>
		public void SetDouble   (double val)    {}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        public void SetDecimal(decimal val) { }
		/// <summary>
		/// Sets the int.
		/// </summary>
		/// <param name="val">The value.</param>
		public void SetInt      (int val)       {}
		/// <summary>
		/// Sets the type of the json.
		/// </summary>
		/// <param name="type">The type.</param>
		public void SetJsonType (JsonType type) {}
		/// <summary>
		/// Sets the long.
		/// </summary>
		/// <param name="val">The value.</param>
		public void SetLong     (long val)      {}
		/// <summary>
		/// Sets the string.
		/// </summary>
		/// <param name="val">The value.</param>
		public void SetString   (string val)    {}

		/// <summary>
		/// To the json.
		/// </summary>
		/// <returns></returns>
		public string ToJson ()                  { return ""; }
		/// <summary>
		/// To the json.
		/// </summary>
		/// <param name="writer">The writer.</param>
		public void   ToJson (JsonWriter writer) {}


		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
		/// </summary>
		bool IList.IsFixedSize { get { return true; } }
		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> is read-only.
		/// </summary>
		bool IList.IsReadOnly  { get { return true; } }

		/// <summary>
		/// Gets or sets the <see cref="System.Object"/> at the specified index.
		/// </summary>
		/// <value>
		/// The <see cref="System.Object"/>.
		/// </value>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		object IList.this[int index] {
            get { return null; }
            set {}
        }

		/// <summary>
		/// Adds the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		int IList.Add (object value)       { return 0; }
		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.IList" />.
		/// </summary>
		void IList.Clear ()                 {}
		/// <summary>
		/// Determines whether [contains] [the specified value].
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		bool IList.Contains (object value)  { return false; }
		/// <summary>
		/// Indexes the of.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		int IList.IndexOf (object value)   { return -1; }
		/// <summary>
		/// Inserts the specified i.
		/// </summary>
		/// <param name="i">The i.</param>
		/// <param name="v">The v.</param>
		void IList.Insert (int i, object v) {}
		/// <summary>
		/// Removes the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		void IList.Remove (object value)    {}
		/// <summary>
		/// Removes at.
		/// </summary>
		/// <param name="index">The index.</param>
		void IList.RemoveAt (int index)     {}


		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		int ICollection.Count          { get { return 0; } }
		/// <summary>
		/// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
		/// </summary>
		bool ICollection.IsSynchronized { get { return false; } }
		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		object ICollection.SyncRoot       { get { return null; } }

		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="index">The index.</param>
		void ICollection.CopyTo (Array array, int index) {}


		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator () { return null; }


		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
		/// </summary>
		bool IDictionary.IsFixedSize { get { return true; } }
		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> is read-only.
		/// </summary>
		bool IDictionary.IsReadOnly  { get { return true; } }

		/// <summary>
		/// Gets an <see cref="T:System.Collections.ICollection" /> object containing the keys of the <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		ICollection IDictionary.Keys   { get { return null; } }
		/// <summary>
		/// Gets an <see cref="T:System.Collections.ICollection" /> object containing the values in the <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		ICollection IDictionary.Values { get { return null; } }

		/// <summary>
		/// Gets or sets the <see cref="System.Object"/> with the specified key.
		/// </summary>
		/// <value>
		/// The <see cref="System.Object"/>.
		/// </value>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		object IDictionary.this[object key] {
            get { return null; }
            set {}
        }

		/// <summary>
		/// Adds the specified k.
		/// </summary>
		/// <param name="k">The k.</param>
		/// <param name="v">The v.</param>
		void IDictionary.Add (object k, object v) {}
		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.IList" />.
		/// </summary>
		void IDictionary.Clear ()                 {}
		/// <summary>
		/// Determines whether [contains] [the specified key].
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		bool IDictionary.Contains (object key)    { return false; }
		/// <summary>
		/// Removes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		void IDictionary.Remove (object key)      {}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns></returns>
		IDictionaryEnumerator IDictionary.GetEnumerator () { return null; }


		/// <summary>
		/// Gets or sets the <see cref="System.Object"/> with the specified index.
		/// </summary>
		/// <value>
		/// The <see cref="System.Object"/>.
		/// </value>
		/// <param name="idx">The index.</param>
		/// <returns></returns>
		object IOrderedDictionary.this[int idx] {
            get { return null; }
            set {}
        }

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns></returns>
		IDictionaryEnumerator IOrderedDictionary.GetEnumerator () {
            return null;
        }
		/// <summary>
		/// Inserts the specified i.
		/// </summary>
		/// <param name="i">The i.</param>
		/// <param name="k">The k.</param>
		/// <param name="v">The v.</param>
		void IOrderedDictionary.Insert   (int i, object k, object v) {}
		/// <summary>
		/// Removes at.
		/// </summary>
		/// <param name="i">The i.</param>
		void IOrderedDictionary.RemoveAt (int i) {}
    }
}
