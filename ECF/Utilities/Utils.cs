using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
namespace ECF
{
	/// <summary>
	///   <see cref="ECF.Utils"/>
	/// utils
	/// Author:  XP
	/// Created: 2011/9/2
	/// </summary>
	public partial class Utils
	{
		#region 将数据转换为字符串

		/// <summary>
		/// 将数据转换为字符串
		/// </summary>
		/// <param name="data">数据</param>
		public static string ToString(object data)
		{
			return ToString(data, false);
		}

		/// <summary>
		/// 将数据转换为字符串
		/// </summary>
		/// <param name="data">数据</param>
		/// <param name="isFormatNumber">是否格式化浮点数,如果为true,则格式化，保留浮点数的两位小数，
		/// 小数部分为0则不显示,范例：1.00显示为1,1.55显示为1.55</param>
		public static string ToString(object data, bool isFormatNumber)
		{
            try
            {
                //有效性验证
                if (data == null)
                {
                    return string.Empty;
                }

                //格式化
                if (isFormatNumber)
                {
                    return string.Format("{0:.##}", data);
                }
                else
                {
                    return data.ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex.Message, ex);
                return string.Empty;
            }
		}

		/// <summary>
		/// Returns a <see cref="System.String" /> that represents this instance.
		/// </summary>
		/// <param name="data">The data.</param>
		/// <param name="format">The format.</param>
		/// <returns>
		/// A <see cref="System.String" /> that represents this instance.
		/// </returns>
		public static string ToString(object data, string format)
		{
			if (data == null)
				return string.Empty;

			if (String.IsNullOrEmpty(format))
			{
				return data.ToString().Trim();
			}
			//输出格式化
			return string.Format(format, data);
		}

		#endregion


		#region SQLFilter
		/// <summary>
		/// The SQL filter pattern
		/// </summary>
		const string SqlFilterPattern = @"insert|delete|drop table|update|truncate|asc\(|mid\(|char\(|xp_cmdshell|exec master|netlocalgroup administrators|net user|or|and";

		/// <summary>
		/// 数据库安全字符过滤.
		/// </summary>
		/// <param name="str">The string.</param>
		/// <returns>System.String.</returns>
		public static string SqlFilter(string str)
		{
			if (string.IsNullOrEmpty(str)) return str;

			if (Regex.IsMatch(str, SqlFilterPattern, RegexOptions.IgnoreCase))
			{
				string[] patterns = SqlFilterPattern.Split('|');
				for (int i = 0; i < patterns.Length; i++)
				{
					Regex regex = new Regex(@"\s+" + patterns[i] + @"\s+", RegexOptions.IgnoreCase | RegexOptions.Multiline);
					str = regex.Replace(" " + str, " ").Trim();
				}

			}
			return str.Replace("';", "");
		}



		#endregion

		#region Escape 加解密

		/// <summary>
		/// Escape 加密
		/// </summary>
		/// <param name="s">The s.</param>
		/// <returns>
		/// System.String
		/// </returns>
		public static string Escape(string s)
		{
            try
            {
                StringBuilder sb = new StringBuilder();
                byte[] byteArr = System.Text.Encoding.Unicode.GetBytes(s);

                for (int i = 0; i < byteArr.Length; i += 2)
                {

                    sb.Append("\\u");
                    sb.Append(byteArr[i + 1].ToString("x2"));//把字节转换为十六进制的字符串表现形式

                    sb.Append(byteArr[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
                return s;
            }
        }


		/// <summary>
		/// Un escape 解决
		/// </summary>
		/// <param name="s">The s.</param>
		/// <returns>
		/// System.String
		/// </returns>
		public static string UnEscape(string s)
		{
            try
            {
                string str = s.Remove(0, 2);//删除最前面两个＂%u＂
                string[] strArr = str.Split(new string[] { "\\u" }, StringSplitOptions.None);//以子字符串＂%u＂分隔
                byte[] byteArr = new byte[strArr.Length * 2];
                for (int i = 0, j = 0; i < strArr.Length; i++, j += 2)
                {
                    byteArr[j + 1] = Convert.ToByte(strArr[i].Substring(0, 2), 16); //把十六进制形式的字串符串转换为二进制字节
                    byteArr[j] = Convert.ToByte(strArr[i].Substring(2, 2), 16);
                }
                str = System.Text.Encoding.Unicode.GetString(byteArr); //把字节转为unicode编码
                return str;
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
                return s;
            }
        }
		#endregion

		/// <summary>
		/// 中文转为UNICODE字符
		/// </summary>
		/// <param name="input">中文字条.</param>
		/// <param name="isSemiangle">是否转换非中文字符.</param>
		/// <returns>
		/// System.String
		/// </returns>
		public static string ChineseToUnicode(string input, bool isSemiangle)
		{
			StringBuilder sb = new StringBuilder();
			if (input != null)
			{
                try
                {
                    input = input.Replace("&amp;", "&");

                    int chfrom = Convert.ToInt32("4e00", 16);    //范围（0x4e00～0x9fff）转换成int（chfrom～chend）
                    int chend = Convert.ToInt32("9fff", 16);
                    for (int i = 0; i < input.Length; i++)
                    {
                        //将中文字符转为10进制整数，然后转为16进制unicode字符
                        string hcode = ((int)input[i]).ToString("x2");

                        if (hcode.Length == 2)
                        {
                            hcode = "00" + hcode;
                        }

                        if (!isSemiangle)
                        {
                            //126 ~ 33 ! 64 @ 35 # 36 $ 37 % 94 ^ 38 & 42 * 40 ( 41 ) 44 , 46 . 47 / 59 ; 39 ' 34 " 58 : 60 < 62 > 63 ? 123 { 125 } 92 \ 124 |
                            int code = Convert.ToInt32(hcode, 16);    //获得字符串input中指定索引index处字符unicode编码
                            List<int> codes = new List<int>()
                        {
                            124
                        };
                            //sb.Append(" " + code + " ");
                            if (code >= chfrom && code <= chend)
                            {
                                sb.Append("\\u" + hcode);
                            }
                            else if (codes.Contains(code))// 处理特殊字符
                            {
                                sb.Append("\\u" + hcode);
                            }
                            else
                            {
                                sb.Append(input[i]);
                            }
                        }
                        else
                        {
                            sb.Append("\\u" + hcode);
                        }

                    }
                }
                catch (Exception ex)
                {
                    ////new ECFException(ex);
                }
            }
			return sb.ToString();
		}

		/// <summary>
		/// UNICODE字符转为中文
		/// </summary>
		/// <param name="input">需要转换的Unicode字符串.</param>
		/// <returns>
		/// System.String
		/// </returns>
		public static string UnicodeToChinese(string input)
		{
			StringBuilder sb = new StringBuilder();
			if (input != null)
			{
				string[] strlist = input.Replace("\\u", "|").Split('|');
				try
				{
					if (!Utils.IsNullOrEmpty(strlist[0]))
					{
						sb.Append(strlist[0]);
					}

					for (int i = 1; i < strlist.Length; i++)
					{
						string hs = strlist[i], ts, rs;
						if (hs.Length > 4)
						{
							ts = hs.Substring(0, 4);
							rs = hs.Remove(0, 4);
							//将unicode字符转为10进制整数，然后转为char中文字符
							sb.Append((char)int.Parse(ts, System.Globalization.NumberStyles.HexNumber) + rs);
						}
						else if (hs.Length == 4)
						{
							sb.Append((char)int.Parse(hs, System.Globalization.NumberStyles.HexNumber));
						}
						else
						{
							sb.Append(hs);
						}
					}
				}
				catch (FormatException ex)
				{
					////new ECFException(ex.Message, ex);
				}
			}
			return sb.ToString();
		}

		#region GetByteCount 字节数量获取

		/// <summary>
		/// 获取内容的单字节数量
		/// </summary>
		/// <param name="Content">具体内容</param>
		/// <returns></returns>
		public static int GetByteCount(string Content)
		{
			if (String.IsNullOrEmpty(Content))
				return 0;

			int nCount = 0;

            try
            {
                char[] charContent = Content.ToCharArray();

                for (int i = 0; i < charContent.Length; i++)
                {
                    if (charContent[i] > 255)
                    {
                        nCount += 2;
                    }
                    else
                    {
                        ++nCount;
                    }
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
            }

            return nCount;
		}

		/// <summary>
		/// 获取内容的单字节数量
		/// </summary>
		/// <param name="charContent">具体内容字符数组</param>
		/// <param name="Index">其实索引</param>
		/// <param name="Count">字符数量</param>
		/// <returns></returns>
		public static int GetByteCount(char[] charContent, int Index, int Count)
		{
			if (charContent == null || charContent.Length == 0)
				return 0;

			int nCount = 0;

            try
            {
                if (Index >= charContent.Length)
                    return 0;

                for (int i = Index; i < charContent.Length && i < Count; i++)
                {
                    if (charContent[i] > 255)
                    {
                        nCount += 2;
                    }
                    else
                    {
                        ++nCount;
                    }
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
            }

            return nCount;
		}

		#endregion

		#region GetEncoding 根据字符串获取编码类型
		/// <summary>
		/// 根据字符串获取编码类型
		/// by XP-PC 2012/5/27
		/// </summary>
		/// <param name="charSet">The char set.</param>
		/// <returns>
		/// System.Text.Encoding
		/// </returns>
		static public System.Text.Encoding GetEncoding(string charSet)
		{
			if (Regex.IsMatch(charSet, ".*ascii.*", RegexOptions.IgnoreCase))
				return System.Text.Encoding.Default;
			try
			{
				return System.Text.Encoding.GetEncoding(charSet);
			}
			catch
			{
				return System.Text.Encoding.Default;
			}
		}
		#endregion

	}
}
