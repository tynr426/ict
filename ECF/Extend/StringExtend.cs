using ECF.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml;

namespace ECF
{
    /// <summary>
    /// FullName： <see cref="ECF.StringExtend"/>
    /// Summary ： 对字符串进行扩展
    /// Version： 1.0.0.0 
    /// DateTime： 2013/4/16 12:09
    /// Author  ： XP-WIN7
    /// </summary>
    public static class StringExtend
    {
        /// <summary>
        /// 判断字符串是否为空.
        /// </summary>
        /// <param name="self">The self.</param>
        /// <returns>
        ///   <c>true</c> if [is null or empty] [the specified self]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsNullOrEmpty(this string self)
        {
            return string.IsNullOrEmpty(self);
        }

        /// <summary>
        /// 判断字符串是否为Json格式
        /// </summary>
        /// <param name="input">输入内容.</param>
        /// <returns>
        /// System.Boolean
        /// </returns>
        public static bool IsJson(this string input)
        {

            try
            {
                if (input.StartsWith("{") && input.EndsWith("}")
                          || input.StartsWith("[") && input.EndsWith("]"))
                {
                    try
                    {
                        JsonData jsd = JsonMapper.ToObject(input);
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                new ECFException(ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// 判断是否为Xml格式数据.
        /// </summary>
        /// <param name="input">当前文本内容.</param>
        /// <returns></returns>
        public static bool IsXml(this string input)
        {
            Regex reXml = new Regex(@"<[^<>]+>", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

            return reXml.IsMatch(input);
        }

        /// <summary>
        /// 对字符串进行Html编码.
        /// </summary>
        /// <param name="self">待编码字符串.</param>
        /// <returns></returns>
        public static string HtmlEncode(this string self)
        {
            return HttpUtility.HtmlEncode(self);
        }

        /// <summary>
        /// 对字符串进行Html解码.
        /// </summary>
        /// <param name="self">待解码字符串.</param>
        /// <returns></returns>
        public static string HtmlDecode(this string self)
        {
            return HttpUtility.HtmlDecode(self);
        }

        /// <summary>
        /// 转全角(SBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>全角字符串</returns>
        public static string ToSBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new string(c);
        }
        /// <summary>
        /// 转半角(DBC case)
        /// </summary>
        /// <param name="input">任意字符串</param>
        /// <returns>半角字符串</returns>
        public static string ToDBC(this string input)
        {
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 12288)
                {
                    c[i] = (char)32;
                    continue;
                }
                if (c[i] > 65280 && c[i] < 65375)
                    c[i] = (char)(c[i] - 65248);
            }
            return new string(c);
        }

        /// <summary>
        /// 将字符串转为枚举类型.
        /// </summary>
        /// <typeparam name="T">枚举类型</typeparam>
        /// <returns></returns>
        public static T ToEnum<T>(this string input)
        {
            try
            {
                if (!Utils.IsNullOrEmpty(input))
                {
                    return (T)Enum.Parse(typeof(T), input, true);
                }
                else
                {
                    return default(T);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return default(T);
            }
        }

        /// <summary>
        /// 获取转码后的字符串.
        /// </summary>
        /// <param name="source">源字符.</param>
        /// <param name="encodingName">编码字符.</param>
        /// <returns>System.String.</returns>
        public static string ToEncoding(this string source, string encodingName)
        {

            if (string.IsNullOrEmpty(source))
                return source;


            return Encoding.GetEncoding(encodingName).GetString(Encoding.GetEncoding(encodingName).GetBytes(source));
        }

        /// <summary>
        /// 必须填写
        /// </summary>
        /// <param name="self">本身字符.</param>
        /// <param name="fieldName">字段名称.</param>
        /// <param name="length">限定长段.</param>
        /// <returns>
        /// System.String
        /// </returns>
        /// <exception cref="ECFException">此项不能为空!</exception>
        public static string Required(this string self, string fieldName = "", int length = 0)
        {
            if (string.IsNullOrEmpty(self))
                throw new ECFException("字段:" + fieldName + "不能为空!");

            if (length > 0 && self.Length > length)
                throw new ECFException("字段:" + fieldName + "超出指定长度!");

            return self;
        }

        /// <summary>把一个列表组合成为一个字符串，默认逗号分隔</summary>
        /// <param name="value"></param>
        /// <param name="separator">组合分隔符，默认逗号</param>
        /// <returns></returns>
        public static String Join(this IEnumerable value, String separator = ",")
        {
            var sb = new StringBuilder();
            try
            {
                if (value != null)
                {
                    foreach (var item in value)
                    {
                        sb.Separate(separator).Append(item + "");
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return sb.ToString();
        }

        /// <summary>把一个列表组合成为一个字符串，默认逗号分隔</summary>
        /// <param name="value"></param>
        /// <param name="separator">组合分隔符，默认逗号</param>
        /// <param name="func">把对象转为字符串的委托</param>
        /// <returns></returns>
        public static String Join<T>(this IEnumerable<T> value, String separator = ",", Func<T, String> func = null)
        {
            var sb = new StringBuilder();
            try
            {
                if (value != null)
                {
                    if (func == null) func = obj => obj + "";
                    foreach (var item in value)
                    {
                        sb.Separate(separator).Append(func(item));
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return sb.ToString();
        }

        /// <summary>追加分隔符字符串，忽略开头，常用于拼接</summary>
        /// <param name="sb">字符串构造者</param>
        /// <param name="separator">分隔符</param>
        /// <returns></returns>
        public static StringBuilder Separate(this StringBuilder sb, String separator)
        {
            if (sb == null || String.IsNullOrEmpty(separator)) return sb;

            if (sb.Length > 0) sb.Append(separator);

            return sb;
        }



        /// <summary>拆分字符串成为名值字典。逗号分号分组，等号分隔</summary>
        /// <param name="value">字符串</param>
        /// <param name="nameValueSeparator">名值分隔符，默认等于号</param>
        /// <param name="separators">分组分隔符，默认and符号</param>
        /// <returns></returns>
        public static Dictionary<String, String> SplitAsDictionary(this String value, String nameValueSeparator = "=", params String[] separators)
        {
            var dic = new Dictionary<String, String>();
            try
            {
                if (value.IsNullOrEmpty()) return dic;

                if (String.IsNullOrEmpty(nameValueSeparator)) nameValueSeparator = "=";
                if (separators == null || separators.Length < 1) separators = new String[] { "&" };

                var ss = value.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                if (ss == null || ss.Length < 1) return null;

                foreach (var item in ss)
                {
                    var p = item.IndexOf(nameValueSeparator);
                    // 在前后都不行
                    if (p <= 0 || p >= item.Length - 1) continue;

                    var key = item.Substring(0, p).Trim();
                    dic[key] = item.Substring(p + nameValueSeparator.Length).Trim();
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return dic;
        }

        /// <summary>
        ///  转换为Xml节点数据.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  20160824
        /// </summary>
        /// <param name="value">数据值 .</param>
        /// <param name="nodeName">节点名称.</param>
        /// <returns>System.String.</returns>
        public static string ToXmlNodeString(this string value, string nodeName)
        {
            try
            {
                if (string.IsNullOrEmpty(nodeName))
                {
                    nodeName = "Node";
                }

                if (value != string.Empty)
                {
                    if (value.IndexOf("<") != -1 || value.IndexOf(">") != -1)
                    {
                        return "<" + nodeName + "><![CDATA[" + (value) + "]]></" + nodeName + ">";
                    }
                    else
                    {
                        return "<" + nodeName + ">" + value + "</" + nodeName + ">";
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return "";
        }

        /// <summary>
        /// UrlEncode
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>
        /// String
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加Url加密 added by Shaipe 2018/10/12</description></item>
        ///   </list>
        /// </remarks>
        public static string UrlEncode(this string text, string encoding = "utf-8")
        {
            return Utils.UrlEncode(text);
        }

        /// <summary>
        /// 字符串转换为键值对
        /// </summary>
        /// <param name="parameters">字符串.</param>
        /// <param name="urlEncode"></param>
        /// <param name="first">第一个间隔符.</param>
        /// <param name="second">第二个间隔符.</param>
        /// <returns>
        /// Dictionary&lt;System.String, System.String&gt;
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>新增字符转字典扩展方法 added by Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        public static Dictionary<string, string> ToDictionary(this string parameters, bool urlEncode = true, char first = '&', char second = '=')
        {
            if (string.IsNullOrEmpty(parameters))
            {
                return null;
            }

            Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            try
            {
                string[] parameterArr = parameters.Split(first);

                if (parameterArr.Length > 0)
                {
                    foreach (string item in parameterArr)
                    {
                        string[] temp = item.Split(second);

                        if (temp.Length >= 2)
                        {
                            string val = temp[1];
                            //if (urlEncode)
                            //{
                            //    val = val.UrlEncode();
                            //}
                            if (!result.ContainsKey(temp[0]))
                            {
                                result.Add(temp[0], val);
                            }
                            else
                            {
                                result[temp[0]] = val;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return result;
        }


        #region ToLong
        /// <summary>
        /// 将string转换成为long
        /// </summary>
        /// <param name="source"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static long ToLong(this string source, long defaultValue = 0)
        {
            if (string.IsNullOrEmpty(source))
            {
                return defaultValue;
            }

            long result;
            if (!long.TryParse(source, out result))
            {
                return defaultValue;
            }

            return result;
        }
        #endregion


        #region ToXml 转换为XML

        /// <summary>
        /// 转换为XML.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="rootName">根节点名,如果xml仅为某xml一部分，请定义根节点，如：root</param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument(this string source, string rootName)
        {
            XmlDocument xml = new XmlDocument();

            try
            {
                if (string.IsNullOrEmpty(source))
                    return xml;

                //判断是否具有根节点
                if (!source.ToUpper().StartsWith("<" + rootName.ToUpper() + ">") && !source.ToUpper().EndsWith("</" + rootName.ToUpper() + ">"))
                {
                    xml.LoadXml("<" + rootName + ">" + source + "</" + rootName + ">");
                }
                else if (source.ToUpper().StartsWith("<" + rootName.ToUpper() + ">") && source.ToUpper().EndsWith("</" + rootName.ToUpper() + ">"))
                {
                    xml.LoadXml(source);
                }
            }
            catch
            {
                xml = new XmlDocument();
            }

            return xml;
        }
        #endregion


    }
}
