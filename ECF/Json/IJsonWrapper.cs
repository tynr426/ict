using System.Collections;
using System.Collections.Specialized;


namespace ECF.Json
{
	/// <summary>
	/// Json类型
	/// </summary>
	public enum JsonType
	{
		/// <summary>
		/// 无
		/// </summary>
		None,
		/// <summary>
		/// 对象
		/// </summary>
		Object,
		/// <summary>
		/// 数组
		/// </summary>
		Array,
		/// <summary>
		/// 字符
		/// </summary>
		String,
		/// <summary>
		/// 整型
		/// </summary>
		Int,
		/// <summary>
		/// 长整型
		/// </summary>
		Long,
		/// <summary>
		/// 双精度数据
		/// </summary>
		Double,
		/// <summary>
		/// Boolen
		/// </summary>
		Boolean,
        /// <summary>
        /// Decimal
        /// </summary>
        Decimal
    }

	/// <summary>
	/// Json的最外层包装接口
	/// IJsonWrapper.cs
	///Interface that represents a type capable of handling all kinds of JSON
	/// data.This is mainly used when mapping objects through JsonMapper, and
	///it's implemented by JsonData.
	///The authors disclaim copyright to this source code. For more details, see
	///the COPYING file included with this distribution.
	/// </summary>
	public interface IJsonWrapper : IList, IOrderedDictionary
	{
		/// <summary>
		/// 判断是否为数据.
		/// </summary>
		bool IsArray { get; }

		/// <summary>
		/// 是否为Boolen型.
		/// </summary>
		bool IsBoolean { get; }

		/// <summary>
		/// 是否为双精度数据.
		/// </summary>
		bool IsDouble { get; }

        /// <summary>
        /// 是否为Decimal
        /// </summary>
        bool IsDecimal { get; }

		/// <summary>
		/// 是否为整型.
		/// </summary>
		bool IsInt { get; }
		/// <summary>
		/// Gets a value indicating whether this instance is long.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is long; otherwise, <c>false</c>.
		/// </value>
		bool IsLong { get; }
		/// <summary>
		/// Gets a value indicating whether this instance is object.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is object; otherwise, <c>false</c>.
		/// </value>
		bool IsObject { get; }
		/// <summary>
		/// Gets a value indicating whether this instance is string.
		/// </summary>
		/// <value>
		///   <c>true</c> if this instance is string; otherwise, <c>false</c>.
		/// </value>
		bool IsString { get; }

		/// <summary>
		/// Gets the boolean.
		/// </summary>
		/// <returns></returns>
		bool GetBoolean();
		/// <summary>
		/// Gets the double.
		/// </summary>
		/// <returns></returns>
		double GetDouble();

        /// <summary>
        /// Get Decimal
        /// </summary>
        /// <returns></returns>
        decimal GetDecimal();

		/// <summary>
		/// Gets the int.
		/// </summary>
		/// <returns></returns>
		int GetInt();
		/// <summary>
		/// Gets the type of the json.
		/// </summary>
		/// <returns></returns>
		JsonType GetJsonType();
		/// <summary>
		/// Gets the long.
		/// </summary>
		/// <returns></returns>
		long GetLong();
		/// <summary>
		/// Gets the string.
		/// </summary>
		/// <returns></returns>
		string GetString();

		/// <summary>
		/// Sets the boolean.
		/// </summary>
		/// <param name="val">if set to <c>true</c> [value].</param>
		void SetBoolean(bool val);
		/// <summary>
		/// Sets the double.
		/// </summary>
		/// <param name="val">The value.</param>
		void SetDouble(double val);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        void SetDecimal(decimal val);
		/// <summary>
		/// Sets the int.
		/// </summary>
		/// <param name="val">The value.</param>
		void SetInt(int val);
		/// <summary>
		/// Sets the type of the json.
		/// </summary>
		/// <param name="type">The type.</param>
		void SetJsonType(JsonType type);
		/// <summary>
		/// Sets the long.
		/// </summary>
		/// <param name="val">The value.</param>
		void SetLong(long val);
		/// <summary>
		/// Sets the string.
		/// </summary>
		/// <param name="val">The value.</param>
		void SetString(string val);

		/// <summary>
		/// To the json.
		/// </summary>
		/// <returns></returns>
		string ToJson();
		/// <summary>
		/// To the json.
		/// </summary>
		/// <param name="writer">The writer.</param>
		void ToJson(JsonWriter writer);
	}
}
