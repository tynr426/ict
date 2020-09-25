using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF.Caching
{
    /// <summary>
    /// 版权信息：CopyRight (c) 2018 任我行科技研发中心
    /// 文件名称：<see cref="ECF.Caching.AbstractCache"/>
    /// 作者：XP-COMPANY-Shaipe
    /// 日期：2018/10/22
    /// 摘要：ECF系统级缓存构造化基类
    /// 版本：3.1.9
    /// </summary>
    public abstract class AbstractCache
    {
        #region GetCacheType
        /// <summary>
        /// 获取缓存类型.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public Type GetCacheType(IEntity entity)
        {
            Object o = DefaultCache.Get(entity.EntityFullName);
            if (Utils.IsNullOrEmpty(o))
            {
                Type type = entity.GetType();
                DefaultCache.Max(entity.EntityFullName, type);
                return type;
            }
            return (Type)o;
        }
        #endregion
    }
}
