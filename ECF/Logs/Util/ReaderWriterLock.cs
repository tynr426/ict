#if (!NETCF)
#define HAS_READERWRITERLOCK
#endif

using System;

namespace ECF.Logs.Util
{
    /// <summary>
    ///  <see cref="ECF.Logs.Util.ReaderWriterLock"/>
    /// 定义一种文件锁能支持单个写多个读的共享模式
    /// Author:  LY
    /// Created: 2011/11/18
    /// </summary>
	public sealed class ReaderWriterLock
	{
		/// <summary>
		/// 构造函数
		/// </summary>
		/// <remarks>
		/// <para>
		/// 实例化一个ReaderWriterLock对象
		/// </para>
		/// </remarks>
		public ReaderWriterLock()
		{
#if HAS_READERWRITERLOCK
			m_lock = new System.Threading.ReaderWriterLock();
#endif
		}

		/// <summary>
		/// 获取可读锁
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
		/// 释放可读锁
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
		/// 获取一个可写锁
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
		/// 思想一个可写锁
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
