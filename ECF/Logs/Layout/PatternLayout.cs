using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace ECF.Logs.Layout
{
	/// <summary>
	///   <see cref="ECF.Logs.Layout.PatternLayout"/>
	/// 基于表达式的布局实现
	/// Author:  LY
	/// Created: 2011/11/18
	/// </summary>
	public class PatternLayout : ILayout
    {
        /// <summary>
        /// 实例化PatternLayout对象
        /// </summary>
        public PatternLayout()
        {

        }

        /// <summary>
        /// 实例化PatternLayout对象
        /// </summary>
        /// <param name="pattern">Layout表达式</param>
        public PatternLayout(string pattern)
        {
            this._FormatPattern = pattern;
        }

        private string _FormatPattern = String.Empty;

        /// <summary>
        /// 日志输出格式
        /// </summary>
        public string FormatPattern
        {
            get
            {
                if (String.IsNullOrEmpty(_FormatPattern))
                    _FormatPattern = LogConfig.Instance.DefaultLayoutPattern;

                return _FormatPattern;
            }
            set
            {
                _FormatPattern = value;
            }
        }

        /// <summary>
        /// 实现此方法建立你的布局格式
        /// </summary>
        /// <param name="writer">用于输出格式化日志的底层文本输出流</param>
        /// <param name="loggingEvent">需要格式化的日志事件</param>
        public void Format(TextWriter writer, LoggingEvent loggingEvent)
        {
            try
            {
                if (writer == null)
                {
                    throw new ArgumentNullException("writer");
                }

                if (loggingEvent == null)
                {
                    throw new ArgumentNullException("loggingEvent");
                }

                if (String.IsNullOrEmpty(FormatPattern))
                {
                    throw new NullReferenceException("日志输出格式不能为空");
                }

                string loggingEventPattern = FormatPattern.Clone().ToString();

                Regex rePattern = null;

                rePattern = new Regex("%Code%", RegexOptions.IgnoreCase);
                loggingEventPattern = rePattern.Replace(loggingEventPattern, loggingEvent.EventData.LogCode);

                rePattern = new Regex("%Module%", RegexOptions.IgnoreCase);
                loggingEventPattern = rePattern.Replace(loggingEventPattern, loggingEvent.EventData.Module);

                rePattern = new Regex("%TimeStamp%", RegexOptions.IgnoreCase);
                loggingEventPattern = rePattern.Replace(loggingEventPattern, loggingEvent.EventData.TimeStamp.ToString(LogConfig.Instance.DataPattern));

                rePattern = new Regex("%Message%", RegexOptions.IgnoreCase);
                loggingEventPattern = rePattern.Replace(loggingEventPattern, loggingEvent.EventData.Message + (loggingEvent.EventData.StackTrace != null ? loggingEvent.EventData.StackTrace : ""));

                rePattern = new Regex("%UserName%", RegexOptions.IgnoreCase);
                loggingEventPattern = rePattern.Replace(loggingEventPattern, loggingEvent.EventData.UserName);

                rePattern = new Regex("%Domain%", RegexOptions.IgnoreCase);
                loggingEventPattern = rePattern.Replace(loggingEventPattern, loggingEvent.EventData.Domain);

                rePattern = new Regex("%/n%", RegexOptions.IgnoreCase);
                loggingEventPattern = rePattern.Replace(loggingEventPattern, Environment.NewLine);

                if (loggingEvent.EventData.Properties != null)
                {
                    foreach (KeyValuePair<string, string> Property in loggingEvent.EventData.Properties)
                    {
                        rePattern = new Regex("%" + Property.Key + "%", RegexOptions.IgnoreCase);
                        loggingEventPattern = rePattern.Replace(loggingEventPattern, Property.Value);
                    }
                }
                //writer.Write(
                writer.Write(loggingEventPattern + Environment.NewLine);
            }
            catch (Exception ex)
            {
                EventLogs.Instance.Log(ex.Message + "\n\r" + ex.StackTrace, System.Diagnostics.EventLogEntryType.Error);
            }
        }
    }
}
