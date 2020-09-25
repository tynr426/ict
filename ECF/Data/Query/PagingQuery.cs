using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF.Data.Query
{
    /// <summary>
    /// Paging query Class 
    /// 2017-05-25 by shaipe
    /// </summary>
    /// <seealso cref="ECF.Data.Query.ListQuery" />
    [Serializable]
    public class PagingQuery : ListQuery
    {
        /// <summary>
        /// 实体的类的全名，用于类型缓存提高反射效率
        /// </summary>
        public override string EntityFullName
        {
            get { return "ECF.Data.Query.PagingQuery,ECF"; }
        }
        /// <summary>
        /// 当前页码【小于1：不分页】
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 合计字体
        /// </summary>
        public string SumFields { get; set; }
    }
}
