using ECF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ECF.Data.Query
{
    /// <summary>
    /// FullName: <seealso cref="ECF.Data.Query.Condition" />
    /// Summary : 查询条件类
    /// Author: hfshan
    /// DateTime: 2016-09-01
    /// </summary>
    [Serializable]
    public class Condition : Entity
    {
        //条件值
        private string _value;

        #region 属性
        /// <summary>
        /// 条件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 条件值（此处已统一处理了单引号，其它地方不需要再处理）
        /// </summary>
        public string Value { get { return _value.Replace("'", "''"); } set { _value = value; } }
        /// <summary>
        /// 查询运算符
        /// </summary>
        public ConditionalOperator Oper { get; set; }
        /// <summary>
        /// 逻辑运算符 AND或者OR
        /// </summary>
        public LogicalOperator Link { get; set; }

        /// <summary>
        /// 实体的全名
        /// </summary>
        public override string EntityFullName => "ECF.DATA.Query.Condition_ECF";
        #endregion

        #region 构造函数 +Condition(string name, string value, ConditionalOperator oper, LogicalOperator link)
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="oper"></param>
        /// <param name="link"></param>
        public Condition(string name, string value, ConditionalOperator oper, LogicalOperator link)
        {
            Name = name;
            Value = value;
            Oper = oper;
            Link = link;
        }
        #endregion

        #region 收集查询条件 +static List<Condition> Collection(XmlDocument xmlDoc)
        /// <summary>
        /// 收集查询条件
        /// </summary>
        /// <param name="xmlDoc"><![CDATA[格式：<Condition><Id Oper="" Link="">1</Id></Condition>]]></param>
        /// <returns></returns>
        public static List<Condition> Collection(XmlDocument xmlDoc)
        {
            List<Condition> result = new List<Condition>();
            if (xmlDoc == null) return result;

            try
            {
                XmlNode cond = xmlDoc.SelectSingleNode("//Condition");
                if (cond == null) return result;

                foreach (XmlNode xn in cond.ChildNodes)
                {
                    result.Add(Collection(xn));
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return result;
        }
        #endregion

        #region 收集查询条件 +static Condition Collection(XmlNode node)
        /// <summary>
        /// 收集查询条件
        /// </summary>
        /// <param name="node"><![CDATA[格式：<Id Oper="" Link="">1</Id>]]></param>
        /// <returns></returns>
        public static Condition Collection(XmlNode node)
        {
            //条件运算符 "="相等[Equal]，"!"不等[Unequal]，"%"包含[Contain]，"^"开头[Front]，"$"结尾[End]
            ConditionalOperator oper = ParseOper(node.Attributes != null && node.Attributes["Oper"] != null ? node.Attributes["Oper"].Value : "=");
            LogicalOperator link = node.Attributes != null && node.Attributes["Link"] != null
                && node.Attributes["Link"].Value.ToUpper() == "OR" ? LogicalOperator.Or : LogicalOperator.And;
            return new Condition(node.Name, node.InnerText, oper, link);
        }
        #endregion

        #region 解析条件运算符 -static ConditionalOperator ParseOper(string oper)
        /// <summary>
        /// 解析条件运算符
        /// </summary>
        /// <param name="oper"></param>
        /// <returns></returns>
        private static ConditionalOperator ParseOper(string oper)
        {
            //如果为空直接返回默认值
            if (string.IsNullOrEmpty(oper)) return ConditionalOperator.Equal;

            #region 获得条件运算符的类型
            switch (oper.ToLower())
            {
                //相等
                case "=":
                case "1":
                case "equal":
                    return ConditionalOperator.Equal;
                //包含
                case "%":
                case "2":
                case "contain":
                case "like":
                    return ConditionalOperator.Contain;
                //开头
                case "^":
                case "3":
                case "front":
                    return ConditionalOperator.Front;
                //结尾
                case "$":
                case "4":
                case "end":
                    return ConditionalOperator.End;
                //小于
                case "<":
                case "5":
                case "less":
                    return ConditionalOperator.Less;
                //大于
                case ">":
                case "6":
                case "greater":
                    return ConditionalOperator.Greater;
                //小于或等于
                case "<=":
                case "7":
                case "lessequal":
                    return ConditionalOperator.LessEqual;
                //大于或等于
                case ">=":
                case "8":
                case "greaterequal":
                    return ConditionalOperator.GreaterEqual;
                //包含在
                case "in":
                case "9":
                    return ConditionalOperator.In;
                //为空
                case "isnull":
                case "10":
                    return ConditionalOperator.IsNull;
                //分词
                case "segment":
                case "11":
                    return ConditionalOperator.Segment;
                //不等
                case "!=":
                case "-1":
                case "unequal":
                    return ConditionalOperator.Unequal;
                //非包含
                case "!%":
                case "-2":
                case "uncontain":
                case "notlike":
                    return ConditionalOperator.Uncontain;
                //非开头
                case "!^":
                case "-3":
                case "notfront":
                    return ConditionalOperator.NotFront;
                //非结尾
                case "!$":
                case "-4":
                case "notend":
                    return ConditionalOperator.NotEnd;
                //不包含在
                case "notin":
                case "-9":
                    return ConditionalOperator.NotIn;
                //不为空
                case "notnull":
                case "-10":
                    return ConditionalOperator.NotNull;
                //默认为相等
                default:
                    return ConditionalOperator.Equal;
            }
            #endregion
        }
        #endregion

        #region 转换为sql条件字符串 +string ToWhereString(string key = null)
        /// <summary>
        /// 转换为sql条件字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ToWhereString(string key = null)
        {
            return ToWhereString(key, this.Oper);
        }
        #endregion

        #region 转换为sql条件字符串 +string ToWhereString(string key, ConditionalOperator oper)
        /// <summary>
        /// 转换为sql条件字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="oper"></param>
        /// <returns></returns>
        public string ToWhereString(string key, ConditionalOperator oper)
        {
            return ToWhereString(key, oper, this.Link);
        }
        #endregion

        #region 转换为sql条件字符串 +string ToWhereString(string key, ConditionalOperator oper, LogicalOperator link)
        /// <summary>
        /// 转换为sql条件字符串
        /// </summary>
        /// <param name="key">关键字.</param>
        /// <param name="oper">条件运算符</param>
        /// <param name="link">操作类型.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public string ToWhereString(string key, ConditionalOperator oper, LogicalOperator link)
        {
            if (Value == null) return "";
            string result = "";
            string column = !string.IsNullOrEmpty(key) ? key : Name;
            string query = ToCriteria(column, Value, oper);

            #region 处理逻辑运算符
            if (query != "")
            {
                switch (link)
                {
                    case LogicalOperator.Or:
                        result = " OR (" + query + ")";
                        break;
                    case LogicalOperator.And:
                    default:
                        result = " AND (" + query + ")";
                        break;
                }
            }
            #endregion
            return result;
        }
        #endregion

        #region 转换为条件（不带连接符） +string ToCriteria(string column, string value, ConditionalOperator oper)
        /// <summary>
        /// 转换为条件（不带连接符）
        /// </summary>
        /// <param name="column"></param>
        /// <param name="value"></param>
        /// <param name="oper"></param>
        /// <returns></returns>
        public string ToCriteria(string column, string value, ConditionalOperator oper)
        {
            string query = string.Empty;
            #region 处理条件运算符
            switch (oper)
            {
                case ConditionalOperator.Unequal:
                    query = column + "<>'" + value + "'";
                    break;
                case ConditionalOperator.Contain:
                    query = column + " LIKE '%" + value + "%'";
                    break;
                case ConditionalOperator.Uncontain:
                    query = column + " NOT LIKE '%" + value + "%'";
                    break;
                case ConditionalOperator.Front:
                    query = column + " LIKE '" + value + "%'";
                    break;
                case ConditionalOperator.NotFront:
                    query = column + " NOT LIKE '" + value + "%'";
                    break;
                case ConditionalOperator.End:
                    query = column + " LIKE '%" + value + "'";
                    break;
                case ConditionalOperator.NotEnd:
                    query = column + " NOT LIKE '%" + value + "'";
                    break;
                case ConditionalOperator.Less:
                    query = column + "<'" + value + "'";
                    break;
                case ConditionalOperator.Greater:
                    query = column + ">'" + value + "'";
                    break;
                case ConditionalOperator.LessEqual:
                    query = column + "<='" + value + "'";
                    break;
                case ConditionalOperator.GreaterEqual:
                    query = column + ">='" + value + "'";
                    break;
                case ConditionalOperator.In:
                    if (value != "")
                    {
                        string[] vals = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        query = column + " IN(" + string.Join(",", vals) + ")";
                    }
                    break;
                case ConditionalOperator.NotIn:
                    if (value != "")
                    {
                        string[] vals = value.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        query = column + " NOT IN(" + string.Join(",", vals) + ")";
                    }
                    break;
                case ConditionalOperator.IsNull:
                    query = column + " IS NULL";
                    break;
                case ConditionalOperator.NotNull:
                    query = column + " IS NOT NULL";
                    break;
                case ConditionalOperator.Segment:
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            string[] segment = value.Split(",".ToCharArray()[0]);
                            List<string> likes = new List<string>();
                            if (segment.Length > 0)
                            {
                                foreach (string s in segment)
                                {
                                    likes.Add("{0} LIKE '%" + s + "%'");
                                }
                            }
                            else
                            {
                                likes.Add("{0} LIKE '%" + value + "%'");
                            }
                            string likeStr = string.Join(" AND ", likes);
                            query = "(" + string.Format(likeStr, column) + ")";
                        }
                    }
                    catch (Exception ex)
                    {
                        new ECFException(ex);
                    }
                    break;
                case ConditionalOperator.NumberRange:
                    try
                    {
                        string[] num = value.Split(',');
                        decimal? minNum = null;
                        decimal? maxNum = null;
                        if (num.Length >= 1 && !string.IsNullOrWhiteSpace(num[0]))
                        {
                            minNum = Utils.ToDecimal(num[0]);
                        }
                        if (num.Length == 2 && !string.IsNullOrWhiteSpace(num[1]))
                        {
                            maxNum = Utils.ToDecimal(num[1]);
                        }

                        if (minNum != null && maxNum != null)
                        {
                            query = "(" + column + " between " + minNum + " AND " + maxNum + ")";
                        }
                        else if (minNum != null)
                        {
                            query = column + ">=" + minNum + "";
                        }
                        else if (maxNum != null)
                        {
                            query = column + "<=" + maxNum + "";
                        }
                    }
                    catch (Exception ex)
                    {
                        new ECFException(ex.Message, ex);
                    }
                    break;
                case ConditionalOperator.DateRange:
                    try
                    {
                        string[] date = value.Split(',');
                        string minDate = null;
                        string maxDate = null;
                        if (date.Length >= 1 && !string.IsNullOrWhiteSpace(date[0]))
                        {
                            minDate = date[0] + " 00:00:00";
                        }
                        if (date.Length == 2 && !string.IsNullOrWhiteSpace(date[1]))
                        {
                            maxDate = date[1] + " 23:59:59";
                        }

                        if (minDate != null && maxDate != null)
                        {
                            query = "(" + column + " between '" + minDate + "' AND '" + maxDate + "')";
                        }
                        else if (minDate != null)
                        {
                            query = column + ">='" + minDate + "'";
                        }
                        else if (maxDate != null)
                        {
                            query = column + "<='" + maxDate + "'";
                        }
                    }
                    catch (Exception ex)
                    {
                        new ECFException(ex.Message, ex);
                    }
                    break;
                case ConditionalOperator.DateTimeRange:
                    try
                    {
                        string[] dateTime = value.Split(',');
                        string minDateTime = null;
                        string maxDateTime = null;
                        if (dateTime.Length >= 1 && !string.IsNullOrWhiteSpace(dateTime[0]))
                        {
                            minDateTime = dateTime[0];
                        }
                        if (dateTime.Length == 2 && !string.IsNullOrWhiteSpace(dateTime[1]))
                        {
                            maxDateTime = dateTime[1];
                        }

                        if (minDateTime != null && maxDateTime != null)
                        {
                            query = "(" + column + " between '" + minDateTime + "' AND '" + maxDateTime + "')";
                        }
                        else if (minDateTime != null)
                        {
                            query = column + ">='" + minDateTime + "'";
                        }
                        else if (maxDateTime != null)
                        {
                            query = column + "<='" + maxDateTime + "'";
                        }
                    }
                    catch (Exception ex)
                    {
                        new ECFException(ex.Message, ex);
                    }
                    break;
                case ConditionalOperator.Equal:
                default:
                    query = column + "='" + value + "'";
                    break;
            }
            #endregion
            return query;
        }
        #endregion
    }
}
