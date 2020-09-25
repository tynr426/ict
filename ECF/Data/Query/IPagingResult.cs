using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF.Models
{
    /// <summary>
    /// i paging result
    /// </summary>
    public interface IPagingResult : IEntity
    {
        #region 属性
        /// <summary>
        /// 每页记录数
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        int PageIndex { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        int TotalCount { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        Dictionary<string, object> Properties { get; set; }
        /// <summary>
        /// 分页数据
        /// </summary>
        object Data { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        int TotalPage { get; }

        #endregion

        /// <summary>
        /// 获取给定的类型
        /// </summary>
        /// <typeparam name="T">类型参数</typeparam>
        /// <returns>
        /// T
        /// </returns>
        T GetData<T>();
    }
}
