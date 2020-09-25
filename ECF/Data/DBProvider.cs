using System;
using System.Collections.Generic;
using System.Data;
using MySql.Data.MySqlClient;
using System.Reflection;
using System.Data.Common;
using ECF.Caching;
using System.Linq;

namespace ECF.Data
{
    /// <summary>
    ///   <see cref="ECF.Data.MySql.DBProvider"/>
    /// DB provider
    /// Author:  XP
    /// Created: 2011/9/14
    /// </summary>
    [Serializable]
    public class DBProvider : AbstractCache, IDBProvider
    {
        #region Insance
        static IDBProvider _Instance;
        static IDBProvider Instance
        {
            get
            {
                if (_Instance == null)
                {
                    _Instance = (IDBProvider)new DBProvider();
                }
                return _Instance;
            }
        }
        #endregion

        /// <summary>
        /// 返回DbProviderFactory实例
        /// </summary>
        /// <returns></returns>
        public System.Data.Common.DbProviderFactory FactoryInstance()
        {
            return MySqlClientFactory.Instance;
        }

        /// <summary>
        /// 检索SQL参数信息并填充
        /// </summary>
        /// <param name="cmd"></param>
        public void DeriveParameters(System.Data.IDbCommand cmd)
        {
            if ((cmd as MySqlCommand) != null)
            {
                MySqlCommandBuilder.DeriveParameters(cmd as MySqlCommand);
            }
        }


        #region MakeParameter
        /// <summary>
        /// 创建SQL参数
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="DbType"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        public System.Data.Common.DbParameter MakeParameter(string ParamName, System.Data.DbType DbType, int Size)
        {
            MySqlParameter param;

            if (Size > 0)
                param = new MySqlParameter(ParamName, (MySqlDbType)DbType, Size);
            else
                param = new MySqlParameter(ParamName, (MySqlDbType)DbType);

            return param;
        }
        #endregion

        #region MakeParameters
        /// <summary>
        /// 创建一个Sql参数数组.
        /// </summary>
        /// <param name="length">数组的长度.</param>
        /// <returns></returns>
        public System.Data.Common.DbParameter[] MakeParameters(int length)
        {
            return new MySqlParameter[length];
        }

        /// <summary>
        /// 创建一个Sql参数数组.
        /// </summary>
        /// <param name="dic">根据键值获取Sql参数数组.</param>
        /// <returns></returns>
        public System.Data.Common.DbParameter[] MakeParameters(System.Collections.Generic.Dictionary<string, object> dic)
        {
            if (dic.Count > 0)
            {
                MySqlParameter[] dps = new MySqlParameter[dic.Count];
                int keyi = 0;
                foreach (KeyValuePair<string, object> d in dic)
                {
                    Type t = d.Value.GetType();
                    dps[keyi] = new MySqlParameter("@" + d.Key, d.Value);
                }
                return dps;
            }
            return new MySqlParameter[0];
        }

        /// <summary>
        /// Makes the parameter.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        /// <param name="paramValue">The parameter value.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public DbParameter MakeParameter(string paramName, object paramValue)
        {
            return new MySqlParameter(paramName, paramValue);
        }

        #endregion

        #region Properties
        /// <summary>
        /// 是否支持全文搜索
        /// </summary>
        /// <returns></returns>
        public bool IsFullTextSearchEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// 是否支持压缩数据库
        /// </summary>
        /// <returns></returns>
        public bool IsCompactDatabase
        {
            get { return true; }
        }

        /// <summary>
        /// 是否支持备份数据库
        /// </summary>
        /// <returns></returns>
        public bool IsBackupDatabase
        {
            get { return true; }
        }

        /// <summary>
        /// 返回刚插入记录的自增ID值, 如不支持则为""
        /// </summary>
        /// <returns></returns>
        public string IdentitySql
        {
            get { return "SELECT LAST_INSERT_ID();"; }
        }

        /// <summary>
        /// 是否支持数据库优化
        /// </summary>
        /// <returns></returns>
        public bool IsDbOptimize
        {
            get { return false; }
        }

        /// <summary>
        /// 是否支持数据库收缩
        /// </summary>
        /// <returns></returns>
        public bool IsShrinkData
        {
            get { return true; }
        }

        /// <summary>
        /// 是否支持存储过程
        /// </summary>
        /// <returns></returns>
        public bool IsStoreProc
        {
            get { return true; }
        }

        /// <summary>
        /// 随机读取数据库记录
        /// 因为每一种数据库获取随机数是写法不一样
        /// </summary>
        public string RndOrder
        {
            get { return ""; }
        }

        /// <summary>
        /// 指定该数据库是否支持事物.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is transaction; otherwise, <c>false</c>.
        /// </value>
        public bool IsTransaction
        {
            get { return true; }
        }
        #endregion

        #region GetDBType
        /// <summary>
        /// 将.net系统类型转换为数据库类型.
        /// </summary>
        /// <param name="type">系统类型.</param>
        /// <returns></returns>
        public System.Data.DbType CovertType(Type type)
        {

            MySqlParameter p1;
            System.ComponentModel.TypeConverter tc;
            p1 = new MySqlParameter();
            tc = System.ComponentModel.TypeDescriptor.GetConverter(p1.DbType);
            if (tc.CanConvertFrom(type))
            {
                p1.DbType = (DbType)tc.ConvertFrom(type.Name);
            }
            else
            { //Try brute force 
                try
                {
                    p1.DbType = (System.Data.DbType)tc.ConvertFrom(type.Name);
                }
                catch
                { //Do Nothing 
                }
            }
            return p1.DbType;

        }
        #endregion



        #region InsertCommandText

        #region private
        /// <summary>
        /// 根据数据和属性进行赋值
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string DataValueString(PropertyInfo pi, object data)
        {
            Type type = Utils.GetNullableType(pi.PropertyType);
            //字符串
            if (type == typeof(string))
            {
                return Utils.SqlFilter(string.Format("{0}", data));
            }
            else if (type == typeof(DateTime)) //日期
            {
                return string.Format(DateTimeFormatString, data);
            }
            else if (type == typeof(Boolean)) //日期
            {
                return ((bool)data ? "1" : "0");
            }
            else
            {
                return string.Format("{0}", data);
            }
        }
        /// <summary>
        /// 根据数据和属性进行赋值
        /// </summary>
        /// <param name="pi"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private string SqlString(PropertyInfo pi, object data)
        {
            Type type = Utils.GetNullableType(pi.PropertyType);
            //字符串
            if (type == typeof(string) || type == typeof(DateTime))
            {
                return string.Format("'{0}'", DataValueString(pi, data));
            }
            else if (type == typeof(Boolean))
            {
                return string.Format("{0}", DataValueString(pi, data));
            }
            else
            {
                return string.Format("{0}", Utils.SqlFilter(data.ToString()));
            }
        }
        #endregion



        /// <summary>
        /// 获取插入数据的sql语句.
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <returns></returns>
        public string InsertCommandText(IEntity entity, string tableName)
        {
            try
            {
                Type type = GetCacheType(entity);
                PropertyInfo[] pis = type.GetProperties();

                // 获取此类中是否包含数据字段验证
                bool isValidateDataField = EntityClassAttribute.IsValidateField(type);

                string sql = "INSERT INTO " + tableName + " (";
                string val = " VALUES(";

                foreach (PropertyInfo pi in pis)
                {
                    if (pi.GetValue(entity, null) != null && pi.CanWrite && pi.Name.ToLower() != "entityfullname")
                    {
                        // 此类中包含有非数据库字段，需要进行排除
                        if (isValidateDataField)
                        {
                            if (EntityPropertyAttribute.IsNoDataField(pi)) continue;
                        }

                        sql += pi.Name + ",";
                        val += SqlString(pi, pi.GetValue(entity, null)) + ",";
                    }
                }

                sql = sql.Remove(sql.Length - 1) + ") ";
                val = val.Remove(val.Length - 1) + ")";

                return sql + val + ";";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        /// <summary>
        /// 获取插入数据的sql语句.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <param name="prefixParameter">The prefix paramter.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.Common.DbException"></exception>
        public string InsertCommandText(IEntity entity, string tableName, out DbParameter[] commandParameters, string prefixParameter = "")
        {
            try
            {
                Type type = GetCacheType(entity);
                PropertyInfo[] pis = type.GetProperties();

                // 获取此类中是否包含数据字段验证
                bool isValidateDataField = EntityClassAttribute.IsValidateField(type);

                string sql = "INSERT INTO " + tableName + " (";
                string val = " VALUES(";

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    foreach (PropertyInfo pi in pis)
                    {

                        if (pi.GetValue(entity, null) != null && pi.CanWrite && pi.Name.ToLower() != "entityfullname")
                        {
                            // 此类中包含有非数据库字段，需要进行排除
                            if (isValidateDataField)
                            {
                                if (EntityPropertyAttribute.IsNoDataField(pi)) continue;
                            }

                            string paraName = "@" + prefixParameter + entity.EntityFullName.Replace(",", "_").Replace(".", "_") + "_" + pi.Name;

                            sql += pi.Name + ",";
                            val += paraName + ",";
                            cmd.Parameters.AddWithValue(paraName, pi.GetValue(entity, null));
                        }
                    }
                    sql = sql.Remove(sql.Length - 1) + ") ";
                    val = val.Remove(val.Length - 1) + ")";


                    //foreach (PropertyInfo pi in pis)
                    //{
                    //    if (pi.GetValue(entity, null) != null && pi.CanWrite && pi.Name.ToLower() != "entityfullname")
                    //    {
                    //        sql += pi.Name + ",";
                    //        val += "@" + pi.Name + ",";
                    //        cmd.Parameters.AddWithValue("@" + pi.Name, pi.GetValue(entity, null));
                    //    }
                    //}
                    //sql = sql.Remove(sql.Length - 1) + ") ";
                    //val = val.Remove(val.Length - 1) + ")";

                    commandParameters = new MySqlParameter[cmd.Parameters.Count];

                    // 将cmd的Parameters参数集复制到discoveredParameters数组.
                    cmd.Parameters.CopyTo(commandParameters, 0);

                    return sql + val + ";";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region UpdateCommandText 获取待更新的数据库查询语句

        /// <summary>
        /// 获取待更新的数据库查询语句.
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="fields">条件字段.</param>
        /// <returns></returns>
        public string UpdateCommandText(IEntity entity, string tableName, string[] fields)
        {
            try
            {
                Type type = GetCacheType(entity);
                PropertyInfo[] pis = type.GetProperties();
                // 获取此类中是否包含数据字段验证
                bool isValidateDataField = EntityClassAttribute.IsValidateField(type);

                string sql = "UPDATE " + tableName + " SET ";

                string condtion = "";
                foreach (PropertyInfo pi in pis)
                {
                    // 此类中包含有非数据库字段，需要进行排除
                    if (isValidateDataField)
                    {
                        if (EntityPropertyAttribute.IsNoDataField(pi))
                            continue;
                    }

                    //string type = pi.PropertyType.UnderlyingSystemType.ToString().ToLower();
                    if (fields != null && fields.Contains<string>(pi.Name, StringComparer.InvariantCultureIgnoreCase))
                    {
                        condtion += (String.IsNullOrEmpty(condtion) ? "" : " and ") + pi.Name + "=" + SqlString(pi, pi.GetValue(entity, null));
                    }
                    else
                    {
                        if (pi.GetValue(entity, null) != null && pi.CanWrite && pi.Name.ToLower() != "entityfullname")
                        {
                            sql += pi.Name + "=" + SqlString(pi, pi.GetValue(entity, null)) + ",";
                        }
                    }
                }

                sql = sql.Remove(sql.Length - 1);
                if (!Utils.IsNullOrEmpty(condtion))
                    sql = sql + " WHERE " + condtion;


                return sql + ";";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取待更新的数据库查询语句.
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="fields">条件字段.</param>
        /// <param name="commandParameters">输出参数.</param>
        /// <param name="prefixParameter">The prefix paramter.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.Common.DbException"></exception>
        public string UpdateCommandText(IEntity entity, string tableName, string[] fields, out DbParameter[] commandParameters, string prefixParameter = "")
        {
            try
            {
                Type type = GetCacheType(entity);
                PropertyInfo[] pis = type.GetProperties();
                string sql = "UPDATE " + tableName + " SET ";

                bool isValidateDataField = EntityClassAttribute.IsValidateField(type);

                using (MySqlCommand cmd = new MySqlCommand())
                {

                    string condition = "";
                    foreach (PropertyInfo pi in pis)
                    {
                        // 此类中包含有非数据库字段，需要进行排除
                        if (isValidateDataField)
                        {
                            if (EntityPropertyAttribute.IsNoDataField(pi))
                                continue;
                        }
                        string paraName = "@" + prefixParameter + entity.EntityFullName.Replace(",", "_").Replace(".", "_") + "_" + pi.Name;
                        //string type = pi.PropertyType.UnderlyingSystemType.ToString().ToLower();
                        if (fields != null && fields.Contains<string>(pi.Name, StringComparer.InvariantCultureIgnoreCase))
                        {
                            condition += (String.IsNullOrEmpty(condition) ? "" : " and ") + pi.Name + "=" + paraName;
                            cmd.Parameters.AddWithValue(paraName, pi.GetValue(entity, null));
                        }
                        else
                        {
                            if (pi.GetValue(entity, null) != null && pi.CanWrite && pi.Name.ToLower() != "entityfullname")
                            {
                                sql += pi.Name + "=" + paraName + ",";
                                cmd.Parameters.AddWithValue(paraName, pi.GetValue(entity, null));
                            }
                        }
                    }

                    sql = sql.Remove(sql.Length - 1);
                    if (!Utils.IsNullOrEmpty(condition))
                        sql = sql + " WHERE " + condition;


                    commandParameters = new MySqlParameter[cmd.Parameters.Count];
                    // 将cmd的Parameters参数集复制到discoveredParameters数组.
                    cmd.Parameters.CopyTo(commandParameters, 0);

                    return sql + ";";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region UpsertCommandText 判断相同记录进行插入或更新
        /// <summary>
        /// 获取插入更新的数据库查询语句.
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="fields">条件字段.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public string UpsertCommandText(IEntity entity, string tableName, string[] fields)
        {
            try
            {
                Type type = GetCacheType(entity);
                PropertyInfo[] pis = type.GetProperties();
                string updateSql = string.Empty, setSql = string.Empty;
                string insertSql = "INSERT INTO " + tableName + "(";
                List<string> vals = new List<string>();
                //                    (name) 
                //select 'test1'
                //FROM DUAL WHERE NOT EXISTS(SELECT 1 FROM test1 WHERE name = 'test1'); "
                bool isValidateDataField = EntityClassAttribute.IsValidateField(type);

                using (MySqlCommand cmd = new MySqlCommand())
                {

                    string condition = "";
                    foreach (PropertyInfo pi in pis)
                    {
                        if (pi.GetValue(entity, null) != null && pi.CanWrite && pi.Name.ToLower() != "entityfullname")
                        {
                            // 此类中包含有非数据库字段，需要进行排除
                            if (isValidateDataField)
                            {
                                if (EntityPropertyAttribute.IsNoDataField(pi))
                                    continue;
                            }

                            string piValue = SqlString(pi, pi.GetValue(entity, null));
                            // 插入数据
                            insertSql += pi.Name + ",";
                            vals.Add(piValue);

                            if (fields != null && fields.Contains<string>(pi.Name, StringComparer.InvariantCultureIgnoreCase))
                            {
                                condition += (String.IsNullOrEmpty(condition) ? "" : " and ") + pi.Name + "=" + piValue;
                            }
                            else
                            {
                                setSql += pi.Name + "=" + piValue + ",";
                            }
                        }
                    }

                    // 组装插入语句
                    insertSql = insertSql.Remove(insertSql.Length - 1) + ") SELECT " + string.Join(",", vals)
                        + " FROM DUAL WHERE NOT EXISTS(SELECT 1 FROM " + tableName + " WHERE " + condition + ");";

                    // 组装更新语句
                    if (!string.IsNullOrEmpty(setSql))
                    {
                        updateSql = "UPDATE " + tableName + " SET " + setSql.Remove(setSql.Length - 1);
                        if (!Utils.IsNullOrEmpty(condition))
                            updateSql = updateSql + " WHERE " + condition;
                    }

                    return insertSql + updateSql + ";";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取待更新和插入的数据库查询语句.
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="fields">条件字段.</param>
        /// <param name="commandParameters">输出参数.</param>
        /// <param name="prefixParameter">参数前缀.</param>
        /// <returns></returns>
        public string UpsertCommandText(IEntity entity, string tableName, string[] fields, out DbParameter[] commandParameters, string prefixParameter = "")
        {
            try
            {
                Type type = GetCacheType(entity);
                PropertyInfo[] pis = type.GetProperties();
                string updateSql = string.Empty, setSql = string.Empty;
                string insertSql = "INSERT INTO " + tableName + "(";
                List<string> vals = new List<string>();
                //                    (name) 
                //select 'test1'
                //FROM DUAL WHERE NOT EXISTS(SELECT 1 FROM test1 WHERE name = 'test1'); "
                bool isValidateDataField = EntityClassAttribute.IsValidateField(type);

                using (MySqlCommand cmd = new MySqlCommand())
                {

                    string condition = "";
                    foreach (PropertyInfo pi in pis)
                    {
                        if (pi.GetValue(entity, null) != null && pi.CanWrite && pi.Name.ToLower() != "entityfullname")
                        {
                            // 此类中包含有非数据库字段，需要进行排除
                            if (isValidateDataField)
                            {
                                if (EntityPropertyAttribute.IsNoDataField(pi))
                                    continue;
                            }
                            string paraName = "@" + prefixParameter + entity.EntityFullName.Replace(",", "_").Replace(".", "_") + "_" + pi.Name;

                            // 插入数据
                            insertSql += pi.Name + ",";
                            vals.Add(paraName);

                            if (fields != null && fields.Contains<string>(pi.Name, StringComparer.InvariantCultureIgnoreCase))
                            {
                                condition += (String.IsNullOrEmpty(condition) ? "" : " and ") + pi.Name + "=" + paraName;
                                cmd.Parameters.AddWithValue(paraName, pi.GetValue(entity, null));
                            }
                            else
                            {

                                setSql += pi.Name + "=" + paraName + ",";
                                cmd.Parameters.AddWithValue(paraName, pi.GetValue(entity, null));
                            }
                        }
                    }

                    // 组装插入语句
                    insertSql = insertSql.Remove(insertSql.Length - 1) + ") SELECT " + string.Join(",", vals)
                        + " FROM DUAL WHERE NOT EXISTS(SELECT 1 FROM " + tableName + " WHERE " + condition + ");";

                    // 组装更新语句
                    if (!string.IsNullOrEmpty(setSql))
                    {
                        updateSql = "UPDATE " + tableName + " SET " + setSql.Remove(setSql.Length - 1);
                        if (!Utils.IsNullOrEmpty(condition))
                            updateSql = updateSql + " WHERE " + condition;
                    }

                    commandParameters = new MySqlParameter[cmd.Parameters.Count];
                    // 将cmd的Parameters参数集复制到discoveredParameters数组.
                    cmd.Parameters.CopyTo(commandParameters, 0);

                    return insertSql + updateSql + ";";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region DeleteCommandText

        /// <summary>
        /// 获取删除数据的Sql语句.
        /// </summary>
        /// <param name="entity">已附值的实体.</param>
        /// <param name="tableName">需要删除数据的表.</param>
        /// <returns></returns>
        public string DeleteCommandText(IEntity entity, string tableName)
        {
            try
            {
                Type type = entity.GetType();
                PropertyInfo[] pis = type.GetProperties();
                bool isValidateDataField = EntityClassAttribute.IsValidateField(type);

                string sql = "DELETE FROM " + tableName + " WHERE ";

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    string where = "";
                    foreach (PropertyInfo pi in pis)
                    {


                        if (pi.GetValue(entity, null) != null && pi.Name.ToLower() != "entityfullname")
                        {
                            // 此类中包含有非数据库字段，需要进行排除
                            if (isValidateDataField)
                            {
                                if (EntityPropertyAttribute.IsNoDataField(pi)) continue;
                            }
                            where += (Utils.IsNullOrEmpty(where) ? "" : " AND ") + pi.Name + "=" + SqlString(pi, pi.GetValue(entity, null));
                        }
                    }

                    return sql + where + ";";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取删除数据的Sql语句.
        /// </summary>
        /// <param name="entity">已附值的实体.</param>
        /// <param name="tableName">需要删除数据的表.</param>
        /// <param name="commandParameters">输出参数.</param>
        /// <param name="prefixParameter">The prefix paramter.</param>
        /// <returns></returns>
        /// <exception cref="System.Data.Common.DbException"></exception>
        public string DeleteCommandText(IEntity entity, string tableName, out DbParameter[] commandParameters, string prefixParameter = "")
        {
            try
            {
                Type type = entity.GetType();
                PropertyInfo[] pis = type.GetProperties();


                bool isValidateDataField = EntityClassAttribute.IsValidateField(type);

                string sql = "DELETE FROM " + tableName + " WHERE ";

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    string where = "";
                    foreach (PropertyInfo pi in pis)
                    {
                        if (pi.GetValue(entity, null) != null && pi.Name.ToLower() != "entityfullname")
                        {
                            // 此类中包含有非数据库字段，需要进行排除
                            if (isValidateDataField)
                            {
                                if (EntityPropertyAttribute.IsNoDataField(pi)) continue;
                            }

                            string paraName = "@" + prefixParameter + entity.EntityFullName.Replace(",", "_").Replace(".", "_") + "_" + pi.Name;

                            where += (Utils.IsNullOrEmpty(where) ? "" : " AND ") + pi.Name + "=" + paraName;
                            cmd.Parameters.AddWithValue(paraName, pi.GetValue(entity, null));
                        }
                    }

                    commandParameters = new MySqlParameter[cmd.Parameters.Count];
                    // 将cmd的Parameters参数集复制到discoveredParameters数组.
                    cmd.Parameters.CopyTo(commandParameters, 0);

                    return sql + where + ";";
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Query CommadText
        /// <summary>
        ///  根据条件获取查询语句.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  04-02-2014
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="condition">条件.</param>
        /// <returns>System.String.</returns>
        public string QueryCommandText(string tableName, string condition)
        {
            return QueryCommandText(0, null, tableName, condition, "");
        }

        /// <summary>
        ///  根据条件获取查询语句.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  04-02-2014
        /// </summary>
        /// <param name="rows">获取数据行数.</param>
        /// <param name="tableName">表名.</param>
        /// <param name="condition">条件.</param>
        /// <param name="orderBy">排序条件.</param>
        /// <returns>System.String.</returns>
        public string QueryCommandText(int rows, string tableName, string condition, string orderBy)
        {
            return QueryCommandText(rows, null, tableName, condition, orderBy);
        }


        /// <summary>
        /// 根据条件获取查询语句.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  04-02-2014
        /// </summary>
        /// <param name="rows">获取数据行数.</param>
        /// <param name="fieldList">需要查询的数据列字段名,用逗号隔开.</param>
        /// <param name="tableName">表名.</param>
        /// <param name="condition">条件.</param>
        /// <param name="orderBy">排序条件.</param>
        public string QueryCommandText(int rows, string fieldList, string tableName, string condition, string orderBy)
        {
            return "SELECT "

                 + (String.IsNullOrEmpty(fieldList) ? " * " : " " + fieldList + " ")
                 + " FROM " + tableName
                 + (String.IsNullOrEmpty(condition) ? "" : " WHERE " + condition)
                 + (String.IsNullOrEmpty(orderBy) ? "" : " ORDER BY " + orderBy)
                 + (rows > 0 ? " limit 0," + rows : "") + ";";
        }


        /// <summary>
        ///  获取Sql查询语句.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  04-02-2014
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="conditions">The conditions.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="paras">The paras.</param>
        /// <returns>System.String.</returns>
        public string QueryCommandText(int rows, string tableName, string fields, ConditionParameter[] conditions, string orderBy, out DbParameter[] paras)
        {
            string condString = GetCondition(conditions, out paras);

            string sqlText = "SELECT " + ((fields != null && fields.Length < 1) ? " * " : " " + String.Join(",", fields) + " ")
            + " FROM " + tableName
            + (String.IsNullOrEmpty(condString) ? "" : " WHERE " + condString)
            + (String.IsNullOrEmpty(orderBy) ? "" : " ORDER BY " + orderBy)
            + (rows > 0 ? " limit 0," + rows : "");

            return sqlText + ";";
        }

        /// <summary>
        /// 获取Sql查询语句.
        /// </summary>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="condition">查询条件.</param>
        /// <param name="orderBy">数据排序方式.</param>
        /// <param name="fields">显示字段名.</param>
        /// <param name="rows">获取数据行数.</param>
        /// <param name="paras">输出组好的参数.</param>
        /// <returns></returns>
        public string QueryCommandText(string tableName, Dictionary<string, object> condition, out DbParameter[] paras, string orderBy = "", string[] fields = null, int rows = 0)
        {
            // 获取带参数型的查询条件
            string condString = GetCondition(condition, out paras);

            string sqlText = "SELECT " + ((fields != null && fields.Length < 1) ? " * " : " " + String.Join(",", fields) + " ")
            + " FROM " + tableName
            + (String.IsNullOrEmpty(condString) ? "" : " WHERE " + condString)
            + (String.IsNullOrEmpty(orderBy) ? "" : " ORDER BY " + orderBy)
            + (rows > 0 ? " limit 0," + rows : "");

            return sqlText + ";";
        }



        #endregion

        #region GetCondition 获取出条件语句

        /// <summary>
        /// 根据传入一参数获取出条件语句.
        /// </summary>
        /// <param name="paras">数据库参数.</param>
        /// <returns></returns>
        public string GetCondition(DbParameter[] paras)
        {
            string condition = "";
            foreach (DbParameter dp in paras)
            {
                condition += (Utils.IsNullOrEmpty(condition) ? "" : " and ") + dp.ParameterName + "=@" + dp.ParameterName;
            }
            return condition;
        }

        /// <summary>
        /// 根据条件参数获取条件和out出数据库参数.
        /// Author Tynan 20130528
        /// </summary>
        /// <param name="list">条件参数列表.</param>
        /// <param name="commandParameters">out出参数.</param>
        /// <returns>System.String.</returns>
        public string GetCondition(ConditionParameter[] list, out DbParameter[] commandParameters)
        {
            string result;
            using (MySqlCommand sqlCommand = new MySqlCommand())
            {
                string text = "";
                foreach (ConditionParameter current in list)
                {
                    string text2 = text;
                    text = string.Concat(new string[]
                    {
                        text2,
                        Utils.IsNullOrEmpty(text) ? "" : " AND ",
                        current.FieldName,
                        " "+current.Operate+" ",
                        current.ParameterName
                    });
                    sqlCommand.Parameters.AddWithValue(current.ParameterName, current.ParameterValue);

                }
                commandParameters = new MySqlParameter[sqlCommand.Parameters.Count];
                sqlCommand.Parameters.CopyTo(commandParameters, 0);
                result = text;
            }
            return result;
        }

        /// <summary>
        /// 根据传入一参数获取出条件语句
        /// </summary>
        /// <param name="dic">The dic.</param>
        /// <param name="commandParameters">The command parameters.</param>
        /// <param name="prefixParamter">The prefix paramter.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public string GetCondition(Dictionary<string, object> dic, out DbParameter[] commandParameters, string prefixParamter = "")
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                string where = "";
                foreach (KeyValuePair<string, object> d in dic)
                {

                    where += (Utils.IsNullOrEmpty(where) ? "" : " AND ") + d.Key + "=@" + d.Key;

                    cmd.Parameters.AddWithValue("@" + d.Key, d.Value);

                }

                commandParameters = new MySqlParameter[cmd.Parameters.Count];
                // 将cmd的Parameters参数集复制到discoveredParameters数组.
                cmd.Parameters.CopyTo(commandParameters, 0);

                return where;
            }
        }

        /// <summary>
        /// 根据实体获取条件
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="commandParameters">dbparameter[]参数.</param>
        /// <param name="prefixParameter">The prefix parameter.</param>
        /// <returns>
        /// System.String
        /// </returns>
        /// <exception cref="System.Data.Common.DbException"></exception>
        public string GetCondition(IEntity entity, out DbParameter[] commandParameters, string prefixParameter = "")
        {
            try
            {
                Type type = GetCacheType(entity);
                bool isValidateDataField = EntityClassAttribute.IsValidateField(type);
                PropertyInfo[] pis = type.GetProperties();

                using (MySqlCommand cmd = new MySqlCommand())
                {
                    string where = "";
                    foreach (PropertyInfo pi in pis)
                    {
                        string paraName = "@" + prefixParameter + entity.EntityFullName.Replace(",", "_").Replace(".", "_") + "_" + pi.Name;

                        if (pi.Name.ToLower() != "entityfullname" && pi.GetValue(entity, null) != null)
                        {
                            // 此类中包含有非数据库字段，需要进行排除
                            if (isValidateDataField)
                            {
                                if (EntityPropertyAttribute.IsNoDataField(pi)) continue;
                            }

                            where += (Utils.IsNullOrEmpty(where) ? "" : " AND ") + pi.Name + "=" + paraName;
                            cmd.Parameters.AddWithValue(paraName, DataValueString(pi, pi.GetValue(entity, null)));
                        }
                    }

                    commandParameters = new MySqlParameter[cmd.Parameters.Count];
                    // 将cmd的Parameters参数集复制到discoveredParameters数组.
                    cmd.Parameters.CopyTo(commandParameters, 0);

                    return where;
                }
                //return "";
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }

        /// <summary>
        /// 根据传入一参数获取出条件语句.
        /// </summary>
        /// <param name="xmlNode">Xml条件参数节点.</param>
        /// <param name="commandParameters">输出命令参数.</param>
        /// <param name="prefixParameter">The prefix parameter.</param>
        /// <returns></returns>
        public string GetCondition(System.Xml.XmlNode xmlNode, out DbParameter[] commandParameters, string prefixParameter = "")
        {
            using (MySqlCommand cmd = new MySqlCommand())
            {
                string where = "";
                if (xmlNode != null)
                {

                    foreach (System.Xml.XmlNode node in xmlNode.ChildNodes)
                    {
                        string paraName = "@" + prefixParameter + node.LocalName;

                        // 判断是否已经名称相同的参数
                        if (!cmd.Parameters.Contains(paraName))
                        {
                            where += (Utils.IsNullOrEmpty(where) ? "" : " AND ") + node.LocalName + "=" + paraName;
                            cmd.Parameters.AddWithValue(paraName, node.InnerText);
                        }
                    }
                }

                commandParameters = new MySqlParameter[cmd.Parameters.Count];
                // 将cmd的Parameters参数集复制到discoveredParameters数组.
                cmd.Parameters.CopyTo(commandParameters, 0);

                return where;
            }
        }






        #endregion

        #region 日期时间格式化字符串


        string _DateTimeFormatString = "{0:yyyy-MM-dd HH:mm:ss}";
        /// <summary>
        /// DateTimeFormatString
        /// </summary>
        public string DateTimeFormatString
        {
            get
            {
                return _DateTimeFormatString;
            }
            set
            {
                _DateTimeFormatString = value;
            }
        }

        string _DateFormatString = "{0:yyyy-MM-dd}";
        /// <summary>
        /// DateFormatString
        /// </summary>
        public string DateFormatString
        {
            get
            {
                return _DateFormatString;
            }
            set
            {
                _DateFormatString = value;
            }
        }
        #endregion

        /// <summary>
        /// 获取数据库分页Sql模板
        /// </summary>
        public string PagingSql
        {
            get
            {
                return @"declare @PageSize int
declare @PageIndex int
set @PageSize = #PageSize#
set @PageIndex = #PageIndex#
select row_number() over ( order by #SortField# #SortDirect# ) as [SortId], #PrimaryKey#
into #DataSourcePager
from (#sql#) a
select count(1) from #DataSourcePager
select * from #DataSourcePager
where [SortId] between @PageSize * (@PageIndex - 1) + 1 and @PageSize * @PageIndex
order by [SortId] asc
drop table #DataSourcePager
";
            }
        }
    }
}
