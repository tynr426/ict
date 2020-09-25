using System;
using System.IO;

namespace ECF.Logs.Util
{
    /// <summary>
    ///  <see cref="ECF.Logs.Util.CountingQuietTextWriter"/>
    /// QuietTextWriter子类, 扩展可统计已写入日志文件的字节总数
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
	public class CountingQuietTextWriter : QuietTextWriter 
	{
		/// <summary>
		/// 实例化CoutingQuietTextWriter对象
		/// </summary>
        /// <param name="writer">底层文本输出流</param>
		public CountingQuietTextWriter(TextWriter writer) : base(writer)
		{
			m_countBytes = 0;
		}

		/// <summary>
        /// 写一个字符到底层输出流
		/// </summary>
        /// <param name="value">需要输出的字符</param>
		public override void Write(char value) 
		{
			try 
			{
				base.Write(value);

				m_countBytes += this.Encoding.GetByteCount(new char[] { value });
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
			if (count > 0)
			{
				try 
				{
					base.Write(buffer, index, count);

					m_countBytes += this.Encoding.GetByteCount(buffer, index, count);
				} 
				catch(Exception) 
				{
					
				}
			}
		}

        /// <summary>
        /// 将字符串写到底层输出流
        /// </summary>
        /// <param name="str">The STR.</param>
		override public void Write(string str) 
		{
			if (str != null && str.Length > 0)
			{
				try 
				{
					base.Write(str);

					m_countBytes += this.Encoding.GetByteCount(str);
				}
				catch(Exception) 
				{
	
				}
			}
		}

        private long m_countBytes;

		/// <summary>
		/// 获取已写入日志文件的字节数
		/// </summary>
		public long Count 
		{
			get { return m_countBytes; }
			set { m_countBytes = value; }
		}
	}
}
