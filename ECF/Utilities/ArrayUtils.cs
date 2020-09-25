using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ECF
{
    /// <summary>
    ///   <see cref="ECF.Utils"/>
    /// 数组处理单元
    /// Author:  XP
    /// Created: 2011/9/8
    /// </summary>
    public partial class Utils
    {
        #region Array 数组处理

        #region DistinctStringArray 清除字符串数组中的重复项

        /// <summary>
        /// 清除字符串数组中的重复项
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <param name="maxElementLength">字符串数组中单个元素的最大长度</param>
        /// <returns></returns>
        public static string[] DistinctStringArray(string[] strArray, int maxElementLength)
        {
            try
            {
                Hashtable h = new Hashtable();

                foreach (string s in strArray)
                {
                    string k = s;
                    if (maxElementLength > 0 && k.Length > maxElementLength)
                    {
                        k = k.Substring(0, maxElementLength);
                    }
                    h[k.Trim()] = s;
                }

                string[] result = new string[h.Count];

                h.Keys.CopyTo(result, 0);

                return result;
            }
            catch (System.Exception ex)
            {
                //new ECFException(ex);
                return new string[] { };
            }
        }

        /// <summary>
        /// 清除字符串数组中的重复项
        /// </summary>
        /// <param name="strArray">字符串数组</param>
        /// <returns></returns>
        public static string[] DistinctStringArray(string[] strArray)
        {
            return DistinctStringArray(strArray, 0);
        }


        #endregion

        #region GetInArrayID 判断指定字符串在指定字符串数组中的位置

        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>
        public static int GetInArrayID(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            try
            {
                for (int i = 0; i < stringArray.Length; i++)
                {
                    if (caseInsensetive)
                    {
                        if (strSearch.ToLower() == stringArray[i].ToLower())
                        {
                            return i;
                        }
                    }
                    else
                    {
                        if (strSearch == stringArray[i])
                        {
                            return i;
                        }
                    }

                }
            }
            catch (System.Exception ex)
            {
                //new ECFException(ex);
            }
            return -1;
        }


        /// <summary>
        /// 判断指定字符串在指定字符串数组中的位置
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <returns>字符串在指定字符串数组中的位置, 如不存在则返回-1</returns>		
        public static int GetInArrayID(string strSearch, string[] stringArray)
        {
            return GetInArrayID(strSearch, stringArray, true);
        }

        #endregion

        #region InArray 判断指定字符串是否属于指定字符串数组中的一个元素

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="strSearch">字符串</param>
        /// <param name="stringArray">字符串数组</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string strSearch, string[] stringArray, bool caseInsensetive)
        {
            return GetInArrayID(strSearch, stringArray, caseInsensetive) >= 0;
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">字符串数组</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string[] stringarray)
        {
            return InArray(str, stringarray, false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray)
        {
            return InArray(str, SplitString(stringarray, ","), false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit)
        {
            return InArray(str, SplitString(stringarray, strsplit), false);
        }

        /// <summary>
        /// 判断指定字符串是否属于指定字符串数组中的一个元素
        /// </summary>
        /// <param name="str">字符串</param>
        /// <param name="stringarray">内部以逗号分割单词的字符串</param>
        /// <param name="strsplit">分割字符串</param>
        /// <param name="caseInsensetive">是否不区分大小写, true为不区分, false为区分</param>
        /// <returns>判断结果</returns>
        public static bool InArray(string str, string stringarray, string strsplit, bool caseInsensetive)
        {
            return InArray(str, SplitString(stringarray, strsplit), caseInsensetive);
        }

        #endregion

        #region PadStringArray 过滤字符串数组中每个元素为合适的大小
        /// <summary>
        /// 过滤字符串数组中每个元素为合适的大小
        /// 当长度小于minLength时，忽略掉,-1为不限制最小长度
        /// 当长度大于maxLength时，取其前maxLength位
        /// 如果数组中有null元素，会被忽略掉
        /// </summary>
        /// <param name="strArray">The STR array.</param>
        /// <param name="minLength">单个元素最小长度</param>
        /// <param name="maxLength">单个元素最大长度</param>
        /// <returns>System.String[]</returns>
        public static string[] PadStringArray(string[] strArray, int minLength, int maxLength)
        {
            try
            {
                if (minLength > maxLength)
                {
                    int t = maxLength;
                    maxLength = minLength;
                    minLength = t;
                }

                int iMiniStringCount = 0;
                for (int i = 0; i < strArray.Length; i++)
                {
                    if (minLength > -1 && strArray[i].Length < minLength)
                    {
                        strArray[i] = null;
                        continue;
                    }
                    if (strArray[i].Length > maxLength)
                    {
                        strArray[i] = strArray[i].Substring(0, maxLength);
                    }
                    iMiniStringCount++;
                }

                string[] result = new string[iMiniStringCount];
                for (int i = 0, j = 0; i < strArray.Length && j < result.Length; i++)
                {
                    if (strArray[i] != null && strArray[i] != string.Empty)
                    {
                        result[j] = strArray[i];
                        j++;
                    }
                }


                return result;
            }
            catch (System.Exception ex)
            {
                //new ECFException(ex);
                return new string[] { };
            }
        }
        #endregion

        #region InIPArray　IP是否在指定的IP数组所限定的范围内


        /// <summary>
        /// 返回指定IP是否在指定的IP数组所限定的范围内, IP数组内的IP地址可以使用*表示该IP段任意, 例如192.168.1.*
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="iparray"></param>
        /// <returns></returns>
        public static bool InIPArray(string ip, string[] iparray)
        {

            try
            {
                string[] userip = Utils.SplitString(ip, @".");
                for (int ipIndex = 0; ipIndex < iparray.Length; ipIndex++)
                {
                    string[] tmpip = Utils.SplitString(iparray[ipIndex], @".");
                    int r = 0;
                    for (int i = 0; i < tmpip.Length; i++)
                    {
                        if (tmpip[i] == "*")
                        {
                            return true;
                        }

                        if (userip.Length > i)
                        {
                            if (tmpip[i] == userip[i])
                            {
                                r++;
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }

                    }
                    if (r == 4)
                    {
                        return true;
                    }


                }
            }
            catch (System.Exception ex)
            {
                //new ECFException(ex);
            }
            return false;

        }

        #endregion

        #region CompareString 字符数组比较

        /// <summary>
        /// 字符数组比较
        /// </summary>
        /// <param name="source">源字符数组.</param>
        /// <param name="target">目标字符数组.</param>
        /// <returns>
        /// System.String[]
        /// </returns>
        public static string[] CompareString(string[] source, string[] target)
        {
            if (target == null) return source;
            string[] s, o;
            return CompareString(source, target, out s, out o);
        }


        /// <summary>
        /// 字符数组比较
        /// </summary>
        /// <param name="source">源字符数组.</param>
        /// <param name="target">目标字符数组.</param>
        /// <param name="noSource">The no source.</param>
        /// <param name="noTarget">The no target.</param>
        /// <returns>
        /// 返回相同的数组
        /// </returns>
        public static string[] CompareString(string[] source, string[] target, out string[] noSource, out string[] noTarget)
        {
            noSource = null;
            noTarget = null;
            if (target == null)
            {
                return source;
            }

            List<string> same = new List<string>();
            List<string> slist = source.ToList<string>();
            List<string> tlist = target.ToList<string>();

            try
            {
                for (int i = slist.Count - 1; i > -1; i--)
                {
                    //如果相同
                    if (tlist.Contains(slist[i]))
                    {
                        same.Add(slist[i]);
                        tlist.Remove(slist[i]);
                        slist.Remove(slist[i]);
                    }
                }
                noSource = tlist.ToArray();
                noTarget = slist.ToArray();
                return same.ToArray();
            }
            catch (System.Exception ex)
            {
                //new ECFException(ex);
                return source;
            }
        }

        #endregion

        #endregion

    }
}
