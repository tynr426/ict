using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ECF.Web.Http
{
    /// <summary>
    /// FullName： <see cref="ECF.Web.Http.QueryParameter"/>
    /// Summary ： 查询参数类
    /// Version： 1.0.0.0 
    /// DateTime： 2012/5/12 8:21 
    /// CopyRight (c) by shaipe
    /// </summary>
    public class QueryParameter
    {
        #region property
        private string name = null;
        private string value = null;


        /// <summary>
        /// Initializes a new instance of the <see cref="QueryParameter"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public QueryParameter(string name, string value)
        {
            this.name = name;
            this.value = value;
        }


        /// <summary>
        /// 参数名
        /// </summary>
        public string Name
        {
            get { return name; }
        }


        /// <summary>
        /// 参数值
        /// </summary>
        public string Value
        {
            get { return value; }
        }

        #endregion

    }

    /// <summary>
    /// Summary : 查询参数类的扩展
    /// Version: 2.1
    /// DateTime: 2015/5/14 
    /// CopyRight (c) Shaipe
    /// </summary>
    public static class QueryParameterExtend
    {
        #region XML扩展
        /// <summary>
        /// xml文档转查询参数.
        /// </summary>
        /// <param name="xmlDoc">xml文档.</param>
        /// <returns></returns>
        public static QueryParameter[] ContextXmlToParas(this XmlDocument xmlDoc)
        {
            List<QueryParameter> paras = new List<QueryParameter>();

            XmlElement root = xmlDoc.DocumentElement;

            foreach (XmlNode node in root.ChildNodes)
            {
                paras.Add(new QueryParameter(node.Name, node.InnerText));
            }

            return paras.ToArray();
        }
        #endregion


        /// <summary>
        /// 查询参数列表转.
        /// </summary>
        /// <param name="queryParameters">参数列表.</param>
        /// <returns></returns>
        public static Dictionary<string, string> ToDictionary(this QueryParameter[] queryParameters)
        {
            var dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            foreach (var queryParameter in queryParameters)
            {
                if (!dictionary.ContainsKey(queryParameter.Name))
                {
                    dictionary.Add(queryParameter.Name, queryParameter.Value);
                }
                else
                {
                    dictionary[queryParameter.Name] = queryParameter.Value;
                }
            }

            return dictionary;
        }


        /// <summary>
        /// 转换为查询参数组.
        /// </summary>
        /// <param name="input">参数字符.</param>
        /// <returns></returns>
        public static QueryParameter[] ToQueryParameters(this string input)
        {
            if (input.StartsWith("?"))
            {
                input = input.Remove(0, 1);
            }

            List<QueryParameter> result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(input))
            {
                string[] p = input.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], temp[1]));
                        }
                    }
                }
            }

            return result.ToArray();

        }

        /// <summary>
        /// 转换为地址栏参数.
        /// </summary>
        /// <param name="parameters">查询参数.</param>
        /// <returns></returns>
        public static string ToLinkString(this QueryParameter[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return "";
            StringBuilder sbList = new StringBuilder();
            int count = 1;
            foreach (QueryParameter para in parameters)
            {
                sbList.AppendFormat("{0}={1}", para.Name, System.Web.HttpUtility.UrlEncode(para.Value));
                if (count < parameters.Length)
                    sbList.Append("&");
                count++;
            }
            return sbList.ToString(); ;
        }

        /// <summary>
        ///  转换为链接参数.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  04-02-2014
        /// </summary>
        /// <param name="parameters">The parameters.</param>
        /// <returns>System.String.</returns>
        public static string ToLinkString(this List<QueryParameter> parameters)
        {
            return parameters.ToArray().ToLinkString();
        }
    }

}
