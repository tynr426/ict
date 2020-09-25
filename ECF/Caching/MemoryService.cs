using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Extensions.Caching.Memory;

namespace ECF.Caching
{
    /// <summary>
    /// System.Web.Cache
    /// </summary>
    public class MemoryService : ICacheService
    {
        private static readonly MemoryCache _instance;

        private static int Factor = 1;

        /// <summary>
        /// 默认过期时长.
        /// </summary>
        public int Expire { get; set; }

        static MemoryService()
        {
            _instance = new MemoryCache(new MemoryCacheOptions());
        }

        /// <summary>
        /// 重设基数
        /// </summary>
        /// <param name="cacheFactor"></param>
        public static void ReSetFactor(int cacheFactor)
        {
            Factor = cacheFactor;
        }

        /// <summary>
        /// 获得
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            try
            {
                return _instance.Get(key);
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            try
            {
                return (T)_instance.Get(key);
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return default(T);
            }
        }
        /// <summary>
        /// 插入对象到Cache中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Insert(string key, object data)
        {
            try
            {
                
                _instance.Set(key, data);
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }
        /// <summary>
        /// 插入对象到Cache中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Insert<T>(string key, T data)
        {
            try
            {
                _instance.Set(key, data);
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }
        /// <summary>
        /// 插入对象到Cache中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        public void Insert(string key, object data, int cacheTime)
        {
            try
            {
                _instance.Set(key, data, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(cacheTime)));
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }
        /// <summary>
        /// 插入对象到Cache中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        public void Insert<T>(string key, T data, int cacheTime)
        {
            try
            {
                _instance.Set(key, data, new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromSeconds(cacheTime)));
            
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }
        /// <summary>
        /// 插入对象到Cache中
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        public void Insert(string key, object data, DateTime cacheTime)
        {
            try
            {
                _instance.Set(key, data, cacheTime);
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime"></param>
        public void Insert<T>(string key, T data, DateTime cacheTime)
        {
            try
            {
                _instance.Set(key, data, cacheTime);
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }

        /// <summary>
        /// 移除指定key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            try
            {
                _instance.Remove(key);
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }
        /// <summary>
        /// 清除带有指定前缀的关键字的缓存
        /// </summary>
        /// <param name="prefixKey"></param>
        public void Clear(string prefixKey)
        {
            try
            {
                _instance.Remove(prefixKey);
                //IDictionaryEnumerator CacheEnum = _instance.GetEnumerator();
                //ArrayList al = new ArrayList();
                //while (CacheEnum.MoveNext())
                //{
                //    if (CacheEnum.Key.ToString().ToLower().IndexOf(prefixKey.ToLower()) == 0)
                //    {
                //        al.Add(CacheEnum.Key);
                //    }
                //}

                //foreach (string key in al)
                //{
                //    _instance.Remove(key);
                //}
            }
            catch (Exception ex)
            {
                new ECFException(ex, "Cache-Memory");
            }
        }

        /// <summary>
        /// 获取所有缓存键.
        /// </summary>
        public string[] Keys
        {
            get
            {
                try
                {
                    return new string[] { };
                    //IDictionaryEnumerator CacheEnum = _instance.GetEnumerator();
                    //List<string> al = new List<string>();
                    //while (CacheEnum.MoveNext())
                    //{
                    //    al.Add(CacheEnum.Key.ToString());
                    //}
                    //return al.ToArray();
                }
                catch (Exception ex)
                {
                    new ECFException(ex);
                    return new string[] { };
                }
            }
        }

        /// <summary>
        /// 获取当前缓存信息
        /// </summary>
        public string Info
        {
            get
            {
                return "Memory Cache: " + _instance.GetType().ToString();
            }
        }


        /// <summary>
        /// 从Cache中移除所有项目
        /// </summary>
        public void Clear()
        {
            try
            {

            }
            catch (Exception ex)
            {
                new ECFException(ex, "Cache-Memory");
            }
        }
        ///// <summary>
        ///// 根据正则表达式匹配符合的Key，然后移除
        ///// </summary>
        ///// <param name="pattern"></param>
        public void RemoveByPattern(string pattern)
        {
            try
            {
                
            }
            catch (Exception ex)
            {
                new ECFException(ex, "Cache-Memory");
            }
        }
    }
}
