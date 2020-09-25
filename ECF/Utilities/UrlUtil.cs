using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Net;

namespace ECF
{
    /// <summary>
    ///   <see cref="ECF.UrlUtil"/>
    /// URL util URL操作类,前期由hwei编写
    ///     xp进行整理
    /// Author:  XP
    /// Created: 2011/10/14
    /// </summary>
    public class UrlUtil
    {
        static System.Text.Encoding encoding = System.Text.Encoding.UTF8;

        #region URL的64位编码
        /// <summary>
        /// URL的64位编码
        /// </summary>
        /// <param name="sourthUrl"></param>
        /// <returns></returns>
        public static string Base64Encrypt(string sourthUrl)
        {
            string eurl = HttpUtility.UrlEncode(sourthUrl);
            eurl = Convert.ToBase64String(encoding.GetBytes(eurl));
            return eurl;
        }
        #endregion

        #region URL的64位解码
        /// <summary>
        /// URL的64位解码
        /// </summary>
        /// <param name="eStr"></param>
        /// <returns></returns>
        public static string Base64Decrypt(string eStr)
        {
            if (!IsBase64(eStr))
            {
                return eStr;
            }
            byte[] buffer = Convert.FromBase64String(eStr);
            string sourthUrl = encoding.GetString(buffer);
            sourthUrl = HttpUtility.UrlDecode(sourthUrl);
            return sourthUrl;
        }
        /// <summary>
        /// 是否是Base64字符串
        /// </summary>
        /// <param name="eStr"></param>
        /// <returns></returns>
        public static bool IsBase64(string eStr)
        {
            if ((eStr.Length % 4) != 0)
            {
                return false;
            }
            if (!Regex.IsMatch(eStr, "^[A-Z0-9/+=]*$", RegexOptions.IgnoreCase))
            {
                return false;
            }
            return true;
        }
        #endregion

        /// <summary>
        /// 更新URL参数
        /// </summary>
        public static string UpdateParam(string url, string paramName, string value)
        {
            string keyWord = paramName + "=";
            int index = url.IndexOf(keyWord) + keyWord.Length;
            if (url.IndexOf(keyWord) != -1)
            {
                int index1 = url.IndexOf("&", index);
                if (index1 == -1)
                {
                    url = url.Remove(index, url.Length - index);
                    url = string.Concat(url, value);
                    return url;
                }
                url = url.Remove(index, index1 - index);
                url = url.Insert(index, value);
            }
            else
            {
                if (url.IndexOf("?") == -1)
                {
                    url += string.Format("?{0}{1}", keyWord, value);
                }
                else
                {
                    url += string.Format("&{0}{1}", keyWord, value);
                }
            }
            return url;
        }

        /// <summary>
        /// Shortens any absolute URL to a specified maximum length
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="max">The maximum.</param>
        /// <returns>
        /// String
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>说明原因 added by Shaipe 2018/10/25</description></item>
        ///   </list>
        /// </remarks>
        public static string ShortenUrl(string url, int max)
        {
            if (url.Length <= max)
                return url;

            // Remove the protocal
            int startIndex = url.IndexOf("://");
            if (startIndex > -1)
                url = url.Substring(startIndex + 3);

            if (url.Length <= max)
                return url;

            // Compress folder structure
            int firstIndex = url.IndexOf("/") + 1;
            int lastIndex = url.LastIndexOf("/");
            if (firstIndex < lastIndex)
            {
                url = url.Remove(firstIndex, lastIndex - firstIndex);
                url = url.Insert(firstIndex, "...");
            }

            if (url.Length <= max)
                return url;

            // Remove URL parameters
            int queryIndex = url.IndexOf("?");
            if (queryIndex > -1)
                url = url.Substring(0, queryIndex);

            if (url.Length <= max)
                return url;

            // Remove URL fragment
            int fragmentIndex = url.IndexOf("#");
            if (fragmentIndex > -1)
                url = url.Substring(0, fragmentIndex);

            if (url.Length <= max)
                return url;

            // Compress page
            firstIndex = url.LastIndexOf("/") + 1;
            lastIndex = url.LastIndexOf(".");
            if (lastIndex - firstIndex > 10)
            {
                string page = url.Substring(firstIndex, lastIndex - firstIndex);
                int length = url.Length - max + 3;
                if (page.Length > length)
                    url = url.Replace(page, "..." + page.Substring(length));
            }

            return url;
        }

        #region 分析URL所属的域
        /// <summary>
        /// Get domain
        /// </summary>
        /// <param name="fromUrl">From URL.</param>
        /// <param name="domain">The domain.</param>
        /// <param name="subDomain">The sub domain.</param>
        public static void GetDomain(string fromUrl, out string domain, out string subDomain)
        {
            domain = "";
            subDomain = "";
            try
            {
                if (fromUrl.IndexOf("的名片") > -1)
                {
                    subDomain = fromUrl;
                    domain = "名片";
                    return;
                }

                UriBuilder builder = new UriBuilder(fromUrl);
                fromUrl = builder.ToString();

                Uri u = new Uri(fromUrl);

                if (u.IsWellFormedOriginalString())
                {
                    if (u.IsFile)
                    {
                        subDomain = domain = "客户端本地文件路径";

                    }
                    else
                    {
                        string Authority = u.Authority;
                        string[] ss = u.Authority.Split('.');
                        if (ss.Length == 2)
                        {
                            Authority = "www." + Authority;
                        }
                        int index = Authority.IndexOf('.', 0);
                        domain = Authority.Substring(index + 1, Authority.Length - index - 1).Replace("comhttp", "com");
                        subDomain = Authority.Replace("comhttp", "com");
                        if (ss.Length < 2)
                        {
                            domain = "不明路径";
                            subDomain = "不明路径";
                        }
                    }
                }
                else
                {
                    if (u.IsFile)
                    {
                        subDomain = domain = "客户端本地文件路径";
                    }
                    else
                    {
                        subDomain = domain = "不明路径";
                    }
                }
            }
            catch
            {
                subDomain = domain = "不明路径";
            }
        }

        /// <summary>
        /// 分析 url 字符串中的参数信息
        /// </summary>
        /// <param name="url">输入的 URL</param>
        /// <param name="baseUrl">输出 URL 的基础部分</param>
        /// <param name="nvc">输出分析后得到的 (参数名,参数值) 的集合</param>
        public static void ParseUrl(string url, out string baseUrl, out NameValueCollection nvc)
        {
            if (url == null)
                throw new ArgumentNullException("url");

            nvc = new NameValueCollection();
            baseUrl = "";

            if (url == "")
                return;

            int questionMarkIndex = url.IndexOf('?');

            if (questionMarkIndex == -1)
            {
                baseUrl = url;
                return;
            }
            baseUrl = url.Substring(0, questionMarkIndex);
            if (questionMarkIndex == url.Length - 1)
                return;
            string ps = url.Substring(questionMarkIndex + 1);

            // 开始分析参数对    
            Regex re = new Regex(@"(^|&)?(\w+)=([^&]+)(&|$)?", RegexOptions.Compiled);
            MatchCollection mc = re.Matches(ps);

            foreach (Match m in mc)
            {
                nvc.Add(m.Result("$2").ToLower(), m.Result("$3"));
            }
        }

        #endregion


        /// <summary>
        /// Url地址参数.
        /// </summary>
        /// <param name="urlParameter">The URL parameter.</param>
        /// <returns></returns>
        public static Dictionary<string, string> UrlToDictionary(string urlParameter)
        {
            Dictionary<string, string> retArray = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(urlParameter))
            {
                if (urlParameter.IndexOf("?") > -1)
                {
                    urlParameter = urlParameter.Remove(0, urlParameter.IndexOf("?"));
                }

                if (urlParameter.IndexOf("#") > -1)
                {
                    urlParameter = urlParameter.Remove(urlParameter.IndexOf("#"));
                }

                string[] pas = urlParameter.Split('&');
                foreach (string s in pas)
                {
                    if (!string.IsNullOrEmpty(s))
                    {
                        string[] pvs = s.Split('=');
                        if (pvs.Length > 1 && !retArray.ContainsKey(pvs[0]) && !string.IsNullOrEmpty(pvs[1]))
                        {
                            retArray.Add(pvs[0], pvs[1]);
                        }
                    }
                }

            }

            return retArray;
        }

        /// <summary>
        ///  判断网络地址是否存在.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>System.Boolean.</returns>
        public static bool UrlExists(string url)
        {
            try
            {
                HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create(url);
                myRequest.Method = "HEAD";               //设置提交方式可以为＂ｇｅｔ＂，＂ｈｅａｄ＂等
                myRequest.Timeout = 10000;              //设置网页响应时间长度
                myRequest.AllowAutoRedirect = false;//是否允许自动重定向
                myRequest.UseDefaultCredentials = true;  // 给定默认的认证
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                return (myResponse.StatusCode == HttpStatusCode.OK);//返回响应的状态
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
                return false;
            }
        }

        ///// <summary>
        ///// 根据传入和字典进行对参数进行签名处理，
        ///// </summary>
        ///// <param name="appId">接口应用id.</param>
        ///// <param name="secret">接口密钥.</param>
        ///// <param name="dic">参数字典.</param>
        ///// <returns>
        ///// 返回原值转并添加了appid和时间戳和追加签名参数
        ///// </returns>
        ///// <remarks>
        /////   <list>
        /////    <item><description>添加签名方法 added by Shaipe 2018/10/17</description></item>
        /////   </list>
        ///// </remarks>
        //public static string UrlSign(string appId, string secret, Dictionary<string, string> dic)
        //{
        //    dic = dic ?? new Dictionary<string, string>();
        //    dic = dic.ToOrdinalIgnoreCase();
        //    dic.AddItem("appid", appId);
        //    dic.AddItem("timestamp", Utils.GetTimstamp().ToString());

        //    Dictionary<string, string> sordDic = dic.SortFilter();
        //    string sign = ECF.Security.Encrypt.MD532(sordDic.ToLinkString() + "&secret=" + secret);
        //    return dic.ToLinkString() + "&sign=" + sign;
        //}

        ///// <summary>
        ///// 根据传入和字典进行对参数进行签名处理，
        ///// </summary>
        ///// <param name="appId">接口应用id.</param>
        ///// <param name="secret">接口密钥.</param>
        ///// <param name="paramString">参数字符.</param>
        ///// <returns>
        ///// 返回原值转并添加了appid和时间戳和追加签名参数
        ///// </returns>
        ///// <remarks>
        /////   <list>
        /////    <item><description>添加签名方法 added by Shaipe 2018/10/17</description></item>
        /////   </list>
        ///// </remarks>
        //public static string UrlSign(string appId, string secret, string paramString)
        //{
        //    return UrlSign(appId, secret, paramString.ToDictionary());
        //}

        ///// <summary>
        ///// 根据传入和字典进行对参数进行签名处理，
        ///// </summary>
        ///// <param name="appId">接口应用id.</param>
        ///// <param name="secret">接口密钥.</param>
        ///// <param name="parameters">参数字符.</param>
        ///// <returns>
        ///// 返回原值转并添加了appid和时间戳和追加签名参数
        ///// </returns>
        ///// <remarks>
        /////   <list>
        /////    <item><description>添加签名方法 added by Shaipe 2018/10/17</description></item>
        /////   </list>
        ///// </remarks>
        //public static string UrlSign(string appId, string secret, QueryParameter[] parameters)
        //{
        //    return UrlSign(appId, secret, parameters.ToDictionary());
        //}

        /// <summary>
        /// 是否包含协议
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// Boolean
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加方法 added by Shaipe 2018/10/25</description></item>
        ///   </list>
        /// </remarks>
        public static bool ContainProtocol(string url)
        {
            if (string.IsNullOrEmpty(url)) return false;

            string tempUrl = url.ToLower();
            return (tempUrl.StartsWith("http://") || tempUrl.StartsWith("https://"));
        }
    }
}
