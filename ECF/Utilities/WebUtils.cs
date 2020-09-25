using System;
using System.Collections.Generic;
using System.Text;

namespace ECF
{
    public partial class Utils
    {
        public static String RootPath
        {
            get
            {
                string rootPath = rootPath = System.AppDomain.CurrentDomain.BaseDirectory;
                // 判断获取到的路径为目录并带\\
                return (rootPath.EndsWith("\\") ? rootPath : rootPath + "\\");
            }
        }
        static string unreservedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";
        public static string UrlEncode(string text)
        {
            StringBuilder result = new StringBuilder();
            foreach (char symbol in text)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }
            return result.ToString();
        }
    }
}
