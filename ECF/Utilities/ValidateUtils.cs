using System;
using System.IO;
using System.Text.RegularExpressions;

namespace ECF
{
    /// <summary>
    ///   <see cref="ECF.Utils"/>
    /// 常用判断处理单元
    /// Author:  XP
    /// Created: 2011/9/8
    /// </summary>
    public partial class Utils
    {
        /// <summary>
        /// 检测是否符合email格式
        /// </summary>
        /// <param name="strEmail">要判断的email字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsValidEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^[\w\.]+([-]\w+)*@[A-Za-z0-9-_]+[\.][A-Za-z0-9-_]");
        }

        /// <summary>
        /// Determines whether [is valid do email] [the specified STR email].
        /// </summary>
        /// <param name="strEmail">The STR email.</param>
        /// <returns>
        ///   <c>true</c> if [is valid do email] [the specified STR email]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsValidDoEmail(string strEmail)
        {
            return Regex.IsMatch(strEmail, @"^@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$");
        }

        /// <summary>
        /// 检测是否是正确的Url
        /// </summary>
        /// <param name="strUrl">要验证的Url</param>
        /// <returns>判断结果</returns>
        public static bool IsURL(string strUrl)
        {
            return Regex.IsMatch(strUrl, @"^(http|https)\://([a-zA-Z0-9\.\-]+(\:[a-zA-Z0-9\.&%\$\-]+)*@)*((25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9])\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[1-9]|0)\.(25[0-5]|2[0-4][0-9]|[0-1]{1}[0-9]{2}|[1-9]{1}[0-9]{1}|[0-9])|localhost|([a-zA-Z0-9\-]+\.)*[a-zA-Z0-9\-]+\.(com|edu|gov|int|mil|net|org|biz|arpa|info|name|pro|aero|coop|museum|[a-zA-Z]{1,10}))(\:[0-9]+)*(/($|[a-zA-Z0-9\.\,\?\'\\\+&%\$#\=~_\-]+))*$");
        }
        /// <summary>
        /// 验证是否为正整数
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsInt(string str)
        {
            return Regex.IsMatch(str, @"^[0-9]*$");
        }

        /// <summary>
        /// 判断是否为数据类型字符串
        /// </summary>
        /// <param name="strNumber">字符串.</param>
        /// <returns>
        /// System.Boolean
        /// </returns>
        public static bool IsNumber(String strNumber)
        {
            Regex objNotNumberPattern = new Regex("[^0-9.-]");
            Regex objTwoDotPattern = new Regex("[0-9]*[.][0-9]*[.][0-9]*");
            Regex objTwoMinusPattern = new Regex("[0-9]*[-][0-9]*[-][0-9]*");
            String strValidRealPattern = "^([-]|[.]|[-.]|[0-9])[0-9]*[.]*[0-9]+$";
            String strValidIntegerPattern = "^([-]|[0-9])[0-9]*$";
            Regex objNumberPattern = new Regex("(" + strValidRealPattern + ")|(" + strValidIntegerPattern + ")");

            return !objNotNumberPattern.IsMatch(strNumber) &&
                   !objTwoDotPattern.IsMatch(strNumber) &&
                   !objTwoMinusPattern.IsMatch(strNumber) &&
                   objNumberPattern.IsMatch(strNumber);
        }

        #region IsImgFilename 判断文件名是否为浏览器可以直接显示的图片文件名

        /// <summary>
        /// 判断文件名是否为浏览器可以直接显示的图片文件名
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <returns>是否可以直接显示</returns>
        public static bool IsImgFilename(string filename)
        {
            filename = filename.Trim();
            if (filename.EndsWith(".") || filename.IndexOf(".") == -1)
            {
                return false;
            }
            string extname = filename.Substring(filename.LastIndexOf(".") + 1).ToLower();
            return (extname == "jpg" || extname == "jpeg" || extname == "png" || extname == "bmp" || extname == "gif");
        }

        #endregion

        /// <summary>
        /// 判断是否为base64字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsBase64String(string str)
        {
            //A-Z, a-z, 0-9, +, /, =
            return Regex.IsMatch(str, @"[A-Za-z0-9\+\/\=]");
        }
        /// <summary>
        /// 检测是否有Sql危险字符
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeSqlString(string str)
        {
            return !Regex.IsMatch(str, @"[-|;|,|\/|\(|\)|\[|\]|\}|\{|%|@|\*|!|\']");
        }

        /// <summary>
        /// 检测是否有危险的可能用于链接的字符串
        /// </summary>
        /// <param name="str">要判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsSafeUserInfoString(string str)
        {
            return !Regex.IsMatch(str, @"^\s*$|^c:\\con\\con$|[%,\*" + "\"" + @"\s\t\<\>\&]|游客|^Guest");
        }
        /// <summary>
        /// 是否为ip
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
        }

        /// <summary>
        /// Determines whether [is IP sect] [the specified ip].
        /// </summary>
        /// <param name="ip">The ip.</param>
        /// <returns>
        ///   <c>true</c> if [is IP sect] [the specified ip]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIPSect(string ip)
        {
            return Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){2}((2[0-4]\d|25[0-5]|[01]?\d\d?|\*)\.)(2[0-4]\d|25[0-5]|[01]?\d\d?|\*)$");
        }


        /// <summary>
        /// 判断字符串是否是yy-mm-dd字符串
        /// </summary>
        /// <param name="str">待判断字符串</param>
        /// <returns>判断结果</returns>
        public static bool IsDateString(string str)
        {
            return Regex.IsMatch(str, @"(\d{4})-(\d{1,2})-(\d{1,2})");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool IsTime(string timeval)
        {
            return Regex.IsMatch(timeval, @"^((([0-1]?[0-9])|(2[0-3])):([0-5]?[0-9])(:[0-5]?[0-9])?)$");
        }

        /// <summary>
        /// 判断文件流是否为UTF8字符集
        /// </summary>
        /// <param name="sbInputStream">文件流</param>
        /// <returns>判断结果</returns>
        private static bool IsUTF8(FileStream sbInputStream)
        {
            int i;
            byte cOctets;  // octets to go in this UTF-8 encoded character 
            byte chr;
            bool bAllAscii = true;
            long iLen = sbInputStream.Length;

            cOctets = 0;
            try
            {
                for (i = 0; i < iLen; i++)
                {
                    chr = (byte)sbInputStream.ReadByte();

                    if ((chr & 0x80) != 0) bAllAscii = false;

                    if (cOctets == 0)
                    {
                        if (chr >= 0x80)
                        {
                            do
                            {
                                chr <<= 1;
                                cOctets++;
                            }
                            while ((chr & 0x80) != 0);

                            cOctets--;
                            if (cOctets == 0)
                                return false;
                        }
                    }
                    else
                    {
                        if ((chr & 0xC0) != 0x80)
                            return false;

                        cOctets--;
                    }
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
            }

            if (cOctets > 0)
                return false;

            if (bAllAscii)
                return false;

            return true;
        }


        #region 检测对象是否为空

        /// <summary>
        /// 检测是否为空，为空返回true
        /// </summary>
        /// <param name="data">要验证的对象</param>
        public static bool IsNullOrEmpty(object data)
        {
            //如果为null
            if (data == null)
            {
                return true;
            }

            //如果为""
            if (data.ToString() == "")
            {
                return true;
            }

            //如果为DBNull
            if (data.GetType() == typeof(DBNull))
            {
                return true;
            }

            //不为空
            return false;
        }

        /// <summary>
        /// 检测是否为空，为空返回true
        /// </summary>
        /// <param name="data">要验证的字符串</param>
        public static bool IsNullOrEmpty(string data)
        {
            //检测是否为null
            if (data == null)
            {
                return true;
            }

            //检测是否为""
            if (string.IsNullOrEmpty(data.Trim()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }



		/// <summary>
		/// 检测是否为空，为空返回true
		/// </summary>
		/// <param name="data">要验证的值</param>
		/// <returns></returns>
		public static bool IsNullOrEmpty(Guid data)
        {

            //检测是否为空值
            if (data == Guid.Empty)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测时期是否为空，为空返回true
        /// </summary>
        /// <param name="data">要验证的日期</param>
        public static bool IsNullOrEmpty(DateTime data)
        {
            if (data == DateTime.MinValue)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 检测时期是否为空，为空返回true
        /// </summary>
        /// <param name="data">要验证的日期</param>
        public static bool IsNullOrEmpty(DateTime? data)
        {
            //检测是否为null
            if (data == null)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}
