using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;

namespace ECF
{
	/// <summary>
	///   <see cref="ECF.Utils"/>
	/// Html处理单元
	/// Author:  XP
	/// Created: 2011/9/8
	/// </summary>
	public partial class Utils
    {
        private static Regex RegexBr = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);

        /// <summary>
        /// 字体匹配正则
        /// </summary>
        public static Regex RegexFont = new Regex(@"<font color=" + "\".*?\"" + @">([\s\S]+?)</font>", RegexOptions.Compiled | RegexOptions.IgnoreCase);


        /// <summary>
        /// 移除Html标记
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveHtml(string content)
        {
            if (String.IsNullOrEmpty(content)) return content;
            string regexstr = @"<[^>]*>";
            return Regex.Replace(content, regexstr, string.Empty, RegexOptions.IgnoreCase);
        }

        /// <summary>
        /// 过滤HTML中的不安全标签
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string RemoveUnsafeHtml(string content)
        {
            if (String.IsNullOrEmpty(content)) return content;
            content = Regex.Replace(content, @"(\<|\s+)o([a-z]+\s?=)", "$1$2", RegexOptions.IgnoreCase);
            content = Regex.Replace(content, @"(script|frame|form|meta|behavior|style)([\s|:|>])+", "$1.$2", RegexOptions.IgnoreCase);
            return content;
        }

        /// <summary>
        /// 将用户组Title中的font标签去掉
        /// </summary>
        /// <param name="title">用户组Title</param>
        /// <returns></returns>
        public static string RemoveFontTag(string title)
        {
            Match m = RegexFont.Match(title);
            if (m.Success)
            {
                return m.Groups[1].Value;
            }
            return title;
        }


        /// <summary>
        /// 从HTML中获取文本,保留br,p,img
        /// </summary>
        /// <param name="HTML"></param>
        /// <returns></returns>
        public static string GetTextFromHTML(string HTML)
        {
            System.Text.RegularExpressions.Regex regEx = new System.Text.RegularExpressions.Regex(@"</?(?!br|/?p|img)[^>]*>", System.Text.RegularExpressions.RegexOptions.IgnoreCase);

            return regEx.Replace(HTML, "");
        }


        /// <summary>
        /// 替换html字符
        /// </summary>
        public static string EncodeHtml(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                return HttpUtility.HtmlEncode(str).Replace("&amp;", "&");
            }
            return str;
        }

        /// <summary>
        /// Decode HTML
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string DecodeHtml(string str)
        {
            if (!String.IsNullOrEmpty(str))
            {
                return HttpUtility.HtmlDecode(str);
            }
            return str;
        }

        /// <summary>
        /// 清除给定字符串中的回车及换行符
        /// </summary>
        /// <param name="str">要清除的字符串</param>
        /// <returns>清除后返回的字符串</returns>
        public static string ClearBR(string str)
        {
            //Regex r = null;
            Match m = null;
            // Regex RegexBr = new Regex(@"(\r\n)", RegexOptions.IgnoreCase);
            //r = new Regex(@"(\r\n)",RegexOptions.IgnoreCase);
            for (m = RegexBr.Match(str); m.Success; m = m.NextMatch())
            {
                str = str.Replace(m.Groups[0].ToString(), "");
            }


            return str;
        }


        /// <summary>
        /// 解析并获取页面的所有链接标题和地址
        /// </summary>
        /// <param name="htmlContent">HTML内容.</param>
        /// <returns>
        /// Dictionary&lt;System.String, System.String&gt;
        /// </returns>
        public static Dictionary<string, string> GetHtmlLinks(string htmlContent)
        {
            Dictionary<string, string> links = new Dictionary<string, string>();
            try
            {
                Regex r = new Regex(@"<a[^>]*href=(""(?<href>[^""]*)""|'(?<href>[^']*)'|(?<href>[^\s>]*))[^>]*>(?<text>.*?)</a>",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

                for (Match m = r.Match(htmlContent); m.Success; m = m.NextMatch())
                {
                    string href = m.Groups["href"].Value;
                    string text = m.Groups["text"].Value;
                    if (href != "" && href != "//" && href.ToLower().IndexOf("javascript:") < 0 && text != string.Empty)
                    {
                        links.Add(href, text);
                    }
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
            }
            return links;
        }


        /// <summary>
        /// 获取Html代码中的所有图片地址
        /// </summary>
        /// <param name="htmlContent">Content of the HTML.</param>
        /// <returns>
        /// System.Collections.Generic.List&lt;System.String&gt;
        /// </returns>
        public static List<string> GetHtmlSrcs(string htmlContent)
        {
            List<string> lstSrc = new List<string>();
            try
            {
                //Regex regsrc = new Regex(@"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<url>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>",
                //    RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);


                // Regex r = new Regex(@"<img\s+[^>]*\s*src\s*=\s*([']?)(?<url>\S+)'?[^>]*>", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);


                Regex regsrc = new Regex(@"\bsrc\b=['|""](?<Url>[^""&^']*?)['|""]",
                    RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

                for (Match m = regsrc.Match(htmlContent); m.Success; m = m.NextMatch())
                {
                    string src = m.Groups["Url"].Value;
                    if (!Utils.IsNullOrEmpty(src))
                    {
                        lstSrc.Add(src);
                    }
                    //str.SetValue(src, i);
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
            }
            return lstSrc;
        }

        /// <summary>
        /// 给html内容添加图片服务器地址
        /// </summary>
        /// <param name="htmlContent">Html内容.</param>
        /// <param name="imageServer">图片服务器地址.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string RepHtmlContentImage(string htmlContent, string imageServer)
        {
            if (Utils.IsNullOrEmpty(htmlContent)) return htmlContent;

            string imgDomain = imageServer;
            // 没有特殊指定图片服务器时不不需处理
            if (imgDomain == "") return htmlContent;

            string HtmlContent = htmlContent;

            Regex reImage = new Regex(@"\<img[^\>]+\>", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

            Regex reSrc = new Regex(@"\bsrc\b=['|""](?<Url>[^""&^']*?)['|""]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

            Regex relazySrc = new Regex(@"\blazy_src\b=['|""](?<Url>[^""&^']*?)['|""]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);


            // 图片控件完整Html、路径完整Html
            string ImageControlHtml = null;


            return reImage.Replace(HtmlContent, delegate(Match m)
            {
                try
                {
                    ImageControlHtml = m.Value;

                    Match mSrc = reSrc.Match(ImageControlHtml);
                    Match mlazySrc = relazySrc.Match(ImageControlHtml);

                    if (mSrc.Success) //对图片地Src进行处理
                    {
                        if (m.Value.IndexOf("http://") < 0)
                            return m.Value.Replace(mSrc.Groups["Url"].Value, imgDomain + mSrc.Groups["Url"].Value);

                    }

                    if (mlazySrc.Success) // 对延时加载的图片地址进行处理
                    {
                        if (m.Value.IndexOf("http://") < 0)
                            return m.Value.Replace(mlazySrc.Groups["Url"].Value, imgDomain + mlazySrc.Groups["Url"].Value);
                    }
                }
                catch (Exception ex)
                {
                    //new ECFException(ex);
                }

                return m.Value;
            });
        }

        //#region Fields
        //private readonly static Regex paragraphStartRegex = new Regex("<p>", RegexOptions.IgnoreCase);
        //private readonly static Regex paragraphEndRegex = new Regex("</p>", RegexOptions.IgnoreCase);
        ////private static Regex ampRegex = new Regex("&(?!(?:#[0-9]{2,4};|[a-z0-9]+;))", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        //#endregion

        //#region Utilities

        //private static string EnsureOnlyAllowedHtml(string text)
        //{
        //    if (String.IsNullOrEmpty(text))
        //        return string.Empty;

        //    const string allowedTags = "br,hr,b,i,u,a,div,ol,ul,li,blockquote,img,span,p,em,strong,font,pre,h1,h2,h3,h4,h5,h6,address,cite";

        //    var m = Regex.Matches(text, "<.*?>", RegexOptions.IgnoreCase);
        //    for (int i = m.Count - 1; i >= 0; i--)
        //    {
        //        string tag = text.Substring(m[i].Index + 1, m[i].Length - 1).Trim().ToLower();

        //        if (!IsValidTag(tag, allowedTags))
        //        {
        //            text = text.Remove(m[i].Index, m[i].Length);
        //        }
        //    }

        //    return text;
        //}

        //private static bool IsValidTag(string tag, string tags)
        //{
        //    string[] allowedTags = tags.Split(',');
        //    if (tag.IndexOf("javascript") >= 0) return false;
        //    if (tag.IndexOf("vbscript") >= 0) return false;
        //    if (tag.IndexOf("onclick") >= 0) return false;

        //    var endchars = new char[] { ' ', '>', '/', '\t' };

        //    int pos = tag.IndexOfAny(endchars, 1);
        //    if (pos > 0) tag = tag.Substring(0, pos);
        //    if (tag[0] == '/') tag = tag.Substring(1);

        //    foreach (string aTag in allowedTags)
        //    {
        //        if (tag == aTag) return true;
        //    }

        //    return false;
        //}
        //#endregion

        //#region Methods
        /////// <summary>
        /////// Formats the text
        /////// </summary>
        /////// <param name="text">Text</param>
        /////// <param name="stripTags">A value indicating whether to strip tags</param>
        /////// <param name="convertPlainTextToHtml">A value indicating whether HTML is allowed</param>
        /////// <param name="allowHtml">A value indicating whether HTML is allowed</param>
        /////// <param name="allowBBCode">A value indicating whether BBCode is allowed</param>
        /////// <param name="resolveLinks">A value indicating whether to resolve links</param>
        /////// <param name="addNoFollowTag">A value indicating whether to add "noFollow" tag</param>
        /////// <returns>Formatted text</returns>
        ////public static string FormatText(string text, bool stripTags,
        ////    bool convertPlainTextToHtml, bool allowHtml,
        ////    bool allowBBCode, bool resolveLinks, bool addNoFollowTag)
        ////{

        ////    if (String.IsNullOrEmpty(text))
        ////        return string.Empty;

        ////    try
        ////    {
        ////        if (stripTags)
        ////        {
        ////            text = HtmlHelper.StripTags(text);
        ////        }

        ////        if (allowHtml)
        ////        {
        ////            text = HtmlHelper.EnsureOnlyAllowedHtml(text);
        ////        }
        ////        else
        ////        {
        ////            text = HttpUtility.HtmlEncode(text);
        ////        }

        ////        if (convertPlainTextToHtml)
        ////        {
        ////            text = HtmlHelper.ConvertPlainTextToHtml(text);
        ////        }

        ////        if (allowBBCode)
        ////        {
        ////            text = BBCodeHelper.FormatText(text, true, true, true, true, true, true);
        ////        }

        ////        if (resolveLinks)
        ////        {
        ////            text = ResolveLinksHelper.FormatText(text);
        ////        }

        ////        if (addNoFollowTag)
        ////        {
        ////            //add noFollow tag. not implemented
        ////        }
        ////    }
        ////    catch (Exception exc)
        ////    {
        ////        text = string.Format("Text cannot be formatted. Error: {0}", exc.Message);
        ////    }
        ////    return text;
        ////}

        ///// <summary>
        ///// Strips tags
        ///// </summary>
        ///// <param name="text">Text</param>
        ///// <returns>Formatted text</returns>
        //public static string StripTags(string text)
        //{
        //    if (String.IsNullOrEmpty(text))
        //        return string.Empty;

        //    text = Regex.Replace(text, @"(>)(\r|\n)*(<)", "><");
        //    text = Regex.Replace(text, "(<[^>]*>)([^<]*)", "$2");
        //    text = Regex.Replace(text, "(&#x?[0-9]{2,4};|&quot;|&amp;|&nbsp;|&lt;|&gt;|&euro;|&copy;|&reg;|&permil;|&Dagger;|&dagger;|&lsaquo;|&rsaquo;|&bdquo;|&rdquo;|&ldquo;|&sbquo;|&rsquo;|&lsquo;|&mdash;|&ndash;|&rlm;|&lrm;|&zwj;|&zwnj;|&thinsp;|&emsp;|&ensp;|&tilde;|&circ;|&Yuml;|&scaron;|&Scaron;)", "@");

        //    return text;
        //}

        ///// <summary>
        ///// replace anchor text (remove a tag from the following url <a href="http://example.com">Name</a> and output only the string "Name")
        ///// </summary>
        ///// <param name="text">Text</param>
        ///// <returns>Text</returns>
        //public static string ReplaceAnchorTags(string text)
        //{
        //    if (String.IsNullOrEmpty(text))
        //        return string.Empty;

        //    text = Regex.Replace(text, @"<a\b[^>]+>([^<]*(?:(?!</a)<[^<]*)*)</a>", "$1", RegexOptions.IgnoreCase);
        //    return text;
        //}

        ///// <summary>
        ///// Converts plain text to HTML
        ///// </summary>
        ///// <param name="text">Text</param>
        ///// <returns>Formatted text</returns>
        //public static string ConvertPlainTextToHtml(string text)
        //{
        //    if (String.IsNullOrEmpty(text))
        //        return string.Empty;

        //    text = text.Replace("\r\n", "<br />");
        //    text = text.Replace("\r", "<br />");
        //    text = text.Replace("\n", "<br />");
        //    text = text.Replace("\t", "&nbsp;&nbsp;");
        //    text = text.Replace("  ", "&nbsp;&nbsp;");

        //    return text;
        //}

        ///// <summary>
        ///// Converts HTML to plain text
        ///// </summary>
        ///// <param name="text">Text</param>
        ///// <param name="decode">A value indicating whether to decode text</param>
        ///// <param name="replaceAnchorTags">A value indicating whether to replace anchor text (remove a tag from the following url <a href="http://example.com">Name</a> and output only the string "Name")</param>
        ///// <returns>Formatted text</returns>
        //public static string ConvertHtmlToPlainText(string text,
        //    bool decode = false, bool replaceAnchorTags = false)
        //{
        //    if (String.IsNullOrEmpty(text))
        //        return string.Empty;

        //    if (decode)
        //        text = HttpUtility.HtmlDecode(text);

        //    text = text.Replace("<br>", "\n");
        //    text = text.Replace("<br >", "\n");
        //    text = text.Replace("<br />", "\n");
        //    text = text.Replace("&nbsp;&nbsp;", "\t");
        //    text = text.Replace("&nbsp;&nbsp;", "  ");

        //    if (replaceAnchorTags)
        //        text = ReplaceAnchorTags(text);

        //    return text;
        //}

        ///// <summary>
        ///// Converts text to paragraph
        ///// </summary>
        ///// <param name="text">Text</param>
        ///// <returns>Formatted text</returns>
        //public static string ConvertPlainTextToParagraph(string text)
        //{
        //    if (String.IsNullOrEmpty(text))
        //        return string.Empty;

        //    text = paragraphStartRegex.Replace(text, string.Empty);
        //    text = paragraphEndRegex.Replace(text, "\n");
        //    text = text.Replace("\r\n", "\n").Replace("\r", "\n");
        //    text = text + "\n\n";
        //    text = text.Replace("\n\n", "\n");
        //    var strArray = text.Split(new char[] { '\n' });
        //    var builder = new StringBuilder();
        //    foreach (string str in strArray)
        //    {
        //        if ((str != null) && (str.Trim().Length > 0))
        //        {
        //            builder.AppendFormat("<p>{0}</p>\n", str);
        //        }
        //    }
        //    return builder.ToString();
        //}
        //#endregion



        #region FormatResolveLink
        /// <summary>
        /// Formats the text
        /// </summary>
        /// <param name="text">Text</param>
        /// <returns>Formatted text</returns>
        public static string FormatResolveLink(string text)
        {
            try
            {
                Regex regex = new Regex("((http://|https://|www\\.)([A-Z0-9.\\-]{1,})\\.[0-9A-Z?;~&\\(\\)#,=\\-_\\./\\+]{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);
                string link = "<a href=\"{0}{1}\" rel=\"nofollow\">{2}</a>";
                int MAX_LENGTH = 50;
                if (String.IsNullOrEmpty(text))
                    return string.Empty;

                var info = CultureInfo.InvariantCulture;
                foreach (Match match in regex.Matches(text))
                {
                    if (!match.Value.Contains("://"))
                    {
                        text = text.Replace(match.Value, string.Format(info, link, "http://", match.Value, UrlUtil.ShortenUrl(match.Value, MAX_LENGTH)));
                    }
                    else
                    {
                        text = text.Replace(match.Value, string.Format(info, link, string.Empty, match.Value, UrlUtil.ShortenUrl(match.Value, MAX_LENGTH)));
                    }
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex);
            }

            return text;
        }
        #endregion

    }
}
