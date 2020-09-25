using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ECF.Data.Query
{
    /// <summary>
    /// FullName: <seealso cref="ECF.Data.Query.Orderby" />
    /// Summary : 排序类
    /// Author: hfshan
    /// DateTime: 2016-09-01
    /// </summary>
    [Serializable]
    public class Orderby:Entity
    {
        #region 属性
        /// <summary>
        /// 排序关键字
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 排序方式
        /// </summary>
        public SortBy Sort { get; set; }

        /// <summary>
        /// 实体的全名
        /// </summary>
        public override string EntityFullName => "ECF.DATA.Query.OrderBy,ECF";
        #endregion

        #region 构造函数 +Orderby(string name, SortBy sort)
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="name">字段名称.</param>
        /// <param name="sort">排序方式.</param>
        public Orderby(string name, SortBy sort)
        {
            this.Name = name;
            this.Sort = sort;
        }
        #endregion

        #region 收集排序集合 +static List<Orderby> Collection(XmlDocument xmlDoc)
        /// <summary>
        /// 收集排序集合
        /// </summary>
        /// <param name="xmlDoc"><![CDATA[格式：<Orderby><Name>0</Name><AddTime>1</AddTime></Orderby>]]></param>
        /// <returns></returns>
        public static List<Orderby> Collection(XmlDocument xmlDoc)
        {
            List<Orderby> result = new List<Orderby>();
            if (xmlDoc == null) return result;

            try
            {
                XmlNode cond = xmlDoc.SelectSingleNode("//Orderby");
                if (cond == null) return result;

                foreach (XmlNode xn in cond.ChildNodes)
                {
                    SortBy sort;
                    switch (xn.InnerText.ToLower())
                    {
                        case "asc":
                        case "0":
                            sort = SortBy.Asc;
                            break;
                        case "desc":
                        case "1":
                            sort = SortBy.Desc;
                            break;
                        default:
                            sort = SortBy.Asc;
                            break;
                    }
                    result.Add(new Orderby(xn.Name, sort));
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return result;
        }
        #endregion

        #region 转换为sql排序字符串 +string ToString(string key = null)
        /// <summary>
        /// 转换为sql条件字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string ToString(string key = null)
        {
            return ToString(key, this.Sort);
        }
        #endregion

        #region 转换为sql排序字符串 +string ToString(string key, SortBy sort)
        /// <summary>
        /// 转换为sql条件字符串
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        public string ToString(string key, SortBy sort)
        {
            string column = !string.IsNullOrEmpty(key) ? key : Name;
            return column + (sort == SortBy.Desc ? " DESC" : " ASC");
        }
        #endregion
    }
}
