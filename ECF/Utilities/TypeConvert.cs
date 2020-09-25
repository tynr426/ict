using System;
using System.Drawing;
using System.Text.RegularExpressions;

namespace ECF
{
    /// <summary>
    ///   <see cref="ECF.Utils.Utils"/>
    /// utils 类型转换处理单元
    /// Author:  XP
    /// Created: 2011/9/1
    /// </summary>
    public partial class Utils
    {
        #region 将全角数字转换为数字

        /// <summary>
        /// 将全角数字转换为数字
        /// </summary>
        /// <param name="SBCCase"></param>
        /// <returns></returns>
        public static string SBCCaseToNumberic(string SBCCase)
        {
            try
            {
                char[] c = SBCCase.ToCharArray();
                for (int i = 0; i < c.Length; i++)
                {
                    byte[] b = System.Text.Encoding.Unicode.GetBytes(c, i, 1);
                    if (b.Length == 2)
                    {
                        if (b[1] == 255)
                        {
                            b[0] = (byte)(b[0] + 32);
                            b[1] = 0;
                            c[i] = System.Text.Encoding.Unicode.GetChars(b)[0];
                        }
                    }
                }
                return new string(c);
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
                return SBCCase;
            }
        }

        #endregion

        #region ToColor 将字符串转换为Color

        /// <summary>
        /// 将字符串转换为Color
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static Color ToColor(string color)
        {
            int red, green, blue = 0;
            char[] rgb;
            color = color.TrimStart('#');
            color = Regex.Replace(color.ToLower(), "[g-zG-Z]", "");
            switch (color.Length)
            {
                case 3:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[0].ToString(), 16);
                    green = Convert.ToInt32(rgb[1].ToString() + rgb[1].ToString(), 16);
                    blue = Convert.ToInt32(rgb[2].ToString() + rgb[2].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                case 6:
                    rgb = color.ToCharArray();
                    red = Convert.ToInt32(rgb[0].ToString() + rgb[1].ToString(), 16);
                    green = Convert.ToInt32(rgb[2].ToString() + rgb[3].ToString(), 16);
                    blue = Convert.ToInt32(rgb[4].ToString() + rgb[5].ToString(), 16);
                    return Color.FromArgb(red, green, blue);
                default:
                    return Color.FromName(color);

            }
        }

        #endregion

        #region ToBool
        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool ToBool(object expression)
        {
            if (expression != null)
            {
                return ToBool(expression, false);
            }
            return false;
        }
        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool ToBool(object expression, bool defValue)
        {
            if (expression != null)
            {
                return ToBool(expression.ToString(), defValue);
            }
            return defValue;
        }

        /// <summary>
        /// string型转换为bool型
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的bool类型结果</returns>
        public static bool ToBool(string expression, bool defValue)
        {
            if (expression != null)
            {
                if (string.Compare(expression, "true", true) == 0 || string.Compare(expression, "1", true) == 0)
                {
                    return true;
                }
                else if (string.Compare(expression, "false", true) == 0)
                {
                    return false;
                }
            }
            return defValue;
        }
        #endregion

        #region ToInt
        /// <summary>
        /// 将对象转换为Int32类型,缺省值0
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ToInt(object expression)
        {
            if (expression != null)
            {

                return ToInt(expression.ToString(), 0);

            }
            return 0;
        }
        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ToInt(object expression, int defValue)
        {
            if (expression != null)
            {
                return ToInt(expression.ToString(), defValue);
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为Int32类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static int ToInt(string str, int defValue)
        {

            if (str == null)
                return defValue;
            try
            {

                str = str.Trim();

                if (str.IndexOf(".") > -1)
                {
                    str = str.Substring(0, str.IndexOf("."));
                }

                Regex reg = new Regex("^(-)?[0-9]+$");
                Match ma = reg.Match(str);
                if (ma.Success)
                {
                    int reslut = defValue;
                    int.TryParse(str, out reslut);
                    return reslut;
                }
                else
                {
                    return defValue;
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex.Message, ex);
                return defValue;
            }
            
        }
        #endregion

        #region ToInt64
        /// <summary>
        /// 将对象转换为ToInt64类型,缺省值0
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>转换后的int类型结果</returns>
        public static long ToInt64(object expression)
        {
            if (expression != null)
            {

                return ToInt64(expression.ToString(), 0);

            }
            return 0;
        }
        /// <summary>
        /// 将对象转换为ToInt64类型
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static long ToInt64(object expression, long defValue)
        {
            if (expression != null)
            {
                return ToInt64(expression.ToString(), defValue);
            }
            return defValue;
        }

        /// <summary>
        /// 将对象转换为ToInt64类型
        /// </summary>
        /// <param name="str">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static long ToInt64(string str, long defValue)
        {

            if (str == null)
                return defValue;
            try
            {

                str = str.Trim();

                if (str.IndexOf(".") > -1)
                {
                    str = str.Substring(0, str.IndexOf("."));
                }

                Regex reg = new Regex("^(-)?[0-9]+$");
                Match ma = reg.Match(str);
                if (ma.Success)
                {
                    long reslut = defValue;
                    long.TryParse(str, out reslut);
                    return reslut;
                }
                else
                {
                    return defValue;
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex.Message, ex);
                return defValue;
            }

        }
        #endregion

        #region ToFloat
        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ToFloat(object strValue, float defValue)
        {
            if ((strValue == null))
            {
                return defValue;
            }

            return ToFloat(strValue.ToString(), defValue);
        }

        /// <summary>
        /// string型转换为float型
        /// </summary>
        /// <param name="strValue">要转换的字符串</param>
        /// <param name="defValue">缺省值</param>
        /// <returns>转换后的int类型结果</returns>
        public static float ToFloat(string strValue, float defValue)
        {
            if ((strValue == null) || (strValue.Length > 10))
            {
                return defValue;
            }

            float intValue = defValue;
            if (strValue != null)
            {
                bool IsFloat = Regex.IsMatch(strValue, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                {
                    intValue = Convert.ToSingle(strValue);
                }
            }
            return intValue;
        }

        #endregion

        #region ToDouble

        /// <summary>
        /// To double
        /// </summary>
        /// <param name="strValue">The STR value.</param>
        /// <param name="defValue">The def value.</param>
        /// <returns>System.Double</returns>
        public static double ToDouble(object strValue, float defValue)
        {
            if ((strValue == null))
            {
                return defValue;
            }

            return ToDouble(strValue.ToString(), defValue);
        }


        /// <summary>
        /// To double
        /// </summary>
        /// <param name="strValue">The STR value.</param>
        /// <param name="defValue">The def value.</param>
        /// <returns>System.Double</returns>
        public static double ToDouble(string strValue, float defValue)
        {
            if (strValue == null)
            {
                return defValue;
            }

            double intValue = defValue;
            if (strValue != null)
            {
                bool IsFloat = Regex.IsMatch(strValue, @"^([-]|[0-9])[0-9]*(\.\w*)?$");
                if (IsFloat)
                {
                    intValue = Convert.ToDouble(strValue);
                }
            }
            return intValue;
        }
        #endregion

        #region ToDecimal
        /// <summary>
        /// 将字符类型转为Decimal类型.
        /// </summary>
        /// <param name="strValue">字符或object类型值.</param>
        /// <returns></returns>
        public static decimal ToDecimal(object strValue)
        {
            decimal defValue = 0;
            if (strValue != null)
            {
                return ToDecimal(strValue.ToString(), defValue);
            }
            return defValue;
        }
        /// <summary>
        /// 将字符类型转为Decimal类型.
        /// </summary>
        /// <param name="strValue">字符或object类型值.</param>
        /// <param name="defValue">默认值.</param>
        /// <returns></returns>
        public static decimal ToDecimal(object strValue, decimal defValue)
        {
            if (strValue != null)
            {
                return ToDecimal(strValue.ToString(), defValue);
            }
            return defValue;
        }
        /// <summary>
        /// 将字符类型转为Decimal类型.
        /// </summary>
        /// <param name="strValue">字符类型值.</param>
        /// <param name="defValue">默认值.</param>
        /// <returns></returns>
        public static decimal ToDecimal(string strValue, decimal defValue)
        {
            if ((strValue == null) || (strValue.Length > 40))
            {
                return defValue;
            }
            decimal intValue = defValue;
            Decimal.TryParse(strValue, out intValue);
            return intValue;
        }


        /// <summary>
        /// ceil 向上入，floor 向下舍，round 四舍五入
        /// </summary>
        public enum MathType
        {
            /// <summary>
            /// 四舍五入
            /// </summary>
            Round = 0,
            /// <summary>
            /// 向下舍
            /// </summary>
            Floor = 1,
            /// <summary>
            /// 向上入
            /// </summary>
            Ceil = 2
        }
        /// <summary>
        /// 保留小数位
        /// </summary>
        /// <param name="obj">数量</param>
        /// <param name="len">保留小数位数</param>
        /// <param name="mathType">舍入类型</param>
        /// <returns></returns>
        public static decimal KeepDecimal(object obj, int len, MathType mathType = MathType.Round)
        {
            decimal result = Utils.ToDecimal(obj);
            switch (mathType)
            {
                case MathType.Floor:
                    return Math.Floor((decimal)Math.Pow(10, len) * result) / (decimal)Math.Pow(10, len);
                case MathType.Ceil:
                    return Math.Ceiling((decimal)Math.Pow(10, len) * result) / (decimal)Math.Pow(10, len);
                case MathType.Round:
                default:
                    return Math.Round(result, len, MidpointRounding.AwayFromZero);
            }
        }
        #endregion



        #region ToDateTime 将输入的字符串转化为日期。如果字符串的格式非法，则返回当前日期。
        /// <summary>
        /// 将输入的字符串转化为日期。如果字符串的格式非法，则返回当前日期。
        /// </summary>
        /// <param name="strInput">输入字符串</param>
        /// <returns>日期对象</returns>
        public static DateTime ToDateTime(string strInput)
        {
            DateTime oDateTime;

            try
            {
                oDateTime = DateTime.Parse(strInput);
            }
            catch (Exception)
            {
                oDateTime = DateTime.Today;
            }

            return oDateTime;
        }


        /// <summary>
        /// To date time
        /// </summary>
        /// <param name="strInput">The STR input.</param>
        /// <returns>System.DateTime</returns>
        public static DateTime ToDateTime(object strInput)
        {
            DateTime oDateTime;

            try
            {
                oDateTime = DateTime.Parse(strInput.ToString());
            }
            catch (Exception)
            {
                oDateTime = DateTime.Now;
            }

            return oDateTime;
        }
        #endregion
    }
}
