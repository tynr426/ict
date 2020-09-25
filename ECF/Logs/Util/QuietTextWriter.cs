using System;
using System.IO;

namespace ECF.Logs.Util
{
    /// <summary>
    ///  <see cref="ECF.Logs.Util.QuietTextWriter"/>
    /// 日志文件文本输出流(异常捕获不会抛出)
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
	public class QuietTextWriter : TextWriterAdapter
	{
        /// <summary>
        /// 构造TextWriter实例
        /// </summary>
        /// <param name="writer">底层文本输出流</param>
		public QuietTextWriter(TextWriter writer) : base(writer)
		{

		}

        private bool m_closed = false;

		/// <summary>
		/// 输出流是否已经关闭
		/// </summary>
		public bool Closed
		{
			get { return m_closed; }
		}

		#region 封装System.IO.TextWriter的相关方法

		/// <summary>
		/// 写一个字符到底层输出流
		/// </summary>
		/// <param name="value">需要输出的字符</param>
		public override void Write(char value) 
		{
			try 
			{
				base.Write(value);
			} 
			catch(Exception) 
			{
				
			}
		}
    
		/// <summary>
		/// 将缓冲区写到底层输出流
		/// </summary>
		/// <param name="buffer">缓冲区所有字符数组</param>
		/// <param name="index">数组起始位置</param>
		/// <param name="count">输出到底层你输出流的大小</param>
		public override void Write(char[] buffer, int index, int count) 
		{
			try 
			{
				base.Write(buffer, index, count);
			} 
			catch(Exception) 
			{
			
			}
		}
    
		/// <summary>
		/// 将字符串写到底层输出流
		/// </summary>
		/// <param name="value">需要输出的字符串</param>
		override public void Write(string value) 
		{
			try 
			{
				base.Write(value);
			} 
			catch(Exception) 
			{
				
			}
		}

		/// <summary>
		/// 关闭底层TextWriter
		/// </summary>
		override public void Close()
		{
			m_closed = true;
			base.Close();
        }

        #endregion
    }
}
