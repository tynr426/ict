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
    /// ʵ����־�ļ����ڹ����ļ���ʵ��
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
    public class FileAppender : TextWriterAppender
    {
        #region �ļ���

        /// <summary>
        ///  ʹ�ô˶������Եײ��������Դ�ĵķ���
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
                    throw new ArgumentException("��������Ϊ��", "locking");
                }
                m_lockingModel = locking;
            }

            #region ͨ������ʵ�����Ļ�������������

            #region ���ط���

            public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
            {
                AssertLocked();
                IAsyncResult ret = m_realStream.BeginRead(buffer, offset, count, callback, state);
                m_readTotal = EndRead(ret);
                return ret;
            }

            /// <summary>
            /// ��֧���������첽д��, ִ��ǿ��ͬ��д��
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
                //�޲���, �Ѿ�����
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

            #region ����Stream����

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
            /// �ж����Ƿ�����
            /// </summary>
            private void AssertLocked()
            {
                if (m_realStream == null)
                {
                    throw new LockStateException("��ǰ�ļ����ܱ�����");
                }
            }

            /// <summary>
            /// ��ȡ��
            /// </summary>
            /// <returns>�Ƿ��ȡ�ɹ�</returns>
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
            /// �ͷ���
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
        /// ���������
        /// </summary>
        public abstract class LockingModelBase
        {
            private FileAppender m_appender = null;

            /// <summary>
            /// ��������־���ļ�
            /// </summary>
            /// <param name="filename">��Ҫ�򿪵��ļ���</param>
            /// <param name="append">����ӵ��ļ�β, ���Ǹ����ļ�</param>
            /// <param name="encoding">ʹ�õ��ļ�����</param>
            public abstract void OpenFile(string filename, bool append, Encoding encoding);

            /// <summary>
            /// �ر������־���ļ�
            /// </summary>
            public abstract void CloseFile();

            /// <summary>
            /// ��ȡ�����־�ļ�����
            /// </summary>
            /// <returns>���ؿ��Խ��ж�д��������</returns>
            public abstract Stream AcquireLock();

            /// <summary>
            /// �ͷ������־�ļ�����
            /// </summary>
            public abstract void ReleaseLock();

            /// <summary>
            /// ���û��ȡ��ǰ������������Appder
            /// </summary>
            public FileAppender CurrentAppender
            {
                get { return m_appender; }
                set { m_appender = value; }
            }

            /// <summary>
            /// ����һ���ļ�д�����
            /// </summary>
            /// <param name="filename">�ļ���</param>
            /// <param name="append">��ӻ��Ǹ���ԭ�ļ�</param>
            /// <param name="fileShare">�ļ������������</param>
            /// <returns></returns>
            protected Stream CreateStream(string filename, bool append, FileShare fileShare)
            {
                // ȷ���ļ��нṹ����
                string directoryFullName = Path.GetDirectoryName(filename);

                if (!Directory.Exists(directoryFullName))
                {
                    Directory.CreateDirectory(directoryFullName);
                }

                FileMode fileOpenMode = append ? FileMode.Append : FileMode.Create;
                return new FileStream(filename, fileOpenMode, FileAccess.Write, fileShare);

            }

            /// <summary>
            /// �ر��ļ������ 
            /// </summary>
            /// <param name="stream">�ļ���</param>
            protected void CloseStream(Stream stream)
            {
                stream.Close();
            }
        }

        /// <summary>
        /// ��������ļ��Ķ�ռ��
        /// </summary>
        /// <remarks>
        /// <para>
        /// ����־����ļ�����д����, ֱ����ʽ���ùر�Close����
        /// ά�����ڼ�������־�ļ��Ķ�ռ��
        /// </para>
        /// </remarks>
        public class ExclusiveLock : LockingModelBase
        {
            private Stream m_stream = null;

            /// <summary>
            /// ���ļ�Ϊд��־��׼��
            /// </summary>
            /// <param name="filename">��־�ļ���</param>
            /// <param name="append">�Ƿ������־��Ϣ��ԭ��־�ļ�β, �򸲸�ԭ��־�ļ�</param>
            /// <param name="encoding">��־�ļ�����</param>
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
            /// �ر��ļ�
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
            /// ��ȡ�ļ���
            /// </summary>
            /// <returns>����д�������ļ���</returns>
            /// <remarks>
            /// <para>
            /// ʲô��û��, OpenFile����ʱ�Ѿ������ļ�, ����ʽ����ReleaseLock֮ǰ��־�ļ������������̽�Ϊֻ��
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
            /// �ͷ��ļ���
            /// </summary>
            /// <remarks>
            /// <para>
            /// ʲôҲû��, �ļ������Ѿ��ڹر��ļ�CloseFileʱ�ͷ�
            /// </para>
            /// </remarks>
            public override void ReleaseLock()
            {

            }
        }

        /// <summary>
        /// ÿ��д��ʱ���ȡ�ļ���
        /// </summary>
        /// <remarks>
        /// <para>
        /// ÿ��д��־�ļ�����Ҫ��ȡ���ͷ���, ���ֻ��Ҫλ�����̵ܶ�ʱ��, �⽫ʹ��д��������
        /// ���������������ƶ���ɾ����־�ļ�, ����־�ļ������ؽ�
        /// </para>
        /// </remarks>
        public class MinimalLock : LockingModelBase
        {
            private string m_filename;
            private bool m_append;
            private Stream m_stream = null;

            /// <summary>
            /// ���ļ�Ϊд��־��׼��
            /// </summary>
            /// <param name="filename">��־�ļ���</param>
            /// <param name="append">�Ƿ������־��Ϣ��ԭ��־�ļ�β, �򸲸�ԭ��־�ļ�</param>
            /// <param name="encoding">��־�ļ�����</param>
            public override void OpenFile(string filename, bool append, Encoding encoding)
            {
                m_filename = filename;
                m_append = append;
            }

            /// <summary>
            /// �ر��ļ�
            /// </summary>
            public override void CloseFile()
            {

            }

            /// <summary>
            /// ��ȡ�ļ���
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
            /// �ͷ��ļ���
            /// </summary>
            public override void ReleaseLock()
            {
                CloseStream(m_stream);
                m_stream = null;
            }
        }

        /// <summary>
        /// �ṩ������ļ�����
        /// </summary>
        public class InterProcessLock : LockingModelBase
        {
            private Mutex m_mutex = null;
            private bool m_mutexClosed = false;
            private Stream m_stream = null;

            /// <summary>
            /// ���ļ�Ϊд��־��׼��
            /// </summary>
            /// <param name="filename">��־�ļ���</param>
            /// <param name="append">�Ƿ������־��Ϣ��ԭ��־�ļ�β, �򸲸�ԭ��־�ļ�</param>
            /// <param name="encoding">��־�ļ�����</param>
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
            /// �ر��ļ�
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
            /// ��ȡ�ļ���
            /// </summary>
            public override Stream AcquireLock()
            {
                if (m_mutex != null)
                {
                    // TODO: �Ƿ���ӳ�ʱ����?
                    m_mutex.WaitOne();

                    // ����FileStream��˵��ԶӦ������(����ø�����ٶ�)
                    if (m_stream.CanSeek)
                    {
                        m_stream.Seek(0, SeekOrigin.End);
                    }
                }

                return m_stream;
            }

            /// <summary>
            /// �ͷ��ļ���
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

        #region ���캯��

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public FileAppender()
        {

        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="LogFolder">��־�ļ���ŵ�Ŀ¼</param>
        /// <param name="AppendToFile">�ǽ���־��Ϣ��ӵ�ĩλ���Ǹ���ԭ����Ϣ</param>
        /// <param name="Module">ģ������</param>
        public FileAppender(string LogFolder, bool AppendToFile, string Module)
        {
            // ���������λ, LogFolder������Ҫģ������
            this.Module = Module;

            this.LogFolder = LogFolder;
            this.AppendToFile = AppendToFile;
            // д��־��ǰ��׼������
            ActivateAppender();
        }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="LogFolder">��־�ļ���ŵ�Ŀ¼</param>
        /// <param name="AppendToFile">�ǽ���־��Ϣ��ӵ�ĩλ���Ǹ���ԭ����Ϣ</param>
        /// <param name="Module">ģ������</param>
        /// <param name="layout">������Ϣ</param>
        public FileAppender(string LogFolder, bool AppendToFile, string Module, ILayout layout)
        {
            // ���������λ, LogFolder������Ҫģ������
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

        #region ��������

        /// <summary>
        /// ���㵱ǰ��Ҫ��������־�ļ�������
        /// </summary>
        /// <param name="IsRollingLogFile">�Ƿ���Ҫ����������־�ļ�</param>
        /// <param name="logFile">The log file.</param>
        /// <remarks>
        ///   <list>
        ///    <item><description>˵��ԭ�� added by Shaipe 2018/11/7</description></item>
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

                // ����Ѿ�������־�ļ���, ������������������
                while (File.Exists(m_fileName))
                {
                    ++IncreaseRollingIndex;
                    m_fileName = Path.Combine(m_logFolder, String.Format(logFile, IncreaseRollingIndex));
                }

                return;
            }

            m_fileName = Path.Combine(m_logFolder, String.Format(logFile, Directory.GetFiles(m_logFolder).Length));

            FileInfo fileInfo = new FileInfo(m_fileName);

            // ���ָ��������־�ļ������ڻ���ڵ������ļ��Ѿ���������������־�ļ��ļ���С����
            if (!fileInfo.Exists || Convert.ToInt32(fileInfo.Length) >= LogConfig.Instance.RollingMaxSize)
            {
                SetLogFilePath(true);
            }
        }

        /// <summary>
        /// ��־�ļ����Ŀ¼
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
        /// ģ������
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
        /// ָ����־�ļ��Ǹ���ԭ�ļ�, �����������־��Ϣ��Դ�ļ�β��
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
        /// ���뷽ʽ
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
        /// ��־�ļ�����ģʽ
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
        /// д��־��ǰ��׼������
        /// </summary>
        override public void ActivateAppender()
        {
            if (m_lockingModel == null)
            {
                switch (LogConfig.Instance.LockType)
                {
                    //��ռ��
                    case LockingType.Exclusive:
                        m_lockingModel = new ExclusiveLock();
                        break;
                    //������ļ�����
                    case LockingType.InterProcess:
                        m_lockingModel = new InterProcessLock();
                        break;
                    //��Сʱ����
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
        /// ���ø���TextWriterAppender��Reset�����ر���ǰ�򿪵���־�ļ�
        /// </summary>
        override protected void Reset()
        {
            base.Reset();
            m_fileName = null;
        }

        /// <summary>
        /// ��ʼ���ļ������
        /// </summary>
        override protected void PrepareWriter()
        {
            SafeOpenFile(m_fileName, m_appendToFile);
        }

        /// <summary>
        /// ���عرյ�ǰ��������豸
        /// </summary>
        public override void OnClose()
        {
            lock (this)
            {
                // ��Ҫ���õ�ǰ
                if (LogConfig.Instance.LockType != LockingType.Minimal)
                    this.LockingModel.CloseFile();
                //�ر��ȴ򿪵���־
                Reset();
            }
        }

        /// <summary>
        /// ���д��־�¼�
        /// </summary>
        /// <param name="loggingEvent">��־�¼�</param>
        override protected void Append(LoggingEvent loggingEvent)
        {
            // �����ǰ�ļ���С�Ѿ������޶�������־�ļ���С, ���ؽ�֮
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
        ///     <description>˵��ԭ�� added by Shaipe 2018/11/6</description>
        ///   </item>
        /// </list>
        /// </remarks>
        override protected void Append(string content)
        {
            // �����ǰ�ļ���С�Ѿ������޶�������־�ļ���С, ���ؽ�֮
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
        /// �رյײ������
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
        /// ���ļ�Ϊд��־������׼��(�Ѳ����쳣)
        /// </summary>
        /// <param name="fileName">�ļ���</param>
        /// <param name="append">�Ƿ������־��Ϣ���ļ�β</param>
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
        /// ���ļ�Ϊд��־������׼��
        /// </summary>
        /// <param name="fileName">�ļ���</param>
        /// <param name="append">�Ƿ������־��Ϣ���ļ�β</param>
        virtual protected void OpenFile(string fileName, bool append)
        {
            lock (this)
            {
                //�Ƚ��йر�֮ǰ�򿪵��ļ�
                Reset();

                // ������Щ��, ��������������ļ�ʧ��
                m_fileName = fileName;
                m_appendToFile = append;

                switch (LogConfig.Instance.LockType)
                {
                    //��ռ
                    case LockingType.Exclusive:
                        m_lockingModel = new ExclusiveLock();
                        break;
                    //������ļ���
                    case LockingType.InterProcess:
                        m_lockingModel = new InterProcessLock();
                        break;
                    //��Сʱ��
                    case LockingType.Minimal:
                        m_lockingModel = new MinimalLock();
                        break;
                }
                //
                LockingModel.CurrentAppender = this;
                //���ļ�
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
        /// ��ʼ��QuietWriter�ı������
        /// </summary>
        virtual protected void SetQWForFiles(Stream fileStream)
        {
            SetQWForFiles(new StreamWriter(fileStream, m_encoding));
        }

        /// <summary>
        /// ��ʼ��QuietWriter�ı������
        /// </summary>
        virtual protected void SetQWForFiles(TextWriter writer)
        {
            QuietWriter = new CountingQuietTextWriter(writer);
        }

        #region ˽������

        /// <summary>
        /// ���������־��Ϣ���ļ�ĩλ, ���Ǹ���ԭ��־�ļ�
        /// </summary>
        private bool m_appendToFile = true;

        /// <summary>
        /// ��־�ļ�����ļ���
        /// </summary>
        private string m_logFolder = null;

        /// <summary>
        /// ��־�ļ�·��
        /// </summary>
        private string m_fileName = null;

        /// <summary>
        /// ģ������
        /// </summary>
        private string m_moduleName = null;

        /// <summary>
        /// Ĭ�ϱ��뷽ʽ
        /// </summary>
        private Encoding m_encoding = Encoding.Default;

        /// <summary>
        /// ����ģʽ��ʹ�õ��ļ���
        /// </summary>
        private FileAppender.LockingStream m_stream = null;

        /// <summary>
        /// ʹ�õ�����ģʽ
        /// </summary>
        private FileAppender.LockingModelBase m_lockingModel = null;

        #endregion
    }

    /// <summary>
    /// �����ļ�ģʽ
    /// </summary>
    public enum LockingType
    {
        /// <summary>
        /// ��ռ��
        /// </summary>
        Exclusive,

        /// <summary>
        /// ��Сʱ��������
        /// </summary>
        Minimal,

        /// <summary>
        /// ������ļ�����
        /// </summary>
        InterProcess
    }
}
