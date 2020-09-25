using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.IO;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Xml;
using ECF.Json;
using System.Collections;

namespace ECF.Web.Http
{
    /// <summary>
    /// FullName： <see cref="ECF.Web.Http.QueryParameters"/>
    /// Summary ： 参数集处理方法类
    /// Verssion： 1.0.0.0
    /// DateTime： 2012/5/12 8:38
    /// CopyRight (c) by shaipe
    /// </summary>
    public class QueryParameters
    {

        /// <summary>
        /// 内部函数，切断了所有非oauth查询字符串参数（所有参数不从”oauth_”）
        /// </summary>
        /// <param name="parameters">查询字符串的一部分的网址</param>
        /// <param name="prefix">The prefix.</param>
        /// <returns>
        /// 名单的queryparameter每个包含参数名称和值
        /// </returns>
        internal static List<QueryParameter> GetQueryParameters(string parameters, string prefix)
        {

            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }
            List<QueryParameter> result = new List<QueryParameter>();
            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!String.IsNullOrEmpty(prefix)) //判断要处理的前缀是否为空,当不这空时才做处理
                    {
                        if (!string.IsNullOrEmpty(s) && !s.StartsWith(prefix))
                        {
                            if (s.IndexOf('=') > -1)
                            {
                                string[] temp = s.Split('=');
                                result.Add(new QueryParameter(temp[0], temp[1]));
                            }
                            else
                            {
                                result.Add(new QueryParameter(s, string.Empty));
                            }
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 将XmlDoc
        /// </summary>
        /// <param name="xmlDoc">XmlDocument.</param>
        /// <returns>
        /// List&lt;QueryParameter&gt;
        /// </returns>
        public static List<QueryParameter> GetQueryParameters(XmlDocument xmlDoc)
        {
            List<QueryParameter> paras = new List<QueryParameter>();

            XmlElement root = xmlDoc.DocumentElement;

            foreach (XmlNode node in root.ChildNodes)
            {
                paras.Add(new QueryParameter(node.Name, node.InnerText));
            }

            return paras;
        }

        /// <summary>
        /// 根据Json格式的数据获取参数信息
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns>
        /// List&lt;QueryParameter&gt;
        /// </returns>
        public List<QueryParameter> GetJsonQueryParameters(string json)
        {
            List<QueryParameter> paras = new List<QueryParameter>();
            try
            {
                Hashtable ht = JsonUtils.Parse(json) as Hashtable;
                if (ht != null)
                {
                    System.Collections.IDictionaryEnumerator enumerator = ht.GetEnumerator();
                    //遍历hashtable中的所有元素
                    while (enumerator.MoveNext())
                    {
                        paras.Add(new QueryParameter(enumerator.Key.ToString(), enumerator.Value.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex.Message, ex);
            }
            return paras;
        }

        
        /// <summary>
        /// 处理地址栏参数为名称值集合
        /// </summary>
        /// <param name="queryString">地址参查询参数.</param>
        /// <returns>
        /// NameValueCollection
        /// </returns>
        /// <exception cref="System.ArgumentNullException">url</exception>
        public static NameValueCollection GetParameters(string queryString)
        {
            if (queryString == null)
                throw new ArgumentNullException("url");

            NameValueCollection nvc = new NameValueCollection();


            if (String.IsNullOrEmpty(queryString))
                return nvc;

            int questionMarkIndex = queryString.IndexOf('?');
            if (questionMarkIndex > -1)
            {
                queryString = queryString.Substring(questionMarkIndex + 1);
            }


            // 开始分析参数对    
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(queryString);

            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2"), m.Result("$3"));
            }

            return nvc;
        }

        // 从Parameters中获取数据
        /// <summary>
        /// Get query from paras
        /// </summary>
        /// <param name="paras">The paras.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string GetQueryFromParas(List<QueryParameter> paras)
        {
            if (paras == null || paras.Count == 0)
                return "";
            StringBuilder sbList = new StringBuilder();
            int count = 1;
            foreach (QueryParameter para in paras)
            {
                sbList.AppendFormat("{0}={1}", para.Name, System.Web.HttpUtility.UrlEncode(para.Value));
                if (count < paras.Count)
                    sbList.Append("&");
                count++;
            }
            return sbList.ToString(); ;
        }

        // 把QueryParameter中加入URL中
        /// <summary>
        /// Add parameters to URL
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="paras">The paras.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string AddParametersToURL(string url, List<QueryParameter> paras)
        {
            string querystring = GetQueryFromParas(paras);
            if (querystring != "")
            {
                url += "?" + querystring;
            }
            return url;
        }

      

        //#region 转换成URL参数
        ///// <summary>
        ///// 转换成URL参数
        ///// </summary>
        ///// <param name="dictionary"></param>
        ///// <returns></returns>
        //public static string ToQueryString(this IDictionary<object, object> dictionary)
        //{
        //    var sb = new StringBuilder();
        //    foreach (var key in dictionary.Keys)
        //    {
        //        var value = dictionary[key];
        //        if (value != null)
        //        {
        //            sb.Append(key + "=" + value + "&");
        //        }
        //    }
        //    return sb.ToString().TrimEnd('&');
        //}

        //public static string ToQueryString(this IList<object> list, string key)
        //{
        //    var sb = new StringBuilder();
        //    foreach (var val in list)
        //    {
        //        if (val != null)
        //        {
        //            sb.Append(key + "=" + Uri.EscapeDataString(val.ToString()) + "&");
        //        }
        //    }
        //    return sb.ToString().TrimEnd('&').Substring(key.Length + 1);
        //}
        //#endregion
    }
}
