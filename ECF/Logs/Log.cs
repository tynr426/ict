using System;
using ECF.Logs.Layout;
using ECF.Logs.Appender;
using ECF.Logs.Util;
using System.Drawing;
using System.Collections.Generic;

namespace ECF.Logs
{
    /// <summary>
    ///   <see cref="ECF.Logs.LogImpl"/>
    /// 日志模块外部调用接口
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
    public abstract class LogImpl : Exception, IDisposable
    {
        #region 处理初始化Exception

        /// <summary>
        /// Initializes a new instance of the <see cref="LogImpl"/> class.
        /// </summary>
        public LogImpl()
        {

        }

        /// <summary>
        /// 日志记录实现构造函数
        /// </summary>
        /// <param name="message">描述信息</param>
        public LogImpl(string message)
            : base(message)
        {

        }

        /// <summary>
        /// 日志记录实现构造函数
        /// </summary>
        /// <param name="message">描述信息</param>
        /// <param name="ex">内部异常</param>
        public LogImpl(string message, Exception ex)
            : base(message, ex)
        {

        }

        /// <summary>
        /// 日志记录实现构造函数
        /// </summary>
        /// <param name="p">行号列号(x = 列号, y = 行号)</param>
        /// <param name="text">模板文本数据</param>
        /// <param name="message">描述信息</param>
        public LogImpl(Point p, string text, string message)
            : this(p.Y, p.X, text, message)
        {

        }

        /// <summary>
        /// 日志记录实现构造函数
        /// </summary>
        /// <param name="line">所在行号</param>
        /// <param name="column">所在列</param>
        /// <param name="text">模板文本数据</param>
        /// <param name="message">描述信息</param>
        public LogImpl(int line, int column, string text, string message)
            : base(string.Format("在解析(行{0}:列{1})的模板文本字符\"{2}\"时,发生错误:{3}", line, column, text, message))
        {

        }

        /// <summary>
        /// 日志记录实现构造函数
        /// </summary>
        /// <param name="fileName">模板文件</param>
        /// <param name="p">行号列号(x = 列号, y = 行号)</param>
        /// <param name="text">模板文本数据</param>
        /// <param name="message">描述信息</param>
        public LogImpl(string fileName, Point p, string text, string message)
            : this(fileName, p.Y, p.X, text, message)
        { }

        /// <summary>
        /// 日志记录实现构造函数
        /// </summary>
        /// <param name="fileName">模板文件</param>
        /// <param name="line">所在行号</param>
        /// <param name="column">所在列</param>
        /// <param name="text">模板文本数据</param>
        /// <param name="message">描述信息</param>
        public LogImpl(string fileName, int line, int column, string text, string message)
            : base(string.Format("在解析文件\"{0}\"(行{1}:列{2})的模板文本字符\"{3}\"时,发生错误:{4}", fileName, line, column, text, message))
        {

        }

        #endregion

        /// <summary>
        /// 用于保护Appder对象
        /// </summary>
        private readonly ReaderWriterLock m_appenderLock = new ReaderWriterLock();

        /// <summary>
        /// 默认日志存放文件夹
        /// </summary>
        virtual protected string LogFolder
        {
            get
            {
                return LogConfig.Instance.LogFolder;
            }
        }

        /// <summary>
        /// 可以通过重载此属性修改日志记录的文件锁定模式
        /// </summary>
        virtual protected Appender.LockingType LockType
        {
            get
            {
                return LogConfig.Instance.LockType;
            }
        }

        /// <summary>
        /// 关闭所有已绑定的Appender
        /// </summary>
        virtual public void Dispose()
        {
            lock (Singleton.Instance)
            {
                foreach (KeyValuePair<string, TextWriterAppender> appender in AppenderContailer.Appenders)
                {
                    appender.Value.OnClose();
                }

                AppenderContailer.Appenders.Clear();
            }
        }

        class AppenderContailer
        {
            private static Dictionary<string, TextWriterAppender> m_appenders = null;

            /// <summary>
            /// 全局添加日志对象
            /// </summary>
            public static Dictionary<string, TextWriterAppender> Appenders
            {
                get
                {
                    lock (Singleton.Instance)
                    {
                        if (m_appenders == null)
                            m_appenders = new Dictionary<string, TextWriterAppender>();

                        System.GC.KeepAlive(m_appenders);
                        System.GC.SuppressFinalize(m_appenders);

                        return m_appenders;
                    }
                }
            }

            /// <summary>
            /// 清除全局Appender
            /// </summary>
            public static void Clear()
            {
                lock (Singleton.Instance)
                {
                    m_appenders.Clear();
                    System.GC.ReRegisterForFinalize(m_appenders);
                    m_appenders = null;
                }
            }
        }

        /// <summary>
        /// 按照记录日志事件指定的格式
        /// </summary>
        /// <param name="loggingEvent">日志事件</param>
        virtual public void Log(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
                throw new ArgumentNullException("loggingEvent");

            if (loggingEvent.EventData == null)
                throw new NullReferenceException("记录日志事件数据不能为空");

            if (String.IsNullOrEmpty(loggingEvent.EventData.LogCode))
                throw new NullReferenceException("记录日志事件日志编码不能为空");

              if (String.IsNullOrEmpty(loggingEvent.EventData.Module))
                throw new NullReferenceException("记录日志事件模块名称不能为空");

              if (String.IsNullOrEmpty(loggingEvent.EventData.Message))
                throw new NullReferenceException("记录日志事件日志信息不能为空");
            
            m_appenderLock.AcquireWriterLock();

            try
            {
                TextWriterAppender appender = null;

                lock (Singleton.Instance)
                {
                    if (!AppenderContailer.Appenders.ContainsKey(loggingEvent.EventData.Module))
                    {
                        appender = new FileAppender(LogFolder, true, loggingEvent.EventData.Module);
                        AppenderContailer.Appenders.Add(loggingEvent.EventData.Module, appender);
                    }
                    else
                    {
                        appender = AppenderContailer.Appenders[loggingEvent.EventData.Module];
                    }

                    appender.DoAppend(loggingEvent);
                }
            }
            finally
            {
                m_appenderLock.ReleaseWriterLock();
            }
        }

        /// <summary>
        /// 按照记录日志事件指定的格式
        /// </summary>
        /// <param name="loggingEvent">日志事件</param>
        /// <param name="layout">记录日志格式</param>
        virtual public void Log(LoggingEvent loggingEvent, ILayout layout)
        {
            if (loggingEvent == null)
                throw new ArgumentNullException("loggingEvent");

            if (loggingEvent.EventData == null)
                throw new NullReferenceException("记录日志事件数据不能为空");

            if (String.IsNullOrEmpty(loggingEvent.EventData.LogCode))
                throw new NullReferenceException("记录日志事件日志编码不能为空");

            if (String.IsNullOrEmpty(loggingEvent.EventData.Module))
                throw new NullReferenceException("记录日志事件模块名称不能为空");

            if (String.IsNullOrEmpty(loggingEvent.EventData.Message))
                throw new NullReferenceException("记录日志事件日志信息不能为空");

            m_appenderLock.AcquireWriterLock();

            try
            {
                TextWriterAppender appender = null;

                lock (Singleton.Instance)
                {

                    if (!AppenderContailer.Appenders.ContainsKey(loggingEvent.EventData.Module))
                    {
                        appender = new FileAppender(LogFolder, true, loggingEvent.EventData.Module, layout);
                        AppenderContailer.Appenders.Add(loggingEvent.EventData.Module, appender);
                    }
                    else
                    {
                        appender = AppenderContailer.Appenders[loggingEvent.EventData.Module];
                    }

                    appender.DoAppend(loggingEvent);
                }
            }
            finally
            {
                m_appenderLock.ReleaseWriterLock();
            }
        }
    }
}
