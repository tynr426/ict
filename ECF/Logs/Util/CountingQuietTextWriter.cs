using System;
using System.IO;

namespace ECF.Logs.Util
{
    /// <summary>
    ///  <see cref="ECF.Logs.Util.CountingQuietTextWriter"/>
    /// QuietTextWriter����, ��չ��ͳ����д����־�ļ����ֽ�����
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
	public class CountingQuietTextWriter : QuietTextWriter 
	{
		/// <summary>
		/// ʵ����CoutingQuietTextWriter����
		/// </summary>
        /// <param name="writer">�ײ��ı������</param>
		public CountingQuietTextWriter(TextWriter writer) : base(writer)
		{
			m_countBytes = 0;
		}

		/// <summary>
        /// дһ���ַ����ײ������
		/// </summary>
        /// <param name="value">��Ҫ������ַ�</param>
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
        /// ��������д���ײ������
        /// </summary>
        /// <param name="buffer">�����������ַ�����</param>
        /// <param name="index">������ʼλ��</param>
        /// <param name="count">������ײ���������Ĵ�С</param>
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
        /// ���ַ���д���ײ������
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
		/// ��ȡ��д����־�ļ����ֽ���
		/// </summary>
		public long Count 
		{
			get { return m_countBytes; }
			set { m_countBytes = value; }
		}
	}
}
