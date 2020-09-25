using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace ECF.Json
{
    /// <summary>
    ///   <see cref="ECF.Json.JsonUtils"/>
    /// json 处理单元
    /// Author:  XP
    /// Created: 2011/9/20
    /// </summary>
    public sealed class JsonUtils
    {
        /// <summary>
        ///  Converts the json.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  02-20-2017
        /// </summary>
        /// <param name="dic">The dic.</param>
        /// <param name="unicode">if set to <c>true</c> [unicode].</param>
        /// <returns>System.String.</returns>
        public static string ConvertJson(IDictionary dic, bool unicode)
        {
            List<string> list = new List<string>();

            foreach (string key in dic.Keys)
            {
                if (dic[key] != null)
                {
                    list.Add("\"" + key + "\":" + Json.JsonUtils.ConvertJson(dic[key], unicode));
                }

            }
            return "{" + string.Join(",", list) + "}";
        }

        /// <summary>
        ///  对可循环的进行再次处理.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  10-17-2014
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="unicode">是否需要对值进行Unicode编码 .</param>
        public static string ConvertJson(IEnumerable enumerable, bool unicode)
        {
            List<string> items = new List<string>();
            string json = string.Empty;
            foreach (object o in enumerable)
            {
                if (o is String)
                {
                    json = Utils.JsonValueOfType(o, typeof(string), unicode);
                }
                else if (o is DataSet)
                {
                    List<string> tables = new List<string>();
                    foreach (DataTable dt in ((DataSet)o).Tables)
                    {
                        tables.Add("\"" + dt.TableName + "\":" + Data.DBHelper.TableToJson(dt, unicode));
                    }
                    json = "{" + string.Join(",", tables) + "}";

                }
                else if (o is DataTable)
                {
                    json = Data.DBHelper.TableToJson((DataTable)o, unicode);
                }
                else if (o is IEntity)
                {
                    json = ((IEntity)o).ToJson();
                }
                else if (o is IDictionary)
                {
                    IDictionary dic = o as IDictionary;
                    if (dic != null)
                    {
                        json = ConvertJson(dic, unicode);
                    }
                }
                else if (o is IEnumerable)
                {
                    json = ConvertJson((IEnumerable)o, unicode);
                }
                else
                {
                    json = Utils.JsonValueOfType(o, o.GetType(), unicode);
                }
                if (!string.IsNullOrEmpty(json))
                {
                    items.Add(json);
                }
            }

            return "[" + string.Join(",", items) + "]";
        }


        /// <summary>
        ///  将object对象转换为Json数据格式
        ///  Author :   XP-PC/Shaipe
        ///  Created:  06-04-2014
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="unicode">if set to <c>true</c> [unicode].</param>
        /// <returns>System.String.</returns>
        public static string ConvertJson(object o, bool unicode)
        {
            string json = string.Empty;

            if (o is String)
            {
                json = Utils.JsonValueOfType(o, typeof(string), unicode);
            }
            else if (o is DataSet)
            {
                List<string> tables = new List<string>();
                foreach (DataTable dt in ((DataSet)o).Tables)
                {
                    tables.Add("\"" + dt.TableName + "\":" + Data.DBHelper.TableToJson(dt, unicode));
                }
                json = "{" + string.Join(",", tables) + "}";

            }
            else if (o is DataTable)
            {
                json = Data.DBHelper.TableToJson((DataTable)o, unicode);
            }
            else if (o is IEntity)
            {
                json = ((IEntity)o).ToJson();
            }
            else if (o is IDictionary)
            {
                json = ConvertJson((IDictionary)o, unicode);
            }
            else if (o is IEnumerable)
            {
                json = ConvertJson((IEnumerable)o, unicode);
            }
            else
            {
                json = Utils.JsonValueOfType(o, o.GetType(), unicode);
            }

            return json;
        }

        /// <summary>
        /// 对数据进行Excape加密处理.
        /// </summary>
        /// <param name="o">需要转换的对象.</param>
        /// <returns>返回Json格式的数据</returns>
        public static string Escape(object o)
        {
            if (o == null)
                return "null";

            if (o is IDictionary)
            {
                return EscapeObject(o as IDictionary);
            }
            else if (o is ArrayList)
                return EscapeArray((ArrayList)o);

            else if (o is Array)
                return EscapeArray((object[])o);

            else if (o is String)
                return EscapeString((string)o);

            else if (o is Boolean)
                return EscapeBoolean((bool)o);

            try
            {
                return EscapeNumber(Convert.ToDouble(o));
            }
            catch (Exception) { }

            throw new ECFException("Unable to escape object to JSON notation.");
        }

        /// <summary>
        /// Escapes the object.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns>返回Json格式的数据</returns>
        private static string EscapeObject(IDictionary d)
        {
            StringBuilder s = new StringBuilder();
            bool first = true;

            s.Append("{");

            foreach (DictionaryEntry entry in d)
            {
                if (!first)
                    s.Append(",");

                s.Append(Escape(entry.Key.ToString()));
                s.Append(":");
                s.Append(Escape(entry.Value));

                first = false;
            }

            s.Append("}");

            return s.ToString();
        }

        /// <summary>
        /// Escapes the array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        private static string EscapeArray(ArrayList array)
        {
            return EscapeArray(array.ToArray());
        }

        /// <summary>
        /// Escapes the array.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <returns></returns>
        private static string EscapeArray(object[] array)
        {
            StringBuilder s = new StringBuilder();

            s.Append("[");

            for (int i = 0; i < array.Length; i++)
            {
                if (i > 0)
                    s.Append(",");

                s.Append(Escape(array[i]));
            }

            s.Append("]");

            return s.ToString();
        }


        /// <summary>
        /// Escapes the number.
        /// </summary>
        /// <param name="n">The n.</param>
        /// <returns></returns>
        private static string EscapeNumber(double n)
        {
            return n.ToString(CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Escapes the string.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <returns></returns>
        private static string EscapeString(string s)
        {
            s = s.Replace(@"\", @"\\");
            s = s.Replace("\"", "\\\"");
            s = s.Replace("\n", "\\n");
            s = s.Replace("\r", "\\r");

            return '"' + s + '"';
        }

        /// <summary>
        /// Escapes the boolean.
        /// </summary>
        /// <param name="b">if set to <c>true</c> [b].</param>
        /// <returns></returns>
        private static string EscapeBoolean(bool b)
        {
            return (b ? "true" : "false");
        }


        /// <summary>
        /// Parses the specified json.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static object Parse(string json)
        {
            if (json == "" || json == "null")
                return null;
            if (json[0] == '{')
                return ParseObject(json);
            else if (json[0] == '[')
                return ParseArray(json);
            else
                return ParseValue(json);
        }

        /// <summary>
        /// Parses the object.
        /// </summary>
        /// <param name="jsobject">The jsobject.</param>
        /// <returns></returns>
        public static Hashtable ParseObject(string jsobject)
        {
            Hashtable ht = new Hashtable();

            if (jsobject.Length < 2 || jsobject[0] != '{' || jsobject[jsobject.Length - 1] != '}')
                throw new Exception("Unable to parse object from JSON notation.");

            jsobject = jsobject.Substring(1, jsobject.Length - 2);

            while (jsobject != "")
            {
                int index = findDelimiterIndex(jsobject, ':');

                if (index == -1)
                    break;

                string name = jsobject.Substring(0, index);
                jsobject = jsobject.Substring(index + 1);

                index = findDelimiterIndex(jsobject, ',');

                string value = "";

                if (index == -1)
                {
                    value = jsobject;
                    jsobject = "";
                }
                else
                {
                    value = jsobject.Substring(0, index);
                    jsobject = jsobject.Substring(index + 1);
                }

                if (IsQuotedString(name))
                    name = name.Substring(1, name.Length - 2);

                ht[name] = Parse(value);
            }

            return ht;
        }

        /// <summary>
        /// Parses the array.
        /// </summary>
        /// <param name="jsarray">The jsarray.</param>
        /// <returns></returns>
        public static ArrayList ParseArray(string jsarray)
        {
            ArrayList arr = new ArrayList();

            if (jsarray.Length < 2 || jsarray[0] != '[' || jsarray[jsarray.Length - 1] != ']')
                throw new Exception("NOT AN ARRAY");

            jsarray = jsarray.Substring(1, jsarray.Length - 2);

            while (jsarray != "")
            {
                int index = findDelimiterIndex(jsarray, ',');
                string value = "";

                if (index == -1)
                {
                    value = jsarray;
                    jsarray = "";
                }
                else
                {
                    value = jsarray.Substring(0, index);
                    jsarray = jsarray.Substring(index + 1);
                }

                arr.Add(Parse(value));
            }

            return arr;
        }

        /// <summary>
        /// Parses the value.
        /// </summary>
        /// <param name="jsvalue">The jsvalue.</param>
        /// <returns></returns>
        public static object ParseValue(string jsvalue)
        {
            object o = null;

            if (!IsQuotedString(jsvalue))
            {
                try { return Double.Parse(jsvalue, CultureInfo.InvariantCulture); }
                catch (Exception) { }
            }
            else
            {
                jsvalue = jsvalue.Substring(1, jsvalue.Length - 2);
            }

            if (jsvalue == "true" || jsvalue == "false")
                o = (jsvalue == "true" ? true : false);
            else
                o = jsvalue;

            return o;
        }

        /// <summary>
        /// Determines whether [is quoted string] [the specified jsvalue].
        /// </summary>
        /// <param name="jsvalue">The jsvalue.</param>
        /// <returns>
        ///   <c>true</c> if [is quoted string] [the specified jsvalue]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsQuotedString(string jsvalue)
        {
            return ((jsvalue.Length >= 2)
                && ((jsvalue[0] == '"' && jsvalue[jsvalue.Length - 1] == '"')
                || (jsvalue[0] == '\'' && jsvalue[jsvalue.Length - 1] == '\'')));
        }


        /// <summary>
        /// Finds the index of the delimiter.
        /// </summary>
        /// <param name="s">The s.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns></returns>
        private static int findDelimiterIndex(string s, char delimiter)
        {
            int obj_count = 0;
            int arr_count = 0;
            bool in_stringS = false;
            bool in_stringD = false;
            bool in_escape = false;

            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];

                switch (c)
                {
                    case '\\':
                        if (in_stringS || in_stringD)
                            in_escape = !in_escape;
                        break;

                    case '\'':
                        if (!in_stringD)
                        {
                            if (!in_stringS)
                                in_stringS = true;
                            else if (!in_escape)
                                in_stringS = false;
                        }

                        in_escape = false;
                        break;

                    case '"':
                        if (!in_stringS)
                        {
                            if (!in_stringD)
                                in_stringD = true;
                            else if (!in_escape)
                                in_stringD = false;
                        }

                        in_escape = false;
                        break;

                    default:
                        in_escape = false;

                        if (!in_stringS && !in_stringD)
                        {
                            switch (c)
                            {
                                case '{':
                                    ++obj_count;
                                    break;

                                case '}':
                                    --obj_count;
                                    break;

                                case '[':
                                    ++arr_count;
                                    break;

                                case ']':
                                    --arr_count;
                                    break;

                                default:
                                    if (c == delimiter && obj_count == 0 && arr_count == 0)
                                        return i;
                                    break;
                            }
                        }
                        break;
                }
            }

            return -1;
        }



    }

}
