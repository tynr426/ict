using Dapper;
using Microsoft.Extensions.Options;
using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace ECF.Data
{
    public class DapperClient
    {
        public ConnectionProvider CurrentConnectionConfig { get; set; }

        public DapperClient(IOptionsMonitor<ConnectionProvider> config)
        {
            CurrentConnectionConfig = config.CurrentValue;
        }

        public DapperClient(ConnectionProvider config) { CurrentConnectionConfig = config; }

        IDbConnection _connection = null;
        public IDbConnection Connection
        {
            get
            {
                switch (CurrentConnectionConfig.DBType)
                {
                    case DatabaseType.MySql:
                        _connection = new MySql.Data.MySqlClient.MySqlConnection(CurrentConnectionConfig.ConnString);
                        break;
                    default:
                        throw new Exception("未指定数据库类型！");
                }
                return _connection;
            }
        }

        /// <summary>
        /// 执行SQL返回集合
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <returns></returns>
        public virtual List<T> Query<T>(string strSql)
        {
            using (IDbConnection conn = Connection)
            {
                return conn.Query<T>(strSql, null).ToList();
            }
        }

        /// <summary>
        /// 执行SQL返回集合
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="obj">参数model</param>
        /// <returns></returns>
        public virtual List<T> Query<T>(string strSql, object param)
        {
            using (IDbConnection conn = Connection)
            {
                return conn.Query<T>(strSql, param).ToList();
            }
        }

        /// <summary>
        /// 执行SQL返回一个对象
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns></returns>
        public virtual T QueryFirst<T>(string strSql)
        {
            using (IDbConnection conn = Connection)
            {
                return conn.Query<T>(strSql).FirstOrDefault<T>();
            }
        }

        /// <summary>
        /// 执行SQL返回一个对象
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <returns></returns>
        public virtual async Task<T> QueryFirstAsync<T>(string strSql)
        {
            using (IDbConnection conn = Connection)
            {
                var res = await conn.QueryAsync<T>(strSql);
                return res.FirstOrDefault<T>();
            }
        }

        /// <summary>
        /// 执行SQL返回一个对象
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="obj">参数model</param>
        /// <returns></returns>
        public virtual T QueryFirst<T>(string strSql, params DbParameter[] commandParameters)
        {
            using (IDbConnection conn = Connection)
            {
                //MySqlParameter[] cmdParameters = null;

                //if (commandParameters != null) //当参数不为空时进行转换
                //{
                //    //将DbParameter转换为MySqlParameter
                //    if (commandParameters is MySqlParameter[])
                //    {
                //        cmdParameters = (MySqlParameter[])commandParameters;
                //    }
                //    else
                //    {
                //        cmdParameters = ConvertParameters(commandParameters);
                //    }
                //}

                return conn.QueryFirst<T>(strSql, ConvertParameters1(commandParameters));
            }
        }

        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="strSql">SQL语句</param>
        /// <param name="param">参数</param>
        /// <returns>0成功，-1执行失败</returns>
        public virtual int Execute(string strSql,params DbParameter[] commandParameters)
        {
            using (IDbConnection conn = Connection)
            {
                try
                {

                    return conn.Execute(strSql, ConvertParameters1(commandParameters)) > 0 ? 0 : -1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        private DynamicParameters ConvertParameters1(DbParameter[] commandParameters)
        {
            try
            {
                if (commandParameters == null)
                {
                    return null;
                }
                DynamicParameters parameter = new DynamicParameters();
                for (int i = 0; i < commandParameters.Length; i++)
                {
                    parameter.Add(commandParameters[i].ParameterName, commandParameters[i].Value);
                }

                return parameter;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private MySqlParameter[] ConvertParameters(DbParameter[] commandParameters)
        {
            try
            {
                if (commandParameters == null)
                {
                    return null;
                }
                MySqlParameter[] _SqlParameters = new MySqlParameter[commandParameters.Length];
                for (int i = 0; i < commandParameters.Length; i++)
                {
                    _SqlParameters[i] = commandParameters[i] as MySqlParameter;
                }
              
                return _SqlParameters;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DataTable GetDataTable(string strSql, DbParameter[] commandParameters)
        {
            using (IDbConnection conn = Connection)
            {
                try
                {
                    DataTable table = new DataTable();
                    
                    var reader=conn.ExecuteReader(strSql, ConvertParameters1(commandParameters));
                    table.Load(reader);
                    return table;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public DataSet GetDataSet(string strSql, DbParameter[] commandParameters)
        {
            using (IDbConnection conn = Connection)
            {
                try
                {
                    DataSet ds = new DataSet();
                    var adapter = new MySqlDataAdapter();
                    var command = new CommandDefinition(strSql, ConvertParameters1(commandParameters), null);
                    //var identity = new Identity(command.CommandText, command.CommandType, conn, null, cmdParameters == null ? null : cmdParameters.GetType(), null);
                   // var info = GetCacheInfo(identity, param, command.AddToCache);
                    //bool wasClosed = conn.State == ConnectionState.Closed;
                    //if (wasClosed) conn.Open();
                    //adapter.SelectCommand = command..SetupCommand(conn, info.ParamReader);
                    adapter.Fill(ds);
                    //if (wasClosed) conn.Close();
                    return ds;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public object GetValue(string strSql, DbParameter[] commandParameters)
        {
            using (IDbConnection conn = Connection)
            {
                try
                {
                    var reader = conn.ExecuteReader(strSql, ConvertParameters1(commandParameters));
                    reader.Read();
                    return reader.GetValue(0);
                   
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="strProcedure">过程名</param>
        /// <returns></returns>
        public virtual int ExecuteStoredProcedure(string strProcedure)
        {
            using (IDbConnection conn = Connection)
            {
                try
                {
                    return conn.Execute(strProcedure, null, null, null, CommandType.StoredProcedure) == 0 ? 0 : -1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="strProcedure">过程名</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public virtual int ExecuteStoredProcedure(string strProcedure, object param)
        {
            using (IDbConnection conn = Connection)
            {
                try
                {
                    return conn.Execute(strProcedure, param, null, null, CommandType.StoredProcedure) == 0 ? 0 : -1;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


    }
}
