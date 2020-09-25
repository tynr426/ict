
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECF.Cache
{
    /// <summary>
    /// redis实例
    /// </summary>
    public class StackExchangeManager
    {
        /// <summary>
        /// 线程同步变量
        /// </summary>
        private static object syncObj = new object();

        /// <summary>
        /// redis链接池管理对象
        /// </summary>
        private static ConnectionMultiplexer _instance = null;

        /// <summary>
        /// 私有构造函数，限制不允许通过new 来实例化该对象
        /// </summary>
        private StackExchangeManager()
        {

        }
        
        /// <summary>
        /// GetMultiplexer
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// ConnectionMultiplexer
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>说明原因 added by Shaipe 2018/10/18</description></item>
        ///   </list>
        /// </remarks>
        public static ConnectionMultiplexer GetMultiplexer(string server, string password = null)
        {
            if (_instance == null || !_instance.IsConnected)
            {
                lock (syncObj)
                {
                    if (_instance == null || !_instance.IsConnected)
                    {
                        ConfigurationOptions option = new ConfigurationOptions
                        {
                            AbortOnConnectFail = false,
                            ConnectTimeout=15000,
                            ResponseTimeout=15000,
                            EndPoints = { server }
                        };
                        if (!string.IsNullOrEmpty(password))
                        {
                            option.Password = password;
                        }
                        _instance = ConnectionMultiplexer.Connect(option);
                    }
                }
            }
            _instance.ErrorMessage += MuxerErrorMessage;
            _instance.HashSlotMoved += MuxerHashSlotMoved;
            _instance.InternalError += MuxerInternalError;
            _instance.ConnectionFailed += MuxerConnectionFailed;
            _instance.ConnectionRestored += MuxerConnectionRestored;
            _instance.ConfigurationChanged += MuxerConfigurationChanged;
            return _instance;
        }

        /// <summary>
        /// GetDatabase
        /// </summary>
        /// <param name="server">The server.</param>
        /// <param name="dbId">The database identifier.</param>
        /// <param name="password">The password.</param>
        /// <returns>
        /// IDatabase
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>说明原因 added by Shaipe 2018/10/18</description></item>
        ///   </list>
        /// </remarks>
        public static IDatabase GetDatabase(string server, int dbId = 0, string password = null)
        {
            if (string.IsNullOrEmpty(server))
            {
                server = "127.0.0.1";
            }
            ConnectionMultiplexer connectionMultiplexer = GetMultiplexer(server, password);
            return connectionMultiplexer.GetDatabase(dbId);
        }
        
        /// <summary>
        /// 获得服务器
        /// </summary>
        /// <returns></returns>
        public static IServer GetServer(string server)
        {
            return _instance.GetServer(server);
        }
        /// <summary>
        /// 配置更改时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConfigurationChanged(object sender, EndPointEventArgs e)
        {
            Console.Write($"Muxer Configuration Changed=>EndPoint：{e.EndPoint}");
        }

        /// <summary>
        /// 发生错误时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerErrorMessage(object sender, RedisErrorEventArgs e)
        {
            Console.Write($"Muxer ErrorMessage=>RedisErrorEventArgs：{e.Message}");
        }

        /// <summary>
        /// 重新建立连接之前的错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionRestored(object sender, ConnectionFailedEventArgs e)
        {
            Console.Write($"Muxer Connection Restored=>ConnectionFailedEventArgs：{e}");
        }

        /// <summary>
        /// 连接失败 ， 如果重新连接成功你将不会收到这个通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerConnectionFailed(object sender, ConnectionFailedEventArgs e)
        {
            Console.Write($"Muxer Connection Failed=>ConnectionFailedEventArgs：{e}");
        }

        /// <summary>
        /// 更改集群
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerHashSlotMoved(object sender, HashSlotMovedEventArgs e)
        {
            Console.Write($"Muxer HashSlot Moved=>HashSlotMovedEventArgs：{e}");
        }

        /// <summary>
        /// redis类库错误
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void MuxerInternalError(object sender, InternalErrorEventArgs e)
        {
            Console.Write($"Muxer Internal Error=>InternalErrorEventArgs：{e}");
        }
    }
}
