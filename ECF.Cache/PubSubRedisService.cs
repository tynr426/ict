
using StackExchange.Redis;
using System;
using System.Collections.Generic;

namespace ECF.Cache
{
    /// <summary>
    /// redis操作
    /// </summary>
    public class PubSubRedisService
    {

        ISubscriber _Subscriber;

        ConnectionMultiplexer _Multiplexer = null;

        static readonly object _locker = new object();

        /// <summary>
        /// Initializes a new instance of the <see cref="StackExchangeRedisService"/> class.
        /// </summary>
        /// <param name="server">服务器信息.</param>
        /// <param name="password">密码.</param>
        /// <param name="dbId">数据库Id.</param>
        public PubSubRedisService(string server, string password)
        {
            try
            {
                lock (_locker)
                {
                    _Multiplexer = StackExchangeManager.GetMultiplexer(server, password);
                    if (_Multiplexer != null && _Multiplexer.IsConnected)
                    {
                        _Subscriber = _Multiplexer.GetSubscriber();
                    }
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex, "Cache-Redis");
            }
        }

       
        /// <summary>
        /// 发布
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="channel"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public long RedisPub<T>(string channel, T data)
        {
            if (_Subscriber == null) return -1;
            try
            {
                byte[] bytes = ByteSerializer.Serialize(data);


                if (bytes != null)
                {
                    return _Subscriber.Publish(channel, bytes);
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex, "publish-Redis");
            }

            return -1;

        }
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="subChannael"></param>
        /// <param name="action"></param>
        public void RedisSub<T>(string subChannael, Action<T> action)
        {
            if (_Subscriber == null) return;

            _Subscriber.Subscribe(subChannael, (channel, message) =>

            {
                byte[] bytes = (byte[])message;

                if (bytes != null && bytes.Length > 0)
                {
                    var value = ByteSerializer.Deserialize<T>(bytes);
                    action(value);
                }         

               

            });

        }
    }
}
