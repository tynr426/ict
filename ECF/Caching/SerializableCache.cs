using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace ECF.Caching
{

    /// <summary>
    /// FullName: <see cref="ECF.Caching.SerializableCache"/>
    /// Summary : 序列化缓存
    /// Version: 2.1
    /// DateTime: 2015/7/30 
    /// CopyRight (c) Shaipe
    /// </summary>
    public class SerializableCache
    {
        /// <summary>
        /// 用于缓存页面内容
        /// </summary>
        private volatile static ConcurrentDictionary<string, object> m_CacheQueue = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        /// <summary>
        /// Url的CDN缓存被访问的次数
        /// </summary>
        private volatile static ConcurrentDictionary<string, long> m_CacheVisitCount = new ConcurrentDictionary<string, long>();

        /// <summary>
        /// 用于存储已经加入执行队列任务的相关信息
        /// </summary>
        private volatile static ConcurrentDictionary<string, CacheTask> m_TaskRecordQueue = new ConcurrentDictionary<string, CacheTask>();

        /// <summary>
        /// 主任务线程
        /// </summary>
        private static System.Threading.Timer mainThreader = null;

        /// <summary>
        /// 已使用的内存大小
        /// </summary>
        private static int m_UsedMemorySize = 0;

        /// <summary>
        /// 任务是否在执行中
        /// </summary>
        private volatile static bool m_TaskExecuting = false;

        /// <summary>
        /// 最大缓存Url的个数
        /// </summary>
        public readonly static int m_MaxCacheUrlCount = 1000;

        /// <summary>
        /// 任务执行线程轮询时间
        /// </summary>
        private readonly static int m_TaskExecutePeriodTime = 5000;

        /// <summary>
        /// 最大内存使用情况
        /// </summary>
        private readonly static int m_MaxMemoryUsedSize = 1073741824;

        ///// <summary>
        ///// 自动更新内容缓存的间隔时间戳
        ///// </summary>
        //private readonly static int m_AutoUpdateCacheSleepTime = 1000;

        /// <summary>
        /// 启动IIS时开启自动自动维护对象的时间戳
        /// </summary>
        static SerializableCache()
        {
            ReStartMainThread();
        }

        /// <summary>
        /// 开启任务执行线程
        /// </summary>
        private static void ReStartMainThread()
        {
            if (mainThreader != null)
            {
                mainThreader.Change(Timeout.Infinite, Timeout.Infinite);
                mainThreader.Dispose();
                mainThreader = null;

                Thread.Sleep(1);
            }

            mainThreader = new System.Threading.Timer(new TimerCallback(RunTaskExecute), null, 1, m_TaskExecutePeriodTime);
        }

        /// <summary>
        /// 缓存分布系统灾难恢复函数
        /// </summary>
        internal static void DisasterRecoveryMechanism()
        {
            if (m_CacheQueue == null)
                m_CacheQueue = new ConcurrentDictionary<string, object>(StringComparer.OrdinalIgnoreCase);

            GC.SuppressFinalize(m_CacheQueue);
            GC.KeepAlive(m_CacheQueue);



            if (m_TaskRecordQueue == null)
                m_TaskRecordQueue = new ConcurrentDictionary<string, CacheTask>();

            GC.SuppressFinalize(m_TaskRecordQueue);
            GC.KeepAlive(m_TaskRecordQueue);

            GC.SuppressFinalize(m_UsedMemorySize);
            GC.KeepAlive(m_UsedMemorySize);

            GC.SuppressFinalize(m_TaskExecuting);
            GC.KeepAlive(m_TaskExecuting);

            if (mainThreader == null)
                ReStartMainThread();

            GC.SuppressFinalize(mainThreader);
            GC.KeepAlive(mainThreader);
        }

        /// <summary>
        /// 任务执行回调函数
        /// </summary>
        /// <param name="args">未使用才参数, 值为null</param>
        internal static void RunTaskExecute(object args)
        {
            if (m_TaskExecuting)
                return;

            m_TaskExecuting = true;

            // 如果任务队列本身为空, 则不用再继续执行函数
            if (m_TaskRecordQueue == null)
            {
                m_TaskRecordQueue = new ConcurrentDictionary<string, CacheTask>();

                GC.SuppressFinalize(m_TaskRecordQueue);
                GC.KeepAlive(m_TaskRecordQueue);

                m_TaskExecuting = false;
                return;
            }

            if (m_TaskRecordQueue.Count == 0)
            {
                m_TaskExecuting = false;
                return;
            }

            List<KeyValuePair<string, long>> lstVisitRecord = new List<KeyValuePair<string, long>>();

            foreach (KeyValuePair<string, CacheTask> taskRecord in m_TaskRecordQueue)
            {
                // 不能直接使用任务队列的Key, 任务队列的唯一标识是Key属性和TaskType的接口, 但添加到List<KeyValuePair<string, long>>中的必须是m_TaskRecordQueue的Key, 方便代码 if (m_TaskRecordQueue.TryGetValue(visitRecord.Key, out pTask))
                if (m_CacheVisitCount.ContainsKey(taskRecord.Value.Key))
                {
                    lstVisitRecord.Add(new KeyValuePair<string, long>(taskRecord.Key, m_CacheVisitCount[taskRecord.Value.Key]));
                }
                else
                {
                    lstVisitRecord.Add(new KeyValuePair<string, long>(taskRecord.Key, 0L));
                }
            }

            // 默认排序顺序是升序排序, 此处为降序, 返回次数多的页面的任务先执行
            lstVisitRecord.Sort(delegate (KeyValuePair<string, long> visitRecord1, KeyValuePair<string, long> visitRecord2)
            {
                return -1 * visitRecord1.Value.CompareTo(visitRecord2.Value);
            });

            CacheTask pTask = null;

            foreach (KeyValuePair<string, long> visitRecord in lstVisitRecord)
            {
                if (m_TaskRecordQueue.TryGetValue(visitRecord.Key, out pTask))
                {
                    if (pTask.ScheduledTime.CompareTo(DateTime.Now) <= 0 && visitRecord.Value > 1L)
                    {
                        RemoveToTaskQueue(pTask);

                        switch (pTask.TaskType)
                        {
                            case CacheTaskType.AutoUpdateCache:

                                //if (!UpdateCache(pTask.Key))
                                //{
                                //    pTask.ScheduledTime = DateTime.Now.AddSeconds(5.00);
                                //    AddToTaskQueue(pTask);
                                //}

                                //Thread.Sleep(m_AutoUpdateCacheSleepTime);

                                break;

                            case CacheTaskType.RemoveCache:

                                RemoveCacheCallback(pTask.Key);
                                break;

                            default:
                                break;
                        }
                    }
                }
            }

            lstVisitRecord.Clear();
            lstVisitRecord = null;

            m_TaskExecuting = false;
        }

        /// <summary>
        /// 获取缓存的页面内容
        /// </summary>
        /// <param name="key">需要获取分布缓存内容的Url</param>
        /// <returns></returns>
        internal static object GetCaches(string key)
        {
            if (String.IsNullOrWhiteSpace(key) || m_CacheQueue == null)
                return null;

            object cacheContent = null;

            if (!m_CacheQueue.TryGetValue(key, out cacheContent))
                return null;

            if (m_CacheVisitCount == null)
                m_CacheVisitCount = new ConcurrentDictionary<string, long>();

            GC.SuppressFinalize(m_CacheVisitCount);
            GC.KeepAlive(m_CacheVisitCount);

            m_CacheVisitCount.AddOrUpdate(key, 1, (updateUrl, visitCount) => { ++visitCount; return visitCount; });

            return cacheContent;
        }

        /// <summary>
        /// 移除分布缓存内容时通知移除相关信息的方法
        /// </summary>`
        /// <param name="key">需要移除分布缓存关键字</param>
        private static void RemoveCacheCallback(string key)
        {
            if (String.IsNullOrWhiteSpace(key) || m_CacheQueue == null)
                return;

            object cacheContent = null;

            if (m_CacheQueue.TryRemove(key, out cacheContent))
            {
                int cacheContentMemorySize = Marshal.SizeOf(cacheContent);
                Interlocked.Add(ref m_UsedMemorySize, -1 * cacheContentMemorySize);

                RemoveToTaskQueueByCacheUrl(key);
            }
        }

        /// <summary>
        /// 设置分布缓存内容
        /// </summary>
        /// <param name="key">需要设置分布缓存关键字</param>
        /// <param name="cacheContent">分布缓存的页面内容</param>
        /// <param name="cacheRelativeExpiration">分布缓存过期时间戳</param>
        internal static bool SetCaches(string key, object cacheContent, TimeSpan cacheRelativeExpiration)
        {
            if (String.IsNullOrWhiteSpace(key) || cacheContent == null)
                return false;


            if (m_CacheQueue == null)
            {
                m_CacheQueue = new ConcurrentDictionary<string, object>();

                GC.SuppressFinalize(m_CacheQueue);
                GC.KeepAlive(m_CacheQueue);
            }

            int cacheUrlCount = (m_CacheQueue == null ? 0 : m_CacheVisitCount.Count);

            if (cacheUrlCount >= m_MaxCacheUrlCount)
                ClearMemoryCaches(false);

            object oldCacheContent = null; int cacheContentMemorySize = 0;

            if (m_CacheVisitCount == null)
                m_CacheVisitCount = new ConcurrentDictionary<string, long>();

            GC.SuppressFinalize(m_CacheVisitCount);
            GC.KeepAlive(m_CacheVisitCount);

            m_CacheVisitCount.AddOrUpdate(key, 1, (updateUrl, visitCount) => { ++visitCount; return visitCount; });

            if (m_CacheQueue.TryGetValue(key, out oldCacheContent))
            {
                if (oldCacheContent == cacheContent)
                    return true;

                if (m_CacheQueue.TryUpdate(key, cacheContent, oldCacheContent))
                {
                    cacheContentMemorySize = Marshal.SizeOf(cacheContent);
                    Interlocked.Add(ref m_UsedMemorySize, cacheContentMemorySize);

                    cacheContentMemorySize = Marshal.SizeOf(oldCacheContent);
                    Interlocked.Add(ref m_UsedMemorySize, -1 * cacheContentMemorySize);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (m_CacheQueue.TryAdd(key, cacheContent))
                {
                    cacheContentMemorySize = Marshal.SizeOf(cacheContent);
                    Interlocked.Add(ref m_UsedMemorySize, cacheContentMemorySize);

                    if (cacheRelativeExpiration != TimeSpan.Zero)
                    {
                        CacheTask pTask = new CacheTask();

                        pTask.ScheduledTime = DateTime.Now.Add(cacheRelativeExpiration);
                        pTask.TaskType = CacheTaskType.RemoveCache;
                        pTask.Key = key;

                        AddToTaskQueue(pTask);
                    }
                }
                else
                {
                    return false;
                }
            }

            if (m_MaxMemoryUsedSize <= m_UsedMemorySize)
                ClearMemoryCaches(false);

            return true;
        }

        /// <summary>
        /// 清楚路由缓存
        /// </summary>
        /// <param name="clearAllCache">是否清除所有缓存</param>
        public static void ClearMemoryCaches(bool clearAllCache)
        {
            List<KeyValuePair<string, long>> lstVisitRecord = new List<KeyValuePair<string, long>>();

            foreach (KeyValuePair<string, long> visitRecord in m_CacheVisitCount)
            {
                lstVisitRecord.Add(new KeyValuePair<string, long>(visitRecord.Key, visitRecord.Value));
            }

            // 默认排序顺序是升序排序
            lstVisitRecord.Sort(delegate (KeyValuePair<string, long> visitRecord1, KeyValuePair<string, long> visitRecord2)
            {
                return visitRecord1.Value.CompareTo(visitRecord2.Value);
            });

            int cacheContentInSize = 0; object cacheContent = null;

            foreach (KeyValuePair<string, long> visitRecord in lstVisitRecord)
            {
                if (m_CacheQueue.TryRemove(visitRecord.Key, out cacheContent))
                {
                    cacheContentInSize = Marshal.SizeOf(cacheContent);
                    Interlocked.Add(ref m_UsedMemorySize, -1 * cacheContentInSize);

                    RemoveToTaskQueueByCacheUrl(visitRecord.Key);
                }

                if (!clearAllCache && m_UsedMemorySize < m_MaxMemoryUsedSize && m_CacheQueue.Count < m_MaxCacheUrlCount)
                    break;
            }

            try
            {
                if (DateTime.Now.Second % 2 == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.WaitForFullGCComplete();
                    GC.Collect();
                }
            }
            catch { }
            finally
            {
                if (lstVisitRecord != null)
                {
                    lstVisitRecord.Clear();
                    lstVisitRecord = null;
                }

                if (clearAllCache)
                    Interlocked.Add(ref m_UsedMemorySize, -1 * m_UsedMemorySize);
            }
        }

        /// <summary>
        /// 注册更新缓存的异步回调
        /// </summary>
        /// <param name="cacheUrl">分布缓存关键字</param>
        /// <param name="updateCacheRelativeTimeSpan">自动更新缓存的时间戳</param>
        internal static void RegisterUpdateCacheObverse(string cacheUrl, TimeSpan updateCacheRelativeTimeSpan)
        {
            if (String.IsNullOrWhiteSpace(cacheUrl))
                return;

            CacheTask pTask = new CacheTask();

            pTask.ScheduledTime = DateTime.Now.Add(updateCacheRelativeTimeSpan);
            pTask.TaskType = CacheTaskType.AutoUpdateCache;
            pTask.Key = cacheUrl;

            AddToTaskQueue(pTask);
        }

        /// <summary>
        /// 根据传入的Url从任务执行队列移除任务
        /// </summary>
        /// <param name="cacheUrl">分布缓存关键字</param>
        private static void RemoveToTaskQueueByCacheUrl(string cacheUrl)
        {
            CacheTask pTask = new CacheTask();

            pTask.TaskType = CacheTaskType.AutoUpdateCache;
            pTask.Key = cacheUrl;

            RemoveToTaskQueue(pTask);

            pTask.TaskType = CacheTaskType.RemoveCache;

            RemoveToTaskQueue(pTask);
        }

        /// <summary>
        /// 从任务执行队列移除任务
        /// </summary>
        /// <param name="pTask">执行任务</param>
        private static void RemoveToTaskQueue(CacheTask pTask)
        {
            if (String.IsNullOrWhiteSpace(pTask.Key))
                return;

            string TaskIdentity = pTask.Key + pTask.TaskType.ToString();

            if (m_TaskRecordQueue == null)
            {
                if (m_TaskRecordQueue == null)
                    m_TaskRecordQueue = new ConcurrentDictionary<string, CacheTask>();

                GC.SuppressFinalize(m_TaskRecordQueue);
                GC.KeepAlive(m_TaskRecordQueue);

                return;
            }

            // 及时键不存在也不会报错
            m_TaskRecordQueue.TryRemove(TaskIdentity, out pTask);
        }

        /// <summary>
        /// 添加到任务执行队列
        /// </summary>
        /// <param name="pTask">执行任务</param>
        private static void AddToTaskQueue(CacheTask pTask)
        {
            if (String.IsNullOrWhiteSpace(pTask.Key))
                return;

            string TaskIdentity = pTask.Key + pTask.TaskType.ToString();

            if (m_TaskRecordQueue == null)
            {
                m_TaskRecordQueue = new ConcurrentDictionary<string, CacheTask>();

                GC.SuppressFinalize(m_TaskRecordQueue);
                GC.KeepAlive(m_TaskRecordQueue);
            }

            m_TaskRecordQueue.TryAdd(TaskIdentity, pTask);
        }

    }

    /// <summary>
    /// CDN执行任务枚举
    /// </summary>
    public enum CacheTaskType
    {
        /// <summary>
        /// 自动循环更新CDN缓存的任务
        /// </summary>
        AutoUpdateCache = 1,

        /// <summary>
        /// 移除CDN缓存的操作
        /// </summary>
        RemoveCache = 2
    }

    /// <summary>
    /// CDN执行任务
    /// </summary>
    public class CacheTask
    {
        private CacheTaskType _TaskType;

        /// <summary>
        /// 任务类型
        /// </summary>
        public CacheTaskType TaskType
        {
            get
            {
                return _TaskType;
            }
            set
            {
                _TaskType = value;
            }
        }

        private DateTime _ScheduledTime;

        /// <summary>
        /// 预定执行时间
        /// </summary>
        public DateTime ScheduledTime
        {
            get
            {
                return _ScheduledTime;
            }
            set
            {
                _ScheduledTime = value;
            }
        }

        private string _Key;

        /// <summary>
        /// 需要执行任务的关键字
        /// </summary>
        public string Key
        {
            get
            {
                return _Key;
            }
            set
            {
                _Key = value;
            }
        }
    }
}
