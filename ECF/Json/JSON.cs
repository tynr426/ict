using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Reflection;
using System.Collections;
using Newtonsoft.Json;

namespace ECF.Json
{
    /// <summary>
    /// FullName： <see cref="ECF.Json.JSON"/>
    /// Summary ： Json处理类 
    /// Version： 1.0.0.0 
    /// DateTime： 2014/5/7 14:45  修改
    /// Author:    XP-PC
    /// </summary>
    public class JSON
    {
        #region T→Json 交换对象序列化为Json字符串
        /// <summary>
        /// 交换对象序列化为Json字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t">The t.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string Serialize<T>(T t)
        {
            if (t.GetType() == typeof(string))
            {
                string str = t as string;

                if (string.IsNullOrEmpty(str)) return "{}";

                return "\"" + str.Replace("\"", "\\\"").Replace("\r\n", "\\r\\n") + "\"";
            }

            using (MemoryStream ms = new MemoryStream())
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                ser.WriteObject(ms, t);

                string jsonString = Encoding.UTF8.GetString(ms.ToArray());
                string p = @"\\/Date\((\d+)\+\d+\)\\/";
                MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertJsonDateToDateString);
                Regex reg = new Regex(p);
                jsonString = reg.Replace(jsonString, matchEvaluator);
                //如果是数组

                if (jsonString.StartsWith("[") && jsonString.EndsWith("]"))
                {
                    string name = t.GetType().Name;
                    if (t is System.Collections.ArrayList)
                    {
                        string[] arr = name.Split(new string[] { "[]" }, StringSplitOptions.None);
                        if (arr.Length > 0) name = arr[0] + "s";
                    }
                    else if (t is System.Collections.IList)
                    {
                        System.Collections.IList list = (System.Collections.IList)t;
                        name = list[0].GetType().Name + "s";
                    }
                    jsonString = "{\"" + name + "\":" + jsonString + "}";
                }
                return jsonString;
            }
        }
        #endregion

        #region Json→T 反序列化指定对象
        /// <summary>
        /// Json反序列化指定对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonString">The json string.</param>
        /// <returns>
        /// ``0
        /// </returns>
        public static T Deserialize<T>(string jsonString)
        {
            //将"yyyy-MM-dd HH:mm:ss"格式的字符串转为"\/Date(1294499956278+0800)\/"格式
            string p = @"\d{4}-\d{2}-\d{2}\s\d{2}:\d{2}:\d{2}";
            MatchEvaluator matchEvaluator = new MatchEvaluator(ConvertDateStringToJsonDate);
            Regex reg = new Regex(p);
            jsonString = reg.Replace(jsonString, matchEvaluator);
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
                T obj = (T)ser.ReadObject(ms);
                return obj;
            }
        }
        #endregion

        #region Serialize Json对象序列化
        /// <summary>
        /// Json对象序列化
        /// </summary>
        /// <param name="obj">The object.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public string Serialize(object obj)
        {
            if (obj == null)
            {
                return "{}";
            }

            // Get the type of obj.
            Type t = obj.GetType();

            if (t.GetType() == typeof(string))
            {
                string str = obj as string;

                if(string.IsNullOrEmpty(str)) return "{}";

                return "\"" + str.Replace("\"", "\\\"").Replace("\r\n", "\\r\\n") + "\"";
            }
            // Just deal with the public instance properties. others ignored.
            BindingFlags bf = BindingFlags.Instance | BindingFlags.Public;

            PropertyInfo[] pis = t.GetProperties(bf);

            StringBuilder json = new StringBuilder("{");

            if (pis != null && pis.Length > 0)
            {
                int i = 0;
                int lastIndex = pis.Length - 1;

                foreach (PropertyInfo p in pis)
                {
                    // Simple string
                    if (p.PropertyType.Equals(typeof(string)))
                    {
                        json.AppendFormat("\"{0}\":\"{1}\"", p.Name, p.GetValue(obj, null));
                    }
                    // Number,boolean.
                    else if (p.PropertyType.Equals(typeof(int)) ||
                        p.PropertyType.Equals(typeof(bool)) ||
                        p.PropertyType.Equals(typeof(double)) ||
                        p.PropertyType.Equals(typeof(decimal))
                        )
                    {
                        json.AppendFormat("\"{0}\":{1}", p.Name, p.GetValue(obj, null).ToString().ToLower());
                    }
                    // Array.
                    else if (isArrayType(p.PropertyType))
                    {
                        // Array case.
                        object o = p.GetValue(obj, null);

                        if (o == null)
                        {
                            json.AppendFormat("\"{0}\":{1}", p.Name, "null");
                        }
                        else
                        {
                            json.AppendFormat("\"{0}\":{1}", p.Name, getArrayValue((Array)p.GetValue(obj, null)));
                        }
                    }
                    // Class type. custom class, list collections and so forth.
                    else if (isCustomClassType(p.PropertyType))
                    {
                        object v = p.GetValue(obj, null);
                        if (v is IList)
                        {
                            IList il = v as IList;
                            string subJsString = getIListValue(il);

                            json.AppendFormat("\"{0}\":{1}", p.Name, subJsString);
                        }
                        else
                        {
                            // Normal class type.
                            string subJsString = Serialize(p.GetValue(obj, null));

                            json.AppendFormat("\"{0}\":{1}", p.Name, subJsString);
                        }
                    }
                    // Datetime
                    else if (p.PropertyType.Equals(typeof(DateTime)))
                    {
                        DateTime dt = (DateTime)p.GetValue(obj, null);

                        if (dt == default(DateTime))
                        {
                            json.AppendFormat("\"{0}\":\"\"", p.Name);
                        }
                        else
                        {
                            json.AppendFormat("\"{0}\":\"{1}\"", p.Name, ((DateTime)p.GetValue(obj, null)).ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                    }
                    else
                    {
                        // TODO: extend.
                    }

                    if (i >= 0 && i != lastIndex)
                    {
                        json.Append(",");
                    }

                    ++i;
                }
            }

            json.Append("}");

            return json.ToString();
        }

        #region private
        /// <summary>
        /// Gets the array value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        string getArrayValue(Array obj)
        {
            if (obj != null)
            {
                if (obj.Length == 0)
                {
                    return "[]";
                }

                object firstElement = obj.GetValue(0);
                Type et = firstElement.GetType();
                bool quotable = et == typeof(string);

                StringBuilder sb = new StringBuilder("[");
                int index = 0;
                int lastIndex = obj.Length - 1;

                if (quotable)
                {
                    foreach (var item in obj)
                    {
                        sb.AppendFormat("\"{0}\"", item.ToString());

                        if (index >= 0 && index != lastIndex)
                        {
                            sb.Append(",");
                        }

                        ++index;
                    }
                }
                else
                {
                    foreach (var item in obj)
                    {
                        sb.Append(item.ToString());

                        if (index >= 0 && index != lastIndex)
                        {
                            sb.Append(",");
                        }

                        ++index;
                    }
                }

                sb.Append("]");

                return sb.ToString();
            }

            return "null";
        }


        /// <summary>
        /// Gets the I list value.
        /// </summary>
        /// <param name="obj">The obj.</param>
        /// <returns></returns>
        string getIListValue(IList obj)
        {
            if (obj != null)
            {
                if (obj.Count == 0)
                {
                    return "[]";
                }

                object firstElement = obj[0];
                Type et = firstElement.GetType();
                bool quotable = et == typeof(string);

                StringBuilder sb = new StringBuilder("[");
                int index = 0;
                int lastIndex = obj.Count - 1;

                if (quotable)
                {
                    foreach (var item in obj)
                    {
                        sb.AppendFormat("\"{0}\"", item.ToString());

                        if (index >= 0 && index != lastIndex)
                        {
                            sb.Append(",");
                        }

                        ++index;
                    }
                }
                else
                {
                    foreach (var item in obj)
                    {
                        sb.Append(item.ToString());

                        if (index >= 0 && index != lastIndex)
                        {
                            sb.Append(",");
                        }

                        ++index;
                    }
                }

                sb.Append("]");

                return sb.ToString();
            }

            return "null";
        }


        /// <summary>
        /// Determines whether [is array type] [the specified t].
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        ///   <c>true</c> if [is array type] [the specified t]; otherwise, <c>false</c>.
        /// </returns>
        bool isArrayType(Type t)
        {
            if (t != null)
            {
                return t.IsArray;
            }

            return false;
        }


        /// <summary>
        /// Determines whether [is custom class type] [the specified t].
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns>
        ///   <c>true</c> if [is custom class type] [the specified t]; otherwise, <c>false</c>.
        /// </returns>
        bool isCustomClassType(Type t)
        {
            if (t != null)
            {
                return t.IsClass && t != typeof(string);
            }

            return false;
        }
        #endregion
        #endregion

        #region Json2Dictionary 转换为字典对象

        /// <summary>
        /// Json转换为Object
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static object JsonTObject(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json)) return null;
                return JsonConvert.DeserializeObject(json);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Json转换为字典对象
        /// </summary>
        /// <param name="json">Json字符串.</param>
        /// <returns>
        /// Dictionary&lt;System.String, System.Object&gt;
        /// </returns>
        public static Dictionary<string, object> Json2Dictionary(string json)
        {
            Dictionary<string, object> dic = null;
            if (!string.IsNullOrEmpty(json))
            {
                try
                {
                    dic = (Dictionary<string, object>)JsonConvert.DeserializeObject(json);
                }
                catch
                {
                    // 开始分析参数对    
                    Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
                    MatchCollection mc = re.Matches(json);
                    dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
                    foreach (Match m in mc)
                    {
                        dic.Add(m.Result("$2").ToLower(), m.Result("$3"));
                    }
                }
            }

            // 让输出的字典在判断和使用时不区分大小写
            return new Dictionary<string, object>(dic, StringComparer.OrdinalIgnoreCase);
        }
        #endregion

        #region Json2Xml json字符串转换为Xml对象
        /// <summary>
        /// json字符串转换为Xml对象
        /// </summary>
        /// <param name="sJson">The s json.</param>
        /// <returns></returns>
        public static XmlDocument Json2Xml(string sJson)
        {

            try
            {
                Dictionary<string, object> Dic = (Dictionary<string, object>)JsonConvert.DeserializeObject(sJson);
                XmlDocument doc = new XmlDocument();
                XmlDeclaration xmlDec;
                xmlDec = doc.CreateXmlDeclaration("1.0", "utf-8", "yes");
                doc.InsertBefore(xmlDec, doc.DocumentElement);
                XmlElement nRoot = doc.CreateElement("root");
                doc.AppendChild(nRoot);
                foreach (KeyValuePair<string, object> item in Dic)
                {
                    XmlElement element = doc.CreateElement(item.Key);
                    KeyValue2Xml(element, item);
                    nRoot.AppendChild(element);
                }
                return doc;
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
                return new XmlDocument();
            }
        }

        /// <summary>
        /// Keys the value2 XML.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="Source">The source.</param>
        private static void KeyValue2Xml(XmlElement node, KeyValuePair<string, object> Source)
        {
            object kValue = Source.Value;
            if (kValue.GetType() == typeof(Dictionary<string, object>))
            {
                foreach (KeyValuePair<string, object> item in kValue as Dictionary<string, object>)
                {
                    XmlElement element = node.OwnerDocument.CreateElement(item.Key);
                    KeyValue2Xml(element, item);
                    node.AppendChild(element);
                }
            }
            else if (kValue.GetType() == typeof(object[]))
            {
                object[] o = kValue as object[];
                for (int i = 0; i < o.Length; i++)
                {
                    XmlElement xitem = node.OwnerDocument.CreateElement("Item");
                    KeyValuePair<string, object> item = new KeyValuePair<string, object>("Item", o);
                    KeyValue2Xml(xitem, item);
                    node.AppendChild(xitem);
                }

            }
            else
            {
                XmlText text = node.OwnerDocument.CreateTextNode(kValue.ToString());
                node.AppendChild(text);
            }
        }
        #endregion

        #region Private ConvertDateTime 时间转换
        /// <summary>
        /// Converts the json date to date string.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }

        /// <summary>
        /// Converts the date string to json date.
        /// </summary>
        /// <param name="m">The m.</param>
        /// <returns></returns>
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }
        #endregion
    }
}
