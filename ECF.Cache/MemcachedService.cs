
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECF.Cache
{
    /// <summary>
    /// 版权信息：CopyRight (c) 2018 任我行科技研发中心
    /// 文件名称：<see cref="ECF.Cache.MemcachedService"/>
    /// 作者：XP-COMPANY-Shaipe
    /// 日期：2018/10/18
    /// 摘要：Redis缓存服务
    /// 版本：3.1.9
    /// </summary>
    /// <seealso cref="ECF.Caching.ICacheService" />
    public class MemcachedService : ICacheService
    {
        int DEFAULT_TMEOUT = 600;//默认超时时间（单位秒）
        
        /// <summary>
        /// 超时时间.
        /// </summary>
        public int TimeOut
        {
            get
            {
                return DEFAULT_TMEOUT ;
            }

            set
            {
                DEFAULT_TMEOUT = value;
            }
        }

        public string[] Keys => throw new NotImplementedException();

        public string Info => throw new NotImplementedException();

        public int Count => throw new NotImplementedException();

        public void Clear(string prefixKey)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public object Get(string key)
        {
            throw new NotImplementedException();
        }

        public T Get<T>(string key)
        {
            throw new NotImplementedException();
        }

        public void Insert(string key, object data)
        {
            throw new NotImplementedException();
        }

        public void Insert(string key, object data, DateTime cacheTime)
        {
            throw new NotImplementedException();
        }

        public void Insert(string key, object data, int cacheTime)
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(string key, T data)
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(string key, T data, DateTime cacheTime)
        {
            throw new NotImplementedException();
        }

        public void Insert<T>(string key, T data, int cacheTime)
        {
            throw new NotImplementedException();
        }

        public void Remove(string key)
        {
            throw new NotImplementedException();
        }

        public void RemoveByPattern(string pattern)
        {
            throw new NotImplementedException();
        }
    }
}
