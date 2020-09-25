


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;


namespace ECF.Json
{
	/// <summary>
	/// FullName: <see cref="ECF.Json.JsonData"/>
	/// Summary : JsonData.cs
	/// Generic type to hold JSON data(objects, arrays, and so on). This is
	/// the default type returned by JsonMapper.ToObject().
	/// The authors disclaim copyright to this source code. For more details, see
	/// the COPYING file included with this distribution.
	/// Version: 2.1
	/// DateTime: 2015/5/14 
	/// CopyRight (c) Shaipe
	/// </summary>
	public class JsonData : IJsonWrapper, IEquatable<JsonData>
	{
		#region Fields
		/// <summary>
		/// The inst_array
		/// </summary>
		private IList<JsonData> inst_array;
		/// <summary>
		/// The inst_boolean
		/// </summary>
		private bool inst_boolean;
		/// <summary>
		/// The inst_double
		/// </summary>
		private double inst_double;
        /// <summary>
        /// 
        /// </summary>
        private decimal inst_decimal;
		/// <summary>
		/// The inst_int
		/// </summary>
		private int inst_int;
		/// <summary>
		/// The inst_long
		/// </summary>
		private long inst_long;
		/// <summary>
		/// The inst_object
		/// </summary>
		private IDictionary<string, JsonData> inst_object;
		/// <summary>
		/// The inst_string
		/// </summary>
		private string inst_string;
		/// <summary>
		/// The json
		/// </summary>
		private string json;
		/// <summary>
		/// The type
		/// </summary>
		private JsonType type;

		// Used to implement the IOrderedDictionary interface
		/// <summary>
		/// The object_list
		/// </summary>
		private IList<KeyValuePair<string, JsonData>> object_list;
		#endregion


		#region Properties
		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		public int Count
		{
			get { return EnsureCollection().Count; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is array.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is array; otherwise, <c>false</c>.
		/// </value>
		public bool IsArray
		{
			get { return type == JsonType.Array; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is boolean.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is boolean; otherwise, <c>false</c>.
		/// </value>
		public bool IsBoolean
		{
			get { return type == JsonType.Boolean; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is double.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is double; otherwise, <c>false</c>.
		/// </value>
		public bool IsDouble
		{
			get { return type == JsonType.Double; }
		}

        /// <summary>
        /// is decimal
        /// </summary>
        public bool IsDecimal
        {
            get
            {
                return type == JsonType.Decimal;
            }
        }
		/// <summary>
		/// Gets a value indicating whether this instance is int.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is int; otherwise, <c>false</c>.
		/// </value>
		public bool IsInt
		{
			get { return type == JsonType.Int; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is long.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is long; otherwise, <c>false</c>.
		/// </value>
		public bool IsLong
		{
			get { return type == JsonType.Long; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is object.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is object; otherwise, <c>false</c>.
		/// </value>
		public bool IsObject
		{
			get { return type == JsonType.Object; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is string.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is string; otherwise, <c>false</c>.
		/// </value>
		public bool IsString
		{
			get { return type == JsonType.String; }
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.ICollection" /> object containing the keys of the <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		public ICollection<string> Keys
		{
			get { EnsureDictionary(); return inst_object.Keys; }
		}
		#endregion


		#region ICollection Properties
		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		int ICollection.Count
		{
			get
			{
				return Count;
			}
		}

		/// <summary>
		/// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection" /> is synchronized (thread safe).
		/// </summary>
		bool ICollection.IsSynchronized
		{
			get
			{
				return EnsureCollection().IsSynchronized;
			}
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection" />.
		/// </summary>
		object ICollection.SyncRoot
		{
			get
			{
				return EnsureCollection().SyncRoot;
			}
		}
		#endregion


		#region IDictionary Properties
		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
		/// </summary>
		bool IDictionary.IsFixedSize
		{
			get
			{
				return EnsureDictionary().IsFixedSize;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> is read-only.
		/// </summary>
		bool IDictionary.IsReadOnly
		{
			get
			{
				return EnsureDictionary().IsReadOnly;
			}
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.ICollection" /> object containing the keys of the <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		ICollection IDictionary.Keys
		{
			get
			{
				EnsureDictionary();
				IList<string> keys = new List<string>();

				foreach (KeyValuePair<string, JsonData> entry in
						 object_list)
				{
					keys.Add(entry.Key);
				}

				return (ICollection)keys;
			}
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.ICollection" /> object containing the values in the <see cref="T:System.Collections.IDictionary" /> object.
		/// </summary>
		ICollection IDictionary.Values
		{
			get
			{
				EnsureDictionary();
				IList<JsonData> values = new List<JsonData>();

				foreach (KeyValuePair<string, JsonData> entry in
						 object_list)
				{
					values.Add(entry.Value);
				}

				return (ICollection)values;
			}
		}
		#endregion



		#region IJsonWrapper Properties
		/// <summary>
		/// Gets a value indicating whether this instance is array.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is array; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsArray
		{
			get { return IsArray; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is boolean.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is boolean; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsBoolean
		{
			get { return IsBoolean; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is double.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is double; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsDouble
		{
			get { return IsDouble; }
		}

        bool IJsonWrapper.IsDecimal
        {
            get { return IsDecimal; }
        }

		/// <summary>
		/// Gets a value indicating whether this instance is int.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is int; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsInt
		{
			get { return IsInt; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is long.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is long; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsLong
		{
			get { return IsLong; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is object.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is object; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsObject
		{
			get { return IsObject; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is string.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is string; otherwise, <c>false</c>.
		/// </value>
		bool IJsonWrapper.IsString
		{
			get { return IsString; }
		}
		#endregion


		#region IList Properties
		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> has a fixed size.
		/// </summary>
		bool IList.IsFixedSize
		{
			get
			{
				return EnsureList().IsFixedSize;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IList" /> is read-only.
		/// </summary>
		bool IList.IsReadOnly
		{
			get
			{
				return EnsureList().IsReadOnly;
			}
		}
		#endregion


		#region IDictionary Indexer
		/// <summary>
		/// Gets or sets the <see cref="System.Object"/> with the specified key.
		/// </summary>
		/// <value>
		/// The <see cref="System.Object"/>.
		/// </value>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException">The key has to be a string</exception>
		object IDictionary.this[object key]
		{
			get
			{
				return EnsureDictionary()[key];
			}

			set
			{
				if (!(key is String))
					throw new ArgumentException(
						"The key has to be a string");

				JsonData data = ToJsonData(value);

				this[(string)key] = data;
			}
		}
		#endregion


		#region IOrderedDictionary Indexer
		/// <summary>
		/// Gets or sets the <see cref="System.Object"/> with the specified index.
		/// </summary>
		/// <value>
		/// The <see cref="System.Object"/>.
		/// </value>
		/// <param name="idx">The index.</param>
		/// <returns></returns>
		object IOrderedDictionary.this[int idx]
		{
			get
			{
				EnsureDictionary();
				return object_list[idx].Value;
			}

			set
			{
				EnsureDictionary();
				JsonData data = ToJsonData(value);

				KeyValuePair<string, JsonData> old_entry = object_list[idx];

				inst_object[old_entry.Key] = data;

				KeyValuePair<string, JsonData> entry =
					new KeyValuePair<string, JsonData>(old_entry.Key, data);

				object_list[idx] = entry;
			}
		}
		#endregion


		#region IList Indexer
		/// <summary>
		/// Gets or sets the <see cref="System.Object"/> at the specified index.
		/// </summary>
		/// <value>
		/// The <see cref="System.Object"/>.
		/// </value>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		object IList.this[int index]
		{
			get
			{
				return EnsureList()[index];
			}

			set
			{
				EnsureList();
				JsonData data = ToJsonData(value);

				this[index] = data;
			}
		}
		#endregion


		#region Public Indexers
		/// <summary>
		/// Gets or sets the <see cref="JsonData"/> with the specified prop_name.
		/// </summary>
		/// <value>
		/// The <see cref="JsonData"/>.
		/// </value>
		/// <param name="prop_name">The prop_name.</param>
		/// <returns></returns>
		public JsonData this[string prop_name]
		{
			get
			{
				EnsureDictionary();
				if (inst_object.ContainsKey(prop_name))
					return inst_object[prop_name];

				return null;
			}

			set
			{
				EnsureDictionary();

				KeyValuePair<string, JsonData> entry =
					new KeyValuePair<string, JsonData>(prop_name, value);

				if (inst_object.ContainsKey(prop_name))
				{
					for (int i = 0; i < object_list.Count; i++)
					{
						if (object_list[i].Key == prop_name)
						{
							object_list[i] = entry;
							break;
						}
					}
				}
				else
					object_list.Add(entry);

				inst_object[prop_name] = value;

				json = null;
			}
		}

		/// <summary>
		/// Gets or sets the <see cref="JsonData"/> at the specified index.
		/// </summary>
		/// <value>
		/// The <see cref="JsonData"/>.
		/// </value>
		/// <param name="index">The index.</param>
		/// <returns></returns>
		public JsonData this[int index]
		{
			get
			{
				EnsureCollection();

				if (type == JsonType.Array)
					return inst_array[index];

				return object_list[index].Value;
			}

			set
			{
				EnsureCollection();

				if (type == JsonType.Array)
					inst_array[index] = value;
				else
				{
					KeyValuePair<string, JsonData> entry = object_list[index];
					KeyValuePair<string, JsonData> new_entry =
						new KeyValuePair<string, JsonData>(entry.Key, value);

					object_list[index] = new_entry;
					inst_object[entry.Key] = value;
				}

				json = null;
			}
		}
		#endregion


		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="JsonData"/> class.
		/// </summary>
		public JsonData()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonData"/> class.
		/// </summary>
		/// <param name="boolean">if set to <c>true</c> [boolean].</param>
		public JsonData(bool boolean)
		{
			type = JsonType.Boolean;
			inst_boolean = boolean;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonData"/> class.
		/// </summary>
		/// <param name="number">The number.</param>
		public JsonData(double number)
		{
			type = JsonType.Double;
			inst_double = number;
		}

        /// <summary>
        /// Decimal
        /// </summary>
        /// <param name="number"></param>
        public JsonData(decimal number)
        {
            type = JsonType.Decimal;
            inst_decimal = number;
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonData"/> class.
		/// </summary>
		/// <param name="number">The number.</param>
		public JsonData(int number)
		{
			type = JsonType.Int;
			inst_int = number;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonData"/> class.
		/// </summary>
		/// <param name="number">The number.</param>
		public JsonData(long number)
		{
			type = JsonType.Long;
			inst_long = number;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonData"/> class.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <exception cref="ArgumentException">Unable to wrap the given object with JsonData</exception>
		public JsonData(object obj)
		{
			if (obj is Boolean)
			{
				type = JsonType.Boolean;
				inst_boolean = (bool)obj;
				return;
			}

			if (obj is Double)
			{
				type = JsonType.Double;
				inst_double = (double)obj;
				return;
			}

			if (obj is Int32)
			{
				type = JsonType.Int;
				inst_int = (int)obj;
				return;
			}

			if (obj is Int64)
			{
				type = JsonType.Long;
				inst_long = (long)obj;
				return;
			}

			if (obj is String)
			{
				type = JsonType.String;
				inst_string = (string)obj;
				return;
			}

			throw new ArgumentException(
				"Unable to wrap the given object with JsonData");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="JsonData"/> class.
		/// </summary>
		/// <param name="str">The string.</param>
		public JsonData(string str)
		{
			type = JsonType.String;
			inst_string = str;
		}
		#endregion


		#region Implicit Conversions
		/// <summary>
		/// Performs an implicit conversion from <see cref="Boolean"/> to <see cref="JsonData"/>.
		/// </summary>
		/// <param name="data">if set to <c>true</c> [data].</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator JsonData(Boolean data)
		{
			return new JsonData(data);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Double"/> to <see cref="JsonData"/>.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator JsonData(Double data)
		{
			return new JsonData(data);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static implicit operator JsonData(Decimal data)
        {
            return new JsonData(data);
        }

		/// <summary>
		/// Performs an implicit conversion from <see cref="Int32"/> to <see cref="JsonData"/>.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator JsonData(Int32 data)
		{
			return new JsonData(data);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="Int64"/> to <see cref="JsonData"/>.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator JsonData(Int64 data)
		{
			return new JsonData(data);
		}

		/// <summary>
		/// Performs an implicit conversion from <see cref="String"/> to <see cref="JsonData"/>.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		public static implicit operator JsonData(String data)
		{
			return new JsonData(data);
		}
		#endregion


		#region Explicit Conversions
		/// <summary>
		/// Performs an explicit conversion from <see cref="JsonData"/> to <see cref="Boolean"/>.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		/// <exception cref="InvalidCastException">Instance of JsonData doesn't hold a double</exception>
		public static explicit operator Boolean(JsonData data)
		{
			if (data.type != JsonType.Boolean)
				throw new InvalidCastException(
					"Instance of JsonData doesn't hold a double");

			return data.inst_boolean;
		}

		/// <summary>
		/// Performs an explicit conversion from <see cref="JsonData"/> to <see cref="Double"/>.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		/// <exception cref="InvalidCastException">Instance of JsonData doesn't hold a double</exception>
		public static explicit operator Double(JsonData data)
		{
			if (data.type != JsonType.Double)
				throw new InvalidCastException(
					"Instance of JsonData doesn't hold a double");

			return data.inst_double;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        public static explicit operator Decimal(JsonData data)
        {
            if (data.type != JsonType.Decimal)
                throw new InvalidCastException(
                    "Instance of JsonData doesn't hold a double");

            return data.inst_decimal;
        }

		/// <summary>
		/// Performs an explicit conversion from <see cref="JsonData"/> to <see cref="Int32"/>.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		/// <exception cref="InvalidCastException">Instance of JsonData doesn't hold an int</exception>
		public static explicit operator Int32(JsonData data)
		{
			if (data.type != JsonType.Int)
				throw new InvalidCastException(
					"Instance of JsonData doesn't hold an int");

			return data.inst_int;
		}

		/// <summary>
		/// Performs an explicit conversion from <see cref="JsonData"/> to <see cref="Int64"/>.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		/// <exception cref="InvalidCastException">Instance of JsonData doesn't hold an int</exception>
		public static explicit operator Int64(JsonData data)
		{
			if (data.type != JsonType.Long)
				throw new InvalidCastException(
					"Instance of JsonData doesn't hold an int");

			return data.inst_long;
		}

		/// <summary>
		/// Performs an explicit conversion from <see cref="JsonData"/> to <see cref="String"/>.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <returns>
		/// The result of the conversion.
		/// </returns>
		/// <exception cref="InvalidCastException">Instance of JsonData doesn't hold a string</exception>
		public static explicit operator String(JsonData data)
		{
			if (data.type != JsonType.String)
				throw new InvalidCastException(
					"Instance of JsonData doesn't hold a string");

			return data.inst_string;
		}
		#endregion


		#region ICollection Methods
		/// <summary>
		/// Copies to.
		/// </summary>
		/// <param name="array">The array.</param>
		/// <param name="index">The index.</param>
		void ICollection.CopyTo(Array array, int index)
		{
			EnsureCollection().CopyTo(array, index);
		}
		#endregion


		#region IDictionary Methods
		/// <summary>
		/// Adds the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		void IDictionary.Add(object key, object value)
		{
			JsonData data = ToJsonData(value);

			EnsureDictionary().Add(key, data);

			KeyValuePair<string, JsonData> entry =
				new KeyValuePair<string, JsonData>((string)key, data);
			object_list.Add(entry);

			json = null;
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.IList" />.
		/// </summary>
		void IDictionary.Clear()
		{
			EnsureDictionary().Clear();
			object_list.Clear();
			json = null;
		}

		/// <summary>
		/// Determines whether [contains] [the specified key].
		/// </summary>
		/// <param name="key">The key.</param>
		/// <returns></returns>
		bool IDictionary.Contains(object key)
		{
			return EnsureDictionary().Contains(key);
		}

		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns></returns>
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return ((IOrderedDictionary)this).GetEnumerator();
		}

		/// <summary>
		/// Removes the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		void IDictionary.Remove(object key)
		{
			EnsureDictionary().Remove(key);

			for (int i = 0; i < object_list.Count; i++)
			{
				if (object_list[i].Key == (string)key)
				{
					object_list.RemoveAt(i);
					break;
				}
			}

			json = null;
		}
		#endregion


		#region IEnumerable Methods
		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return EnsureCollection().GetEnumerator();
		}
		#endregion


		#region IJsonWrapper Methods
		/// <summary>
		/// Gets the boolean.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">JsonData instance doesn't hold a boolean</exception>
		bool IJsonWrapper.GetBoolean()
		{
			if (type != JsonType.Boolean)
				throw new InvalidOperationException(
					"JsonData instance doesn't hold a boolean");

			return inst_boolean;
		}

		/// <summary>
		/// Gets the double.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">JsonData instance doesn't hold a double</exception>
		double IJsonWrapper.GetDouble()
		{
			if (type != JsonType.Double)
				throw new InvalidOperationException(
					"JsonData instance doesn't hold a double");

			return inst_double;
		}

        /// <summary>
        /// Get Decimal
        /// </summary>
        /// <returns></returns>
        decimal IJsonWrapper.GetDecimal()
        {
            if (type != JsonType.Decimal)
                throw new InvalidOperationException(
                    "JsonData instance doesn't hold a double");

            return inst_decimal;
        }

		/// <summary>
		/// Gets the int.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">JsonData instance doesn't hold an int</exception>
		int IJsonWrapper.GetInt()
		{
			if (type != JsonType.Int)
				throw new InvalidOperationException(
					"JsonData instance doesn't hold an int");

			return inst_int;
		}

		/// <summary>
		/// Gets the long.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">JsonData instance doesn't hold a long</exception>
		long IJsonWrapper.GetLong()
		{
			if (type != JsonType.Long)
				throw new InvalidOperationException(
					"JsonData instance doesn't hold a long");

			return inst_long;
		}

		/// <summary>
		/// Gets the string.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">JsonData instance doesn't hold a string</exception>
		string IJsonWrapper.GetString()
		{
			if (type != JsonType.String)
				throw new InvalidOperationException(
					"JsonData instance doesn't hold a string");

			return inst_string;
		}

		/// <summary>
		/// Sets the boolean.
		/// </summary>
		/// <param name="val">if set to <c>true</c> [value].</param>
		void IJsonWrapper.SetBoolean(bool val)
		{
			type = JsonType.Boolean;
			inst_boolean = val;
			json = null;
		}

		/// <summary>
		/// Sets the double.
		/// </summary>
		/// <param name="val">The value.</param>
		void IJsonWrapper.SetDouble(double val)
		{
			type = JsonType.Double;
			inst_double = val;
			json = null;
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        void IJsonWrapper.SetDecimal(decimal val)
        {
            type = JsonType.Decimal;
            inst_decimal = val;
            json = null;
        }

		/// <summary>
		/// Sets the int.
		/// </summary>
		/// <param name="val">The value.</param>
		void IJsonWrapper.SetInt(int val)
		{
			type = JsonType.Int;
			inst_int = val;
			json = null;
		}

		/// <summary>
		/// Sets the long.
		/// </summary>
		/// <param name="val">The value.</param>
		void IJsonWrapper.SetLong(long val)
		{
			type = JsonType.Long;
			inst_long = val;
			json = null;
		}

		/// <summary>
		/// Sets the string.
		/// </summary>
		/// <param name="val">The value.</param>
		void IJsonWrapper.SetString(string val)
		{
			type = JsonType.String;
			inst_string = val;
			json = null;
		}

		/// <summary>
		/// To the json.
		/// </summary>
		/// <returns></returns>
		string IJsonWrapper.ToJson()
		{
			return ToJson();
		}

		/// <summary>
		/// To the json.
		/// </summary>
		/// <param name="writer">The writer.</param>
		void IJsonWrapper.ToJson(JsonWriter writer)
		{
			ToJson(writer);
		}
		#endregion


		#region IList Methods
		/// <summary>
		/// Adds the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		int IList.Add(object value)
		{
			return Add(value);
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.IList" />.
		/// </summary>
		void IList.Clear()
		{
			EnsureList().Clear();
			json = null;
		}

		/// <summary>
		/// Determines whether [contains] [the specified value].
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		bool IList.Contains(object value)
		{
			return EnsureList().Contains(value);
		}

		/// <summary>
		/// Indexes the of.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		int IList.IndexOf(object value)
		{
			return EnsureList().IndexOf(value);
		}

		/// <summary>
		/// Inserts the specified index.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="value">The value.</param>
		void IList.Insert(int index, object value)
		{
			EnsureList().Insert(index, value);
			json = null;
		}

		/// <summary>
		/// Removes the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		void IList.Remove(object value)
		{
			EnsureList().Remove(value);
			json = null;
		}

		/// <summary>
		/// Removes at.
		/// </summary>
		/// <param name="index">The index.</param>
		void IList.RemoveAt(int index)
		{
			EnsureList().RemoveAt(index);
			json = null;
		}
		#endregion


		#region IOrderedDictionary Methods
		/// <summary>
		/// Gets the enumerator.
		/// </summary>
		/// <returns></returns>
		IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
		{
			EnsureDictionary();

			return new OrderedDictionaryEnumerator(
				object_list.GetEnumerator());
		}

		/// <summary>
		/// Inserts the specified index.
		/// </summary>
		/// <param name="idx">The index.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		void IOrderedDictionary.Insert(int idx, object key, object value)
		{
			string property = (string)key;
			JsonData data = ToJsonData(value);

			this[property] = data;

			KeyValuePair<string, JsonData> entry =
				new KeyValuePair<string, JsonData>(property, data);

			object_list.Insert(idx, entry);
		}

		/// <summary>
		/// Removes at.
		/// </summary>
		/// <param name="idx">The index.</param>
		void IOrderedDictionary.RemoveAt(int idx)
		{
			EnsureDictionary();

			inst_object.Remove(object_list[idx].Key);
			object_list.RemoveAt(idx);
		}
		#endregion


		#region Private Methods
		/// <summary>
		/// Ensures the collection.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">The JsonData instance has to be initialized first</exception>
		private ICollection EnsureCollection()
		{
			if (type == JsonType.Array)
				return (ICollection)inst_array;

			if (type == JsonType.Object)
				return (ICollection)inst_object;

			throw new InvalidOperationException(
				"The JsonData instance has to be initialized first");
		}

		/// <summary>
		/// Ensures the dictionary.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Instance of JsonData is not a dictionary</exception>
		private IDictionary EnsureDictionary()
		{
			if (type == JsonType.Object)
				return inst_object as IDictionary;

			if (type != JsonType.None)
				throw new InvalidOperationException(
					"Instance of JsonData is not a dictionary");

			type = JsonType.Object;
			inst_object = new Dictionary<string, JsonData>();
			object_list = new List<KeyValuePair<string, JsonData>>();

			return inst_object as IDictionary;
		}

		/// <summary>
		/// Ensures the list.
		/// </summary>
		/// <returns></returns>
		/// <exception cref="InvalidOperationException">Instance of JsonData is not a list</exception>
		private IList EnsureList()
		{
			if (type == JsonType.Array)
				return (IList)inst_array;

			if (type != JsonType.None)
				throw new InvalidOperationException(
					"Instance of JsonData is not a list");

			type = JsonType.Array;
			inst_array = new List<JsonData>();

			return (IList)inst_array;
		}

		/// <summary>
		/// To the json data.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <returns></returns>
		private JsonData ToJsonData(object obj)
		{
			if (obj == null)
				return null;

			if (obj is JsonData)
				return (JsonData)obj;

			return new JsonData(obj);
		}

		/// <summary>
		/// Writes the json.
		/// </summary>
		/// <param name="obj">The object.</param>
		/// <param name="writer">The writer.</param>
		private static void WriteJson(IJsonWrapper obj, JsonWriter writer)
		{
			if (obj == null)
			{
				writer.Write(null);
				return;
			}

			if (obj.IsString)
			{
				writer.Write(obj.GetString());
				return;
			}

			if (obj.IsBoolean)
			{
				writer.Write(obj.GetBoolean());
				return;
			}
            

			if (obj.IsDouble)
			{
				writer.Write(obj.GetDouble());
				return;
			}

            if (obj.IsDecimal)
            {
                writer.Write(obj.GetDecimal());
                return;
            }

			if (obj.IsInt)
			{
				writer.Write(obj.GetInt());
				return;
			}

			if (obj.IsLong)
			{
				writer.Write(obj.GetLong());
				return;
			}

			if (obj.IsArray)
			{
				writer.WriteArrayStart();
				foreach (object elem in (IList)obj)
					WriteJson((JsonData)elem, writer);
				writer.WriteArrayEnd();

				return;
			}

			if (obj.IsObject)
			{
				writer.WriteObjectStart();

				foreach (DictionaryEntry entry in ((IDictionary)obj))
				{
					writer.WritePropertyName((string)entry.Key);
					WriteJson((JsonData)entry.Value, writer);
				}
				writer.WriteObjectEnd();

				return;
			}
		}
		#endregion


		/// <summary>
		/// Adds the specified value.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		public int Add(object value)
		{
			JsonData data = ToJsonData(value);

			json = null;

			return EnsureList().Add(data);
		}

		/// <summary>
		/// Removes all items from the <see cref="T:System.Collections.IList" />.
		/// </summary>
		public void Clear()
		{
			if (IsObject)
			{
                IDictionary dictionary = this as IDictionary;
                if (dictionary != null)
                {
                    dictionary.Clear();
                }
				return;
			}

			if (IsArray)
			{
				((IList)this).Clear();
				return;
			}
		}

		/// <summary>
		/// Equalses the specified x.
		/// </summary>
		/// <param name="x">The x.</param>
		/// <returns></returns>
		public bool Equals(JsonData x)
		{
			if (x == null)
				return false;

			if (x.type != this.type)
				return false;

			switch (this.type)
			{
				case JsonType.None:
					return true;

				case JsonType.Object:
					return this.inst_object.Equals(x.inst_object);

				case JsonType.Array:
					return this.inst_array.Equals(x.inst_array);

				case JsonType.String:
					return this.inst_string.Equals(x.inst_string);

				case JsonType.Int:
					return this.inst_int.Equals(x.inst_int);

				case JsonType.Long:
					return this.inst_long.Equals(x.inst_long);

				case JsonType.Double:
					return this.inst_double.Equals(x.inst_double);

				case JsonType.Boolean:
					return this.inst_boolean.Equals(x.inst_boolean);
			}

			return false;
		}

		/// <summary>
		/// Gets the type of the json.
		/// </summary>
		/// <returns></returns>
		public JsonType GetJsonType()
		{
			return type;
		}

		/// <summary>
		/// Sets the type of the json.
		/// </summary>
		/// <param name="type">The type.</param>
		public void SetJsonType(JsonType type)
		{
			if (this.type == type)
				return;

			switch (type)
			{
				case JsonType.None:
					break;

				case JsonType.Object:
					inst_object = new Dictionary<string, JsonData>();
					object_list = new List<KeyValuePair<string, JsonData>>();
					break;

				case JsonType.Array:
					inst_array = new List<JsonData>();
					break;

				case JsonType.String:
					inst_string = default(String);
					break;

				case JsonType.Int:
					inst_int = default(Int32);
					break;

				case JsonType.Long:
					inst_long = default(Int64);
					break;

				case JsonType.Double:
					inst_double = default(Double);
					break;

				case JsonType.Boolean:
					inst_boolean = default(Boolean);
					break;
			}

			this.type = type;
		}

		/// <summary>
		/// To the json.
		/// </summary>
		/// <returns></returns>
		public string ToJson()
		{
			if (json != null)
				return json;

			StringWriter sw = new StringWriter();
			JsonWriter writer = new JsonWriter(sw);
			writer.Validate = false;

			WriteJson(this, writer);
			json = sw.ToString();

			return json;
		}

		/// <summary>
		/// To the json.
		/// </summary>
		/// <param name="writer">The writer.</param>
		public void ToJson(JsonWriter writer)
		{
			bool old_validate = writer.Validate;

			writer.Validate = false;

			WriteJson(this, writer);

			writer.Validate = old_validate;
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public override string ToString()
		{
			switch (type)
			{
				case JsonType.Array:
					return "JsonData array";

				case JsonType.Boolean:
					return inst_boolean.ToString();

				case JsonType.Double:
					return inst_double.ToString();

				case JsonType.Int:
					return inst_int.ToString();

				case JsonType.Long:
					return inst_long.ToString();

				case JsonType.Object:
					return "JsonData object";

				case JsonType.String:
					return inst_string;
			}

			return "Uninitialized JsonData";
		}
	}


	/// <summary>
	/// FullName: <see cref="ECF.Json.OrderedDictionaryEnumerator"/>
	/// Summary : Ordered dictionary enumerator
	/// Version: 2.1
	/// DateTime: 2015/5/14 
	/// CopyRight (c) Shaipe
	/// </summary>
	internal class OrderedDictionaryEnumerator : IDictionaryEnumerator
	{
		/// <summary>
		/// The list_enumerator
		/// </summary>
		IEnumerator<KeyValuePair<string, JsonData>> list_enumerator;


		/// <summary>
		/// Gets the current element in the collection.
		/// </summary>
		public object Current
		{
			get { return Entry; }
		}

		/// <summary>
		/// Gets both the key and the value of the current dictionary entry.
		/// </summary>
		public DictionaryEntry Entry
		{
			get
			{
				KeyValuePair<string, JsonData> curr = list_enumerator.Current;
				return new DictionaryEntry(curr.Key, curr.Value);
			}
		}

		/// <summary>
		/// Gets the key of the current dictionary entry.
		/// </summary>
		public object Key
		{
			get { return list_enumerator.Current.Key; }
		}

		/// <summary>
		/// Gets the value of the current dictionary entry.
		/// </summary>
		public object Value
		{
			get { return list_enumerator.Current.Value; }
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="OrderedDictionaryEnumerator"/> class.
		/// </summary>
		/// <param name="enumerator">The enumerator.</param>
		public OrderedDictionaryEnumerator(
			IEnumerator<KeyValuePair<string, JsonData>> enumerator)
		{
			list_enumerator = enumerator;
		}


		/// <summary>
		/// Advances the enumerator to the next element of the collection.
		/// </summary>
		/// <returns>
		/// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
		/// </returns>
		public bool MoveNext()
		{
			return list_enumerator.MoveNext();
		}

		/// <summary>
		/// Sets the enumerator to its initial position, which is before the first element in the collection.
		/// </summary>
		public void Reset()
		{
			list_enumerator.Reset();
		}
	}
}
