using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Globalization;

namespace ECF
{
	/// <summary>
	/// FullName： <see cref="ECF.RuleRegex"/>
	/// Summary ： 格式化数据的正则表达式 
	/// Version： 1.0.0.0 
	/// DateTime： 2012/4/23 23:02 
	/// CopyRight (c) by shaipe
	/// </summary>
	internal class RuleRegex
    { 
        //Quantity | Formatter: &LB0& CON0&RB
        /// <summary>
        /// 获取正则处理选项 
        /// </summary>
        internal static RegexOptions options { get { return RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline; } }

        /// <summary>
        /// 变量字段、控制语句字段扩展属性
        /// </summary>
        internal static Regex ExternalRegex = new Regex(@"\|(?<ExternalName>[^:]+):(?<ExternalValue>[^\|]+)", options);


        /// <summary>
        /// 扩展属性中的简单三目运算
        /// </summary>
        internal static Regex PolymorphismExternalRegex = new Regex(@"(?<Expr>[^\?]*)[\s]*\?[\s]*(?<True>[^:]*)[\s]*:[\s]*(?<False>[\S\s]*)", options);


        /// <summary>
        /// 尖括号处理(元素正则表达式)
        /// </summary>
        internal static Regex AngleRegex = new Regex(@"\<\$(?<Name>(?!for)(?!while)(?!if)[\w]+)(?<Attributes>[\S\s]*?)((\/>)|(>(?<InnerHtml>[\S\s]*?)(<\$\/(?<Name>(?!for)(?!while)(?!if)[\w]+)[\s]*\>)))", options);

        /// <summary>
        /// 带扩展属性变量处理
        /// </summary>
        internal static Regex BraceExternal = new Regex(@"\{#(?<Member>[^\}&^\|&^\.]+\.)?(?<Name>[^\|&^\}]+)(?<External>[^\}]*?)\}", options);

        /// <summary>
        /// 带扩展属性字段处理
        /// </summary>
        internal static Regex BracketExternal = new Regex(@"\[#(?<Member>[^\]&^\|&^\.]+\.)?(?<Name>[^\|&^\]]+)(?<External>[^\]]*?)\]", options);

    }
    /// <summary>
    /// FullName： <see cref="ECF.FormatUtil"/>
    /// Summary ： 格式化处理类
    /// Version： 1.0.0.0 
    /// DateTime： 2012/4/23 16:08 
    /// CopyRight (c) by shaipe
    /// </summary>
    public class FormatUtil
    {

        private const string CurrentDataValueMark = "this";

        /// <summary>
        /// 对数据进行格式化
        /// by XP-PC 2012/4/23
        /// </summary>
        /// <param name="expression">需要解析的字符串.</param>
        /// <param name="dataValue">待格式化的值.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string Formater(string expression, object dataValue)
        {
            if (dataValue == null)
                return null;

            if (String.IsNullOrEmpty(expression))
                return dataValue.ToString();

            // 解析结果、原生数据类型
            string ParseResult = null;

            // 根据指定长度截断变量、重复展示变量次数
            int InterceptLength = 0;

            ParseResult = dataValue.ToString();

            try
            {
                for (Match m = RuleRegex.ExternalRegex.Match(expression); m.Success; m = m.NextMatch())
                {
                    // 扩展属性值、扩展属性名称
                    string ExternalValue = m.Groups["ExternalValue"].Value, ExternalName = m.Groups["ExternalName"].Value;

                    // 扩展属性值及名称判空
                    if (String.IsNullOrEmpty(ExternalName) || String.IsNullOrEmpty(ExternalValue))
                    {
                        continue;
                    }

                    if (String.Compare(ExternalName, "Length", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if (int.TryParse(ExternalValue, out InterceptLength))
                        {
                            ParseResult = ParseLength(ParseResult, InterceptLength, false);
                        }
                    }
                    else if (String.Compare(ExternalName, "Increase", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ParseResult = ParseIncrease(ParseResult, ExternalValue);
                    }
                    else if (String.Compare(ExternalName, "Formatter", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ParseResult = ParseFormatter(ParseResult, ExternalValue, null);
                    }
                    else if (String.Compare(ExternalName, "CultureRelate", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ParseResult = ParseFormatter(ParseResult, ExternalValue, ExternalValue);
                    }
                    else if (String.Compare(ExternalName, "IfNull", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ParseResult = ParseIfNullEmpty(ParseResult, ExternalValue);
                    }
                    else if (String.Compare(ExternalName, "Convert", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        ParseResult = ParsePolymorphism(ParseResult, ExternalValue);
                    }
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
            }

            return ParseResult;
        }

        #region ParseLength
        /// <summary>
        /// 解析输出内容格式化扩展属性
        /// </summary>
        /// <param name="DataValue">字段原始值</param>
        /// <param name="InterceptLength">Length of the intercept.</param>
        /// <param name="IsParseEllipsis">if set to <c>true</c> [is parse ellipsis].</param>
        /// <returns>
        /// System.String
        /// </returns>
        private static string ParseLength(string DataValue, int InterceptLength, bool IsParseEllipsis)
        {
            try
            {
                int dataLength = Utils.GetByteCount(DataValue);

                if (dataLength <= InterceptLength)
                    return DataValue;

                char[] dataChars = DataValue.ToCharArray();
                int SubLength = 0;

                for (int i = DataValue.Length; i > 0; i--)
                {
                    if (Utils.GetByteCount(dataChars, 0, i) > (InterceptLength - (IsParseEllipsis ? 3 : 0)))
                    {
                        continue;
                    }
                    else
                    {
                        SubLength = i;
                        break;
                    }
                }

                return IsParseEllipsis ? ParseEllipsis(DataValue.Substring(0, SubLength)) : DataValue.Substring(0, SubLength);
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
                return DataValue;
            }
        }

        /// <summary>
        /// 添加省略号
        /// </summary>
        /// <param name="DataValue">字段原始值</param>
        private static string ParseEllipsis(string DataValue)
        {
            StringBuilder sb = new StringBuilder(DataValue);
            sb.Append("...");
            return sb.ToString();
        }
        #endregion

        #region ParseFormatter

        
        /// <summary>
        /// 转义左括号
        /// </summary>
        private readonly static Regex reEscapeLeftBrace = new Regex(@"\&LB", RuleRegex.options);

        
        /// <summary>
        /// // 转义右括号
        /// </summary>
        private readonly static Regex reEscapeRightBrace = new Regex(@"\&RB", RuleRegex.options);

        
        /// <summary>
        /// // 转义小数点
        /// </summary>
        private readonly static Regex reEscapeDecimalPoint = new Regex(@"\&PO", RuleRegex.options);

        
        /// <summary>
        /// // 转义冒号
        /// </summary>
        private readonly static Regex reEscapeColon = new Regex(@"\&CO", RuleRegex.options);

        /// <summary>
        /// 解析输出内容格式化扩展属性
        /// </summary>
        /// <param name="DataValue">字段原始值</param>
        /// <param name="ExternalValue">扩展属性值</param>
        /// <param name="CultureRelate">关联区域性</param>
        /// <returns>
        /// System.String
        /// </returns>
        private static string ParseFormatter(object DataValue, string ExternalValue, string CultureRelate)
        {
            ExternalValue = reEscapeRightBrace.Replace(reEscapeDecimalPoint.Replace(reEscapeColon.Replace(reEscapeLeftBrace.Replace(ExternalValue, "{"), ":"), "."), "}");

            Type DataType = DataValue.GetType();

            if (!DataType.IsPrimitive && DataType != typeof(DateTime) && DataType != typeof(string) && DataType != typeof(float) && DataType != typeof(Decimal))
            {
                return String.Empty;
            }

            if (!String.IsNullOrEmpty(CultureRelate))
            {
                return String.Format(new CultureInfo(CultureRelate), ExternalValue, DataValue);
            }

            return String.Format(ExternalValue, DataValue);
        }

        #endregion

        /// <summary>
        /// 解析字段的多态解释
        /// </summary>
        /// <param name="DataValue">字段原始值</param>
        /// <param name="ExternalValue">扩展属性值</param>
        private static string ParseIfNullEmpty(object DataValue, string ExternalValue)
        {
            if (DataValue == null)
                return String.Empty;

            if (DataValue.ToString() == "")
                return String.Empty;

            return String.IsNullOrEmpty(ExternalValue) ? DataValue.ToString() : ExternalValue.Replace(CurrentDataValueMark, DataValue.ToString());
        }

        /// <summary>
        /// 对数字类型变量按照指定数量增加
        /// </summary>
        /// <param name="DataValue">字段原始值</param>
        /// <param name="ExternalValue">ExternalValue</param>
        private static string ParseIncrease(object DataValue, string ExternalValue)
        {
            decimal decimalTemp = 0.0M;

            if (!decimal.TryParse(DataValue.ToString(), out decimalTemp))
                return DataValue.ToString();

            if (!decimal.TryParse(ExternalValue, out decimalTemp))
                return DataValue.ToString();

            return Calculate(DataValue.ToString() + "+" + ExternalValue).ToString();
        }

        // 用于表达需要使用的计算式
        static System.Data.DataTable dtCalculate = new System.Data.DataTable();

        /// <summary>
        /// 计算表达式的值
        /// </summary>
        /// <param name="EvalExpression">表达式字符串</param>
        /// <returns>返回表达式的布尔类型结果, 任何异常情况都会返回null</returns>
        internal static object Calculate(string EvalExpression)
        {
            try
            {
                return dtCalculate.Compute(EvalExpression, null);
            }
            catch (Exception)
            {
                return null;
            }
        }


        /// <summary>
        /// 解析字段的多态解释
        /// </summary>
        /// <param name="DataValue">字段原始值</param>
        /// <param name="ExternalValue">扩展属性值</param>
        private static string ParsePolymorphism(object DataValue, string ExternalValue)
        {
            try
            {
                Match m = RuleRegex.PolymorphismExternalRegex.Match(ExternalValue);

                bool boolTemp = false;

                if (m.Success)
                {
                    if (DataValue is Boolean || bool.TryParse(DataValue.ToString(), out boolTemp))
                    {
                        return Convert.ToBoolean(DataValue) ? m.Groups["True"].Value : m.Groups["False"].Value;
                    }
                    else
                    {
                        string Expr = String.Empty;

                        if (ExternalValue.IndexOf(CurrentDataValueMark) > -1)
                        {
                            Expr = m.Groups["Expr"].Value.Replace(CurrentDataValueMark, DataValue.ToString());
                        }
                        else
                        {
                            Expr = DataValue.ToString() + m.Groups["Expr"].Value;
                        }

                        //return Eval(Expr, "变量\"" + DataUnique + "\"解析Convert扩展属性") ? m.Groups["True"].Value.Replace(CurrentDataValueMark, DataValue.ToString()) : m.Groups["False"].Value.Replace(CurrentDataValueMark, DataValue.ToString());
                    }
                }

            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
            }
            return DataValue.ToString();
        }



    }
}
