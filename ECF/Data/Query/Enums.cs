using ECF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECF.Data.Query
{
    #region 条件运算符枚举 +enum ConditionalOperator
    /// <summary>
    /// FullName: <seealso cref="ECF.Data.Query.ConditionalOperator" />
    /// Summary : 条件运算符枚举
    /// Author: hfshan
    /// DateTime: 2016-09-01
    /// </summary>
    public enum ConditionalOperator
    {
        /// <summary>
        /// 相等
        /// </summary>
        [EnumDescription("相等")]
        Equal = 1,
        /// <summary>
        /// 包含
        /// </summary>
        [EnumDescription("包含")]
        Contain = 2,
        /// <summary>
        /// 开头
        /// </summary>
        [EnumDescription("开头")]
        Front = 3,
        /// <summary>
        /// 结尾
        /// </summary>
        [EnumDescription("结尾")]
        End = 4,
        /// <summary>
        /// 小于
        /// </summary>
        [EnumDescription("小于")]
        Less = 5,
        /// <summary>
        /// 大于
        /// </summary>
        [EnumDescription("大于")]
        Greater = 6,
        /// <summary>
        /// 小于或等于
        /// </summary>
        [EnumDescription("小于或等于")]
        LessEqual = 7,
        /// <summary>
        /// 大于或等于
        /// </summary>
        [EnumDescription("大于或等于")]
        GreaterEqual = 8,
        /// <summary>
        /// 包含在
        /// </summary>
        [EnumDescription("包含在")]
        In = 9,
        /// <summary>
        /// 为空
        /// </summary>
        [EnumDescription("为空")]
        IsNull = 10,
        /// <summary>
        /// 分词
        /// </summary>
        [EnumDescription("分词")]
        Segment = 11,
        /// <summary>
        /// 在数字范围内
        /// </summary>
        [EnumDescription("在数字范围内")]
        NumberRange = 12,
        /// <summary>
        /// 在日期范围内
        /// </summary>
        [EnumDescription("在日期范围内")]
        DateRange = 13,
        /// <summary>
        /// 在时间范围内
        /// </summary>
        [EnumDescription("在时间范围内")]
        DateTimeRange = 14,
        /// <summary>
        /// 不等
        /// </summary>
        [EnumDescription("不等")]
        Unequal = -1,
        /// <summary>
        /// 不包含
        /// </summary>
        [EnumDescription("不包含")]
        Uncontain = -2,
        /// <summary>
        /// 不以开头
        /// </summary>
        [EnumDescription("不以开头")]
        NotFront = -3,
        /// <summary>
        /// 不以结尾
        /// </summary>
        [EnumDescription("不以结尾")]
        NotEnd = -4,
        /// <summary>
        /// 不包含在
        /// </summary>
        [EnumDescription("不包含在")]
        NotIn = -9,
        /// <summary>
        /// 不为空
        /// </summary>
        [EnumDescription("不为空")]
        NotNull = -10
    }
    #endregion

    #region 逻辑运算符枚举 +enum LogicalOperator
    /// <summary>
    /// FullName: <seealso cref="ECF.Data.Query.LogicalOperator" />
    /// Summary : 逻辑运算符枚举
    /// Author: hfshan
    /// DateTime: 2016-09-01
    /// </summary>
    public enum LogicalOperator
    {
        /// <summary>
        /// 并且
        /// </summary>
        [EnumDescription("并且")]
        And = 0,
        /// <summary>
        /// 或者
        /// </summary>
        [EnumDescription("或者")]
        Or = 1
    }
    #endregion

    #region 排序方式 +enum SortBy
    /// <summary>
    /// FullName: <seealso cref="ECF.Data.Query.SortBy" />
    /// Summary : 排序方式
    /// Author: hfshan
    /// DateTime: 2016-09-12
    /// </summary>
    public enum SortBy
    {
        /// <summary>
        /// 正序
        /// </summary>
        [EnumDescription("正序")]
        Asc = 0,
        /// <summary>
        /// 倒序
        /// </summary>
        [EnumDescription("倒序")]
        Desc = 1
    }
    #endregion
}
