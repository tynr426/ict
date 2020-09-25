using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Collections;

namespace ECF
{
    /// <summary>
    ///   <see cref="ECF.Utils"/>
    /// 字符串处理单元
    /// Author:  XP
    /// Created: 2011/9/8
    /// </summary>
    public partial class Utils
    {
        #region RTrim 删除字符串尾部的回车/换行/空格
        /// <summary>
        /// 删除字符串尾部的回车/换行/空格
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RTrim(string str)
        {
            for (int i = str.Length; i >= 0; i--)
            {
                if (str[i].Equals(" ") || str[i].Equals("\r") || str[i].Equals("\n"))
                {
                    str.Remove(i, 1);
                }
            }
            return str;
        }
        #endregion

        #region SubString字符串截取

        /// <summary>
        /// 字符串截取
        /// 2009-3-5 by Shaipe
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="length">The length.</param>
        /// <returns>System.String</returns>
        public static string SubString(string content, int length)
        {

            System.Text.StringBuilder sb = null;
            try
            {
                sb = new System.Text.StringBuilder();
                System.Text.Encoding encoding = System.Text.Encoding.GetEncoding("gb2312");
                int totalLength = 0;
                foreach (char contentChar in content)
                {
                    int size = encoding.GetByteCount(new char[] { contentChar }); //获得1或2，中文2，英文1
                    if (totalLength + size > length - 2)
                    {
                        sb.Append("...");
                        break;
                    }
                    sb.Append(contentChar);
                    totalLength += size;
                }
            }
            catch
            {
                sb.Append("");
            }
            return sb.ToString();
        }
        #endregion

        #region GetSubString 字符串如果操过指定长度则将超出的部分用指定字符串代替

        /// <summary>
        /// 字符串如果操过指定长度则将超出的部分用指定字符串代替
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_Length, string p_TailString)
        {
            return GetSubString(p_SrcString, 0, p_Length, p_TailString);
        }

        /// <summary>
        /// 取指定长度的字符串
        /// </summary>
        /// <param name="p_SrcString">要检查的字符串</param>
        /// <param name="p_StartIndex">起始位置</param>
        /// <param name="p_Length">指定长度</param>
        /// <param name="p_TailString">用于替换的字符串</param>
        /// <returns>截取后的字符串</returns>
        public static string GetSubString(string p_SrcString, int p_StartIndex, int p_Length, string p_TailString)
        {
            string myResult = p_SrcString;

            try
            {
                Byte[] bComments = Encoding.UTF8.GetBytes(p_SrcString);
                foreach (char c in Encoding.UTF8.GetChars(bComments))
                {    //当是日文或韩文时(注:中文的范围:\u4e00 - \u9fa5, 日文在\u0800 - \u4e00, 韩文为\xAC00-\xD7A3)
                    if ((c > '\u0800' && c < '\u4e00') || (c > '\xAC00' && c < '\xD7A3'))
                    {
                        //if (System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\u0800-\u4e00]+") || System.Text.RegularExpressions.Regex.IsMatch(p_SrcString, "[\xAC00-\xD7A3]+"))
                        //当截取的起始位置超出字段串长度时
                        if (p_StartIndex >= p_SrcString.Length)
                        {
                            return "";
                        }
                        else
                        {
                            return p_SrcString.Substring(p_StartIndex,
                                                           ((p_Length + p_StartIndex) > p_SrcString.Length) ? (p_SrcString.Length - p_StartIndex) : p_Length);
                        }
                    }
                }


                if (p_Length >= 0)
                {
                    byte[] bsSrcString = Encoding.Default.GetBytes(p_SrcString);

                    //当字符串长度大于起始位置
                    if (bsSrcString.Length > p_StartIndex)
                    {
                        int p_EndIndex = bsSrcString.Length;

                        //当要截取的长度在字符串的有效长度范围内
                        if (bsSrcString.Length > (p_StartIndex + p_Length))
                        {
                            p_EndIndex = p_Length + p_StartIndex;
                        }
                        else
                        {   //当不在有效范围内时,只取到字符串的结尾

                            p_Length = bsSrcString.Length - p_StartIndex;
                            p_TailString = "";
                        }



                        int nRealLength = p_Length;
                        int[] anResultFlag = new int[p_Length];
                        byte[] bsResult = null;

                        int nFlag = 0;
                        for (int i = p_StartIndex; i < p_EndIndex; i++)
                        {

                            if (bsSrcString[i] > 127)
                            {
                                nFlag++;
                                if (nFlag == 3)
                                {
                                    nFlag = 1;
                                }
                            }
                            else
                            {
                                nFlag = 0;
                            }

                            anResultFlag[i] = nFlag;
                        }

                        if ((bsSrcString[p_EndIndex - 1] > 127) && (anResultFlag[p_Length - 1] == 1))
                        {
                            nRealLength = p_Length + 1;
                        }

                        bsResult = new byte[nRealLength];

                        Array.Copy(bsSrcString, p_StartIndex, bsResult, 0, nRealLength);

                        myResult = Encoding.Default.GetString(bsResult);

                        myResult = myResult + p_TailString;
                    }
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
            }

            return myResult;
        }


        #endregion

        #region CutString 从字符串的指定位置截取指定长度的子字符串

        /// <summary>
        /// 从字符串的指定位置截取指定长度的子字符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <param name="length">子字符串的长度</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex, int length)
        {
            try
            {
                if (startIndex >= 0)
                {
                    if (length < 0)
                    {
                        length = length * -1;
                        if (startIndex - length < 0)
                        {
                            length = startIndex;
                            startIndex = 0;
                        }
                        else
                        {
                            startIndex = startIndex - length;
                        }
                    }


                    if (startIndex > str.Length)
                    {
                        return "";
                    }


                }
                else
                {
                    if (length < 0)
                    {
                        return "";
                    }
                    else
                    {
                        if (length + startIndex > 0)
                        {
                            length = length + startIndex;
                            startIndex = 0;
                        }
                        else
                        {
                            return "";
                        }
                    }
                }

                if (str.Length - startIndex < length)
                {
                    length = str.Length - startIndex;
                }

                return str.Substring(startIndex, length);
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
                return str;
            }
        }

        /// <summary>
        /// 从字符串的指定位置开始截取到字符串结尾的了符串
        /// </summary>
        /// <param name="str">原字符串</param>
        /// <param name="startIndex">子字符串的起始位置</param>
        /// <returns>子字符串</returns>
        public static string CutString(string str, int startIndex)
        {
            return CutString(str, startIndex, str.Length);
        }

        #endregion

        #region GetStringLength 返回字符串真实长度

        /// <summary>
        /// 返回字符串真实长度, 1个汉字长度为2
        /// </summary>
        /// <returns>字符长度</returns>
        public static int GetStringLength(string str)
        {
            return Encoding.Default.GetBytes(str).Length;
        }

        #endregion

        #region SplitString 分割字符串

        /// <summary>
        /// 分割字符串
        /// </summary>
        public static string[] SplitString(string strContent, string strSplit)
        {
            if (!String.IsNullOrEmpty(strContent))
            {
                if (strContent.IndexOf(strSplit) < 0)
                {
                    string[] tmp = { strContent };
                    return tmp;
                }
                return Regex.Split(strContent, Regex.Escape(strSplit), RegexOptions.IgnoreCase);
            }
            else
            {
                return new string[0] { };
            }
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, int count)
        {
            string[] result = new string[count];

            string[] splited = SplitString(strContent, strSplit);

            for (int i = 0; i < count; i++)
            {
                if (i < splited.Length)
                    result[i] = splited[i];
                else
                    result[i] = string.Empty;
            }

            return result;
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">被分割的字符串</param>
        /// <param name="strSplit">分割符</param>
        /// <param name="ignoreRepeatItem">忽略重复项</param>
        /// <param name="maxElementLength">单个元素最大长度</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem, int maxElementLength)
        {
            string[] result = SplitString(strContent, strSplit);

            return ignoreRepeatItem ? DistinctStringArray(result, maxElementLength) : result;
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">Content of the STR.</param>
        /// <param name="strSplit">The STR split.</param>
        /// <param name="ignoreRepeatItem">if set to <c>true</c> [ignore repeat item].</param>
        /// <param name="minElementLength">Length of the min element.</param>
        /// <param name="maxElementLength">Length of the max element.</param>
        /// <returns>System.String[]</returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem, int minElementLength, int maxElementLength)
        {
            string[] result = SplitString(strContent, strSplit);

            if (ignoreRepeatItem)
            {
                result = DistinctStringArray(result);
            }
            return PadStringArray(result, minElementLength, maxElementLength);
        }

        /// <summary>
        /// 分割字符串
        /// </summary>
        /// <param name="strContent">被分割的字符串</param>
        /// <param name="strSplit">分割符</param>
        /// <param name="ignoreRepeatItem">忽略重复项</param>
        /// <returns></returns>
        public static string[] SplitString(string strContent, string strSplit, bool ignoreRepeatItem)
        {
            return SplitString(strContent, strSplit, ignoreRepeatItem, 0);
        }

        #endregion

        #region StrFilter 进行指定的替换
        /// <summary>
        /// 进行指定的替换(脏字过滤)脏字间用回车隔开
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <param name="bantext">The bantext.</param>
        /// <returns>System.String</returns>
        public static string StrFilter(string str, string bantext)
        {
            try
            {
                string text1 = "";
                string text2 = "";
                string[] textArray1 = SplitString(bantext, "\r\n");
                for (int num1 = 0; num1 < textArray1.Length; num1++)
                {
                    text1 = textArray1[num1].Substring(0, textArray1[num1].IndexOf("="));
                    text2 = textArray1[num1].Substring(textArray1[num1].IndexOf("=") + 1);
                    str = str.Replace(text1, text2);
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
            }
            return str;
        }
        #endregion

        #region ToTitleCase 对字条串进行首字母大写转换
        /// <summary>
        /// 对字条串进行首字母大写转换.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns></returns>
        public static string ToTitleCase(string text)
        {
            System.Globalization.CultureInfo cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;

            System.Globalization.TextInfo textInfo = cultureInfo.TextInfo;

            return textInfo.ToTitleCase(text);
        }
        #endregion

        #region MergeString 合并字符
        /// <summary>
        /// 合并字符
        /// </summary>
        /// <param name="source">要合并的源字符串</param>
        /// <param name="target">要被合并到的目的字符串</param>
        /// <returns>
        /// 合并到的目的字符串
        /// </returns>
        public static string MergeString(string source, string target)
        {
            return MergeString(source, target, ",");
        }

        /// <summary>
        /// 合并字符
        /// </summary>
        /// <param name="source">要合并的源字符串</param>
        /// <param name="target">要被合并到的目的字符串</param>
        /// <param name="mergechar">合并符</param>
        /// <returns>并到字符串</returns>
        public static string MergeString(string source, string target, string mergechar)
        {
            if (String.IsNullOrEmpty(target))
                target = source;
            else
                target += mergechar + source;

            return target;
        }
        #endregion

        #region GetNewSeed 获取一个临时数


        /// <summary>
        /// 获取一个临时数
        /// </summary>
        /// <returns>
        /// System.Int32
        /// </returns>
        private static int GetNewSeed()
        {
            byte[] rndBytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(rndBytes);
            return BitConverter.ToInt32(rndBytes, 0);
        }



        /// <summary>
        /// 字节常量
        /// </summary>
        private static char[] constant ={   '0','1','2','3','4','5','6','7','8','9',
                                            'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
                                            'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
                                        };

        /// <summary>
        /// 获取随机码
        /// </summary>
        /// <param name="Length">The length.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string RndString(int Length)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);
            Random rd = new Random(GetNewSeed());
            for (int i = 0; i < Length; i++)
            {
                newRandom.Append(constant[rd.Next(0, 62)]);
            }
            return newRandom.ToString();
        }


        /// <summary>
        /// 获取指定位数的随机数,只返回数字
        /// </summary>
        /// <param name="length">获取位数.</param>
        /// <returns></returns>
        public static string RndNumber(int length)
        {
            int max = 0;
            string mstr = "";
            for (int i = 0; i < length; i++)
            {
                mstr += "9";
            }
            max = Utils.ToInt(mstr);
            Random rd = new Random(GetNewSeed());

            string ret = rd.Next(max).ToString();

            return ret.PadLeft(length, '0');
        }


        #endregion

        #region GetNumber 获取字符串中的数字
        /// <summary>
        /// 获取字符串中的数字
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        private int GetNumber(string str)
        {
            string result = string.Empty;
            Match m = Regex.Match(str, @"\d+");
            if (m.Success)
            {
                result = m.Value;
            }
            return Utils.ToInt(result);
        }
        #endregion


        // possible types:
        // http://msdn.microsoft.com/en-us/library/system.data.datacolumn.datatype(VS.80).aspx
        private static Type[] numeric = new Type[] {typeof(byte), typeof(decimal), typeof(double),
                                     typeof(Int16), typeof(Int32), typeof(SByte), typeof(Single),
                                     typeof(UInt16), typeof(UInt32), typeof(UInt64),typeof(int)};

        private static long EpochTicks = new DateTime(1970, 1, 1).Ticks;

        /// <summary>
        ///  获取Json格式的数据
        ///  Author :   XP-PC/Shaipe
        ///  Created:  07-30-2014
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="DataType">Type of the data.</param>
        /// <param name="unicode">if set to <c>true</c> [unicode].</param>
        /// <returns>System.String</returns>
        public static string JsonValueOfType(object value, Type DataType, bool unicode = true)
        {

            try
            {
                if (value == DBNull.Value) return "null";


                if (Array.IndexOf(numeric, DataType) > -1)
                {
                    if (value is decimal)
                    {
                        return ((decimal)value).ToString();
                    }
                    else
                    {
                        return ToString(value);
                    }
                }

                if (DataType == typeof(long))
                    return value.ToString();


                // boolean
                if (DataType == typeof(bool))
                    return ((bool)value) ? "true" : "false";

                if (DataType == typeof(byte[]))
                {
                    byte[] temp = value as byte[];

                    return string.Join("", temp);
                }


                // date -- see http://weblogs.asp.net/bleroy/archive/2008/01/18/dates-and-json.aspx  \\/Date(" + new TimeSpan(((DateTime)value).ToUniversalTime().Ticks - EpochTicks).TotalMilliseconds.ToString() + ")\\/
                if (DataType == typeof(DateTime))
                    return "\"" + value + "\"";


                if (value.ToString().IsJson())  // 若字符串为Json格式字符串直接输出不转换为字符串形式输出
                {
                    if (unicode)
                        return Utils.ChineseToUnicode(value.ToString(), false);
                    else
                        return value.ToString();
                }
                else
                {
                    char emptyChar = (char)127;

                    value = value.ToString().Replace(@"\", @"\\")
                   .Replace(Environment.NewLine, @"\n").Replace("\"", @"\""")
                   .Replace((char)13, emptyChar).Replace((char)10, emptyChar).Replace((char)9, emptyChar);

                    if (unicode)
                        return "\"" + Utils.ChineseToUnicode(value.ToString(), false) + "\"";
                    else
                        return "\"" + value.ToString() + "\"";
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex.Message, ex);
                return "";
            }
        }


        /// <summary>
		/// 转换成Xml格式的字符串
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="val">The val.</param>
		/// <returns></returns>
		public static string ConvertXmlData(string name, string val)
        {

            if (name != string.Empty)
            {
                if (val == null) val = "";

                if (val.IndexOf("<") > -1 || val.IndexOf(">") > -1 || val.IndexOf("&") > -1 || val.IndexOf("\"") > -1 || val.IndexOf("'") > -1)
                {
                    if (val.IndexOf("]]>") > -1) //如果已是xml格式且其中已含有CData的数据就不再添加Cdata
                    {
                        return "<" + name + ">" + val + "</" + name + ">";
                    }
                    else
                    {
                        return "<" + name + "><![CDATA[" + (val) + "]]></" + name + ">";
                    }
                }
                else
                {
                    return "<" + name + ">" + val + "</" + name + ">";
                }
            }
            return "";
        }


        /// <summary>
        /// 转换为Json数据格式.
        /// Author :   XP-PC/Shaipe
        /// Created:  07-30-2014
        /// </summary>
        /// <param name="enumerable">可以.</param>
        /// <param name="unicode">if set to <c>true</c> [unicode].</param>
        /// <returns>
        /// System.String.
        /// </returns>
        public static string ConvertJsonData(IEnumerable enumerable, bool unicode)
        {
            if (enumerable == null)
            {
                return null;
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            try
            {
                IEnumerator DataEnumerator = enumerable.GetEnumerator();
                int row = 0;
                while (DataEnumerator.MoveNext())
                {
                    object o = DataEnumerator.Current;
                    Type type = o.GetType();
                    if (type.IsValueType || type.Name.StartsWith("String"))
                    {
                        sb.Append(JsonValueOfType(o, type, unicode));
                    }
                    if (type.GetInterface("IEntity", false) != null)
                    {
                        sb.Append((row == 0 ? "" : ",") + ((IEntity)o).ToJson());
                    }

                    else
                    {
                        sb.Append((row == 0 ? "" : ",") + "\"" + o.ToString() + "\"");
                    }
                    row++;
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
            }

            sb.Append("]");

            return sb.ToString();
        }
    }
}
