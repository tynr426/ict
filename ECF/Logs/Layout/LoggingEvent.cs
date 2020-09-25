using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ECF.Logs.Layout
{
	/// <summary>
	///   <see cref="ECF.Logs.Layout.LoggingData"/>
	/// 记录日志事件的定义
	/// Author:  LY
	/// Created: 2011/11/18
	/// </summary>
	public class LoggingData
    {
        #region 公有属性

        /// <summary>
        /// 日志编号
        /// </summary>
        public string LogCode { get; set; }

        /// <summary>
        /// 记录日志的时间
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// 日志信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 模块名称
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// 网络域名
        /// </summary>
        public string Domain = System.Environment.UserDomainName;

        /// <summary>
        /// 当前的登录用户名
        /// </summary>
        public string UserName = System.Environment.UserName;

        /// <summary>
        /// 键值对属性
        /// </summary>
        public Dictionary<string, string> Properties;

        /// <summary>
        /// 日志标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 异常对象
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// 异常信息收集器
        /// </summary>
        public Exception Exception { get; set; }

        #endregion
    }

    /// <summary>
    ///   <see cref="ECF.Logs.Layout.LoggingEvent"/>
    /// 记录日志事件的定义
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
    [Serializable]
    public class LoggingEvent : ISerializable
    {
        #region LoggingData 日志想关数据
        private LoggingData m_data;

        /// <summary>
        /// 日志想关数据
        /// </summary>
        public LoggingData EventData
        {
            get
            {
                return m_data;
            }
            set
            {
                m_data = value;
            }
        }
        #endregion

        #region 构造
        /// <summary>
        /// 构造日志事件对象
        /// </summary>
        /// <param name="Message">日志信息</param>
        /// <param name="ModuleName">模块名称</param>
        /// <param name="Properties">特殊属性</param>
        public LoggingEvent(string Message, string ModuleName, Dictionary<string, string> Properties)
        {
            m_data = new LoggingData();

            m_data.Message = Message;
            m_data.Module = ModuleName;
            m_data.LogCode = null;
            m_data.Properties = Properties;
            m_data.TimeStamp = DateTime.Now;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingEvent"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="ModuleName">Name of the module.</param>
        /// <param name="Properties">The properties.</param>
        public LoggingEvent(string message, Exception ex, string ModuleName, Dictionary<string, string> Properties)
        {
            m_data = new LoggingData();

            m_data.Message = message;
            m_data.Module = ModuleName;
            m_data.LogCode = "00001";
            m_data.Properties = Properties;
            m_data.StackTrace = ex.StackTrace;
            if (ex.InnerException != null)
            {
                m_data.StackTrace += Environment.NewLine + "InnerException:" + ex.InnerException.Message + Environment.NewLine + "Trace:" + ex.InnerException.StackTrace;
            }
            m_data.TimeStamp = DateTime.Now;
        }


        /// <summary>
        /// 构造日志事件对象
        /// </summary>
        /// <param name="LogCode">日志代码</param>
        /// <param name="Message">日志信息</param>
        /// <param name="ModuleName">模块名称</param>
        /// <param name="Properties">特殊属性</param>
        public LoggingEvent(string LogCode, string Message, string ModuleName, Dictionary<string, string> Properties)
        {
            m_data = new LoggingData();

            m_data.Message = Message;
            m_data.Module = ModuleName;
            m_data.LogCode = LogCode;
            m_data.Properties = Properties;
            m_data.TimeStamp = DateTime.Now;
        }
        #endregion

        #region 实现序列化

        /// <summary>
        /// 实现序列化
        /// </summary>
#if NET_4_0
        [System.Security.SecurityCritical]
#else
        [System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter = true)]
#endif
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("LogCode", m_data.LogCode);
            info.AddValue("Message", m_data.Message);
            info.AddValue("Module", m_data.Module);
            info.AddValue("Properties", m_data.Properties);
            info.AddValue("TimeStamp", m_data.TimeStamp);
            info.AddValue("UserName", m_data.UserName);
            info.AddValue("Domain", m_data.Domain);
            info.AddValue("StackTrace", m_data.StackTrace);
        }

        #endregion

        #region 公有方法 查找属性

        /// <summary>
        /// 查找属性
        /// </summary>
        /// <param name="key">属性关键字</param>
        /// <returns></returns>
        public string LookupProperty(string key)
        {
            if (m_data.Properties != null)
            {
                return m_data.Properties[key];
            }

            return String.Empty;
        }

        #endregion
    }
}
