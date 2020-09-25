using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF.Web.Http
{
    /// <summary>
    /// FullName： <see cref="ECF.Web.Http.QueryParameterComparer"/>
    /// Summary ： 比较器类进行排序的查询参数 
    /// Version： 1.0.0.0 
    /// DateTime： 2012/5/12 8:24 
    /// CopyRight (c) by shaipe
    /// </summary>
    public class QueryParameterComparer : IComparer<QueryParameter>
    {
        #region IComparer<QueryParameter> Members


        /// <summary>
        /// 返回比较结果
        /// by XP-PC 2012/5/12
        /// </summary>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        public int Compare(QueryParameter x, QueryParameter y)
        {
            if (x.Name == y.Name)
            {
                return string.Compare(x.Value, y.Value);
            }
            else
            {
                return string.Compare(x.Name, y.Name);
            }
        }
        #endregion
    }
}
