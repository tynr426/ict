using System;
using System.Text;
using System.IO;
using System.Globalization;

namespace ECF.Logs.Util
{
    /// <summary>
    ///   <see cref="ECF.Logs.Util.TextWriterAdapter"/>
    /// 底层输出流封装抽象类
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
	public abstract class TextWriterAdapter : TextWriter
	{
		private TextWriter m_writer;

		#region 构造函数

		/// <summary>
		/// 构造函数用于实例化TextWriterAdapter对象
		/// </summary>
		/// <param name="writer"></param>
		protected TextWriterAdapter(TextWriter writer) :  base(CultureInfo.InvariantCulture)
		{
			m_writer = writer;
		}

		#endregion

		#region 重载TextWriter的相关属性

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
        /// <returns>用来写入输出的 Encoding。</returns>
		override public Encoding Encoding 
		{
			get { return m_writer.Encoding; }
		}

        /// <summary>
        /// format provider
        /// </summary>
        /// <returns>特定区域性的 <see cref="T:System.IFormatProvider"/> 对象，或者如果未指定任何其他区域性，则为当前区域性的格式设置。</returns>
		override public IFormatProvider FormatProvider 
		{
			get { return m_writer.FormatProvider; }
		}

        /// <summary>
        /// new line
        /// </summary>
        /// <returns>当前 TextWriter 的行结束符字符串。</returns>
		override public String NewLine 
		{
			get { return m_writer.NewLine; }
			set { m_writer.NewLine = value; }
		}

		#endregion

        #region 重载TextWriter的相关方法

        /// <summary>
        /// 关闭当前编写器并释放任何与该编写器关联的系统资源。
        /// </summary>
        override public void Close() 
		{
			m_writer.Close();
		}

        /// <summary>
        /// 释放由 <see cref="T:System.IO.TextWriter"/> 占用的非托管资源，还可以另外再释放托管资源。
        /// </summary>
        /// <param name="disposing">为 true 则释放托管资源和非托管资源；为 false 则仅释放非托管资源。</param>
		override protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				((IDisposable)m_writer).Dispose();
			}
		}

        /// <summary>
        /// 清理当前编写器的所有缓冲区，使所有缓冲数据写入基础设备。
        /// </summary>
		override public void Flush() 
		{
			m_writer.Flush();
		}


        /// <summary>
        /// 将字符写入文本流。
        /// </summary>
        /// <param name="value">要写入文本流中的字符。</param>
        /// <exception cref="T:System.ObjectDisposedException">
        ///   <see cref="T:System.IO.TextWriter"/> 是关闭的。</exception>
        ///   
        /// <exception cref="T:System.IO.IOException">发生 I/O 错误。</exception>
		override public void Write(char value) 
		{
			m_writer.Write(value);
		}


        /// <summary>
        /// 将字符的子数组写入文本流。
        /// </summary>
        /// <param name="buffer">要从中写出数据的字符数组。</param>
        /// <param name="index">在缓冲区中开始索引。</param>
        /// <param name="count">要写入的字符数。</param>
        /// <exception cref="T:System.ArgumentException">缓冲区长度减去 <paramref name="index"/> 小于 <paramref name="count"/>。</exception>
        ///   
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="buffer"/> parameter is null.</exception>
        ///   
        /// <exception cref="T:System.ArgumentOutOfRangeException">
        ///   <paramref name="index"/> 或 <paramref name="count"/> 为负。</exception>
        ///   
        /// <exception cref="T:System.ObjectDisposedException">
        ///   <see cref="T:System.IO.TextWriter"/> 是关闭的。</exception>
        ///   
        /// <exception cref="T:System.IO.IOException">发生 I/O 错误。</exception>
		override public void Write(char[] buffer, int index, int count) 
		{
			m_writer.Write(buffer, index, count);
		}


        /// <summary>
        /// 将字符串写入文本流。
        /// </summary>
        /// <param name="value">要写入的字符串。</param>
        /// <exception cref="T:System.ObjectDisposedException">The <see cref="T:System.IO.TextWriter"/> is closed.</exception>
        ///   
        /// <exception cref="T:System.IO.IOException">发生 I/O 错误。</exception>
		override public void Write(String value) 
		{
			m_writer.Write(value);
        }

        #endregion
    }
}
