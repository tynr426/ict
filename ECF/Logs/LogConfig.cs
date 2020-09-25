using System.IO;
using ECF.Logs.Appender;

namespace ECF.Logs
{
	/// <summary>
	/// FullName： <see cref="ECF.Logs.LogConfig"/>
	/// Summary ： 日志配置信息
	/// Version： 1.0.0.0 
	/// DateTime： 2012/4/14 12:48 
	/// CopyRight (c) by shaipe
	/// </summary>
	public class LogConfig
    {
        #region 构造化

        /// <summary>
        /// 单例模式获取配置对象的唯一实例
        /// </summary>
        public static LogConfig Instance
        {
            get
            {
                return GetConfig();
            }
        }

        /// <summary>
        /// 获取唯一的模板引擎
        /// </summary>
        internal static LogConfig GetConfig()
        {
            return Singleton.config;
        }

        /// <summary>
        /// 单例模式获取配置对象的唯一实例
        /// </summary>
        static class Singleton
        {
            public static LogConfig config = new LogConfig();
        }

        #endregion

        #region Propertys

        #region DataPattern 输出日志时间格式表达式

        private string _DataPattern = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// 输出日志时间格式表达式
        /// </summary>
        public string DataPattern
        {
            get
            {
                return _DataPattern;
            }
            set
            {
                _DataPattern = value;
            }
        }
        #endregion

        #region DefaultLogFile 默认日志文件名格式

        private string _DefaultLogFile = "Log{0}.log";

        /// <summary>
        /// 默认日志文件名格式
        /// </summary>
        public string DefaultLogFile
        {
            get
            {
                return _DefaultLogFile;
            }
            set
            {
                _DefaultLogFile = value;
            }
        }

        #endregion

        #region LockType 锁定文件模式

        private LockingType _LockType = LockingType.Exclusive;

        /// <summary>
        /// 锁定文件模式, 默认为独占模式锁
        /// </summary>
        public LockingType LockType
        {
            get
            {
                return _LockType;
            }
            set
            {
                _LockType = value;
            }
        }
        #endregion

        #region RollingMaxSize 日志文件最大容量

        private int _RollingMaxSize = 1024 * 1024 * 2;

        /// <summary>
        /// 日志文件最大容量, 超过此容量则重新生成, 默认为10MB
        /// </summary>
        public int RollingMaxSize
        {
            get
            {
                return _RollingMaxSize;
            }
            set
            {
                _RollingMaxSize = value;
            }
        }
        #endregion

        #region LogFolder 存放日志的文件夹

        private string _LogFolder = Path.Combine(Path.GetPathRoot(System.AppDomain.CurrentDomain.BaseDirectory), "ECFLog");

        /// <summary>
        /// 存放日志的文件夹, 默认当前项目所在驱动器根目录的ECFLog文件夹
        /// </summary>
        public string LogFolder
        {
            get
            {
                return _LogFolder;
            }
            set
            {
                _LogFolder = value;
            }
        }

        #endregion

        #region DefaultLayoutPattern 默认布局表达式

        private string _DefaultLayoutPattern = "错误代码:%Code%%/n%模块:%Module%%/n%时间:%TimeStamp%%/n%内容:%Message%%/n%";

        /// <summary>
        /// 默认布局表达式
        /// </summary>
        public string DefaultLayoutPattern
        {
            get
            {
                return _DefaultLayoutPattern;
            }
            set
            {
                _DefaultLayoutPattern = value;
            }
        }

        #endregion 

        #endregion
    }
}
