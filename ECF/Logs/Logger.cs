using ECF.Logs.Appender;
using ECF.Logs.Util;
using System.Collections.Generic;
using System.IO;

namespace ECF.Logs
{
    public class Logger
    {

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
        /// 用于保护Appder对象
        /// </summary>
        private static readonly ECF.Logs.Util.ReaderWriterLock m_appenderLock = new ECF.Logs.Util.ReaderWriterLock();

        /// <summary>
        /// 重载日志输出的目录
        /// </summary>
        protected static string LogFolder
        {
            get
            {
                return Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "SysLogs");
            }
        }

        /// <summary>
        /// Writer
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="module">The module.</param>
        /// <param name="fileFormat">The file format.</param>
        /// <param name="filePath">The file path.</param>
        /// <remarks>
        /// <list>
        ///   <item>
        ///     <description>说明原因 added by Shaipe 2018/11/6</description>
        ///   </item>
        /// </list></remarks>
        public static void Writer(string content, string module = "log", string fileFormat = "log{0}.log", string filePath = "")
        {
            //获取一个可写锁
            m_appenderLock.AcquireWriterLock();

            try
            {
                TextWriterAppender appender = null;

                if (string.IsNullOrEmpty(filePath)) filePath = LogFolder;

                lock (Singleton.Instance)
                {
                    if (!AppenderContailer.Appenders.ContainsKey(module))
                    {
                        appender = new FileAppender(filePath, module, fileFormat, true);
                        AppenderContailer.Appenders.Add(module, appender);
                    }
                    else
                    {
                        appender = AppenderContailer.Appenders[module];
                    }

                    appender.DoAppend(content + "\r\n");
                }
            }
            finally
            {
                m_appenderLock.ReleaseWriterLock();
            }
        }
    }

}
