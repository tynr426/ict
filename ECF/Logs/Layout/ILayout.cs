using System.IO;

namespace ECF.Logs.Layout
{
	/// <summary>
	///   <see cref="ECF.Logs.Layout.ILayout"/>
	/// ʵ�ֲ��ֵĽӿ�
	/// Author:  LY
	/// Created: 2011/11/18
	/// </summary>
	public interface ILayout
	{
		/// <summary>
		/// ʵ�ִ˷���������Ĳ��ָ�ʽ
		/// </summary>
		/// <param name="writer">���������ʽ����־�ĵײ��ı������</param>
		/// <param name="loggingEvent">��Ҫ��ʽ������־�¼�</param>
		void Format(TextWriter writer, LoggingEvent loggingEvent);

		/// <summary>
		/// ��ʾ�����ʽ�ı��ʽ
		/// </summary>
        string FormatPattern { get; set; }
	}
}
