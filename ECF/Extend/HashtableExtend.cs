using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF
{
    /// <summary>
    ///  对哈希表（散序列表）进行扩展
    /// </summary>
    public static class HashtableExtend
    {
        /// <summary>
        ///  转换为不区分大小写的Hashtable
        ///  Author :   XP-PC/Shaipe
        ///  Created:  03-20-2016
        /// </summary>
        /// <param name="table">源数据.</param>
        /// <returns>Hashtable.</returns>
        public static Hashtable ToIgnoreCase(this Hashtable table)
        {
            if (table == null) return table;

            return new Hashtable(table, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        ///  忽略大小写的方式判断key是否存在.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  03-20-2016
        /// </summary>
        /// <param name="table">The table.</param>
        /// <param name="key">The key.</param>
        /// <returns><c>true</c> if [contains ignore case] [the specified key]; otherwise, <c>false</c>.</returns>
        public static bool ContainsIgnoreCase(this Hashtable table,string key)
        {
            if (table != null)
            {
                table = table.ToIgnoreCase();
                return table.Contains(key);
            }

            return false;
        }

        /// <summary>
        ///  根据key获取指定类型的值
        ///  Author :   XP-PC/Shaipe
        ///  Created:  03-20-2016
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table">The table.</param>
        /// <param name="key">The key.</param>
        /// <returns>T.</returns>
        public static T ToValue<T>(this Hashtable table, string key)
        {
            T result = default(T);

            try
            {
                if (table != null)
                {
                    // 转换为不区分大小写
                    table = table.ToIgnoreCase();

                    //判断配置项是否存在
                    if (table.Contains(key) && table[key] != null)
                    {
                        result = (T)table[key];
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return result;
        }

        /// <summary>
        /// 将字典数据转换为Json数据.
        /// </summary>
        /// <param name="table">待转的数据字典.</param>
        /// <returns></returns>
        public static string ToJson(this Hashtable table)
        {
            return table.ToJson(false);
        }

        /// <summary>
        /// 将字典数据转换为Json数据.
        /// </summary>
        /// <param name="table">待转的数据字典.</param>
        /// <param name="unicode">是否进行Unicode编码.</param>
        /// <returns></returns>
        public static string ToJson(this Hashtable table, bool unicode )
        {
            List<string> list = new List<string>();
            try
            {
                foreach (KeyValuePair<string, object> temp in table)
                {
                    if (temp.Key != null && temp.Value != null)
                    {
                        list.Add("\"" + temp.Key + "\":" + (unicode ? Utils.ChineseToUnicode(temp.Value.ToString(), false).Replace("\\\"", "\"") : temp.Value.ToString()));
                    }

                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return "{" + string.Join(",", list) + "}";
        }

        /// <summary>
        /// 鼗字典数据转换为Xml格式数据.
        /// </summary>
        /// <typeparam name="T">指字的数据类型</typeparam>
        /// <param name="table">待转的数据字典.</param>
        /// <returns></returns>
        public static string ToXml<T>(this Hashtable table)
        {
            return table.ToXml<T>("node");
        }

        /// <summary>
        /// 鼗字典数据转换为Xml格式数据.
        /// </summary>
        /// <typeparam name="T">指字的数据类型</typeparam>
        /// <param name="table">待转的数据字典.</param>
        /// <param name="nodeName">输出的主节点名称.</param>
        /// <returns></returns>
        public static string ToXml<T>(this Hashtable table, string nodeName )
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("<" + nodeName + ">");
                foreach (KeyValuePair<string, T> temp in table)
                {
                    if (temp.Key != null && temp.Value != null)
                    {
                        sb.Append(Utils.ConvertXmlData(temp.Key, temp.Value.ToString()));
                    }
                }
                sb.Append("</" + nodeName + ">");
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return sb.ToString();
        }
    }
}
