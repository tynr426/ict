using System.IO;

namespace ECF.Logs.Layout
{
	/// <summary>
	///   <see cref="ECF.Logs.Layout.ILayout"/>
	/// 实现布局的接口
	/// Author:  LY
	/// Created: 2011/11/18
	/// </summary>
	public interface ILayout
	{
		/// <summary>
		/// 实现此方法建立你的布局格式
		/// </summary>
		/// <param name="writer">用于输出格式化日志的底层文本输出流</param>
		/// <param name="loggingEvent">需要格式化的日志事件</param>
		void Format(TextWriter writer, LoggingEvent loggingEvent);

		/// <summary>
		/// 表示输出格式的表达式
		/// </summary>
        string FormatPattern { get; set; }
	}
}
