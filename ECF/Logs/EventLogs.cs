using System;
using System.Diagnostics;

namespace ECF.Logs
{
	/// <summary>
	/// FullName： <see cref="ECF.Logs.EventLogs"/>
	/// Summary ： Windows事件处理 
	/// Version： 1.0.0.0 
	/// DateTime： 2012/4/24 23:07 
	/// CopyRight (c) by shaipe
	/// </summary>
	public class EventLogs
    {

        #region Instance

        static EventLogs _instance;

        /// <summary>
        /// 获取实例化
        /// </summary>
        public static EventLogs Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new EventLogs();
                }
                return EventLogs._instance;
            }
        } 

        #endregion

        #region EventLog 定义事件日志
        private EventLog _eventLog;

        /// <summary>
        /// 定义事件日志
        /// </summary>
        public EventLog EventLog
        {
            get
            {
                if (_eventLog == null)
                {
                    Initialize();
                    //_eventLog = new EventLog();
                }
                return _eventLog;
            }
            set { _eventLog = value; }
        }
        #endregion

        /// <summary>
        /// 构造函数.
        /// </summary>
        public EventLogs()
        {
            Initialize();
        }

        #region Log 记录事件日志

        /// <summary>
        /// 记录事件日志
        /// by XP-PC 2012/4/24
        /// </summary>
        /// <param name="messsage">日志消息.</param>
        /// <param name="entryType">Type of the entry.</param>
        public void Log(string messsage, EventLogEntryType entryType)
        {
            this.EventLog.WriteEntry(messsage, entryType);

        }

        /// <summary>
        /// 记录事件日志
        /// by XP-PC 2012/4/24
        /// </summary>
        /// <param name="message">日志消息.</param>
        public void Log(string message)
        {
            this.EventLog.WriteEntry(message);
        }

        /// <summary>
        /// Log
        /// by XP-PC 2012/4/24
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        /// <param name="eventID">The event ID.</param>
        /// <param name="category">The category.</param>
        public void Log(string message, EventLogEntryType type, int eventID, short category)
        {
            this.EventLog.WriteEntry(message, type, eventID, category);
        }

        /// <summary>
        /// Log
        /// by XP-PC 2012/4/24
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        /// <param name="eventID">The event ID.</param>
        public void Log(string message, EventLogEntryType type, int eventID)
        {
            this.EventLog.WriteEntry(message, type, eventID);
        }
        #endregion

        /// <summary>
        /// 初始化
        /// by XP-PC 2012/4/24
        /// </summary>
        void Initialize()
        {
            try
            {
                this._eventLog = new System.Diagnostics.EventLog();

                ((System.ComponentModel.ISupportInitialize)(this._eventLog)).BeginInit();

                ((System.ComponentModel.ISupportInitialize)(this._eventLog)).EndInit();

                // TODO: 在 InitComponent 调用后添加任何初始化
                if (!System.Diagnostics.EventLog.SourceExists("EC框架"))
                {
                    System.Diagnostics.EventLog.CreateEventSource("EC Framework Event Service", "EC框架");
                }
                _eventLog.Source = "EC Framework Event Service";
                _eventLog.Log = "EC框架";
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
