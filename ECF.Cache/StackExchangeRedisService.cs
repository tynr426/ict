
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace ECF.Cache
{
    /// <summary>
    /// redis操作
    /// </summary>
    public class StackExchangeRedisService : ICacheService
    {
        /// <summary>
        /// 默认超时时间（单位秒）
        /// </summary>
        int DEFAULT_TMEOUT = 10;
        /// <summary>
        /// database
        /// </summary>
        IDatabase _Database;

        IServer _Server;

        string _serverLocal = "";
        int _DatabaseId = 0;

        ConnectionMultiplexer _Multiplexer = null;

        static readonly object _locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="StackExchangeRedisService"/> class.
        /// </summary>
        /// <param name="server">服务器信息.</param>
        /// <param name="password">密码.</param>
        /// <param name="dbId">数据库Id.</param>
        public StackExchangeRedisService(string server, string password, int dbId)
        {
            try
            {
                lock (_locker)
                {
                    _serverLocal = server;
                    _DatabaseId = dbId;
                    _Multiplexer = StackExchangeManager.GetMultiplexer(server, password);
                    if (_Multiplexer != null && _Multiplexer.IsConnected)
                    {
                        _Server = _Multiplexer.GetServer(server);
                        _Database = _Multiplexer.GetDatabase(dbId);
                    }
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex, "Cache-Redis");
            }
        }

        /// <summary>
        /// 连接超时设置
        /// </summary>
        public int TimeOut
        {
            get
            {
                return DEFAULT_TMEOUT;
            }
            set
            {
                DEFAULT_TMEOUT = value;
            }
        }

        /// <summary>
        /// 获取缓存中所有的key
        /// </summary>
        public string[] Keys
        {
            get
            {
                List<string> all_keys = new List<string>();
                if (_Database == null) return all_keys.ToArray();
                // 清除Redis指字的数据库keys
                // if (_Server != null) _Server.FlushDatabase(_Database.Database); 此模式不允许在非admin模式下使用
                var keys = _Server.Keys(_Database.Database);
                foreach (var key in keys)
                {
                    all_keys.Add(key.ToString());
                }
                return all_keys.ToArray();
            }
        }

        public string Info
        {
            get { return "Redis Cache: " + _serverLocal + "[" + _DatabaseId.ToString() + "]"; }
        }
        
        /// <summary>
        /// 根据key获得值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object Get(string key)
        {
            return Get<object>(key);
        }
        /// <summary>
        /// 根据key获得值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public T Get<T>(string key)
        {
            //默认值
            var value = default(T);
            if (_Database == null) return value;
            try
            {
                //cache值
                var cacheValue = _Database.StringGet(key);

                if (!cacheValue.IsNull)
                {
                    byte[] bytes = (byte[])cacheValue;

                    if (bytes != null && bytes.Length > 0)
                    {
                        value = ByteSerializer.Deserialize<T>(bytes);
                    }
                    if (value == null)
                    {
                        Remove(key);
                    }
                    return value;
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex, "Cache-Redis");
            }
            return value;
        }
        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Insert<T>(string key, T data)
        {
            if (_Database == null) return;
            try
            {
                byte[] bytes = ByteSerializer.Serialize(data);
                if (bytes != null)
                {
                    _Database.StringSet(key, bytes);
                }
                _Database.KeyExpire(key, new TimeSpan(0, 0, TimeOut));
            }
            catch (Exception ex)
            {
                //new ECFException(ex, "Cache-Redis");
            }
        }

        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime">秒钟</param>
        public void Insert<T>(string key, T data, int cacheTime)
        {
            if (_Database == null) return;
            try
            {
                var timeSpan = TimeSpan.FromSeconds(cacheTime);
                byte[] bytes = ByteSerializer.Serialize(data);
                if (bytes != null)
                {
                    _Database.StringSet(key, bytes, timeSpan);
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex, "Cache-Redis");
            }
        }

        /// <summary>
        /// 插入缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime">时间类型</param>
        public void Insert<T>(string key, T data, DateTime cacheTime)
        {
            if (_Database == null) return;
            try
            {
                var currentTime = DateTime.Now;
                var timeSpan = cacheTime - DateTime.Now;
                byte[] bytes = ByteSerializer.Serialize(data);
                if (bytes != null)
                {
                    _Database.StringSet(key, bytes, timeSpan);
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex, "Cache-Redis");
            }
        }

        /// <summary>
        /// 移除
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            if (_Database == null) return;
            _Database.KeyDelete(key, CommandFlags.HighPriority);
        }

        /// <summary>
        /// Redis的keys模糊查询：
        /// </summary>
        /// <param name="prefixKey"></param>
        public void Clear(string prefixKey)
        {
            if (_Database == null) return;
            try
            {
                _Database.ScriptEvaluate(LuaScript.Prepare(
                        //Redis的keys模糊查询：

                        " local ks = redis.call('KEYS', @keypattern) " + //local ks为定义一个局部变量，其中用于存储获取到的keys
                        " for i=1,#ks,5000 do " +    //#ks为ks集合的个数, 语句的意思： for(int i = 1; i <= ks.Count; i+=5000)
                        "     redis.call('del', unpack(ks, i, math.min(i+4999, #ks))) " + //Lua集合索引值从1为起始，unpack为解包，获取ks集合中的数据，每次5000，然后执行删除
                        " end " +
                        " return true "), new { keypattern = prefixKey + "*" });
            }
            catch (Exception ex)
            {
                //new ECFException(ex, "Cache-Redis");
            }
        }

        /// <summary>
        /// 移除所有项目
        /// </summary>
        public void Clear()
        {
            if (_Database == null) return;
            // 清除Redis指字的数据库keys
            // if (_Server != null) _Server.FlushDatabase(_Database.Database); 此模式不允许在非admin模式下使用
            var keys = _Server.Keys(_Database.Database);
            foreach (var key in keys)
            {
                _Database.KeyDelete(key);
            }
        }
        /// <summary>
        /// 根据正则表达式匹配符合的Key，然后移除
        /// </summary>
        /// <param name="pattern"></param>
        public void RemoveByPattern(string pattern)
        {
            if (_Database == null) return;
            try
            {
                _Database.ScriptEvaluate(LuaScript.Prepare(
                       //Redis的keys模糊查询：

                       " local ks = redis.call('KEYS', @keypattern) " + //local ks为定义一个局部变量，其中用于存储获取到的keys
                       " for i=1,#ks,5000 do " +    //#ks为ks集合的个数, 语句的意思： for(int i = 1; i <= ks.Count; i+=5000)
                       "     redis.call('del', unpack(ks, i, math.min(i+4999, #ks))) " + //Lua集合索引值从1为起始，unpack为解包，获取ks集合中的数据，每次5000，然后执行删除
                       " end " +
                       " return true "), new { keypattern = pattern });
            }
            catch (Exception ex)
            {
                //new ECFException(ex, "Cache-Redis");
            }
        }


    }
}
