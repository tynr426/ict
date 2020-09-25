using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF.Data.Query
{
    /// <summary>
    /// 分页查询
    /// </summary>
    /// <seealso cref="ECF.Data.Query.IListQuery" />
    public interface IPagingQuery : IListQuery
    {
        /// <summary>
        /// 当前页码【小于1：不分页】
        /// </summary>
        int PageIndex { get; set; }
    }
}
