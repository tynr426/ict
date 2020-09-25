using Dapper;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ECF.Data
{
    public class DBAccess
    {
        private readonly DapperClient _SqlDB;
        public DBAccess(IDapperFactory dapperFactory)
        {
            _SqlDB = dapperFactory.CreateClient(dapperFactory.DBKey);
        }
        public IDBProvider Provider
        {
            get { return new DBProvider(); }
        }
        public int ExecuteNonQuery(string sql, DbParameter[] parameters = null)
        {
            if (parameters != null && parameters.Length > 0)
            {
                return _SqlDB.Execute(sql, parameters);
            }
            else
            {
                return _SqlDB.Execute(sql);
            }
        }
        /// <summary>
        /// 根据一个已附值的实体向数据表中插入数据，返回影响行数
        /// </summary>
        /// <param name="entity">待插入的已附值的实体</param>
        /// <param name="tableName">待插入的数据表名</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        public int ExecuteInsert(string tableName, IEntity entity)
        {
            DbParameter[] spc;

            //通过公用的方法获取插入语句和相对应的参数
            string cmdText = Provider.InsertCommandText(entity, tableName, out spc);
            //执行数据库服务层的方法把数据插入数据库
            return _SqlDB.Execute(cmdText, spc);
           
            //return Utils.ToInt(DbService.ExecuteScalar(CommandType.Text, cmdText, spc));

        }
        /// <summary>
        /// 根据一个已附值的实体更新数据表中数据，返回影响行数
        /// </summary>
        /// <param name="entity">实体</param>
        /// <param name="tableName">待更新数据库的数据表</param>
        /// <param name="fields">根据相应的条件字段</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        public int ExecuteUpdate(string tableName, IEntity entity, string[] fields)
        {
            DbParameter[] spc;
            try
            {

                //通过公用的方法获取插入语句和相对应的参数
                string cmdText = Provider.UpdateCommandText(entity, tableName, fields, out spc);
                return _SqlDB.Execute(cmdText, spc);
            }
            catch (Exception ex)
            {
                //new DbException(ex.Message, ex);
                return 0;
            }
        }
        /// <summary>
        /// 根据实体和条件进行数据插入如已存在更新反之插入
        /// </summary>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="entity">实体.</param>
        /// <param name="fields">条件字段集.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        public int ExecuteUpsert(string tableName, IEntity entity, string[] fields)
        {
            DbParameter[] spc;

            //通过公用的方法获取插入语句和相对应的参数
            string cmdText = Provider.UpsertCommandText(entity, tableName, fields, out spc);

            return _SqlDB.Execute(cmdText, spc);
        }
        public int ExecuteDeletes(string tableName, string[] ids)
        {
            List<string> ls = new List<string>();
            DbParameter[] dbParameters = new DbParameter[ids.Length];
            for (int i=0;i<ids.Length;i++)
            {
                ls.Add("DELETE FROM " + tableName + " WHERE Id=@id"+i);
                dbParameters[i] = Provider.MakeParameter("id" + i, ids[i]);             

     
            }
            return _SqlDB.Execute(ls.Join(""), dbParameters);
        }
        /// <summary>
        /// 根据id删除表中的记录，这一点必须要是表中有id这样的字段
        /// </summary>
        /// <param name="tableName">需要删除数据的表.</param>
        /// <param name="id">主键.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        public int ExecuteDelete(string tableName, int id)
        {
            string sql = "DELETE FROM " + tableName + " WHERE Id=@Id";
            Dictionary<string, object> dic = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);
            dic.Add("id", id);

            DbParameter[] dbps = Provider.MakeParameters(dic);
            return _SqlDB.Execute(sql, dbps);
        }
        /// <summary>
        /// 根据id删除表中的记录.
        /// </summary>
        /// <param name="tableName">需要删除数据的表.</param>
        /// <param name="condition">条件语句不需要加where.</param>
        /// <returns></returns>
        public int ExecuteDelete(string tableName, string condition)
        {
            return ExecuteDelete(tableName, condition, null);
        }

        /// <summary>
        /// 根据id删除表中的记录.
        /// </summary>
        /// <param name="tableName">需要删除数据的表.</param>
        /// <param name="condition">条件语句不需要加where.</param>
        /// <param name="parameters">数据参数.</param>
        /// <returns></returns>
        public int ExecuteDelete(string tableName, string condition, System.Data.Common.DbParameter[] parameters)
        {
            if (Utils.IsNullOrEmpty(condition))
                return 0;

            string sql = "DELETE FROM " + tableName + " WHERE " + condition;

            if (parameters != null && parameters.Length > 0)
            {
                return _SqlDB.Execute(sql, parameters);                
            }
            else
            {
                return _SqlDB.Execute(sql);
            }
        }
        /// <summary>
        /// 删除数据表中的值
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="paras">数据库执时的Command参数.</param>
        /// <returns>
        /// System.Int32 返回影响行数
        /// </returns>
        public int ExecuteDelete(string tableName, params System.Data.Common.DbParameter[] paras)
        {
            string sql = "DELETE FROM " + tableName + " WHERE ";
            string condition = "";
            foreach (DbParameter dbp in paras)
            {
                condition += Utils.IsNullOrEmpty(condition) ? "" : " and @" + dbp.ParameterName;
            }
            return _SqlDB.Execute(sql, paras);
        }
        #region Exists

        /// <summary>
        /// 判断表中是否有指定条件的记录.
        /// </summary>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="condition">查询条件.</param>
        /// <returns>
        /// true | false
        /// </returns>
        public bool Exists(string tableName, string condition)
        {
            return Exists(tableName, condition, null);
        }

        /// <summary>
        /// 判断表中是否有指定条件的记录.
        /// </summary>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="condition">查询条件.</param>
        /// <param name="parameters">数据参数.</param>
        /// <returns>
        /// true | false
        /// </returns>
        public bool Exists(string tableName, string condition, System.Data.Common.DbParameter[] parameters = null)
        {
            try
            {
                object ds = _SqlDB.GetValue("SELECT COUNT(0) FROM " + tableName + " WHERE " + condition, parameters);

                if (ds != null)
                    return Utils.ToInt(ds) > 0;

                return false;
            }
            catch (MySqlException ex)
            {
                //new DbException(ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// 判断数据表中是否存在该值.
        /// </summary>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="parameters">数据查询参数组.</param>
        /// <returns></returns>
        public bool Exists(string tableName, params ConditionParameter[] parameters)
        {
            try
            {
                DbParameter[] dbParamerters;

                object ds = _SqlDB.GetValue("SELECT COUNT(0) FROM " + tableName + " WHERE " + Provider.GetCondition(parameters, out dbParamerters), dbParamerters);

                
                if (ds != null)
                    return Utils.ToInt(ds) > 0;

                return false;
            }
            catch (MySqlException ex)
            {
                //new DbException(ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// 判断表中是否有指定条件的记录.
        /// </summary>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="paras">需要执行的参数.</param>
        /// <returns>
        /// bool
        /// </returns>
        public bool Exists(string tableName, params DbParameter[] paras)
        {
            string condition = Provider.GetCondition(paras);

            try
            {
                object ds = _SqlDB.GetValue("SELECT COUNT(0) FROM " + tableName + " WHERE " + condition, paras);

                if (ds != null)
                    return Utils.ToInt(ds) > 0;

                return false;
            }
            catch (MySqlException ex)
            {
                //new DbException(ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// 判断表中是否有指定条件的记录.
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="entity">实体.</param>
        /// <returns></returns>
        public bool Exists(string tableName, IEntity entity)
        {
            DbParameter[] spc;

            string condition = Provider.GetCondition(entity, out spc);

            try
            {
                if (spc.Length > 0)
                {
                    object ds = _SqlDB.GetValue("SELECT COUNT(0) FROM " + tableName + " WHERE " + condition, spc);

                    if (ds != null)
                        return Utils.ToInt(ds) > 0;
                }
                return false;
            }
            catch (MySqlException ex)
            {
                //new DbException(ex.Message, ex);
                return false;
            }

        }

        /// <summary>
        /// 判断表中是否有指定条件的记录
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="dic">条件字典.</param>
        /// <returns>
        /// System.Boolean
        /// </returns>
        public bool Exists(string tableName, Dictionary<string, object> dic)
        {
            DbParameter[] spc;

            string condition = Provider.GetCondition(dic, out spc);

            try
            {
                if (spc.Length > 0)
                {
                    object ds = _SqlDB.GetValue("SELECT COUNT(0) FROM " + tableName + " WHERE " + condition, spc);

                    if (ds != null)
                        return Utils.ToInt(ds) > 0;

                }
                return false;
            }
            catch (MySqlException ex)
            {
                //new DbException(ex.Message, ex);
                return false;
            }
        }
        #endregion
        #region GetDataTable 获取数据表中的数据

        /// <summary>
        /// 获取数据表中的数据.
        /// </summary>
        /// <param name="sql">数据库查询脚本.</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql)
        {
            return _SqlDB.GetDataTable(sql, null);
        }

        /// <summary>
        /// 获取数据表中的数据.
        /// </summary>
        /// <param name="sql">数据库查询脚本.</param>
        /// <param name="parameters">查询数据参数.</param>
        /// <returns></returns>
        public DataTable GetDataTable(string sql, DbParameter[] parameters)
        {
            return _SqlDB.GetDataTable(sql, parameters);
        }

        /// <summary>
        /// 获取数据表中的数据.
        /// </summary>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="conditions">查询条件.</param>
        /// <param name="fields">字段列表.</param>
        /// <param name="rows">获取数据行数.</param>
        /// <param name="orderBy">排序方式.</param>
        /// <returns></returns>
        public DataTable GetDataTable(string tableName, ConditionParameter[] conditions, string orderBy, string[] fields, int rows)
        {
            DbParameter[] dps = null;

            string condition = Provider.GetCondition(conditions, out dps);

            string commandText = Provider.QueryCommandText(rows,
                (fields != null && fields.Length > 0 ? string.Join(",", fields) : "*"),
            tableName, condition, orderBy);

            return _SqlDB.GetDataTable(commandText, dps);
        }
        /// <summary>
        /// 获取数据表中的数据.
        /// </summary>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="fields">字段列表.</param>
        /// <param name="condition">The condition.</param>
        /// <returns></returns>
        public DataTable GetDataTable(string tableName, string fields, string condition)
        {
            return GetDataTable(tableName, fields, condition, null, 0, null);
        }
        /// <summary>
        /// 获取数据表中的数据.
        /// </summary>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="fields">字段列表.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="rows">获取数据行数.</param>
        /// <param name="orderBy">排序方式.</param>
        /// <returns></returns>
        public DataTable GetDataTable(string tableName, string fields, string condition, string orderBy, int rows)
        {
            return GetDataTable(tableName, fields, condition, orderBy, rows, null);
        }
        /// <summary>
        /// 获取数据表中的数据.
        /// </summary>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="fields">字段列表.</param>
        /// <param name="condition">The condition.</param>
        /// <param name="rows">获取数据行数.</param>
        /// <param name="orderBy">排序方式.</param>
        /// <param name="parameters">数据查询参数.</param>
        /// <returns></returns>
        public DataTable GetDataTable(string tableName, string fields, string condition, string orderBy, int rows, DbParameter[] parameters)
        {
            string sql = Provider.QueryCommandText(rows, fields, tableName, condition, orderBy);
            return _SqlDB.GetDataTable(sql, parameters);
          
        }

        /// <summary>
		/// 获取数据表中的数据.
		/// </summary>
		/// <param name="tableName">数据库表名.</param>
		/// <param name="conditionDic">查询条件.</param>
		/// <returns></returns>
		public DataTable GetDataTable(string tableName, Dictionary<string, object> conditionDic)
        {

            return GetDataTable(tableName, conditionDic, null);
        }

        /// <summary>
        /// 获取数据表中的数据.
        /// </summary>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="conditionDic">查询条件.</param>
        /// <param name="orderBy">排序方式.</param>
        /// <returns></returns>
        public DataTable GetDataTable(string tableName, Dictionary<string, object> conditionDic, string orderBy)
        {

            return GetDataTable(tableName, conditionDic, orderBy, 0, null);
        }

        /// <summary>
        /// 获取数据表中的数据.
        /// </summary>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="conditionDic">查询条件.</param>
        /// <param name="rows">获取数据行数.</param>
        /// <param name="fields">字段列表.</param>
        /// <param name="orderBy">排序方式.</param>
        /// <returns></returns>
        public DataTable GetDataTable(string tableName, Dictionary<string, object> conditionDic, string orderBy, int rows, string fields)
        {
            DbParameter[] spc;

            string where = Provider.GetCondition(conditionDic, out spc);

            string sql = Provider.QueryCommandText(rows, fields, tableName, where, orderBy);

            return _SqlDB.GetDataTable(sql, spc);
        }

        #endregion
        public DataSet ExecuteDataset(string sql, DbParameter[] parameters = null)
        {
            return _SqlDB.GetDataSet(sql, parameters);
        }

        public Tuple<int, DataTable> GetPageTable(Tuple<string,string> sql, DbParameter[] parameters = null)
        {
            var dt=_SqlDB.GetDataTable(sql.Item2, parameters);
            var count =_SqlDB.GetValue(sql.Item1, parameters);
            return new Tuple<int, DataTable>(Utils.ToInt(count), dt);
        }
        #region GetPageTable 获取一个Sql查询后的分页数据
        /// <summary>
        /// 获取一个sql查询后的分页数据
        /// </summary>
        /// <param name="commandText">Sql查询语句</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pageIndex">当前需要取的页码</param>
        /// <returns></returns>
        public DataTable GetPageTable(string commandText, int pageSize, int pageIndex)
        {
            return new DataTable();
        }

        /// <summary>
        /// Gets the page table.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="pageIndex">Index of the page.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public DataTable GetPageTable(string commandText, int pageSize, int pageIndex, DbParameter[] parameters = null)
        {
            return new DataTable();
        }

        #endregion

        #region GetValue 获取指定字段的值
        /// <summary>
        /// 获取指定字段的值
        /// 只返回查询结果的第一个字段值
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="fieldName">字段名.</param>
        /// <param name="condition">条件.</param>
        /// <returns>
        /// System.Object
        /// </returns>
        public object GetValue(string tableName, string fieldName, string condition)
        {
            return GetValue(tableName, fieldName, condition, null, null);
        }

        /// <summary>
        /// 获取指定字段的值
        /// 只返回查询结果的第一个字段值
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="fieldName">字段名.</param>
        /// <param name="condition">条件.</param>
        /// <param name="orderBy">排序.</param>
        /// <param name="parameters">数据参数.</param>
        /// <returns>
        /// System.Object
        /// </returns>
        public object GetValue(string tableName, string fieldName, string condition, string orderBy, DbParameter[] parameters)
        {
            string sql = Provider.QueryCommandText(1, fieldName, tableName, condition, orderBy);

            return _SqlDB.GetValue(sql, parameters);
        }

        /// <summary>
        /// 获取指定字段的值
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="fieldName">字段名.</param>
        /// <param name="conditionDic">条件.</param>
        /// <param name="orderBy">排序.</param>
        /// <returns>
        /// System.Object
        /// </returns>
        public object GetValue(string tableName, string fieldName, Dictionary<string, object> conditionDic, string orderBy = null)
        {
            DbParameter[] parameters = null;
            string sql = Provider.QueryCommandText(tableName, conditionDic,out parameters, orderBy,new string[] { fieldName},1);

            return _SqlDB.GetValue(sql, parameters);
        }

        /// <summary>
        /// 获取指定字段的值.
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="fieldName">字段名.</param>
        /// <param name="conditions">条件.</param>
        /// <param name="orderBy">排序方式.</param>
        /// <returns></returns>
        public object GetValue(string tableName, string fieldName, ConditionParameter[] conditions, string orderBy = null)
        {
            DbParameter[] parameters = null;
            string sql = Provider.QueryCommandText(1, tableName, fieldName, conditions, orderBy, out parameters);

            return _SqlDB.GetValue(sql, parameters);
        }




        #endregion

        #region GetEntity<T>
        /// <summary>
        /// 获取数据实体.
        /// </summary>
        /// <typeparam name="T">需要获取的实体类型</typeparam>
        /// <param name="sql">查询Sql语句.</param>
        /// <param name="parameters">数据查询条件参数.</param>
        /// <returns></returns>
        public T GetEntity<T>(string sql, DbParameter[] parameters = null) where T : class
        {
            return _SqlDB.Query<T>(sql, parameters).FirstOrDefault();
        }
        /// <summary>
        /// 获取数据实体
        /// </summary>
        /// <typeparam name="T">需要获取的实体类型</typeparam>
        /// <param name="tableName">数据表名.</param>
        /// <param name="conditionDic">数据查询条件.</param>
        /// <returns>
        /// 返回T类型的实体(已附值)
        /// </returns>
        public T GetEntity<T>(string tableName, Dictionary<string, object> conditionDic) where T : class
        {
            DbParameter[] spc;

            string condition = Provider.GetCondition(conditionDic, out spc);

            string sql = Provider.QueryCommandText(tableName, condition);

           
            return _SqlDB.QueryFirst<T>(sql, spc);
        }
        public T GetEntity<T>(string tableName, int id) where T : class
        {
            //DbParameter[] spc=new DbParameter[] { 
            //    Provider.MakeParameter("id",id)
            //};
            DynamicParameters parameters = new DynamicParameters();
            parameters.Add("id", id);
            string condition = "id=@id";

            string sql = Provider.QueryCommandText(tableName, condition);


            return _SqlDB.Query<T>(sql, parameters).FirstOrDefault();
        }
        /// <summary>
        /// 获取数据实体
        /// </summary>
        /// <typeparam name="T">需要获取的实体类型</typeparam>
        /// <param name="tableName">表名.</param>
        /// <param name="condParameter">条件参数.</param>
        /// <returns>
        /// T
        /// </returns>
        public T GetEntity<T>(string tableName, ConditionParameter[] condParameter) where T : class
        {
            DbParameter[] spc = null;

            string condition = Provider.GetCondition(condParameter, out spc);

            string sql = Provider.QueryCommandText(tableName, condition);

            return _SqlDB.QueryFirst<T>(sql, spc);
        }
        #endregion
    }
}
