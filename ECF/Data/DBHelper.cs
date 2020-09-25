using ECF.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using ECF.Xml;

namespace ECF.Data
{
    /// <summary>
    ///   <see cref="ECF.Data.DBHelper"/>
    /// 数据库操作处理公用方法处理
    /// Author:  XP
    /// Created: 2011/11/21
    /// </summary>
    [Serializable]
    public class DBHelper
    {
        #region PreNextCondition

        /// <summary>
        /// 获取上下篇中当前id的上一个id或下一个id
        /// </summary>
        /// <param name="id">当前id.</param>
        /// <param name="pre">是否获取前一篇的id</param>
        /// <param name="condition">条件.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string PreNextCondition(int id, bool pre, string condition)
        {
            string cond = "";
            if (pre)
            {
                cond += " Id<" + id;
            }
            else
            {
                cond += " Id >" + id;
            }
            if (!String.IsNullOrEmpty(condition))
            {
                cond += " AND " + condition;
            }
            if (pre)
            {
                cond += " ORDER BY Id DESC";
            }
            else
            {
                cond += " ORDER BY Id ASC";
            }
            return cond;
        }

        #endregion

        #region 获取分页后的DataTable GetPagedTable


        /// <summary>
        /// 根据页面和当前页索引获取当前页的数据.
        /// </summary>
        /// <param name="dt">表数据源.</param>
        /// <param name="PageIndex">当前页索引.</param>
        /// <param name="PageSize">每页显示大小.</param>
        /// <returns></returns>
        public static DataTable GetPagedTable(DataTable dt, int PageIndex, int PageSize)
        {
            if (PageIndex == 0)
                return dt;
            DataTable newdt = dt.Copy();
            newdt.Clear();

            int rowbegin = (PageIndex - 1) * PageSize;
            int rowend = PageIndex * PageSize;

            if (rowbegin >= dt.Rows.Count)
                return newdt;

            if (rowend > dt.Rows.Count)
                rowend = dt.Rows.Count;
            for (int i = rowbegin; i <= rowend - 1; i++)
            {
                DataRow newdr = newdt.NewRow();
                DataRow dr = dt.Rows[i];
                foreach (DataColumn column in dt.Columns)
                {
                    newdr[column.ColumnName] = dr[column.ColumnName];
                }
                newdt.Rows.Add(newdr);
            }

            return newdt;
        }

        #endregion

        #region 过滤相同列的相同记录

        /// <summary>
        /// 过滤相同列的相同记录
        /// </summary>
        /// <param name="pColumnNames">需要过滤的列名数组.</param>
        /// <param name="pOriginalTable">数据源.</param>
        /// <returns>System.Data.DataTable</returns>
        public static DataTable SelectDistinct(string[] pColumnNames, DataTable pOriginalTable)
        {

            DataTable distinctTable = new DataTable();

            int numColumns = pColumnNames.Length;

            for (int i = 0; i < numColumns; i++)
            {

                distinctTable.Columns.Add(pColumnNames[i], pOriginalTable.Columns[pColumnNames[i]].DataType);

            }

            Hashtable trackData = new Hashtable();

            foreach (DataRow currentOriginalRow in pOriginalTable.Rows)
            {

                StringBuilder hashData = new StringBuilder();

                DataRow newRow = distinctTable.NewRow();

                for (int i = 0; i < numColumns; i++)
                {

                    hashData.Append(currentOriginalRow[pColumnNames[i]].ToString());

                    newRow[pColumnNames[i]] = currentOriginalRow[pColumnNames[i]];

                }

                if (!trackData.ContainsKey(hashData.ToString()))
                {

                    trackData.Add(hashData.ToString(), null);

                    distinctTable.Rows.Add(newRow);

                }

            }

            return distinctTable;

        }

        #endregion

        #region 将数据表转换成Json类型串
        /// <summary>
        /// Table to json
        /// </summary>
        /// <param name="dt">The dt.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string TableToJson(DataTable dt)
        {
            return TableToJson(dt, true, null, false);
        }

        /// <summary>
        /// 将Table转为Json格式数据.
        /// </summary>
        /// <param name="dt">DataTable数据表.</param>
        /// <param name="unicode">if set to <c>true</c> [unicode].</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string TableToJson(DataTable dt, bool unicode)
        {
            return TableToJson(dt, true, null, unicode);
        }

        /// <summary>
        /// 将数据表转换成JSON类型字符串.
        /// </summary>
        /// <param name="dt">DataTable数据表.</param>
        /// <param name="filter">是否过滤空数据.</param>
        /// <param name="fields">只转换的字段.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string TableToJson(DataTable dt, bool filter, string[] fields)
        {
            return TableToJson(dt, filter, fields, false);
        }
        /// <summary>
        /// 将数据表转换成JSON类型字符串.
        /// </summary>
        /// <param name="dt">DataTable数据表.</param>
        /// <param name="filter">是否过滤空数据.</param>
        /// <param name="fields">只转换的字段.</param>
        /// <param name="unicode">是否进行Unicode编码.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string TableToJson(DataTable dt, bool filter, string[] fields, bool unicode)
        {
            string rowDelimiter = "";
            if (dt == null)
            {
                return "[]";
            }
            StringBuilder result = new StringBuilder("[");
            foreach (DataRow row in dt.Rows)
            {
                result.Append(rowDelimiter);
                result.Append(RowToJson(row, filter, fields, unicode));
                rowDelimiter = ",";
            }
            result.Append("]");

            return result.ToString();
        }

        /// <summary>
        /// 将Table转为Json格式数据.
        /// </summary>
        /// <param name="dt">DataTable数据表.</param>
        /// <param name="filter">是否过滤DataNull的数据.</param>
        /// <param name="fields">只转换的字段.</param>
        /// <param name="kfName">主键字段名.</param>
        /// <param name="pkfName">父级字段名.</param>
        /// <param name="orderBy">排序方式.</param>
        /// <returns>
        /// 返回Json格式字符串数据
        /// </returns>
        public static string TableToJson(DataTable dt, bool filter, string[] fields, string kfName, string pkfName, string orderBy)
        {
            return TableToJson(dt, filter, fields, kfName, pkfName, orderBy, false);
        }

        /// <summary>
        /// 将Table转为Json格式数据.
        /// </summary>
        /// <param name="dt">DataTable数据表.</param>
        /// <param name="filter">是否过滤DataNull的数据.</param>
        /// <param name="fields">只是转换的字段.</param>
        /// <param name="kfName">主键字段名.</param>
        /// <param name="pkfName">父级字段名.</param>
        /// <param name="orderBy">排序方式.</param>
        /// <param name="unicode">是否进行Unicode编码.</param>
        /// <returns>
        /// 返回Json格式字符串数据
        /// </returns>
        public static string TableToJson(DataTable dt, bool filter, string[] fields, string kfName, string pkfName, string orderBy, bool unicode)
        {
            if (dt == null)
            {
                return "[]";
            }
            StringBuilder sb = new StringBuilder("[");
            string firstCondition = "[" + pkfName + "] is null or [" + pkfName + "]=0";

            DataRow[] drs = dt.Select(firstCondition, orderBy); //获取第一级的数据行

            Recursive(drs, filter, fields, kfName, pkfName, orderBy, sb, unicode);

            sb.Replace(",", "", sb.Length - 1, 1);
            sb.Append("]");
            return sb.ToString();
        }

        /// <summary>
        /// DataRow转换为Json格式字符串
        /// </summary>
        /// <param name="row">数据行.</param>
        /// <param name="filter">是否过滤DataNull的数据.</param>
        /// <param name="fields">The fields.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string RowToJson(DataRow row, bool filter, string[] fields)
        {
            return RowToJson(row, filter, fields, false);
        }
        /// <summary>
        /// DataRow转换为Json格式字符串.
        /// </summary>
        /// <param name="row">数据行.</param>
        /// <param name="filter">是否过滤空数据.</param>
        /// <param name="fields">只是转换的字符串.</param>
        /// <param name="unicode">是否进行Unicode编码.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string RowToJson(DataRow row, bool filter, string[] fields, bool unicode)
        {
            if (row == null) return "";

            DataColumnCollection cols = row.Table.Columns;
            string colDelimiter = "";

            StringBuilder result = new StringBuilder("{");

            bool fldNull = Utils.IsNullOrEmpty(fields);
            for (int i = 0; i < cols.Count; i++)
            {
                if (!fldNull && !fields.Contains<string>(cols[i].ColumnName))
                    continue;

                // 获取当前单元格的值
                object val = row[i];

                // 值为null时直接进行下一个数据处理
                if (val == null) continue;

                if (val is DataTable)
                {
                    result.Append(colDelimiter).Append("\"")
                          .Append(cols[i].ColumnName).Append("\":")
                          .Append(TableToJson((DataTable)val, filter, fields, unicode));

                    colDelimiter = ",";

                }
                else if (val is DataRow)
                {
                    result.Append(colDelimiter).Append("\"")
                          .Append(cols[i].ColumnName).Append("\":")
                          .Append(RowToJson((DataRow)val, filter, fields, unicode));

                    colDelimiter = ",";

                }
                else if (val is IDictionary)
                {
                    IDictionary dictionary = val as IDictionary;
                    if (dictionary != null)
                    {
                        result.Append(colDelimiter).Append("\"")
                          .Append(cols[i].ColumnName).Append("\":")
                          .Append(JsonUtils.ConvertJson(dictionary, unicode));

                        colDelimiter = ",";
                    }

                }
                else
                {
                    // use index rather than foreach, so we can use the index for both the row and cols collection
                    string vals = Utils.JsonValueOfType(row[i], cols[i].DataType, unicode);
                    if (filter && vals == "null")
                        continue;
                    else
                    {
                        result.Append(colDelimiter).Append("\"")
                              .Append(cols[i].ColumnName).Append("\":")
                              .Append(vals);

                        colDelimiter = ",";
                    }
                }

            }
            result.Append("}");
            return result.ToString();
        }

        #region privates


        /// <summary>
        /// Recursive 内部函数,递归处理无限级的处理
        /// </summary>
        /// <param name="drs">/DataRow集合.</param>
        /// <param name="filter">是否过滤DataNull的数据.</param>
        /// <param name="fields">只是转换的字段.</param>
        /// <param name="kfName">主键字段名.</param>
        /// <param name="pkfName">父级字段名.</param>
        /// <param name="orderBy">排序方式.</param>
        /// <param name="sb">递归加载处理的字条串容器.</param>
        /// <param name="unicode">if set to <c>true</c> [unicode].</param>
        private static void Recursive(DataRow[] drs, bool filter, string[] fields, string kfName, string pkfName, string orderBy, StringBuilder sb, bool unicode)
        {
            foreach (DataRow dr in drs)
            {
                sb.Append(RowToJson(dr, filter, fields, unicode));
                if (Utils.ToInt(dr[kfName]) > 0)
                {
                    DataRow[] tdrs = dr.Table.Select(pkfName + "='" + dr[kfName] + "'", orderBy);
                    if (tdrs.Length > 0)
                    {
                        sb.Replace("}", ",\"children\":[", sb.Length - 1, 1);
                        Recursive(tdrs, filter, fields, kfName, pkfName, orderBy, sb, unicode);
                        sb.Append("]}");
                    }
                }
                sb.Append(",");
            }
            sb.Replace(",", "", sb.Length - 1, 1);
        }



        #endregion


        #endregion

        #region DataRow2Xml

        /// <summary>
        /// 把DataRow转换成Xml格式
        /// </summary>
        /// <param name="dataSource">数据源.</param>
        /// <param name="rowNode">行节点名.</param>
        /// <param name="fields">要转换的列名.</param>
        /// <returns>
        /// System.String
        /// </returns>
        static public string DataRow2Xml(DataRow dataSource, string rowNode, string[] fields)
        {
            DataColumnCollection cols = dataSource.Table.Columns;

            StringBuilder sb = new StringBuilder();

            bool IsContainRow = !String.IsNullOrEmpty(rowNode);

            if (IsContainRow)
                sb.Append("<" + rowNode + ">");

            for (int j = 0; j < cols.Count; j++)
            {
                bool fldNull = Utils.IsNullOrEmpty(fields);
                //判断是否在只是要显示和转换的列中
                if (!fldNull && !fields.Contains<string>(cols[j].ColumnName))
                    continue;

                string columnName = cols[j].ColumnName;

                // 获取当前单元格的值
                object val = dataSource[j];

                // 值为null时直接进行下一个数据处理
                if (val == null)
                {
                    sb.Append(Utils.ConvertXmlData(columnName, ""));
                    continue;
                }

                // sting 也是 IEnumerable中的一种 情况 
                if (val is string)
                {
                    sb.Append(Utils.ConvertXmlData(columnName, dataSource[j].ToString()));
                }
                else if (val is DataTable)
                {
                    sb.Append("<" + columnName + ">");
                    sb.Append(Table2Xml((DataTable)val, null, fields));
                    sb.Append("</" + columnName + ">");

                }
                else if (val is DataRow)
                {
                    sb.Append("<" + columnName + ">");
                    sb.Append(DataRow2Xml((DataRow)val, null, fields));
                    sb.Append("</" + columnName + ">");

                }
                else if (val is IDictionary)
                {
                    IDictionary dictionary = val as IDictionary;
                    if(dictionary != null)
                    {
                        sb.Append("<" + columnName + ">");
                        sb.Append(XmlUtils.ConvertXml(dictionary, columnName, false));
                        sb.Append("</" + columnName + ">");
                    }
                    

                }
                else if (val is IEnumerable)
                {
                    sb.Append("<" + columnName + ">");
                    sb.Append(XmlUtils.ConvertXml((IEnumerable)val, columnName, false));
                    sb.Append("</" + columnName + ">");

                }
                else
                {
                    //string val = dataSource[j].ToString();
                    sb.Append(Utils.ConvertXmlData(columnName, val.ToString()));
                }
            }

            if (IsContainRow)
                sb.Append("</" + rowNode + ">");

            return sb.ToString();
        }

        /// <summary>
        /// 把DataRow转换成Xml格式
        /// </summary>
        /// <param name="dataSource">数据源.</param>
        /// <param name="rowNode">行节点名.</param>
        /// <param name="fields">要转换的列名.</param>
        /// <param name="isAttribute">是否以属性形式进行转换.</param>
        /// <returns> 
        /// System.String
        /// </returns>
        static public string DataRow2Xml(DataRow dataSource, string rowNode, string[] fields, bool isAttribute)
        {
            DataColumnCollection cols = dataSource.Table.Columns;

            StringBuilder sb = new StringBuilder();

            rowNode = String.IsNullOrEmpty(rowNode) ? "row" : rowNode;
            sb.Append("<" + rowNode + " ");

            for (int j = 0; j < cols.Count; j++)
            {
                bool fldNull = Utils.IsNullOrEmpty(fields);
                //判断是否在只是要显示和转换的列中
                if (!fldNull && !fields.Contains<string>(cols[j].ColumnName))
                    continue;
                sb.Append(cols[j].ColumnName + "=\"" + dataSource[j].ToString() + "\" ");
            }

            sb.Append("/>");

            return sb.ToString();
        }

        #endregion

        #region Table2Xml 把DataTable转换成Xml格式的数据


        /// <summary>
        /// 把DataTable转换成Xml格式,默认返回的行节点名row
        /// </summary>
        /// <param name="dataSource">DataTable数据源.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string Table2Xml(DataTable dataSource)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dataSource.Rows.Count; i++)
            {
                sb.Append(DataRow2Xml(dataSource.Rows[i], "row", null));
            }
            return sb.ToString();

        }

        /// <summary>
        /// 把DataTable转换成Xml格式
        /// </summary>
        /// <param name="dataSource">数据源.</param>
        /// <param name="rowNode">行节点名.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string Table2Xml(DataTable dataSource, string rowNode)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dataSource.Rows.Count; i++)
            {
                sb.Append(DataRow2Xml(dataSource.Rows[i], rowNode, null));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 把DataTable转换成Xml格式
        /// </summary>
        /// <param name="dataSource">数据源.</param>
        /// <param name="rowNode">行节点名.</param>
        /// <param name="fields">要转换的列名组.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string Table2Xml(DataTable dataSource, string rowNode, string[] fields)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dataSource.Rows.Count; i++)
            {
                sb.Append(DataRow2Xml(dataSource.Rows[i], rowNode, fields));
            }

            return sb.ToString();
        }

        /// <summary>
        /// 把DataTable转换成Xml格式
        /// </summary>
        /// <param name="dataSource">数据源.</param>
        /// <param name="rowNode">行节点名.</param>
        /// <param name="fields">要转换的列名组.</param>
        /// <param name="isAttribute">是否以属性形式进行转换.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string Table2Xml(DataTable dataSource, string rowNode, string[] fields, bool isAttribute)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < dataSource.Rows.Count; i++)
            {
                sb.Append(DataRow2Xml(dataSource.Rows[i], rowNode, fields, isAttribute));
            }

            return sb.ToString();
        }
        /// <summary>
        /// 将DataTable数据转换为Xml格式的数据
        /// </summary>
        /// <param name="dataSource">The data source.</param>
        /// <param name="isAttribute">指定是否把列以属性的形式进行Xml格式组合</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string Table2Xml(DataTable dataSource, bool isAttribute)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < dataSource.Rows.Count; i++)
            {
                sb.Append(DataRow2Xml(dataSource.Rows[i], null, null, isAttribute));
            }
            return sb.ToString();

        }





        #endregion

        #region 把数据表转换成NameValueCollection

        /// <summary>
        /// 把Datatable的数据转换为NameValue集合.
        /// </summary>
        /// <param name="dt">数据表.</param>
        /// <param name="keyFeild">键字段名.</param>
        /// <param name="valFeild">键值字段名.</param>
        /// <returns></returns>
        static public NameValueCollection Table2NVC(DataTable dt, string keyFeild, string valFeild)
        {
            return Table2NVC(dt, keyFeild, valFeild, false);
        }
        /// <summary>
        /// 把Datatable的数据转换为NameValue集合.
        /// </summary>
        /// <param name="dt">数据表.</param>
        /// <param name="keyFeild">键字段名.</param>
        /// <param name="valFeild">键值字段名.</param>
        /// <param name="isKeyLower">是否把键转为小写.</param>
        /// <returns>NameValueCollection</returns>
        static public NameValueCollection Table2NVC(DataTable dt, string keyFeild, string valFeild, bool isKeyLower)
        {
            NameValueCollection nvc = new NameValueCollection();
            try
            {
                for (int r = 0; r < dt.Rows.Count; r++)
                {
                    if (dt.Rows[r][keyFeild] == null) continue;

                    string key = dt.Rows[r][keyFeild].ToString();
                    if (!String.IsNullOrEmpty(key))
                    {
                        if (isKeyLower)
                        {
                            key = key.ToLower();
                        }
                        nvc.Add(key, dt.Rows[r][valFeild].ToString());
                    }
                }
                return nvc;
            }
            catch
            {
                return nvc;
            }
        }

        #endregion

        #region Xml2Table

        /// <summary>
        /// 将xml格式数据转换为DataTable数据
        /// </summary>
        /// <param name="xmlString">Xml字符串.</param>
        /// <returns>System.Data.DataTable</returns>
        public static DataTable Xml2Table(string xmlString)
        {
            return Xml2Table(xmlString, 0);
        }

        /// <summary>
        /// 将xml格式数据转换为DataTable数据
        /// </summary>
        /// <param name="xmlString">Xml字符串.</param>
        /// <param name="TableIndex">获取第几表的数据,默认为和一张表的数据.</param>
        /// <returns>
        /// System.Data.DataTable
        /// </returns>
        public static DataTable Xml2Table(string xmlString, int TableIndex)
        {
            DataSet ds = Xml2DataSet(xmlString);
            if (ds.Tables.Count > 0)
            {
                if (TableIndex < ds.Tables.Count)
                {
                    return ds.Tables[TableIndex];
                }
                else
                {
                    return ds.Tables[0];
                }
            }
            else
            {
                return new DataTable();
            }
        }

        #endregion

        #region Xml2DataSet

        /// <summary>
        /// 将xml数据转换为DataSet格式数据
        /// </summary>
        /// <param name="xmlString">xml字条串.</param>
        /// <returns>System.Data.DataSet</returns>
        public static DataSet Xml2DataSet(string xmlString)
        {
            try
            {
                DataSet ds = new DataSet();

                ds.ReadXmlSchema(new System.IO.StringReader(xmlString));
                foreach (DataTable MyTab in ds.Tables)
                {
                    MyTab.BeginLoadData();
                }
                ds.ReadXml(new System.IO.StringReader(xmlString), XmlReadMode.IgnoreSchema);
                foreach (DataTable MyTab in ds.Tables)
                {
                    MyTab.EndLoadData();
                }

                return ds;
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
                return new DataSet();
            }
        }

        #endregion

        /// <summary>
        /// 合并DbParameter数组.
        /// </summary>
        /// <param name="first">第一个数组.</param>
        /// <param name="second">第二个数组.</param>
        /// <returns></returns>
        public static DbParameter[] MergeParameters(DbParameter[] first, DbParameter[] second)
        {
            if (first == null) return second;

            if (second == null) return first;

            DbParameter[] dbparameters = new DbParameter[first.Length + second.Length];

            first.CopyTo(dbparameters, 0);
            second.CopyTo(dbparameters, first.Length);

            return dbparameters;
        }

        /// <summary>
        ///  合并为层级数据.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  11-17-2014
        /// </summary>
        /// <param name="primaryTable">主表.</param>
        /// <param name="primaryField">主表主键.</param>
        /// <param name="joinTable">从表.</param>
        /// <param name="joinField">从表外键.</param>
        /// <returns>List&lt;LayerEntity&gt;.</returns>
        public static List<DataRow> MergeLayerData(DataTable primaryTable, string primaryField, DataTable joinTable, string joinField)
        {
            try
            {
                if (!primaryTable.Columns.Contains("Children"))
                {
                    DataColumn column = new DataColumn("Children");
                    column.DataType = typeof(List<DataRow>);
                    primaryTable.Columns.Add(column);
                }

                List<DataRow> _LayerData = new List<DataRow>();

                foreach (DataRow pdr in primaryTable.Rows)
                {
                    DataRow[] jdrs = joinTable.Select(joinField + "='" + pdr[primaryField] + "'");
                    List<DataRow> cList = new List<DataRow>();
                    foreach (DataRow jdr in jdrs)
                    {
                        cList.Add(jdr);
                    }
                    pdr["Children"] = cList;
                    _LayerData.Add(pdr);
                }

                return _LayerData;
            }
            catch (Exception ex)
            {
                //new ECFException(ex);
                return new List<DataRow>();
            }
        }


    }
}
