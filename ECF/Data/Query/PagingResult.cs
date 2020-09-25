using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF
{
    /// <summary>
    /// 分页结果数据集 
    /// 2017-05-10 by shaipe
    /// </summary>
    /// <seealso cref="ECF.Entity" />
    [Serializable]
    public class PagingResult : Entity
    {
        #region 属性
        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 当前页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 扩展属性
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }
        /// <summary>
        /// 分页数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get
            {
                return (int)Math.Ceiling((decimal)TotalCount / PageSize);
            }
        }

        /// <summary>
        /// 实体的类的全名，用于类型缓存提高反射效率
        /// </summary>
        public override string EntityFullName
        {
            get { return "ECF.Models.PagingResult,ECF"; }
        }
        #endregion

        #region 方法
        /// <summary>
        /// 把分页数据转换成指定的类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetData<T>()
        {
            try
            {
                return (T)Data;
            }
            catch (Exception ex)
            {
                new ECFException(ex.Message, ex);
                return default(T);
            }
        }
        #endregion
    }
}
