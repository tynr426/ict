
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Reflection;

namespace ECF
{
    /// <summary>
    /// FullName： <see cref="ECF.ObjectExtend"/>
    /// Summary ： 对Object对象进行扩展
    /// Version： 1.0.0.0 
    /// DateTime： 2013/4/16 12:13
    /// Author  ： XP-WIN7
    /// </summary>
    public static class ObjectExtend
    {
        /// <summary>
        /// 将object转为int.
        /// </summary>
        /// <param name="self">待转对象.</param>
        /// <returns></returns>
        public static object ToInt(this object self)
        {
            return 0;
        }

        #region DataTable 转换为Json 字符串
        /// <summary>
        /// DataTable 对象 转换为Json 字符串
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToJson(this DataTable dt)
        {
            return ECF.Data.DBHelper.TableToJson(dt);
        }
        #endregion

        #region 转换为string字符串类型
        /// <summary>
        ///  转换为string字符串类型
        /// </summary>
        /// <param name="s">获取需要转换的值</param>
        /// <param name="format">需要格式化的位数</param>
        /// <returns>返回一个新的字符串</returns>
        public static string ToStr(this object s, string format = "")
        {
            string result = "";
            try
            {
                if (format == "")
                {
                    result = s.ToString();
                }
                else
                {
                    result = string.Format("{0:" + format + "}", s);
                }
            }
            catch
            {
            }
            return result;
        }
        #endregion

        #region Json 字符串 转换为 DataTable数据集合
        /// <summary>
        /// Json 字符串 转换为 DataTable数据集合
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static DataTable ToDataTable(this string json)
        {
            DataTable dataTable = new DataTable();  //实例化
            DataTable result;
            try
            {
                ArrayList arrayList = JsonConvert.DeserializeObject<ArrayList>(json);
                if (arrayList.Count > 0)
                {
                    foreach (Dictionary<string, object> dictionary in arrayList)
                    {
                        if (dictionary.Keys.Count == 0)
                        {
                            result = dataTable;
                            return result;
                        }
                        if (dataTable.Columns.Count == 0)
                        {
                            foreach (string current in dictionary.Keys)
                            {
                                dataTable.Columns.Add(current, dictionary[current].GetType());
                            }
                        }
                        DataRow dataRow = dataTable.NewRow();
                        foreach (string current in dictionary.Keys)
                        {
                            dataRow[current] = dictionary[current];
                        }

                        dataTable.Rows.Add(dataRow); //循环添加行到DataTable中
                    }
                }
            }
            catch
            {
            }
            result = dataTable;
            return result;
        }
        #endregion


        static bool ContainColunmn(string ColumnName, DataTable DataSource)
        {
            foreach (DataColumn dataColumn in DataSource.Columns)
            {
                if (String.Compare(dataColumn.ColumnName, ColumnName, StringComparison.OrdinalIgnoreCase) == 0)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// 获取数据容器内的属性
        /// </summary>
        /// <param name="DataContainer">数据容器</param>
        /// <param name="PropertyName">属性名称</param>
        public static object GetPropertyValue(this object DataContainer, string PropertyName)
        {
            try
            {
                if (DataContainer == null)
                    return null; //  throw new ArgumentNullException("参数\"DataContainer\"不能为空");

                if (String.IsNullOrWhiteSpace(PropertyName))
                    return null; // throw new ArgumentNullException("参数\"DataContainer\"不能为空");

                if (DataContainer is System.Data.DataRowView)
                {
                    DataRowView dr = (DataRowView)DataContainer;

                    if (ContainColunmn(PropertyName, dr.Row.Table))
                        return dr[PropertyName] == DBNull.Value ? null : dr[PropertyName];

                    // throw new NullReferenceException("无法在DataRowView数据源中找到与参数\"" + PropertyName + "\"匹配的属性");
                }
                else if (DataContainer is Dictionary<string, object>)
                {
                    Dictionary<string, object> dict = (Dictionary<string, object>)DataContainer;

                    if (dict.ContainsKey(PropertyName))
                        return dict[PropertyName];

                    // throw new NullReferenceException("无法在Dictionary<string, object>数据源中找到与参数\"" + PropertyName + "\"匹配的属性");
                }
                else if (DataContainer is KeyValuePair<string, object>)
                {
                    KeyValuePair<string, object> pair = (KeyValuePair<string, object>)DataContainer;

                    if (String.Compare("0", PropertyName) == 0)
                        return pair.Key;

                    if (String.Compare("1", PropertyName) == 0)
                        return pair.Value;

                    if (String.Compare(pair.Key, PropertyName) == 0)
                        return pair.Value;

                   // throw new NullReferenceException("无法在KeyValuePair<string, object>数据源中找到与参数\"" + PropertyName + "\"匹配的属性");
                }
                else if (DataContainer is NameValueCollection)
                {
                    NameValueCollection data = (NameValueCollection)DataContainer;

                    foreach (string key in data.AllKeys)
                    {
                        if (String.Compare(key, PropertyName, StringComparison.OrdinalIgnoreCase) == 0)
                            return data[key];
                    }

                   // throw new NullReferenceException("无法在NameValueCollection数据源中找到与参数\"" + PropertyName + "\"匹配的属性");
                }
                else if (DataContainer is System.Data.DataRow)
                {
                    DataRow dr = (DataRow)DataContainer;

                    if (ContainColunmn(PropertyName, dr.Table))
                        return dr[PropertyName] == DBNull.Value ? null : dr[PropertyName];

                    // throw new NullReferenceException("无法在DataRow数据源中找到与参数\"" + PropertyName + "\"匹配的属性");
                }
                else if (DataContainer is System.Data.DataTable)
                {
                    DataTable dt = (DataTable)DataContainer;

                    if (ContainColunmn(PropertyName, dt))
                    {
                        if (dt.Rows.Count == 0 || dt.Rows.Count > 1)
                        {
                            return null; // throw new FormatException("DataTable类型数据源有且只能有一行数据");
                        }
                        else
                        {
                            return dt.Rows[0][PropertyName] == DBNull.Value ? null : dt.Rows[0][PropertyName];
                        }
                    }

                    // throw new NullReferenceException("无法在DataTable数据源中找到与参数\"" + PropertyName + "\"匹配的属性");
                }

                Type baseType = DataContainer.GetType();

                if (String.Compare(PropertyName, "0") == 0 && (baseType.IsPrimitive || String.Compare(baseType.FullName, typeof(string).FullName, StringComparison.OrdinalIgnoreCase) == 0))
                    return DataContainer.ToString();

                PropertyInfo pi = baseType.GetProperty(PropertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                if (pi == null)
                    return null; // throw new ArgumentException("参数\"" + PropertyName + "\"无效, 数据源中找不到与之对应的属性");

                return pi.GetValue(DataContainer, null);
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Get string
        /// </summary>
        /// <param name="DataContainer">The data container.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string GetString(this object DataContainer, string propertyName)
        {
            object o = GetPropertyValue(DataContainer, propertyName);
            if (o != null)
            {
                return o.ToString();
            }
            else
                return string.Empty;
        }

        /// <summary>
        /// Get int
        /// </summary>
        /// <param name="DataContainer">The data container.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        public static int GetInt(this object DataContainer, string propertyName)
        {
            object o = GetPropertyValue(DataContainer, propertyName);
            if (o != null)
            {
                return Utils.ToInt(o, 0);
            }
            else
                return 0;
        }
    }
}
