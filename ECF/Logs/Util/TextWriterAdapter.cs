using System;
using System.Text;
using System.IO;
using System.Globalization;

namespace ECF.Logs.Util
{
    /// <summary>
    ///   <see cref="ECF.Logs.Util.TextWriterAdapter"/>
    /// �ײ��������װ������
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
	public abstract class TextWriterAdapter : TextWriter
	{
		private TextWriter m_writer;

		#region ���캯��

		/// <summary>
		/// ���캯������ʵ����TextWriterAdapter����
		/// </summary>
		/// <param name="writer"></param>
		protected TextWriterAdapter(TextWriter writer) :  base(CultureInfo.InvariantCulture)
		{
			m_writer = writer;
		}

		#endregion

		#region ����TextWriter���������

        /// <summary>
        /// writer
        /// </summary>
		protected TextWriter Writer 
		{
			get { return m_writer; }
			set { m_writer = value; }
		}

        /// <summary>
        /// encoding
        /// </summary>
        /// <returns>����д������� Encoding��</returns>
		override public Encoding Encoding 
		{
			get { return m_writer.Encoding; }
		}

        /// <summary>
        /// format provider
        /// </summary>
        /// <returns>�ض������Ե� <see cref="T:System.IFormatProvider"/> ���󣬻������δָ���κ����������ԣ���Ϊ��ǰ�����Եĸ�ʽ���á�</returns>
		override public IFormatProvider FormatProvider 
		{
			get { return m_writer.FormatProvider; }
		}

        /// <summary>
        /// new line
        /// </summary>
        /// <returns>��ǰ TextWriter ���н������ַ�����</returns>
		override public String NewLine 
		{
			get { return m_writer.NewLine; }
			set { m_writer.NewLine = value; }
		}

		#endregion

        #region ����TextWriter����ط���

        /// <summary>
        /// �رյ�ǰ��д�����ͷ��κ���ñ�д��������ϵͳ��Դ��
        /// </summary>
        override public void Close() 
		{
			m_writer.Close();
		}

        /// <summary>
        /// �ͷ��� <see cref="T:System.IO.TextWriter"/> ռ�õķ��й���Դ���������������ͷ��й���Դ��
        /// </summary>
        /// <param name="disposing">Ϊ true ���ͷ��й���Դ�ͷ��й���Դ��Ϊ false ����ͷŷ��й���Դ��</param>
		override protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				((IDisposable)m_writer).Dispose();
			}
		}

        /// <summary>
        /// ����ǰ��д�������л�������ʹ���л�������д������豸��
        /// </summary>
		override public void Flush() 
		{
			m_writer.Flush();
		}


        /// <summary>
        /// ���ַ�д���ı�����
        /// </summary>
        /// <param name="value">Ҫд���ı����е��ַ���</param>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   <see cref="T:System.IO.TextWriter"/> �ǹرյġ�</exception>
        ///   
        /// <exception cref="T:System.IO.IOException">���� I/O ����</exception>
		override public void Write(char value) 
		{
			m_writer.Write(value);
		}


        /// <summary>
        /// ���ַ���������д���ı�����
        /// </summary>
        /// <param name="buffer">Ҫ����д�����ݵ��ַ����顣</param>
        /// <param name="index">�ڻ������п�ʼ������</param>
        /// <param name="count">Ҫд����ַ�����</param>
        /// <exception cref="T:System.ArgumentException">���������ȼ�ȥ <paramref name="index"/> С�� <paramref name="count"/>��</exception>
        ///   
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer"/> parameter is null.</exception>
        ///   
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index"/> �� <paramref name="count"/> Ϊ����</exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        ///   <see cref="T:System.IO.TextWriter"/> �ǹرյġ�</exception>
        ///   
        /// <exception cref="T:System.IO.IOException">���� I/O ����</exception>
		override public void Write(char[] buffer, int index, int count) 
		{
			m_writer.Write(buffer, index, count);
		}


        /// <summary>
        /// ���ַ���д���ı�����
        /// </summary>
        /// <param name="value">Ҫд����ַ�����</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed.</exception>
        ///   
        /// <exception cref="T:System.IO.IOException">���� I/O ����</exception>
		override public void Write(String value) 
		{
			m_writer.Write(value);
        }

        #endregion
    }
}
