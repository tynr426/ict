using System;
using System.IO;
using ECF.Logs.Layout;
using ECF.Logs.Util;

namespace ECF.Logs.Appender
{
    /// <summary>
    ///   <see cref="ECF.Logs.Appender.TextWriterAppender"/>
    /// 实现日志文件基于文件的基类实现
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
	public abstract class TextWriterAppender
    {
        #region 构造函数
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public TextWriterAppender()
        {
        }

        /// <summary>
        /// 替代默认的构造函数并设置Writer属性
        /// </summary>
        /// <param name="writer">底层文本输出流</param>
        public TextWriterAppender(TextWriter writer)
        {
            Writer = writer;
        }
        #endregion

        #region ImmediateFlush 是否立即刷新文件

        /// <summary>
        /// 是否立即刷新文件
        /// </summary>
        private bool m_ImmediateFlush = true;

        /// <summary>
        /// 在经历了Append操作后是否立即清理IO缓存, 使其所有内容输出到文件中
        /// </summary>
        public bool ImmediateFlush
        {
            get
            {
                return m_ImmediateFlush;
            }
            set
            {
                m_ImmediateFlush = value;
            }
        }

        #endregion

        #region QuietWriter 写日志文件辅助工具类

        /// <summary>
        /// 写日志文件辅助工具类
        /// </summary>
        protected CountingQuietTextWriter m_qtw;

        /// <summary>
        /// 写日志文件辅助工具类
        /// </summary>
        protected CountingQuietTextWriter QuietWriter
        {
            get
            {
                return m_qtw;
            }
            set
            {
                m_qtw = value;
            }
        }
        #endregion

        #region Layout 当前Appender的布局

        private ILayout m_layout = new PatternLayout();

        /// <summary>
        /// 当前Appender的布局
        /// </summary>
        public ILayout Layout
        {
            get
            {
                return m_layout;
            }
            set
            {
                m_layout = value;
            }
        }
        #endregion

        #region abstracts

        /// <summary>
        /// 激活Appender用于写日志文件
        /// </summary>
        abstract public void ActivateAppender();

        /// <summary>
        /// 实现滚动生成日志文件, 通过滚动生成索引
        /// </summary>
        abstract public void SetLogFilePath(bool IsRollingLogFile);


        /// <summary>
        /// 关闭当前Appder实例, 底层文本输出流也将被关闭, 且Appder将不能再被恢复重用
        /// </summary>
        abstract public void OnClose();

        #endregion

        #region publics

        /// <summary>
        /// 底层输出流
        /// </summary>
        virtual public TextWriter Writer
        {
            get
            {
                return m_qtw;
            }
            set
            {
                lock (this)
                {
                    Reset();

                    if (value != null)
                    {
                        m_qtw = new CountingQuietTextWriter(value);
                    }
                }
            }
        }


        /// <summary>
        /// 添加写日志事件
        /// </summary>
        /// <param name="loggingEvent">日志事件</param>
        public void DoAppend(LoggingEvent loggingEvent)
        {
            lock (this)
            {
                if (PreAppendCheck())
                {
                    Append(loggingEvent);
                }
            }
        }

        /// <summary>
        /// 添加写日志事件
        /// </summary>
        /// <param name="content">日志事件</param>
        public void DoAppend(string content)
        {
            lock (this)
            {
                if (PreAppendCheck())
                {
                    Append(content);
                }
            }
        }
        #endregion

        #region protecteds

        /// <summary>
        /// 子类需要延长实例化writer可以重载此方法, 方法允许子类序列化writer对象多次
        /// </summary>
        virtual protected void PrepareWriter()
        {
        }


        /// <summary>
        /// 方法将检查底层输出流是否被实例化和布局对象是否被实例化.
        /// </summary>
        /// <returns>某个前提条件未实现</returns>
        virtual protected bool PreAppendCheck()
        {
            if (m_layout == null)
                return false;

            if (m_qtw == null)
            {
                // 允许子类延迟实例化Writer对象
                PrepareWriter();

                if (m_qtw == null)
                    return false;
            }

            if (m_qtw.Closed)
                return false;

            return true;
        }


        /// <summary>
        /// 将日志记录事情根据指定格式呈现
        /// </summary>
        /// <param name="writer">底层文本输出流</param>
        /// <param name="loggingEvent">日志记录事件</param>
        protected void RenderLoggingEvent(TextWriter writer, LoggingEvent loggingEvent)
        {
            Layout.Format(writer, loggingEvent);
        }

        /// <summary>
        /// 执行传入的LoggingEvent事件
        /// </summary>
        /// <param name="loggingEvent">日志记录事件</param>
        virtual protected void Append(LoggingEvent loggingEvent)
        {
            RenderLoggingEvent(m_qtw, loggingEvent);

            // 刷新文件将输出路缓冲区的加入文件中
            if (ImmediateFlush)
            {
                m_qtw.Flush();
            }
        }

        /// <summary>
        /// 内容追加
        /// </summary>
        /// <param name="content">内容.</param>
        /// <remarks>
        ///   <list>
        ///    <item><description>说明原因 added by Shaipe 2018/11/6</description></item>
        ///   </list>
        /// </remarks>
        virtual protected void Append(string content)
        {
            m_qtw.Write(content);

            // 刷新文件将输出路缓冲区的加入文件中
            if (ImmediateFlush)
            {
                m_qtw.Flush();
            }
        }

        /// <summary>
        /// 关闭输出流
        /// </summary>
        virtual protected void CloseWriter()
        {
            if (m_qtw != null)
            {
                try
                {
                    m_qtw.Close();
                }
                catch (Exception)
                {

                }
            }
        }

        /// <summary>
        /// 重置当前Appder
        /// </summary>
        virtual protected void Reset()
        {
            CloseWriter();
            m_qtw = null;
        }
        #endregion

    }
}
