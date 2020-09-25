using System;
using System.Runtime.Serialization;
using System.Collections.Generic;

namespace ECF.Logs.Layout
{
	/// <summary>
	///   <see cref="ECF.Logs.Layout.LoggingData"/>
	/// ��¼��־�¼��Ķ���
	/// Author:  LY
	/// Created: 2011/11/18
	/// </summary>
	public class LoggingData
    {
        #region ��������

        /// <summary>
        /// ��־���
        /// </summary>
        public string LogCode { get; set; }

        /// <summary>
        /// ��¼��־��ʱ��
        /// </summary>
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// ��־��Ϣ
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// ģ������
        /// </summary>
        public string Module { get; set; }

        /// <summary>
        /// ��������
        /// </summary>
        public string Domain = System.Environment.UserDomainName;

        /// <summary>
        /// ��ǰ�ĵ�¼�û���
        /// </summary>
        public string UserName = System.Environment.UserName;

        /// <summary>
        /// ��ֵ������
        /// </summary>
        public Dictionary<string, string> Properties;

        /// <summary>
        /// ��־����
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// �쳣����
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        /// �쳣��Ϣ�ռ���
        /// </summary>
        public Exception Exception { get; set; }

        #endregion
    }

    /// <summary>
    ///   <see cref="ECF.Logs.Layout.LoggingEvent"/>
    /// ��¼��־�¼��Ķ���
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
    [Serializable]
    public class LoggingEvent : ISerializable
    {
        #region LoggingData ��־�������
        private LoggingData m_data;

        /// <summary>
        /// ��־�������
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

        #region ����
        /// <summary>
        /// ������־�¼�����
        /// </summary>
        /// <param name="Message">��־��Ϣ</param>
        /// <param name="ModuleName">ģ������</param>
        /// <param name="Properties">��������</param>
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
        /// ������־�¼�����
        /// </summary>
        /// <param name="LogCode">��־����</param>
        /// <param name="Message">��־��Ϣ</param>
        /// <param name="ModuleName">ģ������</param>
        /// <param name="Properties">��������</param>
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

        #region ʵ�����л�

        /// <summary>
        /// ʵ�����л�
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

        #region ���з��� ��������

        /// <summary>
        /// ��������
        /// </summary>
        /// <param name="key">���Թؼ���</param>
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
