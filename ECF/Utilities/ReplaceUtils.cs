using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using System.Collections;
using System.IO;

namespace ECF
{
    /// <summary>
    /// FullName： <see cref="ECF.ReplaceUtil" />
    /// Summary ： 对字符串根据正则进行解析
    /// Version： 1.0.0.0
    /// DateTime： 2012/4/23 16:14
    /// CopyRight (c) by shaipe
    /// </summary>
    public class ReplaceUtil
    {

        /// <summary>
        /// 正则类型
        /// </summary>
        public enum RegexType
        {
            /// <summary>
            /// 
            /// </summary>
            Angle,
            /// <summary>
            /// 
            /// </summary>
            Brace,
            /// <summary>
            /// 
            /// </summary>
            Bracket
        }

        #region ReplaceData 对数据进行处理


        /// <summary>
        /// 对数据进行处理
        /// by XP-PC 2012/4/23
        /// </summary>
        /// <param name="regexType">正则表达式的类型.</param>
        /// <param name="data">待替换的数据.</param>
        /// <param name="dataValue">用于替换的值对象，可以支持Dictionary (string object)  Hashtable DataRow.</param>
        /// <param name="repNull">是否把不存在的字段替换为空.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string ReplaceData(RegexType regexType, string data, object dataValue, bool repNull)
        {
            //待处理数据为空时直接返回空字符
            if (data == null) return string.Empty;
            Regex regex = RuleRegex.BraceExternal;
            if (regexType == RegexType.Bracket)
            {
                regex = RuleRegex.BracketExternal;
            }
            //
            StringBuilder sb = new StringBuilder(data);
            try
            {
                bool isNull = repNull && dataValue == null;
                for (Match m = regex.Match(sb.ToString()); m.Success; m = m.NextMatch()) //处理普通模板
                {
                    string regval = m.Groups["Name"].Value;
                    string expression = m.Groups["External"].Value;
                    if (isNull)
                    {
                        sb.Replace(m.ToString(), "");
                    }
                    else
                    {
                        object val = null;
                        if (dataValue is Dictionary<string, object>)
                        {
                            Dictionary<string, object> vic = (Dictionary<string, object>)dataValue;
                            if (vic.ContainsKey(regval))
                            {
                                val = vic[regval];
                            }
                        }
                        else if (dataValue is DataRow)
                        {
                            DataRow dr = (DataRow)dataValue;
                            if (dr.Table.Columns.Contains(regval))
                            {
                                val = dr[regval];
                            }
                        }
                        else if (dataValue is Hashtable)
                        {
                            Hashtable ht = (Hashtable)dataValue;
                            if (ht.ContainsKey(regval))
                            {
                                val = ht[regval];
                            }
                        }
                        if (val == null)
                        {
                            if (repNull) sb.Replace(m.ToString(), "");
                        }
                        else
                        {
                            sb.Replace(m.Value, FormatUtil.Formater(expression, val));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
            }
            return sb.ToString();
        }


        /// <summary>
        /// 对数据进行处理
        /// </summary>
        /// <param name="data">待替换的数据.</param>
        /// <param name="dataValue">用于替换的值对象，可以支持.</param>
        /// <param name="repNull">是否把不存在的字段替换为空.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string ReplaceData(string data, object dataValue, bool repNull)
        {
            return ReplaceData(RegexType.Brace, data, dataValue, repNull);
        }

        /// <summary>
        /// Replace data
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="dataValue">The data value.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string ReplaceData(string data, object dataValue)
        {
            return ReplaceData(data, dataValue, true);
        }

        #endregion


        #region 正则表达式获取图片元素以及它的Src属性

        internal static Regex reChinese = new Regex(@"[\u0100-\uFFFF]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        internal static Regex reImage = new Regex(@"\<img[^\>]+\>", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        internal static Regex reSrc = new Regex(@"\bsrc\b=['|""](?<Url>[^""&^']*?)['|""]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        #endregion

        #region RemoteSrc 本地图片远程化
        /// <summary>
        /// 本地图片远程化.
        /// 修改对图片服务器地址为空时的判断 by 20121025
        /// </summary>
        /// <param name="htmlContent">需要处理的Html内容.</param>
        /// <param name="imageServer">图片服务器地址.</param>
        /// <returns>图片远程化处理后的String</returns>
        public static string RemoteSrc(string htmlContent, string imageServer)
        {
            if (Utils.IsNullOrEmpty(htmlContent) || Utils.IsNullOrEmpty(imageServer))
                return htmlContent;

            // 图片控件完整Html、路径完整Html
            string ImageControlHtml = null, SrcHtml = null;

            return reImage.Replace(htmlContent, delegate (Match m)
            {
                ImageControlHtml = m.Value;

                Match mSrc = reSrc.Match(ImageControlHtml);

                if (!mSrc.Success)
                    return m.Value;

                SrcHtml = mSrc.Groups[1].Value;

                if (SrcHtml.IndexOf("http://") < 0)
                {
                    return m.Value.Replace(SrcHtml, imageServer + SrcHtml);
                }

                return m.Value;
            });
        }

        #endregion

        #region LocalSrc 将网图片本地化
        ///// <summary>
        ///// 上传Editor中的网络图片，并替换网络图片地址 by xp 20121011
        ///// </summary>
        ///// <param name="htmlContent">Editor编辑器HTML内容</param>
        ///// <param name="relativePath">存储文件的相对路径</param>
        ///// <param name="imageServer">图片服务器地址.</param>
        ///// <param name="downloadFiles">out出需要下载的文件列表.</param>
        ///// <returns>
        ///// 返回替换后的内容
        ///// </returns>
        //public static string LocalSrc(string htmlContent, string relativePath, string imageServer, out Dictionary<string, string> downloadFiles)
        //{
        //    return LocalSrc(htmlContent, relativePath, "", imageServer, out downloadFiles);
        //}
        /// <summary>
        /// 上传Editor中的网络图片，并替换网络图片地址 by xp 20121011
        /// </summary>
        /// <param name="htmlContent">Editor编辑器HTML内容</param>
        /// <param name="relativePath">存储文件的相对路径</param>
        /// <param name="domain">当前网站的域名.</param>
        /// <param name="imageServer">图片服务器地址.</param>
        /// <param name="downloadFiles">out出需要下载的文件列表.</param>
        /// <returns>
        /// 返回替换后的内容
        /// </returns>
        public static string LocalSrc(string htmlContent, string relativePath, string domain, string imageServer, out Dictionary<string, string> downloadFiles)
        {
            // 需要下载的文件
            Dictionary<string, string> downFiles = new Dictionary<string, string>();

            downloadFiles = downFiles;

            if (Utils.IsNullOrEmpty(htmlContent))
                return htmlContent;

            // 图片控件完整Html、路径完整Html
            string ImageControlHtml = null, SrcHtml = null;


            return reImage.Replace(htmlContent, delegate (Match m)
            {
                ImageControlHtml = m.Value;

                Match mSrc = reSrc.Match(ImageControlHtml);

                if (!mSrc.Success)
                    return m.Value;

                SrcHtml = mSrc.Groups[1].Value;
                Regex httpReg = new Regex("^https?://.+$");

                if (!httpReg.IsMatch(SrcHtml) && String.IsNullOrEmpty(imageServer))
                {
                    return m.Value;
                }

                //if (SrcHtml.IndexOf("http://") < 0 && String.IsNullOrEmpty(imageServer)) //没有图片服务器为空且是本机图片时不做任何处理
                //{
                //    return m.Value;
                //}
                // /^https?:\/\/.+$/i
                if (downFiles.ContainsKey((!httpReg.IsMatch(SrcHtml) ? domain : "") + SrcHtml)) //判断是否有相同的图片
                    return m.Value.Replace(SrcHtml, downFiles[(!httpReg.IsMatch(SrcHtml) ? domain : "") + SrcHtml]);

                string FileName = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(SrcHtml);  //SrcHtml.Substring(SrcHtml.LastIndexOf("/") + 1).Replace("'", String.Empty).Replace("\"", String.Empty);

                //if (reChinese.Match(FileName).Success) //处理中文

                string RelativeUrl = SrcHtml.Replace(mSrc.Groups["Url"].Value, relativePath + (relativePath.EndsWith("/") ? "" : "/") + FileName);

                //重新生成新的文件名,用于需要下载时使用
                FileName = Guid.NewGuid().ToString() + Path.GetExtension(FileName);

                if (Utils.IsNullOrEmpty(imageServer))   // 不指定图片服务器
                {
                    if (httpReg.IsMatch(SrcHtml)) //网络图片
                    {
                        if (SrcHtml.IndexOf(domain) > -1)    //图片服务器(也是本服务器图片)
                        {
                            RelativeUrl = SrcHtml.Replace(domain, "");
                            return m.Value.Replace(SrcHtml, RelativeUrl); //无需下载
                        }
                        else
                        {
                            downFiles.Add(SrcHtml, RelativeUrl);
                            return m.Value.Replace(SrcHtml, RelativeUrl);
                        }
                    }
                }
                else if (imageServer.ToLower() == domain.ToLower())      // 图片服务器与域名相同
                {
                    if (httpReg.IsMatch(SrcHtml)) //网络图片
                    {
                        if (SrcHtml.IndexOf(domain) > -1)    //图片服务器(也是本服务器图片)
                        {
                            RelativeUrl = SrcHtml.Replace(domain, "");
                            return m.Value.Replace(SrcHtml, RelativeUrl); //无需下载
                        }
                        else
                        {
                            downFiles.Add(SrcHtml, RelativeUrl);
                            return m.Value.Replace(SrcHtml, RelativeUrl);
                        }
                    }
                }
                else //图片服务器与域名不在同一服务器
                {
                    if (httpReg.IsMatch(SrcHtml)) //网络图片
                    {
                        if (SrcHtml.IndexOf(imageServer.ToLower()) > -1)    //图片服务器(也是本服务器图片)
                        {
                            RelativeUrl = SrcHtml.Replace(imageServer, "");
                            return m.Value.Replace(SrcHtml, RelativeUrl); //无需下载
                        }
                        else if (SrcHtml.IndexOf(domain.ToLower()) > -1)    //图片服务器(也是本服务器图片)
                        {
                            downFiles.Add(SrcHtml, RelativeUrl);
                            return m.Value.Replace(SrcHtml, RelativeUrl); //无需下载
                        }
                        else
                        {
                            downFiles.Add(SrcHtml, RelativeUrl);
                            return m.Value.Replace(SrcHtml, RelativeUrl);
                        }
                    }
                    else
                    {
                        downFiles.Add(domain + SrcHtml, RelativeUrl);
                        return m.Value.Replace(SrcHtml, RelativeUrl);
                    }
                }

                //if (SrcHtml.IndexOf("http://") > -1) //处理带有http的图片
                //{

                //    if (SrcHtml.IndexOf(imageServer.ToLower()) > -1) // 图片服务器的图片
                //    {
                //    }

                //}
                //else
                //{
                //}

                // 不是相对路径并且不是图片服务器路径
                //if (
                //   (SrcHtml.IndexOf("http://") > -1 ||  //是否为带有http://的图片地址
                //   (SrcHtml.IndexOf("http://") < 0 && (!String.IsNullOrEmpty(domain) && domain.ToLower() != imageServer.ToLower())) //非网络图片且
                //   ) &&
                //   (String.IsNullOrEmpty(imageServer) || (!String.IsNullOrEmpty(imageServer) && SrcHtml.IndexOf(imageServer) == -1))//图片服务器地址不为空，且图片地址
                //   )
                //{
                //    FileName = Guid.NewGuid().ToString() + Path.GetExtension(FileName);
                //    downFiles.Add((SrcHtml.IndexOf("http://") < 0 ? domain : "") + SrcHtml, RelativeUrl);
                //    return m.Value.Replace(SrcHtml, RelativeUrl);
                //}
                //else if (SrcHtml.IndexOf("http://") > -1 && domain.ToLower() != imageServer.ToLower()) //图片服务器上的图片处理（图片服务器与域名服务器不相同时
                //{
                //    return m.Value.Replace(SrcHtml, RelativeUrl);
                //}
                //else if (SrcHtml.IndexOf("http://") > -1) //处理图片路径与图片服务器域名相同的图片
                //{
                //    return m.Value.Replace(SrcHtml, SrcHtml.Replace(imageServer.ToLower(), ""));
                //}

                return m.Value;
            });
        }
        #endregion

        #region LocalSrc 将网图片本地化
        /// <summary>
        /// 上传Editor中的网络图片，并替换网络图片地址 by xp 20121011
        /// </summary>
        /// <param name="htmlContent">Editor编辑器HTML内容</param>
        /// <param name="relativePath">存储文件的相对路径</param>
        /// <param name="domain">当前网站的域名.</param>
        /// <param name="imageServer">图片服务器地址.</param>
        /// <param name="downloadFiles">out出需要下载的文件列表,Xml格式.</param>
        /// <returns>
        /// 返回替换后的内容
        /// </returns>
        public static string LocalSrc(string htmlContent, string relativePath, string domain, string imageServer, out string downloadFiles)
        {
            downloadFiles = "";
            Dictionary<string, string> dic;
            string retstr = LocalSrc(htmlContent, relativePath, domain, imageServer, out dic);
            foreach (KeyValuePair<string, string> val in dic)
            {
                downloadFiles += "<src><url>" + val.Key + "</url><path>" + val.Value + "</path></src>";
            }
            return retstr;
        }
        ///// <summary>
        ///// 上传Editor中的网络图片，并替换网络图片地址 by xp 20121011
        ///// </summary>
        ///// <param name="htmlContent">Editor编辑器HTML内容</param>
        ///// <param name="relativePath">存储文件的相对路径</param>
        ///// <param name="imageServer">图片服务器地址.</param>
        ///// <param name="downloadFiles">out出需要下载的文件列表,Xml格式.</param>
        ///// <returns>
        ///// 返回替换后的内容
        ///// </returns>
        //public static string LocalSrc(string htmlContent, string relativePath, string imageServer, out string downloadFiles)
        //{
        //    return LocalSrc(htmlContent, relativePath, "", imageServer, out downloadFiles);
        //}
        #endregion

    }
}
