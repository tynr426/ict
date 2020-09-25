using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Controller
{
    public class Constant
    {
        public const string Keyword = "vast";
        public const string SyncKeyword = "sync";
        public const string LoginTicket = "LOGINTICKET{0}_{1}{2}_{3}";

        /// <summary>
        /// 各终端的appid和Secret定义.
        /// </summary>
        public static Dictionary<string, string> AppSecret
        {
            get
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase){
                    { "096d4009072c927c","3b56198f096d4009072c927c96fbc8b6"}, // Wap
                    { "c1091facfbd385d9","25dbfe5cc1091facfbd385d9c8ac1d88"}, // Android
                    { "c4af8fa9cd688a3c","67da1269c4af8fa9cd688a3c01062d9e"}, // IOS
                    { "4a364f2d1f1fb842","0a2b7ae94a364f2d1f1fb8423e9efd00"},  // Weapp
                    { "148fbe3a5d00218a","8daac14be0948fbb1a692a49e6bd1f0b"}  // Sync
                };
            }

        }
        public static string GetSecret(string appid)
        {
            if (!AppSecret.ContainsKey(appid))
                return "";
            return AppSecret[appid];
        }

    }
}
