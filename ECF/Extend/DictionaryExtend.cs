using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace ECF
{
    /// <summary>
    /// Summary ： 对Dictionary进行扩展处理
    /// Version： 1.0
    /// DateTime： 2014/7/8
    /// CopyRight (c) shaipe
    /// </summary>
    public static class DictionaryExtend
    {
        /// <summary>
        /// 添加或获取字典值.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  03-09-2017
        /// </summary>
        /// <typeparam name="TKey">The type of the t key.</typeparam>
        /// <typeparam name="TValue">The type of the t value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="factory">The factory.</param>
        /// <param name="synchronization">The synchronization.</param>
        /// <returns>TValue.</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> factory, object synchronization)
        {
            var value = default(TValue);

            try
            {
                if (dictionary.TryGetValue(key, out value))
                {
                    return value;
                }

                lock (synchronization)
                {
                    if (dictionary.TryGetValue(key, out value))
                    {
                        return value;
                    }

                    value = factory(key);
                    dictionary.Add(key, value);
                    return value;
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return value;
        }

        /// <summary>
        /// 将Dictionary追加到目标Dictionary之后.
        /// </summary>
        /// <typeparam name="TKey">The type of the key.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="source">The source.</param>
        /// <returns></returns>
        public static Dictionary<TKey, TValue> AddRange<TKey, TValue>(this Dictionary<TKey, TValue> dictionary,
            Dictionary<TKey, TValue> source)
        {
            try
            {
                foreach (var item in source)
                {
                    if (!dictionary.ContainsKey(item.Key))
                    {
                        dictionary.Add(item.Key, item.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return dictionary;
        }

        /// <summary>
        ///  向字典中添加项，可以忽略重复.
        /// Author :   XP-PC/Shaipe
        /// Created:  07-09-2014
        /// </summary>
        /// <param name="sourceDictionary">源字典.</param>
        /// <param name="key">关键字.</param>
        /// <param name="value">值.</param>
        /// <returns>Dictionary&lt;System.String, System.Object&gt;.</returns>
        public static Dictionary<string, object> AddItem(this Dictionary<string, object> sourceDictionary, string key, object value)
        {
            try
            {
                return sourceDictionary.AddItem(key, value, true);
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return sourceDictionary;
        }

        /// <summary>
        ///  向字典中添加项，可以忽略重复.
        /// Author :   XP-PC/Shaipe
        /// Created:  07-09-2014
        /// </summary>
        /// <param name="sourceDictionary">源字典.</param>
        /// <param name="key">关键字.</param>
        /// <param name="value">值.</param>
        /// <param name="replace">是否替换重复</param>
        public static Dictionary<string, object> AddItem(this Dictionary<string, object> sourceDictionary, string key, object value, bool replace)
        {
            try
            {
                if (sourceDictionary.ContainsKey(key))
                {
                    sourceDictionary[key] = value;
                    if (replace)
                    {
                        sourceDictionary.Remove(key);
                        sourceDictionary.Add(key, value);
                    }
                }
                else
                {
                    sourceDictionary.Add(key, value);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return sourceDictionary;
        }

        /// <summary>
        /// 返回一个字符串.
        /// </summary>
        /// <param name="dictionary">源数据字典.</param>
        /// <param name="key">配置中的关键字.</param>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public static string ToValue(this Dictionary<string, object> dictionary, string key)
        {
            string result = string.Empty;

            try
            {
                //判断配置项是否存在
                if (dictionary != null && dictionary.Keys.Contains(key) && dictionary[key] != null)
                {
                    result = dictionary[key].ToString();
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return result;
        }

        /// <summary>
        /// 转换为值.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dictionary">配置.</param>
        /// <param name="key">配置项关键字.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static T ToValue<T>(this Dictionary<string, T> dictionary, string key, T defaultValue = default(T))
        {
            T result = defaultValue;

            try
            {
                //判断配置项是否存在
                if (dictionary != null && dictionary.Keys.Contains(key) && dictionary[key] != null)
                {
                    T v = dictionary[key];
                    return v;
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return result;
        }

        /// <summary>
        /// 转换为值.
        /// </summary>
        /// <param name="dictionary">配置.</param>
        /// <param name="key">配置项关键字.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string ToValue(this Dictionary<string, object> dictionary, string key, string defaultValue = "")
        {
            string result = defaultValue;

            try
            {
                //判断配置项是否存在
                if (dictionary != null && dictionary.Keys.Contains(key) && dictionary[key] != null)
                {
                    object v = dictionary[key];
                    if (v != null)
                        return v.ToString();
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return result;
        }


        /// <summary>
        /// 转换为值.
        /// </summary>
        /// <param name="dictionary">配置.</param>
        /// <param name="key">配置项关键字.</param>
        /// <returns></returns>
        public static T ToValue<T>(this Dictionary<string, object> dictionary, string key)
        {
            T result = default(T);

            try
            {
                //判断配置项是否存在
                if (dictionary != null && dictionary.Keys.Contains(key) && dictionary[key] != null)
                {
                    result = (T)dictionary[key];
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return result;
        }



        /// <summary>
        /// 直接转换为int型数据
        /// </summary>
        /// <param name="dic"> 字典对象.</param>
        /// <param name="key">关键字.</param>
        /// <param name="defaultValue">默认值.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        public static int ToInt(this Dictionary<string, object> dic, string key, int defaultValue)
        {
            try
            {
                if (dic != null && dic.ContainsKey(key) && dic[key] != null)
                {
                    return Utils.ToInt(dic[key]);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return defaultValue;
        }

        /// <summary>
        /// 直接转换为int型数据
        /// </summary>
        /// <param name="dic"> 字典对象.</param>
        /// <param name="key">关键字.</param>
        /// <param name="defaultValue">默认值.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        public static int ToInt<T>(this Dictionary<string, T> dic, string key, int defaultValue)
        {
            try
            {
                if (dic != null && dic.ContainsKey(key) && dic[key] != null)
                {
                    return Utils.ToInt(dic[key]);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return defaultValue;
        }

        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“ ＆ ”字符拼接成字符串，并对参数值做urlencode
        /// </summary>
        /// <param name="dicArray">需要拼接的数组</param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string ToLinkString(this Dictionary<string, object> dicArray)
        {
            return dicArray.ToLinkString(null);
        }
        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“ ＆ ”字符拼接成字符串，并对参数值做urlencode
        /// </summary>
        /// <param name="dicArray"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        public static string ToLinkString(this Dictionary<string, object> dicArray, Encoding encoder)
        {
            return dicArray.ToLinkString(encoder, false);
        }
        /// <summary>
        /// 把数组所有元素，按照“参数=参数值”的模式用“ ＆ ”字符拼接成字符串，并对参数值做urlencode
        /// </summary>
        /// <param name="dicArray">需要拼接的数组</param>
        /// <param name="encoder">字符编码</param>
        /// <param name="filterEmpty"></param>
        /// <returns>拼接完成以后的字符串</returns>
        public static string ToLinkString(this Dictionary<string, object> dicArray, Encoding encoder, bool filterEmpty)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (KeyValuePair<string, object> temp in dicArray)
                {
                    object val = temp.Value;
                    if ((!filterEmpty && val != null) || (filterEmpty && !string.IsNullOrWhiteSpace(Utils.ToString(val))))
                    {
                        if (encoder != null)
                        {

                            sb.Append(temp.Key + "=" + HttpUtility.UrlEncode(val.ToString(), encoder) + "&");
                        }
                        else
                        {
                            sb.Append(temp.Key + "=" + temp.Value + "&");
                        }
                    }
                }

                if (sb.ToString().EndsWith("&"))
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 将字符串型字典转换为url型字符
        /// </summary>
        /// <param name="dic">待处理字典.</param>
        /// <param name="encoding">编码方式.</param>
        /// <param name="filterEmpty">是否过滤空值.</param>
        /// <returns>String</returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加对字符串字典转换 added by xiepeng 2018/9/20</description></item>
        ///   </list>
        /// </remarks>
        public static string ToLinkString(this Dictionary<string, string> dic, bool filterEmpty = false, string encoding = "")
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (KeyValuePair<string, string> temp in dic)
                {
                    object val = temp.Value;
                    if ((!filterEmpty && val != null) || (filterEmpty && !string.IsNullOrWhiteSpace(Utils.ToString(val))))
                    {
                        if (!string.IsNullOrEmpty(encoding))
                        {
                            Encoding encoder = Encoding.GetEncoding(encoding);
                            sb.Append(temp.Key + "=" + HttpUtility.UrlEncode(val.ToString(), encoder) + "&");
                        }
                        else
                        {
                            sb.Append(temp.Key + "=" + temp.Value + "&");
                        }
                    }
                }

                if (sb.ToString().EndsWith("&"))
                {
                    sb.Remove(sb.Length - 1, 1);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return sb.ToString();
        }

        /// <summary>
        /// 向字典中添加值
        /// </summary>
        /// <param name="sourceDictionary">数据字典.</param>
        /// <param name="key">关键字.</param>
        /// <param name="value">值 .</param>
        /// <param name="replace">是否替换原有的值.</param>
        /// <remarks>
        ///   <list>
        ///    <item><description>新增字符串字典添加项方法 added by Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        public static void AddItem(this Dictionary<string, string> sourceDictionary, string key, string value, bool replace = false)
        {
            try
            {
                if (sourceDictionary.ContainsKey(key))
                {
                    sourceDictionary[key] = value;
                    if (replace)
                    {
                        sourceDictionary.Remove(key);
                        sourceDictionary.Add(key, value);
                    }
                }
                else
                {
                    sourceDictionary.Add(key, value);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }

        /// <summary>
        ///  转换为查询条件.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  07-09-2014
        /// </summary>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="defaultValue">The default value.</param>
        /// <param name="operatorString">The operator string.</param>
        /// <returns>System.String.</returns>
        public static string ToCondition(this Dictionary<string, object> dictionary, string defaultValue, string operatorString)
        {
            string result = defaultValue;

            try
            {
                List<string> conditions = new List<string>();

                foreach (var item in dictionary)
                {
                    string condition = string.Format(@"{0} = '{1}'", item.Key, item.Value);

                    conditions.Add(condition);
                }

                if (conditions.Count > 0)
                {
                    result = string.Join(" " + operatorString + " ", conditions);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return result;
        }

        /// <summary>
        /// 对数据字典中的值进行排序和过滤.
        /// </summary>
        /// <param name="dicArray">需要处理的字典.</param>
        /// <returns></returns>
        public static Dictionary<string, string> SortFilter(this Dictionary<string, string> dicArray)
        {
            return dicArray.SortFilter(true, null);
        }

        /// <summary>
        /// 对数据字典中的值进行排序和过滤.
        /// </summary>
        /// <param name="dicArray">需要处理的字典.</param>
        /// <param name="filterKeys">需要过滤的关键字.</param>
        /// <returns></returns>
        public static Dictionary<string, string> SortFilter(this Dictionary<string, string> dicArray, string[] filterKeys)
        {
            return dicArray.SortFilter(true, filterKeys);
        }

        /// <summary>
        /// 对数据字典中的值进行排序和过滤.
        /// </summary>
        /// <param name="dicArray">需要处理的字典.</param>
        /// <param name="sort">是否进行从a-z的排序.</param>
        /// <param name="filterKeys">需要过滤的关键字.</param>
        /// <returns></returns>
        public static Dictionary<string, string> SortFilter(this Dictionary<string, string> dicArray, bool sort, string[] filterKeys)
        {
            Dictionary<string, string> retArray = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                foreach (KeyValuePair<string, string> temp in dicArray)
                {
                    if (temp.Value != "" && temp.Value != null)
                    {
                        if (filterKeys == null)
                        {
                            retArray.Add(temp.Key, temp.Value);
                        }
                        else
                        {
                            if (!filterKeys.Contains<string>(temp.Key))
                            {
                                retArray.Add(temp.Key, temp.Value);
                            }
                        }
                    }
                }

                if (sort)
                {
                    SortedDictionary<string, string> dicTemp = new SortedDictionary<string, string>(retArray);
                    retArray = new Dictionary<string, string>(dicTemp);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return retArray;

        }


        /// <summary>
        /// 对数据字典中的值进行排序和过滤.
        /// </summary>
        /// <param name="dicArray">需要处理的字典.</param>
        /// <param name="sort">是否进行从a-z的排序.</param>
        /// <param name="filterKeys">需要过滤的关键字.</param>
        /// <returns></returns>
        public static Dictionary<string, object> SortFilter(this Dictionary<string, object> dicArray, bool sort, string[] filterKeys)
        {
            Dictionary<string, object> retArray = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            try
            {
                foreach (KeyValuePair<string, object> temp in dicArray)
                {
                    if (temp.Value != null)
                    {
                        if (filterKeys == null)
                        {
                            retArray.Add(temp.Key, temp.Value);
                        }
                        else
                        {
                            if (!filterKeys.Contains<string>(temp.Key))
                            {
                                retArray.Add(temp.Key, temp.Value);
                            }
                        }
                    }
                }

                if (sort)
                {
                    SortedDictionary<string, object> dicTemp = new SortedDictionary<string, object>(retArray);
                    retArray = new Dictionary<string, object>(dicTemp);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return retArray;

        }


        /// <summary>
        ///  NameValueCollection转Dictionary.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  07-09-2014
        /// </summary>
        /// <param name="nvc">The NVC.</param>
        /// <returns>Dictionary&lt;System.String, System.String&gt;.</returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection nvc)
        {
            return nvc.ToDictionary(null);
        }


        /// <summary>
        /// NameValueCollection转Dictionary.
        /// </summary>
        /// <param name="nvc">The NVC.</param>
        /// <param name="filterKeys">需要过滤的关键词Key.</param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this NameValueCollection nvc, string[] filterKeys = null)
        {
            Dictionary<string, string> retArray = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                foreach (string key in nvc)
                {
                    if (filterKeys == null)
                    {
                        retArray.Add(key, nvc[key]);
                    }
                    else
                    {
                        if (!filterKeys.Contains<string>(key))
                        {
                            retArray.Add(key, nvc[key]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return retArray;
        }

        /// <summary>
        /// 让写换为不区分大小写的字典.
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="dic">The dic.</param>
        /// <returns></returns>
        public static Dictionary<string, T> ToOrdinalIgnoreCase<T>(this Dictionary<string, T> dic)
        {
            return new Dictionary<string, T>(dic, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 将字典数据转换为Json数据.
        /// </summary>
        /// <typeparam name="T">指字的数据类型</typeparam>
        /// <param name="dic">待转的数据字典.</param>
        /// <returns></returns>
        public static string ToJson<T>(this Dictionary<string, T> dic)
        {
            return dic.ToJson<T>(false);
        }

        /// <summary>
        /// 将字典数据转换为Json数据.
        /// </summary>
        /// <typeparam name="T">指字的数据类型</typeparam>
        /// <param name="dic">待转的数据字典.</param>
        /// <param name="unicode">是否进行Unicode编码.</param>
        /// <returns></returns>
        public static string ToJson<T>(this Dictionary<string, T> dic, bool unicode = false)
        {
            List<string> list = new List<string>();
            try
            {
                foreach (KeyValuePair<string, T> temp in dic)
                {
                    if (temp.Key != null && temp.Value != null)
                    {
                        list.Add("\"" + temp.Key + "\":" + Json.JsonUtils.ConvertJson(temp.Value, unicode));
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return "{" + string.Join(",", list) + "}";
        }

        /// <summary>
        /// 鼗字典数据转换为Xml格式数据.
        /// </summary>
        /// <typeparam name="T">指字的数据类型</typeparam>
        /// <param name="dic">待转的数据字典.</param>
        /// <returns></returns>
        public static string ToXml<T>(this Dictionary<string, T> dic)
        {
            return dic.ToXml("node");
        }


        /// <summary>
        /// 鼗字典数据转换为Xml格式数据.
        /// </summary>
        /// <typeparam name="T">指字的数据类型</typeparam>
        /// <param name="dic">待转的数据字典.</param>
        /// <param name="nodeName">输出的主节点名称.</param>
        /// <returns></returns>
        public static string ToXml<T>(this Dictionary<string, T> dic, string nodeName)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                if (!string.IsNullOrEmpty(nodeName))
                {
                    sb.Append("<" + nodeName + ">");
                }

                foreach (KeyValuePair<string, T> temp in dic)
                {
                    if (temp.Key != null && temp.Value != null)
                    {
                        sb.Append(Utils.ConvertXmlData(temp.Key, temp.Value.ToString()));
                    }
                }

                if (!string.IsNullOrEmpty(nodeName))
                {
                    sb.Append("</" + nodeName + ">");
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return sb.ToString();
        }


    }
}
