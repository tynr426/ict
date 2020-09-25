using ECF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECF.Caching
{
    /// <summary>
    /// 缓存枚举
    /// </summary>
    public enum CacheType
    {
        /// <summary>
        /// 不使用缓存
        /// </summary>
        None = 0,

        /// <summary>
        /// System.Web.Cache缓存
        /// </summary>
        Memory = 1,

        /// <summary>
        /// StackExchange.Redis缓存
        /// </summary>
        Redis = 2,
        /// <summary>
        /// Memcached缓存
        /// </summary>3
        Memcached = 3,
    }
}
