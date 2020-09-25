#if (!NETCF)
#define HAS_READERWRITERLOCK
#endif

using System;

namespace ECF.Logs.Util
{
    /// <summary>
    ///  <see cref="ECF.Logs.Util.ReaderWriterLock"/>
    /// ����һ���ļ�����֧�ֵ���д������Ĺ���ģʽ
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
	public sealed class ReaderWriterLock
	{
		/// <summary>
		/// ���캯��
		/// </summary>
		/// <remarks>
		/// <para>
		/// ʵ����һ��ReaderWriterLock����
		/// </para>
		/// </remarks>
		public ReaderWriterLock()
		{
#if HAS_READERWRITERLOCK
			m_lock = new System.Threading.ReaderWriterLock();
#endif
		}

		/// <summary>
		/// ��ȡ�ɶ���
		/// </summary>
		public void AcquireReaderLock()
		{
#if HAS_READERWRITERLOCK
			m_lock.AcquireReaderLock(-1);
#else
			System.Threading.Monitor.Enter(this);
#endif
		}

		/// <summary>
		/// �ͷſɶ���
		/// </summary>
		public void ReleaseReaderLock()
		{
#if HAS_READERWRITERLOCK
			m_lock.ReleaseReaderLock();
#else
			System.Threading.Monitor.Exit(this);
#endif
		}

		/// <summary>
		/// ��ȡһ����д��
		/// </summary>
		public void AcquireWriterLock()
		{
#if HAS_READERWRITERLOCK
			m_lock.AcquireWriterLock(-1);
#else
			System.Threading.Monitor.Enter(this);
#endif
		}

		/// <summary>
		/// ˼��һ����д��
		/// </summary>
		public void ReleaseWriterLock()
		{
#if HAS_READERWRITERLOCK
			m_lock.ReleaseWriterLock();
#else
			System.Threading.Monitor.Exit(this);
#endif
		}

#if HAS_READERWRITERLOCK
		private System.Threading.ReaderWriterLock m_lock;
#endif
	}
}
