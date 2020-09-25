using System;
using System.IO;
using ECF.Logs.Layout;
using ECF.Logs.Util;

namespace ECF.Logs.Appender
{
    /// <summary>
    ///   <see cref="ECF.Logs.Appender.TextWriterAppender"/>
    /// ʵ����־�ļ������ļ��Ļ���ʵ��
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
	public abstract class TextWriterAppender
    {
        #region ���캯��
        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public TextWriterAppender()
        {
        }

        /// <summary>
        /// ���Ĭ�ϵĹ��캯��������Writer����
        /// </summary>
        /// <param name="writer">�ײ��ı������</param>
        public TextWriterAppender(TextWriter writer)
        {
            Writer = writer;
        }
        #endregion

        #region ImmediateFlush �Ƿ�����ˢ���ļ�

        /// <summary>
        /// �Ƿ�����ˢ���ļ�
        /// </summary>
        private bool m_ImmediateFlush = true;

        /// <summary>
        /// �ھ�����Append�������Ƿ���������IO����, ʹ����������������ļ���
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

        #region QuietWriter д��־�ļ�����������

        /// <summary>
        /// д��־�ļ�����������
        /// </summary>
        protected CountingQuietTextWriter m_qtw;

        /// <summary>
        /// д��־�ļ�����������
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

        #region Layout ��ǰAppender�Ĳ���

        private ILayout m_layout = new PatternLayout();

        /// <summary>
        /// ��ǰAppender�Ĳ���
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
        /// ����Appender����д��־�ļ�
        /// </summary>
        abstract public void ActivateAppender();

        /// <summary>
        /// ʵ�ֹ���������־�ļ�, ͨ��������������
        /// </summary>
        abstract public void SetLogFilePath(bool IsRollingLogFile);


        /// <summary>
        /// �رյ�ǰAppderʵ��, �ײ��ı������Ҳ�����ر�, ��Appder�������ٱ��ָ�����
        /// </summary>
        abstract public void OnClose();

        #endregion

        #region publics

        /// <summary>
        /// �ײ������
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
        /// ���д��־�¼�
        /// </summary>
        /// <param name="loggingEvent">��־�¼�</param>
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
        /// ���д��־�¼�
        /// </summary>
        /// <param name="content">��־�¼�</param>
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
        /// ������Ҫ�ӳ�ʵ����writer�������ش˷���, ���������������л�writer������
        /// </summary>
        virtual protected void PrepareWriter()
        {
        }


        /// <summary>
        /// ���������ײ�������Ƿ�ʵ�����Ͳ��ֶ����Ƿ�ʵ����.
        /// </summary>
        /// <returns>ĳ��ǰ������δʵ��</returns>
        virtual protected bool PreAppendCheck()
        {
            if (m_layout == null)
                return false;

            if (m_qtw == null)
            {
                // ���������ӳ�ʵ����Writer����
                PrepareWriter();

                if (m_qtw == null)
                    return false;
            }

            if (m_qtw.Closed)
                return false;

            return true;
        }


        /// <summary>
        /// ����־��¼�������ָ����ʽ����
        /// </summary>
        /// <param name="writer">�ײ��ı������</param>
        /// <param name="loggingEvent">��־��¼�¼�</param>
        protected void RenderLoggingEvent(TextWriter writer, LoggingEvent loggingEvent)
        {
            Layout.Format(writer, loggingEvent);
        }

        /// <summary>
        /// ִ�д����LoggingEvent�¼�
        /// </summary>
        /// <param name="loggingEvent">��־��¼�¼�</param>
        virtual protected void Append(LoggingEvent loggingEvent)
        {
            RenderLoggingEvent(m_qtw, loggingEvent);

            // ˢ���ļ������·�������ļ����ļ���
            if (ImmediateFlush)
            {
                m_qtw.Flush();
            }
        }

        /// <summary>
        /// ����׷��
        /// </summary>
        /// <param name="content">����.</param>
        /// <remarks>
        ///   <list>
        ///    <item><description>˵��ԭ�� added by Shaipe 2018/11/6</description></item>
        ///   </list>
        /// </remarks>
        virtual protected void Append(string content)
        {
            m_qtw.Write(content);

            // ˢ���ļ������·�������ļ����ļ���
            if (ImmediateFlush)
            {
                m_qtw.Flush();
            }
        }

        /// <summary>
        /// �ر������
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
        /// ���õ�ǰAppder
        /// </summary>
        virtual protected void Reset()
        {
            CloseWriter();
            m_qtw = null;
        }
        #endregion

    }
}
