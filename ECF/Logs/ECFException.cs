using System;
using System.IO;
using System.Collections.Generic;
using ECF.Logs.Util;
using ECF.Logs.Appender;
using ECF.Logs.Layout;


namespace ECF
{
    /// <summary>
    /// FullName： <see cref="ECF.ECFException"/>
    /// Summary ： ECF exception Class 
    /// Version： 1.0.0.0 
    /// DateTime： 2011/12/25 10:21 
    /// CopyRight (c) by shaipe
    /// </summary>
    public class ECFException : Exception, IDisposable
    {
        #region 处理初始化Exception

        /// <summary>
        /// Initializes a new instance of the <see cref="ECF.ECFException" /> class.
        /// </summary>
        public ECFException()
        {

        }

        /// <summary>
        /// 日志记录实现构造函数
        /// </summary>
        /// <param name="message">描述信息</param>
        public ECFException(string message)
            : base(message)
        {
            if (string.IsNullOrEmpty(message)) return;

            if (Debuger.AppType == ApplicationType.Console)
            {
                Console.WriteLine(message);
            }
            else if (Debuger.IsDebug)
            {
                LoggingEvent loggingEvent = new LoggingEvent("DEBUG-00001", message, Module, null);
                this.Log(loggingEvent);
            }
        }

        /// <summary>
        /// 给定模板和消息进行日志记录
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="module">The module.</param>
        public ECFException(string message, string module)
            : base(message)
        {
            if (string.IsNullOrEmpty(message)) return;

            // 给定默认值
            module = string.IsNullOrEmpty(module) ? Module : module;

            if (Debuger.AppType == ApplicationType.Console)
            {
                Console.WriteLine(message);
            }
            else if (Debuger.IsDebug)
            {
                LoggingEvent loggingEvent = new LoggingEvent("DEBUG-00001", message, module, null);
                this.Log(loggingEvent);
            }

        }


        /// <summary>
        /// 日志记录实现构造函数
        /// </summary>
        /// <param name="ex">内部异常</param>
        public ECFException(Exception ex)
        {
            LoggingEvent loggingEvent = new LoggingEvent(ex.Message, ex, Module, null);

            this.Log(loggingEvent);
        }


        /// <summary>
        /// 日志记录实现构造函数
        /// </summary>
        /// <param name="ex">内部异常</param>
        /// <param name="module">存放模块.</param>
        public ECFException(Exception ex, string module)
        {
            LoggingEvent loggingEvent = new LoggingEvent(ex.Message, ex, module, null);

            this.Log(loggingEvent);
        }

        /// <summary>
        /// 日志记录实现构造函数
        /// </summary>
        /// <param name="message">描述信息</param>
        /// <param name="ex">内部异常</param>
        public ECFException(string message, Exception ex)
            : base(message, ex)
        {
            if (string.IsNullOrEmpty(message)) return;

            LoggingEvent loggingEvent = new LoggingEvent(message, ex, Module, null);

            this.Log(loggingEvent);
        }

        /// <summary>
        /// 给定模板和消息进行日志记录
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="ex">The ex.</param>
        /// <param name="module">The module.</param>
        public ECFException(string message, Exception ex, string module)
            : base(message, ex)
        {
            if (string.IsNullOrEmpty(message)) return;

            LoggingEvent loggingEvent = new LoggingEvent(message, ex, module, null);

            this.Log(loggingEvent);
        }



        /// <summary>
        /// 记录日志的模块名称
        /// </summary>
        public virtual string Module
        {
            get
            {
                return "ECF";
            }
        }


        #endregion

        /// <summary>
        /// 用于保护Appder对象
        /// </summary>
        private readonly ReaderWriterLock m_appenderLock = new ReaderWriterLock();

        /// <summary>
        /// 重载日志输出的目录
        /// </summary>
        protected string LogFolder
        {
            get
            {
                return Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SysLogs");
            }
        }

        #region DefaultLogFile 默认日志文件名格式

        private string _DefaultLogFile = "Log{0}.log";

        /// <summary>
        /// 默认日志文件名格式
        /// </summary>
        public string DefaultLogFile
        {
            get
            {
                return _DefaultLogFile;
            }
            set
            {
                _DefaultLogFile = value;
            }
        }

        #endregion

        /// <summary>
        /// 可以通过重载此属性修改日志记录的文件锁定模式
        /// </summary>
        virtual protected LockingType LockType
        {
            get
            {
                return LockingType.Exclusive;
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


        /// <summary>
        /// FullName： <see cref="ECF.ECFException.AppenderContailer"/>
        /// Summary ： 追加容器 
        /// Version： 1.0.0.0 
        /// DateTime： 2012/4/24 22:41 
        /// CopyRight (c) by shaipe
        /// </summary>
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

            //if (String.IsNullOrEmpty(loggingEvent.EventData.LogCode))
            //    throw new NullReferenceException("记录日志事件日志编码不能为空");

            if (String.IsNullOrEmpty(loggingEvent.EventData.Module))
                throw new NullReferenceException("记录日志事件模块名称不能为空");

            if (String.IsNullOrEmpty(loggingEvent.EventData.Message))
                throw new NullReferenceException("记录日志事件日志信息不能为空");

            //获取一个可写锁
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
