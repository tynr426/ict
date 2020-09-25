using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace ECF
{
    /// <summary>
    /// Summary ： 对DataRow进行处理
    /// Version： 1.0
    /// DateTime： 2014/7/9
    /// CopyRight (c) shaipe
    /// </summary>
    public static class DataRowExtend
    {
        /// <summary>
        /// 将数据行转换为字典 by xp 20140709
        /// </summary>
        /// <param name="dataSource">数据表行.</param>
        /// <param name="isFilterNull">if set to <c>true</c> [is filter null].</param>
        /// <returns>
        /// Dictionary&lt;System.String, System.Object&gt;
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加是否过滤空值的处理 modify by Shaipe 2018/11/1</description></item>
        ///   </list>
        /// </remarks>
        public static Dictionary<string, object> ToDictionary(this DataRow dataSource, bool isFilterNull = true)
        {
            return ToDictionary(dataSource, null, isFilterNull);
        }

        /// <summary>
        /// 将数据行转换为字典 by xp 20140709
        /// </summary>
        /// <param name="dataSource">数据表行.</param>
        /// <param name="fields">需要转换字段数组.</param>
        /// <param name="isFilterNull">if set to <c>true</c> [is filter null].</param>
        /// <returns>
        /// Dictionary&lt;System.String, System.Object&gt;
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加参数判断是否过滤空值 modify by Shaipe 2018/11/1</description></item>
        ///   </list>
        /// </remarks>
        public static Dictionary<string, object> ToDictionary(this DataRow dataSource, string[] fields, bool isFilterNull = true)
        {
            if (dataSource == null) return null;
            Dictionary<string, object> dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            try
            {
                DataColumnCollection cols = dataSource.Table.Columns;
                for (int j = 0; j < cols.Count; j++)
                {

                    // 当数据库值为DbNull时跳过
                    if (dataSource[j] is DBNull && isFilterNull) continue;


                    bool fldNull = Utils.IsNullOrEmpty(fields);

                    //判断是否在只是要显示和转换的列中
                    if (!fldNull && !fields.Contains<string>(cols[j].ColumnName))
                        continue;

                    // 值不为空时才添加到字典中
                    if (!Utils.IsNullOrEmpty(dataSource[j]))
                    {
                        if (cols[j].DataType.ToString().IndexOf("DateTime", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            dic.Add(cols[j].ColumnName, Utils.ToDateTime(dataSource[j]));
                        }
                        else
                        {
                            dic.Add(cols[j].ColumnName, dataSource[j]);
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
            }
            return dic;
        }
    }
}
