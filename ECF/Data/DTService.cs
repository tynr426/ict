using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MySql.Data.MySqlClient;
using System.Data;
using System.Data.Common;
using Dapper;

namespace ECF.Data
{
    /// <summary>
    /// FullName： <see cref="ECF.Data.MySql.DTService"/>
    /// Summary ： 数据处理服务类
    ///             处理过程将由Begin事务开始,再由提交事务或回滚事务结束整个数据库操作服务
    /// Version： 1.0.0.0 
    /// DateTime： 2012/1/31 20:44 
    /// Author：   XP-PC
    /// </summary>
    public class DTService:IDisposable
    {
        #region Instance And Struct
        private readonly DapperClient _SqlDB;
        public DTService(IDapperFactory dapperFactory)
        {
            _SqlDB = dapperFactory.CreateClient(dapperFactory.DBKey);
        }
        public IDBProvider Provider
        {
            get { return new DBProvider(); }
        }

        #endregion

        #region BeginTransaction 开始处理事务
        IDbTransaction _transaction = null;
        /// <summary>
        /// 开始处理事务
        /// </summary>
        public void BeginTransaction()
        {
            try
            {
                //打开连接
                if (_SqlDB.Connection.State != ConnectionState.Open)
                {
                    _SqlDB.Connection.Open();
                }

                //开始事务
                if (_transaction == null)
                    _transaction = _SqlDB.Connection.BeginTransaction();

            }
            catch (Exception ex)
            {
                Dispose();
                throw ex;
            }
        }

        #endregion

        #region ExecuteNonQuery

        /// <summary>
        /// 执行指定连接字符串,类型的DbCommand.如果没有提供参数,不返回结果.
        /// </summary>
        /// <param name="commandText">存储过程名称或MySql语句.</param>
        /// <returns>
        /// 返回命令影响的行数
        /// </returns>
        public int ExecuteNonQuery(string commandText)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 执行指定连接字符串,类型的DbCommand.如果没有提供参数,不返回结果.
        /// </summary>
        /// <param name="commandText">存储过程名称或MySql语句.</param>
        /// <param name="commandParameters">DbParameter参数数组.</param>
        /// <returns>
        /// 返回命令影响的行数
        /// </returns>
        public int ExecuteNonQuery(string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteNonQuery(CommandType.Text, commandText, commandParameters);
        }

        /// <summary>
        /// 执行指定连接字符串,类型的DbCommand.如果没有提供参数,不返回结果.
        /// </summary>
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">存储过程名称或MySql语句.</param>
        /// <param name="commandParameters">DbParameter参数数组.</param>
        /// <returns>
        /// 返回命令影响的行数
        /// </returns>
        public int ExecuteNonQuery(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            MySqlParameter[] cmdParameters = null;

            if (commandParameters != null) //当参数不为空时进行转换
            {
                //将DbParameter转换为MySqlParameter
                if (commandParameters is MySqlParameter[])
                {
                    cmdParameters = (MySqlParameter[])commandParameters;
                }
                else
                {
                    cmdParameters = ConvertParameters(commandParameters);
                }
            }
            var cmd = new CommandDefinition(commandText, cmdParameters, _transaction, null, commandType);


            return _SqlDB.Connection.Execute(cmd);          

        }

        #endregion

        #region ExecuteScalar

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回结果集中的第一行第一列.
        /// </summary>
        /// <param name="commandText">存储过程名称或MySql语句.</param>
        /// <returns>返回结果集中的第一行第一列</returns>
        public object ExecuteScalar(string commandText)
        {
            return ExecuteScalar(CommandType.Text, commandText, null);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回结果集中的第一行第一列.
        /// </summary>
        /// <param name="commandText">存储过程名称或MySql语句.</param>
        /// <param name="commandParameters">DbParameter参数数组.</param>
        /// <returns>
        /// 返回结果集中的第一行第一列
        /// </returns>
        public object ExecuteScalar(string commandText, params DbParameter[] commandParameters)
        {
            return ExecuteScalar(CommandType.Text, commandText, commandParameters);
        }

        /// <summary>
        /// 执行指定数据库连接字符串的命令,返回结果集中的第一行第一列.
        /// </summary>
        /// <param name="commandType">命令类型 (存储过程,命令文本, 其它.)</param>
        /// <param name="commandText">存储过程名称或MySql语句.</param>
        /// <param name="commandParameters">DbParameter参数数组.</param>
        /// <returns>
        /// 返回结果集中的第一行第一列
        /// </returns>
        public object ExecuteScalar(CommandType commandType, string commandText, params DbParameter[] commandParameters)
        {
            MySqlParameter[] cmdParameters = null;

            if (commandParameters != null) //当参数不为空时进行转换
            {
                //将DbParameter转换为MySqlParameter
                if (commandParameters is MySqlParameter[])
                {
                    cmdParameters = (MySqlParameter[])commandParameters;
                }
                else
                {
                    cmdParameters = ConvertParameters(commandParameters);
                }
            }
            var cmd = new CommandDefinition(commandText, cmdParameters, _transaction,null, commandType);
        
           
            return _SqlDB.Connection.ExecuteScalar(cmd);
         

        }

        #endregion

        #region CommitTransaction 提交事务
        /// <summary>
        /// 提交事务
        /// </summary>
        public bool CommitTransaction()
        {
            try
            {
                //提交事务
                if (!Utils.IsNullOrEmpty(_transaction.Connection))
                {
                    _transaction.Commit();
                }
                return true;
            }
            catch (Exception ex)
            {
                if (_transaction != null)
                {
                    //回滚事务
                    _transaction.Rollback();
                }
                
                throw ex;
            }
            finally
            {
                Dispose();
            }

        }

        #endregion

        #region RollbackTransaction 回滚事务
        /// <summary>
        /// 回滚事务
        /// </summary>
        public void RollbackTransaction()
        {
            try
            {
                if (_transaction != null && _SqlDB !=null && !Utils.IsNullOrEmpty(_transaction.Connection))
                {
                    // 判断连接为打开状态才回滚
                    if ( _SqlDB.Connection.State == ConnectionState.Open)
                    {
                        //回滚事务
                        _transaction.Rollback();
                    }                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                Dispose();
            }
        }
        #endregion

        #region 私有方法

        /// <summary>
		/// 将DbParameter转换为SqlParameter.
		/// </summary>
		/// <param name="commandParameters">The command parameters.</param>
		/// <returns></returns>
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
    
        #endregion

        #region Dispose
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            //释放事务
            if (_transaction != null)
            {
                _transaction.Dispose();
                _transaction = null;
            }

            //释放非托管资源
            if (_SqlDB != null)
            {
                _SqlDB.Connection.Close();
                _SqlDB.Connection.Dispose();
            }
        }
        #endregion


    }
}
