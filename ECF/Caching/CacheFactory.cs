using ECF.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF.Caching
{
    /// <summary>
    /// 版权信息：CopyRight (c) 2018 任我行科技研发中心
    /// 文件名称：<see cref="ECF.Caching.CacheFactory"/>
    /// 作者：XP-COMPANY-Shaipe
    /// 日期：2018/10/18
    /// 摘要：缓存工厂类
    /// 版本：3.1.9
    /// </summary>
    public class CacheFactory
    {

        private static object lockHelper = new object();
        /// <summary>
        /// The _ instance
        /// </summary>
        private static Hashtable _Instance = new Hashtable();

        static ICacheService cacheService = null;

        /// <summary>
        /// 获取缓存服务
        /// </summary>
        /// <param name="cacheProvider">The cache provider.</param>
        /// <returns>
        /// ICacheService
        /// </returns>
        /// <remarks>
        /// <list>
        ///   <item>
        ///     <description>说明原因 added by Shaipe 2018/10/18</description>
        ///   </item>
        /// </list>
        /// </remarks>
        public static ICacheService GetCacheService(CacheServerProvider cacheProvider = null)
        {
            if (cacheService == null)
            {
                if (cacheProvider == null) return null;

                lock (lockHelper)
                {
                    switch (cacheProvider.Type)
                    {
                        case CacheType.Memcached:

                            break;
                        case CacheType.Redis:
                            object redis = null;
                            if (cacheProvider != null)
                            {
                                redis = GetInstance("ECF.Cache", "StackExchangeRedisService",
                                new object[] { cacheProvider.Server, cacheProvider.Password, Utils.ToInt(cacheProvider.DBName) });
                            }
                            else
                            {
                                redis = GetInstance("ECF.Cache", "StackExchangeRedisService");
                            }

                            if (redis is ICacheService)
                            {
                                cacheService = (ICacheService)redis;
                            }
                            break;
                        default:
                            cacheService = new MemoryService();
                            break;
                    }
                }

            }

            return cacheService;
        }

        static ICacheService _defaultCache = null;

        /// <summary>
        /// 获取默认的缓存服务对象
        /// </summary>
        /// <returns>
        /// ICacheService
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加获取默认服务方法 added by Shaipe 2018/10/22</description></item>
        ///   </list>
        /// </remarks>
        public static ICacheService GetDefaultService()
        {
            if (_defaultCache == null)
            {
                lock (lockHelper)
                {
                    _defaultCache = new MemoryService();
                }
            }
            return _defaultCache;
        }

        /// <summary>
        /// 获取缓存提供者对象
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>
        /// CacheServerProvider
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加方法 added by Shaipe 2018/10/22</description></item>
        ///   </list>
        /// </remarks>
        public static CacheServerProvider GetCacheServerProvider(string key)
        {
            if (CacheConfig.Instance.Caches != null)
            {
                return CacheConfig.Instance.Caches[key];
            }

            return null;
        }



        /// <summary>
        /// 根据配置获取类的实例
        /// </summary>
        /// <param name="assemblyName">程序集名称.</param>
        /// <param name="className">类名.</param>
        /// <param name="parameters">构造参数.</param>
        /// <returns>
        /// Object
        /// </returns>
        /// <exception cref="ECFException">
        /// </exception>
        /// <exception cref="Exceptions">实例化" + className + "失败</exception>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加方法 added by Shaipe 2018/10/18</description></item>
        ///   </list>
        /// </remarks>
        static object GetInstance(string assemblyName, string className, object[] parameters = null)
        {
            string staticName = assemblyName + "_" + className;
            if (parameters != null)
            {
                staticName += "_" + (parameters.GetHashCode());
            }
            if (_Instance.ContainsKey(staticName))
            {
                if (_Instance[staticName] != null)
                    return _Instance[staticName];
                else
                    _Instance.Remove(staticName);
            }

            lock (lockHelper)
            {
                if (!_Instance.ContainsKey(staticName))
                {
                    object p = null;
                    try
                    {
                        Type t = Type.GetType(assemblyName + "." + className + ", " + assemblyName, true, false);
                        if (parameters != null)
                        {
                            p = Activator.CreateInstance(t, parameters);
                        }
                        else
                        {
                            p = Activator.CreateInstance(t);
                        }
                    }
                    catch (Exception ex)
                    {
                        //try
                        //{
                        //    Assembly asm = Assembly.LoadFile(ECF.Utils.RootPath + assemblyName + ".dll");
                        //    p = Activator.CreateInstance(asm.GetType(className, false, false));
                        //}
                        //catch (Exception ex)
                        //{
                        //    throw new ECFException(ex.Message, ex);
                        //}

                        if (ex.InnerException != null)
                        {
                            throw new ECFException(ex.InnerException.Message, ex.InnerException);
                        }
                        throw new ECFException(ex.Message, ex);

                    }

                    if (p != null)
                    {
                        if (_Instance.ContainsKey(staticName))
                        {
                            _Instance.Remove(staticName);
                        }
                        _Instance.Add(staticName, p);
                    }
                    else
                    {
                        throw new ECFException("实例化" + className + "失败");
                    }
                }
            }

            return _Instance[staticName];
        }
    }
}
