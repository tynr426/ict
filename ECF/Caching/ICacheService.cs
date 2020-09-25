using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECF.Caching
{
    /// <summary>
    /// Cache接口
    /// </summary>
    public interface ICacheService
    {

        /// <summary>
        /// 默认过期时长.
        /// </summary>
        // int Expire { get; set; }

        /// <summary>
        /// 获得指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存值</returns>
        object Get(string key);

        /// <summary>
        /// 获得指定键的缓存值
        /// </summary>
        T Get<T>(string key);

        /// <summary>
        /// 从缓存中移除指定键的缓存值
        /// </summary>
        /// <param name="key">缓存键</param>
        void Remove(string key);

        /// <summary>
        /// 从缓存中移除指定前缀的缓存值
        /// </summary>
        /// <param name="prefixKey"></param>
        void Clear(string prefixKey);

        /// <summary>
        /// 清空所有缓存对象
        /// </summary>
        void Clear();

        /// <summary>
        /// 根据正则表达式匹配符合的Key，然后移除
        /// </summary>
        /// <param name="pattern"></param>
        void RemoveByPattern(string pattern);

        ///// <summary>
        ///// 将指定键的对象添加到缓存中
        ///// </summary>
        ///// <param name="key">缓存键</param>
        ///// <param name="data">缓存值</param>
        //void Insert(string key, object data);

        /// <summary>
        /// 将指定键的对象添加到缓存中
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        void Insert<T>(string key, T data);

        ///// <summary>
        ///// 将指定键的对象添加到缓存中，并指定过期时间
        ///// </summary>
        ///// <param name="key">缓存键</param>
        ///// <param name="data">缓存值</param>
        ///// <param name="cacheTime">缓存过期时间(秒钟)</param>
        //void Insert(string key, object data, int cacheTime);

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间(秒钟)</param>
        void Insert<T>(string key, T data, int cacheTime);

        ///// <summary>
        ///// 将指定键的对象添加到缓存中，并指定过期时间
        ///// </summary>
        ///// <param name="key">缓存键</param>
        ///// <param name="data">缓存值</param>
        ///// <param name="cacheTime">缓存过期时间</param>
        //void Insert(string key, object data, DateTime cacheTime);

        /// <summary>
        /// 将指定键的对象添加到缓存中，并指定过期时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="data">缓存值</param>
        /// <param name="cacheTime">缓存过期时间</param>
        void Insert<T>(string key, T data, DateTime cacheTime);

        /// <summary>
        /// 返回缓存中的所有缓存关键字
        /// </summary>
        string[] Keys { get; }

        /// <summary>
        /// 获取缓存的相关信息
        /// </summary>
        string Info { get; }
        
    }
}
