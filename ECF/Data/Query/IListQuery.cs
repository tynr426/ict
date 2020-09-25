using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF.Data.Query
{
    /// <summary>
    /// 列表查询接口
    /// </summary>
    public interface IListQuery
    {
        #region Properties
        /// <summary>
        /// 每页记录数
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// 要返回的字段
        /// </summary>
        string Fields { get; set; }
        /// <summary>
        /// 查询条件
        /// </summary>
        List<Condition> Condition { get; set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        List<Orderby> Orderby { get; set; }
        /// <summary>
        /// 分组字段
        /// </summary>
        string Groupby { get; set; }
        #endregion

        #region Methods
        /// <summary>
        ///  是否已经包含指定名称的条件
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasCondition(string name);

        /// <summary>
        /// 用于特殊情况时直接取指定条件名称的值，不建议使用此方法。小提示：你可以直接在遍历拼接条件的时候取出想要的参数
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetConditionValue(string name);

        /// <summary>
        /// 追加查询条件,name存在时作更新处理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void AppendCondition(string name, string value);

        /// <summary>
        /// 追加查询条件,name存在时作更新处理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="oper"></param>
        void AppendCondition(string name, string value, ConditionalOperator oper);

        /// <summary>
        /// 追加查询条件,name存在时作更新处理
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="oper"></param>
        /// <param name="link"></param>
        void AppendCondition(string name, string value, ConditionalOperator oper, LogicalOperator link);

        /// <summary>
        /// 追加查询条件,name存在时作更新处理
        /// </summary>
        /// <param name="condition">The condition.</param>
        void AppendCondition(Condition condition);

        /// <summary>
        ///  是否已经包含指定名称的排序
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasOrderby(string name);

        /// <summary>
        /// 追加排序方式(默认倒序)
        /// </summary>
        /// <param name="name"></param>
        void AppendOrderby(string name);

        /// <summary>
        /// 追加排序方式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sort"></param>
        void AppendOrderby(string name, SortBy sort);

        /// <summary>
        /// 获取排序Sql
        /// </summary>
        /// <returns>
        /// System.String
        /// </returns>
        string GetOrderbyString();

        /// <summary>
        /// 获取条件字符
        /// </summary>
        /// <returns>
        /// System.String
        /// </returns>
        string GetWhereString();
        #endregion




    }
}
