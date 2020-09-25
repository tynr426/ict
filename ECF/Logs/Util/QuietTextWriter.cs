using System;
using System.IO;

namespace ECF.Logs.Util
{
    /// <summary>
    ///  <see cref="ECF.Logs.Util.QuietTextWriter"/>
    /// ��־�ļ��ı������(�쳣���񲻻��׳�)
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
	public class QuietTextWriter : TextWriterAdapter
	{
        /// <summary>
        /// ����TextWriterʵ��
        /// </summary>
        /// <param name="writer">�ײ��ı������</param>
		public QuietTextWriter(TextWriter writer) : base(writer)
		{

		}

        private bool m_closed = false;

		/// <summary>
		/// ������Ƿ��Ѿ��ر�
		/// </summary>
		public bool Closed
		{
			get { return m_closed; }
		}

		#region ��װSystem.IO.TextWriter����ط���

		/// <summary>
		/// дһ���ַ����ײ������
		/// </summary>
		/// <param name="value">��Ҫ������ַ�</param>
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
		/// ��������д���ײ������
		/// </summary>
		/// <param name="buffer">�����������ַ�����</param>
		/// <param name="index">������ʼλ��</param>
		/// <param name="count">������ײ���������Ĵ�С</param>
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
		/// ���ַ���д���ײ������
		/// </summary>
		/// <param name="value">��Ҫ������ַ���</param>
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
		/// �رյײ�TextWriter
		/// </summary>
		override public void Close()
		{
			m_closed = true;
			base.Close();
        }

        #endregion
    }
}
