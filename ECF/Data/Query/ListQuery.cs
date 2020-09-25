using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF.Data.Query
{
    /// <summary>
    /// 列表查询类 
    /// 2017-05-25 by shaipe
    /// </summary>
    /// <seealso cref="ECF.Entity" />
    [Serializable]
    public class ListQuery : Entity, IListQuery
    {
        #region Properties
        /// <summary>
        /// 实体的类的全名，用于类型缓存提高反射效率
        /// </summary>
        public override string EntityFullName
        {
            get { return "ECF.Data.Query.ListQuery,ECF"; }
        }

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 要返回的字段
        /// </summary>
        public string Fields { get; set; }
        /// <summary>
        /// 查询条件
        /// </summary>
        public List<Condition> Condition { get; set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        public List<Orderby> Orderby { get; set; }
        /// <summary>
        /// 分组字段
        /// </summary>
        public string Groupby { get; set; }
        
        #endregion

        #region 是否已经包含指定名称的条件 +bool HasCondition(string name)
        /// <summary>
        ///  是否已经包含指定名称的条件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasCondition(string name)
        {
            if (Condition != null)
                return Condition.Any(n => n.Name == name);
            else
                return false;
        }
        #endregion

        #region 用于特殊情况时直接取指定条件名称的值，不建议使用此方法 +string GetConditionValue(string name)
        /// <summary>
        /// 用于特殊情况时直接取指定条件名称的值，不建议使用此方法。小提示：你可以直接在遍历拼接条件的时候取出想要的参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string GetConditionValue(string name)
        {
            if (Condition.Any(n => n.Name == name))
            {
                return Condition.FirstOrDefault(c => c.Name == name).Value;
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region 追加查询条件 +void AppendCondition(string name, string value)
        /// <summary>
        /// 追加查询条件,name存在时作更新处理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void AppendCondition(string name, string value)
        {
            AppendCondition(name, value, ConditionalOperator.Equal, LogicalOperator.And);
        }
        #endregion

        #region 追加查询条件 +void AppendCondition(string name, string value, ConditionalOperator oper)
        /// <summary>
        /// 追加查询条件,name存在时作更新处理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="oper"></param>
        public void AppendCondition(string name, string value, ConditionalOperator oper)
        {
            AppendCondition(name, value, oper, LogicalOperator.And);
        }
        #endregion

        #region 追加查询条件 +void AppendCondition(string name, string value, ConditionalOperator oper, LogicalOperator link)
        /// <summary>
        /// 追加查询条件,name存在时作更新处理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="oper"></param>
        /// <param name="link"></param>
        public void AppendCondition(string name, string value, ConditionalOperator oper, LogicalOperator link)
        {
            if (Condition == null)
            {
                Condition = new List<Condition>();
            }
            if (Condition.Any(n => n.Name == name))
            {
                Condition cond = Condition.FirstOrDefault(c => c.Name == name);
                cond.Value = value;
                cond.Oper = oper;
                cond.Link = link;
            }
            else
            {
                Condition.Add(new Condition(name, value, oper, link));
            }
        }
        #endregion

        #region 追加查询条件 +void AppendCondition(Condition condition)
        /// <summary>
        /// 追加查询条件,name存在时作更新处理
        /// </summary>
        /// <param name="condition">添加具体的条件.</param>
        public void AppendCondition(Condition condition)
        {
            Condition.Add(condition);
        }
        #endregion

        #region 是否已经包含指定名称的排序 +bool HasOrderby(string name)
        /// <summary>
        ///  是否已经包含指定名称的排序
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool HasOrderby(string name)
        {
            if (Orderby != null)
                return Orderby.Any(n => n.Name == name);
            else
                return false;
        }
        #endregion

        #region 追加排序方式(默认倒序) +void AppendOrderby(string name)
        /// <summary>
        /// 追加排序方式(默认倒序)
        /// </summary>
        /// <param name="name"></param>
        public void AppendOrderby(string name)
        {
            AppendOrderby(name, SortBy.Desc);
        }
        #endregion

        #region 追加排序方式 +void AppendOrderby(string name, SortBy sort)
        /// <summary>
        /// 追加排序方式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sort"></param>
        public void AppendOrderby(string name, SortBy sort)
        {
            if (Orderby == null)
            {
                Orderby = new List<Orderby>();
            }
            Orderby.Add(new Orderby(name, sort));
        }
        #endregion

        /// <summary>
        /// 获取查询条件
        /// </summary>
        public string GetWhereString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (Condition c in Condition)
            {
                if (!string.IsNullOrEmpty(c.Value))
                {
                    sb.Append(c.ToWhereString());
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取排序的排序Sql
        /// </summary>
        /// <returns>
        /// System.String
        /// </returns>
        public string GetOrderbyString()
        {
            string orderby;
            List<string> sortby = new List<string>();
            foreach (Orderby ob in Orderby)
            {
                sortby.Add(ob.ToString());
            }
            orderby = sortby.Count > 0 ? "order by " + string.Join(",", sortby) : "";

            return orderby;
        }

        //public string GetFieldString(ref Dictionary<string,object> customFields)
        //{
        //    Dictionary<string, object> fieldDict = InitFields();

        //    if (string.IsNullOrWhiteSpace(Fields) || Fields == "*")
        //    {
        //        return fieldDict != null && fieldDict.ContainsKey("*") ? fieldDict["*"].ToString() : "*";
        //    }
        //    List<string> columns = new List<string>();
        //    string[] fieldKeys = Fields.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        //    foreach (string key in fieldKeys)
        //    {
        //        if (fieldDict.ContainsKey(key))
        //        {
        //            if (fieldDict[key] is Type)
        //            {
        //                customFields.Add(key, (Type)fieldDict[key]);
        //            }
        //            else if (key == "*")
        //            {
        //                columns.Add(fieldDict[key] + "");
        //            }
        //            else
        //            {
        //                columns.Add(fieldDict[key] + " as " + key);
        //            }
        //        }
        //        else if (key == "*")
        //        {
        //            columns.Add("*");
        //        }
        //    }
        //    return columns.Count > 0 ? string.Join(",", columns) : "";
        //}
    }
}
