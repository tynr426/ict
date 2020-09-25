using System;

namespace ECF.Logs.Util
{
    /// <summary>
    /// 单例模式对象
    /// </summary>
    public class Singleton
    {
        private static volatile Singleton instance;
        private static object syncRoot = new Object();

        private Singleton() { }

        /// <summary>
        /// 系统单例模式唯一对象
        /// </summary>
        public static Singleton Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        instance = new Singleton();
                    }
                }

                return instance;
            }
        }
    }
}
