using System;
using System.IO;
using System.Text;
using System.Threading;
using ECF.Logs.Layout;
using ECF.Logs.Util;

namespace ECF.Logs.Appender
{
    /// <summary>
    ///   <see cref="ECF.Logs.Appender.FileAppender"/>
    /// 实现日志文件基于滚动文件的实现
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
    public class FileAppender : TextWriterAppender
    {
        #region 文件锁

        /// <summary>
        ///  使用此对象管理对底层输出流资源的的访问
        /// </summary>
        private sealed class LockingStream : Stream, IDisposable
        {
            public sealed class LockStateException : Exception
            {
                public LockStateException(string message)
                    : base(message)
                {

                }
            }

            private Stream m_realStream = null;
            private LockingModelBase m_lockingModel = null;
            private int m_readTotal = -1;
            private int m_lockLevel = 0;

            public LockingStream(LockingModelBase locking)
                : base()
            {
                if (locking == null)
                {
                    throw new ArgumentException("锁定对象为空", "locking");
                }
                m_lockingModel = locking;
            }

            #region 通过重载实现流的基本方法和属性

            #region 重载方法

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                AssertLocked();
                IAsyncResult ret = m_realStream.BeginRead(buffer, offset, count, callback, state);
                m_readTotal = EndRead(ret);
                return ret;
            }

            /// <summary>
            /// 不支持真正的异步写入, 执行强制同步写入
            /// </summary>
            public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                AssertLocked();
                IAsyncResult ret = m_realStream.BeginWrite(buffer, offset, count, callback, state);
                EndWrite(ret);
                return ret;
            }

            public override void Close()
            {
                m_lockingModel.CloseFile();
            }

            public override int EndRead(IAsyncResult asyncResult)
            {
                AssertLocked();
                return m_readTotal;
            }

            public override void EndWrite(IAsyncResult asyncResult)
            {
                //无操作, 已经处理
            }

            public override void Flush()
            {
                AssertLocked();
                m_realStream.Flush();
            }
            public override int Read(byte[] buffer, int offset, int count)
            {
                return m_realStream.Read(buffer, offset, count);
            }

            public override int ReadByte()
            {
                return m_realStream.ReadByte();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                AssertLocked();
                return m_realStream.Seek(offset, origin);
            }

            public override void SetLength(long value)
            {
                AssertLocked();
                m_realStream.SetLength(value);
            }

            void IDisposable.Dispose()
            {
                Close();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                AssertLocked();
                m_realStream.Write(buffer, offset, count);
            }

            public override void WriteByte(byte value)
            {
                AssertLocked();
                m_realStream.WriteByte(value);
            }

            #endregion

            #region 重载Stream属性

            public override bool CanRead
            {
                get { return false; }
            }

            public override bool CanSeek
            {
                get
                {
                    AssertLocked();
                    return m_realStream.CanSeek;
                }
            }

            public override bool CanWrite
            {
                get
                {
                    AssertLocked();
                    return m_realStream.CanWrite;
                }
            }

            public override long Length
            {
                get
                {
                    AssertLocked();
                    return m_realStream.Length;
                }
            }

            public override long Position
            {
                get
                {
                    AssertLocked();
                    return m_realStream.Position;
                }
                set
                {
                    AssertLocked();
                    m_realStream.Position = value;
                }
            }

            #endregion

            /// <summary>
            /// 判断流是否被锁定
            /// </summary>
            private void AssertLocked()
            {
                if (m_realStream == null)
                {
                    throw new LockStateException("当前文件不能被锁定");
                }
            }

            /// <summary>
            /// 获取锁
            /// </summary>
            /// <returns>是否获取成功</returns>
            public bool AcquireLock()
            {
                bool ret = false;

                lock (this)
                {
                    if (m_lockLevel == 0)
                    {
                        m_realStream = m_lockingModel.AcquireLock();
                    }
                    if (m_realStream != null)
                    {
                        m_lockLevel++;
                        ret = true;
                    }
                }
                return ret;
            }

            /// <summary>
            /// 释放锁
            /// </summary>
            public void ReleaseLock()
            {
                lock (this)
                {
                    m_lockLevel--;

                    if (m_lockLevel == 0)
                    {
                        m_lockingModel.ReleaseLock();
                        m_realStream = null;
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// 锁对象基类
        /// </summary>
        public abstract class LockingModelBase
        {
            private FileAppender m_appender = null;

            /// <summary>
            /// 打开输入日志的文件
            /// </summary>
            /// <param name="filename">需要打开的文件名</param>
            /// <param name="append">是添加到文件尾, 还是覆盖文件</param>
            /// <param name="encoding">使用的文件编码</param>
            public abstract void OpenFile(string filename, bool append, Encoding encoding);

            /// <summary>
            /// 关闭输出日志的文件
            /// </summary>
            public abstract void CloseFile();

            /// <summary>
            /// 获取输出日志文件的锁
            /// </summary>
            /// <returns>返回可以进行读写操作的流</returns>
            public abstract Stream AcquireLock();

            /// <summary>
            /// 释放输出日志文件的锁
            /// </summary>
            public abstract void ReleaseLock();

            /// <summary>
            /// 设置或获取当前锁对象依附的Appder
            /// </summary>
            public FileAppender CurrentAppender
            {
                get { return m_appender; }
                set { m_appender = value; }
            }

            /// <summary>
            /// 建立一个文件写输出流
            /// </summary>
            /// <param name="filename">文件名</param>
            /// <param name="append">添加或是覆盖原文件</param>
            /// <param name="fileShare">文件共享访问类型</param>
            /// <returns></returns>
            protected Stream CreateStream(string filename, bool append, FileShare fileShare)
            {
                // 确保文件夹结构存在
                string directoryFullName = Path.GetDirectoryName(filename);

                if (!Directory.Exists(directoryFullName))
                {
                    Directory.CreateDirectory(directoryFullName);
                }

                FileMode fileOpenMode = append ? FileMode.Append : FileMode.Create;
                return new FileStream(filename, fileOpenMode, FileAccess.Write, fileShare);

            }

            /// <summary>
            /// 关闭文件输出流 
            /// </summary>
            /// <param name="stream">文件流</param>
            protected void CloseStream(Stream stream)
            {
                stream.Close();
            }
        }

        /// <summary>
        /// 保持输出文件的独占锁
        /// </summary>
        /// <remarks>
        /// <para>
        /// 打开日志输出文件进行写操作, 直到显式调用关闭Close方法
        /// 维护这期间的输出日志文件的独占锁
        /// </para>
        /// </remarks>
        public class ExclusiveLock : LockingModelBase
        {
            private Stream m_stream = null;

            /// <summary>
            /// 打开文件为写日志做准备
            /// </summary>
            /// <param name="filename">日志文件名</param>
            /// <param name="append">是否添加日志信息到原日志文件尾, 或覆盖原日志文件</param>
            /// <param name="encoding">日志文件编码</param>
            public override void OpenFile(string filename, bool append, Encoding encoding)
            {
                try
                {
                    m_stream = CreateStream(filename, append, FileShare.Read);
                }
                catch //(Exception ex)
                {
                    // EventLogs.Instance.Log(ex.Message + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
                }
            }

            /// <summary>
            /// 关闭文件
            /// </summary>
            public override void CloseFile()
            {
                if (m_stream != null)
                {
                    CloseStream(m_stream);
                    m_stream = null;
                }
            }

            /// <summary>
            /// 获取文件锁
            /// </summary>
            /// <returns>用于写操作的文件流</returns>
            /// <remarks>
            /// <para>
            /// 什么都没做, OpenFile方法时已经锁定文件, 在显式调用ReleaseLock之前日志文件对于其他进程将为只读
            /// </para>
            /// </remarks>
            public override Stream AcquireLock()
            {
                //try
                //{
                //    m_stream.Position = 0;
                //}
                //catch (Exception) { }
                return m_stream;
            }

            /// <summary>
            /// 释放文件锁
            /// </summary>
            /// <remarks>
            /// <para>
            /// 什么也没做, 文件锁定已经在关闭文件CloseFile时释放
            /// </para>
            /// </remarks>
            public override void ReleaseLock()
            {

            }
        }

        /// <summary>
        /// 每次写的时候获取文件锁
        /// </summary>
        /// <remarks>
        /// <para>
        /// 每次写日志文件均需要获取、释放锁, 因此只需要位置锁很短的时间, 这将使读写操作变慢
        /// 但允许其他进程移动或删除日志文件, 而日志文件可以重建
        /// </para>
        /// </remarks>
        public class MinimalLock : LockingModelBase
        {
            private string m_filename;
            private bool m_append;
            private Stream m_stream = null;

            /// <summary>
            /// 打开文件为写日志做准备
            /// </summary>
            /// <param name="filename">日志文件名</param>
            /// <param name="append">是否添加日志信息到原日志文件尾, 或覆盖原日志文件</param>
            /// <param name="encoding">日志文件编码</param>
            public override void OpenFile(string filename, bool append, Encoding encoding)
            {
                m_filename = filename;
                m_append = append;
            }

            /// <summary>
            /// 关闭文件
            /// </summary>
            public override void CloseFile()
            {

            }

            /// <summary>
            /// 获取文件锁
            /// </summary>
            public override Stream AcquireLock()
            {
                if (m_stream == null)
                {
                    try
                    {
                        m_stream = CreateStream(m_filename, m_append, FileShare.Read);
                        m_append = true;
                    }
                    catch (Exception)
                    {

                    }
                }
                return m_stream;
            }

            /// <summary>
            /// 释放文件锁
            /// </summary>
            public override void ReleaseLock()
            {
                CloseStream(m_stream);
                m_stream = null;
            }
        }

        /// <summary>
        /// 提供跨进程文件锁定
        /// </summary>
        public class InterProcessLock : LockingModelBase
        {
            private Mutex m_mutex = null;
            private bool m_mutexClosed = false;
            private Stream m_stream = null;

            /// <summary>
            /// 打开文件为写日志做准备
            /// </summary>
            /// <param name="filename">日志文件名</param>
            /// <param name="append">是否添加日志信息到原日志文件尾, 或覆盖原日志文件</param>
            /// <param name="encoding">日志文件编码</param>
#if NET_4_0
            [System.Security.SecuritySafeCritical]
#endif
            public override void OpenFile(string filename, bool append, Encoding encoding)
            {
                try
                {
                    m_stream = CreateStream(filename, append, FileShare.ReadWrite);

                    string mutextFriendlyFilename = filename
                            .Replace("\\", "_")
                            .Replace(":", "_")
                            .Replace("/", "_");

                    m_mutex = new Mutex(false, mutextFriendlyFilename);
                }
                catch (Exception)
                {

                }
            }

            /// <summary>
            /// 关闭文件
            /// </summary>
            public override void CloseFile()
            {
                try
                {
                    if (m_stream != null)
                    {
                        CloseStream(m_stream);
                        m_stream = null;
                    }
                }
                finally
                {
                    m_mutex.ReleaseMutex();
                    m_mutex.Close();
                    m_mutexClosed = true;
                }
            }

            /// <summary>
            /// 获取文件锁
            /// </summary>
            public override Stream AcquireLock()
            {
                if (m_mutex != null)
                {
                    // TODO: 是否添加超时处理?
                    m_mutex.WaitOne();

                    // 对于FileStream来说永远应该是真(将获得更快的速度)
                    if (m_stream.CanSeek)
                    {
                        m_stream.Seek(0, SeekOrigin.End);
                    }
                }

                return m_stream;
            }

            /// <summary>
            /// 释放文件锁
            /// </summary>
            public override void ReleaseLock()
            {
                if (m_mutexClosed == false && m_mutex != null)
                {
                    m_mutex.ReleaseMutex();
                }
            }
        }

        #endregion

        #region 构造函数

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public FileAppender()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="LogFolder">日志文件存放的目录</param>
        /// <param name="AppendToFile">是将日志信息添加到末位还是覆盖原有信息</param>
        /// <param name="Module">模块名称</param>
        public FileAppender(string LogFolder, bool AppendToFile, string Module)
        {
            // 必须放在首位, LogFolder参数需要模块名称
            this.Module = Module;

            this.LogFolder = LogFolder;
            this.AppendToFile = AppendToFile;
            // 写日志以前的准备工作
            ActivateAppender();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="LogFolder">日志文件存放的目录</param>
        /// <param name="AppendToFile">是将日志信息添加到末位还是覆盖原有信息</param>
        /// <param name="Module">模块名称</param>
        /// <param name="layout">布局信息</param>
        public FileAppender(string LogFolder, bool AppendToFile, string Module, ILayout layout)
        {
            // 必须放在首位, LogFolder参数需要模块名称
            this.Module = Module;

            this.LogFolder = LogFolder;
            this.AppendToFile = AppendToFile;
            base.Layout = layout;

            ActivateAppender();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileAppender"/> class.
        /// </summary>
        /// <param name="folder">The folder.</param>
        /// <param name="module">The module.</param>
        /// <param name="fileFormat">The file format.</param>
        /// <param name="appendToFile">if set to <c>true</c> [append to file].</param>
        public FileAppender(string folder, string module, string fileFormat = "log_{0}.log", bool appendToFile = true)
        {
            this.Module = module;
            AppendToFile = appendToFile;
            FileFormat = fileFormat;
            this.LogFolder = folder;
            ActivateAppender();
            
        }

        #endregion

        #region 公有属性

        /// <summary>
        /// 计算当前需要新生成日志文件的索引
        /// </summary>
        /// <param name="IsRollingLogFile">是否需要滚动生成日志文件</param>
        /// <param name="logFile">The log file.</param>
        /// <remarks>
        ///   <list>
        ///    <item><description>说明原因 added by Shaipe 2018/11/7</description></item>
        ///   </list>
        /// </remarks>
        override public void SetLogFilePath(bool IsRollingLogFile)
        {
            string logFile = FileFormat;
            if (string.IsNullOrEmpty(logFile))
            {
                logFile = LogConfig.Instance.DefaultLogFile;
            }

            if (IsRollingLogFile)
            {
                int IncreaseRollingIndex = Directory.GetFiles(m_logFolder).Length + 1;

                m_fileName = Path.Combine(m_logFolder, String.Format(logFile, IncreaseRollingIndex));

                // 如果已经存在日志文件名, 则增加索引增长因子
                while (File.Exists(m_fileName))
                {
                    ++IncreaseRollingIndex;
                    m_fileName = Path.Combine(m_logFolder, String.Format(logFile, IncreaseRollingIndex));
                }

                return;
            }

            m_fileName = Path.Combine(m_logFolder, String.Format(logFile, Directory.GetFiles(m_logFolder).Length));

            FileInfo fileInfo = new FileInfo(m_fileName);

            // 如果指定索引日志文件不存在或存在的索引文件已经超出滚动生成日志文件文件大小上限
            if (!fileInfo.Exists || Convert.ToInt32(fileInfo.Length) >= LogConfig.Instance.RollingMaxSize)
            {
                SetLogFilePath(true);
            }
        }

        /// <summary>
        /// 日志文件存放目录
        /// </summary>
        virtual protected string LogFolder
        {
            set
            {
                m_logFolder = Path.Combine(value, Module);

                if (!Directory.Exists(m_logFolder))
                    Directory.CreateDirectory(m_logFolder);

                SetLogFilePath(false);
            }
        }

        string _fileFormat;

        /// <summary>
        /// Gets or sets the file format.
        /// </summary>
        virtual protected string FileFormat
        {
            get
            {
                return _fileFormat;
            }
            set
            {
                _fileFormat = value;
            }
        }

        /// <summary>
        /// 模块名称
        /// </summary>
        virtual protected string Module
        {
            get
            {
                return m_moduleName;
            }
            set
            {
                m_moduleName = value;
            }
        }

        /// <summary>
        /// 指定日志文件是覆盖原文件, 还是添加新日志信息到源文件尾部
        /// </summary>
        public bool AppendToFile
        {
            get
            {
                return m_appendToFile;
            }
            set
            {
                m_appendToFile = value;
            }
        }

        /// <summary>
        /// 编码方式
        /// </summary>
        public Encoding Encoding
        {
            get
            {
                return m_encoding;
            }
            set
            {
                m_encoding = value;
            }
        }

        /// <summary>
        /// 日志文件锁定模式
        /// </summary>
        public FileAppender.LockingModelBase LockingModel
        {
            get
            {
                return m_lockingModel;
            }
            set
            {
                m_lockingModel = value;
            }
        }

        #endregion

        /// <summary>
        /// 写日志以前的准备工作
        /// </summary>
        override public void ActivateAppender()
        {
            if (m_lockingModel == null)
            {
                switch (LogConfig.Instance.LockType)
                {
                    //独占锁
                    case LockingType.Exclusive:
                        m_lockingModel = new ExclusiveLock();
                        break;
                    //跨进程文件锁定
                    case LockingType.InterProcess:
                        m_lockingModel = new InterProcessLock();
                        break;
                    //最小时间锁
                    case LockingType.Minimal:
                        m_lockingModel = new MinimalLock();
                        break;
                }
            }

            m_lockingModel.CurrentAppender = this;

            if (m_fileName != null)
            {
                PrepareWriter();
            }
        }

        /// <summary>
        /// 调用父类TextWriterAppender的Reset方法关闭先前打开的日志文件
        /// </summary>
        override protected void Reset()
        {
            base.Reset();
            m_fileName = null;
        }

        /// <summary>
        /// 初始化文件输出流
        /// </summary>
        override protected void PrepareWriter()
        {
            SafeOpenFile(m_fileName, m_appendToFile);
        }

        /// <summary>
        /// 重载关闭当前所有输出设备
        /// </summary>
        public override void OnClose()
        {
            lock (this)
            {
                // 需要重置当前
                if (LogConfig.Instance.LockType != LockingType.Minimal)
                    this.LockingModel.CloseFile();
                //关闭先打开的日志
                Reset();
            }
        }

        /// <summary>
        /// 添加写日志事件
        /// </summary>
        /// <param name="loggingEvent">日志事件</param>
        override protected void Append(LoggingEvent loggingEvent)
        {
            // 如果当前文件大小已经超过限定单个日志文件大小, 则重建之
            if (m_qtw.Count >= LogConfig.Instance.RollingMaxSize)
            {
                OnClose();

                this.SetLogFilePath(true);
                this.ActivateAppender();
            }
            //m_stream.Position = 0;
            if (m_stream.AcquireLock())
            {
                try
                {
                    base.Append(loggingEvent);
                }
                finally
                {
                    m_stream.ReleaseLock();
                }
            }
        }

        /// <summary>
        /// Append
        /// </summary>
        /// <param name="content">The content.</param>
        /// <remarks>
        /// <list>
        ///   <item>
        ///     <description>说明原因 added by Shaipe 2018/11/6</description>
        ///   </item>
        /// </list>
        /// </remarks>
        override protected void Append(string content)
        {
            // 如果当前文件大小已经超过限定单个日志文件大小, 则重建之
            if (m_qtw.Count >= LogConfig.Instance.RollingMaxSize)
            {
                OnClose();

                this.SetLogFilePath(true);
                this.ActivateAppender();
            }
            //m_stream.Position = 0;
            if (m_stream.AcquireLock())
            {
                try
                {
                    base.Append(content);
                }
                finally
                {
                    m_stream.ReleaseLock();
                }
            }
        }

        /// <summary>
        /// 关闭底层输出流
        /// </summary>
        protected override void CloseWriter()
        {
            if (m_stream != null)
            {
                m_stream.AcquireLock();

                try
                {
                    base.CloseWriter();
                }
                finally
                {
                    m_stream.ReleaseLock();
                }
            }
        }

        /// <summary>
        /// 打开文件为写日志操作做准备(已捕获异常)
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="append">是否添加日志信息到文件尾</param>
        virtual protected void SafeOpenFile(string fileName, bool append)
        {
            try
            {
                OpenFile(fileName, append);
            }
            catch // (Exception ex)
            {
                // EventLogs.Instance.Log(ex.Message + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
            }
        }

        /// <summary>
        /// 打开文件为写日志操作做准备
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="append">是否添加日志信息到文件尾</param>
        virtual protected void OpenFile(string fileName, bool append)
        {
            lock (this)
            {
                //先进行关闭之前打开的文件
                Reset();

                // 保存这些后, 允许重试如果打开文件失败
                m_fileName = fileName;
                m_appendToFile = append;

                switch (LogConfig.Instance.LockType)
                {
                    //独占
                    case LockingType.Exclusive:
                        m_lockingModel = new ExclusiveLock();
                        break;
                    //跨进程文件锁
                    case LockingType.InterProcess:
                        m_lockingModel = new InterProcessLock();
                        break;
                    //最小时间
                    case LockingType.Minimal:
                        m_lockingModel = new MinimalLock();
                        break;
                }
                //
                LockingModel.CurrentAppender = this;
                //打开文件
                LockingModel.OpenFile(fileName, append, m_encoding);

                m_stream = new LockingStream(LockingModel);

                if (m_stream != null)
                {
                    m_stream.AcquireLock();

                    try
                    {
                        SetQWForFiles(new StreamWriter(m_stream, m_encoding));
                    }
                    finally
                    {
                        m_stream.ReleaseLock();
                    }
                }
            }
        }

        /// <summary>
        /// 初始化QuietWriter文本输出流
        /// </summary>
        virtual protected void SetQWForFiles(Stream fileStream)
        {
            SetQWForFiles(new StreamWriter(fileStream, m_encoding));
        }

        /// <summary>
        /// 初始化QuietWriter文本输出流
        /// </summary>
        virtual protected void SetQWForFiles(TextWriter writer)
        {
            QuietWriter = new CountingQuietTextWriter(writer);
        }

        #region 私有属性

        /// <summary>
        /// 是添加新日志信息到文件末位, 还是覆盖原日志文件
        /// </summary>
        private bool m_appendToFile = true;

        /// <summary>
        /// 日志文件存放文件夹
        /// </summary>
        private string m_logFolder = null;

        /// <summary>
        /// 日志文件路径
        /// </summary>
        private string m_fileName = null;

        /// <summary>
        /// 模块名称
        /// </summary>
        private string m_moduleName = null;

        /// <summary>
        /// 默认编码方式
        /// </summary>
        private Encoding m_encoding = Encoding.Default;

        /// <summary>
        /// 锁定模式下使用的文件流
        /// </summary>
        private FileAppender.LockingStream m_stream = null;

        /// <summary>
        /// 使用的锁定模式
        /// </summary>
        private FileAppender.LockingModelBase m_lockingModel = null;

        #endregion
    }

    /// <summary>
    /// 锁定文件模式
    /// </summary>
    public enum LockingType
    {
        /// <summary>
        /// 独占锁
        /// </summary>
        Exclusive,

        /// <summary>
        /// 最小时间锁定锁
        /// </summary>
        Minimal,

        /// <summary>
        /// 跨进程文件锁定
        /// </summary>
        InterProcess
    }
}
