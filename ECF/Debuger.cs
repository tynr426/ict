
using System;
using System.Reflection;
using ECF.Data;
using System.Configuration;

namespace ECF
{

    

    /// <summary>
    /// 应用程序类型
    /// 2017-05-30 by shaipe
    /// </summary>
    [Serializable]
    public enum ApplicationType
    {
        /// <summary>
        /// 控制台程序
        /// </summary>
        Console,

        /// <summary>
        /// Web应用
        /// </summary>
        WebApp,

        /// <summary>
        /// 窗体应用
        /// </summary>
        Winform,

        /// <summary>
        /// windows服务
        /// </summary>
        Service
    }

    /// <summary>
    ///   <see cref="ECF.Debuger"/>
    /// 调试信息类
    /// Author:  XP
    /// Created: 2011/9/15
    /// </summary>
    public class Debuger
    {

        static bool _IsDebug = Utils.ToBool(ConfigurationManager.AppSettings["IsDebug"]);

        /// <summary>
        /// 是否启用Debug
        /// </summary>
        public static bool IsDebug
        {
            get { return _IsDebug; }
            set { _IsDebug = value; }
        }

        static ApplicationType _AppType = ApplicationType.WebApp;
        /// <summary>
        /// 应用程序类型
        /// </summary>
        public static ApplicationType AppType
        {
            get { return _AppType; }
            set { _AppType = value; }
        }

//#if DEBUG

//        /// <summary>
//        /// 获取数据库查询时的详细调试信息.
//        /// </summary>
//        public static string DbQueryDetail(DatabaseType dbType)
//        {
//            try
//            {
//                Assembly assembly = Assembly.Load("ECF.Data." + dbType);
//                Type type = assembly.GetType("ECF.Data." + dbType + ".DBService");
//                return type.GetProperty("QueryDetail").GetValue(null, null).ToString();
//            }
//            catch (Exception ex)
//            {
//                return ex.Message;
//            }

//        }

//#endif


    }
}
