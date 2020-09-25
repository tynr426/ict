using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace ECF.Data
{
    /// <summary>
    /// 数据库参数
    /// </summary>
    [Serializable]
    public class ConditionParameter
    {
        //@name=value
        /// <summary>
        /// 参数名.
        /// </summary>
        public string ParameterName { get; set; }

        /// <summary>
        /// 字段名.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// 参数值.
        /// </summary>
        public string ParameterValue { get; set; }

        /// <summary>
        /// 操作符.
        /// </summary>
        public string Operate { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionParameter"/> class.
        /// </summary>
        public ConditionParameter()
        { }


        /// <summary>
        /// 条件参数构造化方法.
        /// </summary>
        /// <param name="fieldName">字段名.</param>
        /// <param name="fieldValue">字段值.</param>
        /// <param name="operate">操作符.</param>
        public ConditionParameter(string fieldName, string fieldValue, string operate = "=")
        {
            this.FieldName = fieldName;
            this.ParameterName = "@" + fieldName;
            this.ParameterValue = fieldValue;
            this.Operate = operate;
        }
    }

    /// <summary>
    /// 数据库提供者接口
    /// </summary>
    public interface IDBProvider
    {

        /// <summary>
        /// 返回DbProviderFactory实例
        /// </summary>
        /// <returns></returns>
        DbProviderFactory FactoryInstance();

        /// <summary>
        /// 检索SQL参数信息并填充
        /// </summary>
        /// <param name="cmd"></param>
        void DeriveParameters(IDbCommand cmd);

        #region 创建参数
        /// <summary>
        /// 创建SQL参数
        /// </summary>
        /// <param name="ParamName"></param>
        /// <param name="DbType"></param>
        /// <param name="Size"></param>
        /// <returns></returns>
        DbParameter MakeParameter(string ParamName, DbType DbType, Int32 Size);

        /// <summary>
        /// 创建Sql参数.
        /// </summary>
        /// <param name="paramName">参数名称.</param>
        /// <param name="paramValue">参数值.</param>
        /// <returns></returns>
        DbParameter MakeParameter(string paramName, object paramValue);

        /// <summary>
        /// 创建一个Sql参数数组.
        /// </summary>
        /// <param name="length">数组的长度.</param>
        /// <returns></returns>
        DbParameter[] MakeParameters(int length);

        /// <summary>
        /// 创建一个Sql参数数组.
        /// </summary>
        /// <param name="dic">根据键值获取Sql参数数组.</param>
        /// <returns></returns>
        DbParameter[] MakeParameters(Dictionary<string, object> dic);

        #endregion

        #region CovertType 将.net系统类型转换为数据库类型
        /// <summary>
        /// 将.net系统类型转换为数据库类型.
        /// </summary>
        /// <param name="type">系统类型.</param>
        /// <returns></returns>
        DbType CovertType(Type type);
        #endregion

        #region 属性处理

        /// <summary>
        /// 是否支持全文搜索
        /// </summary>
        /// <returns></returns>
        bool IsFullTextSearchEnabled { get; }

        /// <summary>
        /// 是否支持压缩数据库
        /// </summary>
        /// <returns></returns>
        bool IsCompactDatabase { get; }

        /// <summary>
        /// 是否支持备份数据库
        /// </summary>
        /// <returns></returns>
        bool IsBackupDatabase { get; }

        /// <summary>
        /// 返回刚插入记录的自增ID值, 如不支持则为""
        /// </summary>
        /// <returns></returns>
        string IdentitySql { get; }

        /// <summary>
        /// 是否支持数据库优化
        /// </summary>
        /// <returns></returns>
        bool IsDbOptimize { get; }

        /// <summary>
        /// 是否支持数据库收缩
        /// </summary>
        /// <returns></returns>
        bool IsShrinkData { get; }

        /// <summary>
        /// 是否支持存储过程
        /// </summary>
        /// <returns></returns>
        bool IsStoreProc { get; }

        /// <summary>
        /// 随机读取数据库记录
        /// 因为每一种数据库获取随机数是写法不一样
        /// </summary>
        string RndOrder { get; }

        /// <summary>
        /// 指定该数据库是否支持事物.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if this instance is transaction; otherwise, <c>false</c>.
        /// </value>
        bool IsTransaction { get; }
        #endregion

        #region InsertCommandText 获取数据库插入语句

        /// <summary>
        /// 根据实体和表名获取
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">数据库表名.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string InsertCommandText(IEntity entity, string tableName);

        /// <summary>
        /// 根据实体和表名获取
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="commandParameters">输出参数.</param>
        /// <param name="prefixParameter">参数前缀.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string InsertCommandText(IEntity entity, string tableName, out DbParameter[] commandParameters, string prefixParameter = "");

        #endregion

        #region UpdateCommandText 获取待更新的数据库查询语句
        /// <summary>
        /// 获取待更新的数据库查询语句.
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="fields">条件字段.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string UpdateCommandText(IEntity entity, string tableName, string[] fields);

        /// <summary>
        /// 获取待更新的数据库查询语句.
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="fields">条件字段.</param>
        /// <param name="commandParameters">输出参数.</param>
        /// <param name="prefixParameter">参数前缀.</param>
        /// <returns></returns>
        string UpdateCommandText(IEntity entity, string tableName, string[] fields, out DbParameter[] commandParameters, string prefixParameter = "");

        #endregion

        #region UpsertCommandText
        /// <summary>
        /// 获取插入更新的数据库查询语句.
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="fields">条件字段.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string UpsertCommandText(IEntity entity, string tableName, string[] fields);

        /// <summary>
        /// 获取待更新和插入的数据库查询语句.
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="fields">条件字段.</param>
        /// <param name="commandParameters">输出参数.</param>
        /// <param name="prefixParameter">参数前缀.</param>
        /// <returns></returns>
        string UpsertCommandText(IEntity entity, string tableName, string[] fields, out DbParameter[] commandParameters, string prefixParameter = "");

        #endregion

        #region DeleteCommandText 获取删除数据库语句
        /// <summary>
        /// 获取删除数据库语句
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">数据库表名.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string DeleteCommandText(IEntity entity, string tableName);

        /// <summary>
        /// 获取删除数据库语句
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="tableName">数据库表名.</param>
        /// <param name="commandParameters">输出参数.</param>
        /// <param name="prefixParameter">参数前缀.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string DeleteCommandText(IEntity entity, string tableName, out DbParameter[] commandParameters, string prefixParameter = "");

        #endregion

        #region QueryCommandText 获取查询语句

        /// <summary>
        ///  根据条件获取查询语句.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  04-02-2014
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="condition">条件.</param>
        /// <returns>System.String.</returns>
        string QueryCommandText(string tableName, string condition);

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
        string QueryCommandText(int rows, string tableName, string condition, string orderBy);

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
        string QueryCommandText(int rows, string fieldList, string tableName, string condition, string orderBy);

        /// <summary>
        ///  根据条件获取查询语句.
        /// Author :   XP-PC/Shaipe
        /// Created:  04-02-2014
        /// </summary>
        /// <param name="rows">The rows.</param>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="fields">The fields.</param>
        /// <param name="conditions">The conditions.</param>
        /// <param name="orderBy">The order by.</param>
        /// <param name="paras">The paras.</param>
        /// <returns>System.String.</returns>
        string QueryCommandText(int rows, string tableName, string fields, ConditionParameter[] conditions, string orderBy, out DbParameter[] paras);


        /// <summary>
        ///  根据条件获取查询语句.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  04-02-2014
        /// </summary>
        /// <param name="tableName">表名.</param>
        /// <param name="condition">条件.</param>
        /// <param name="paras">返回参数.</param>
        /// <param name="orderBy">排序条件.</param>
        /// <param name="fields">获取字段.</param>
        /// <param name="rows">获取行数.</param>
        /// <returns>System.String.</returns>
        string QueryCommandText(string tableName, Dictionary<string, object> condition, out DbParameter[] paras, string orderBy = "", string[] fields = null, int rows = 0);

        #endregion

        #region GetCondition 获取条件
        /// <summary>
        /// 根据传入一参数获取出条件语句
        /// </summary>
        /// <param name="paras">数据库参数组.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string GetCondition(DbParameter[] paras);

        /// <summary>
        /// 获取查询条件.
        /// </summary>
        /// <param name="list">查询条件参数组.</param>
        /// <param name="commandParameters">输出数据库命令参数组.</param>
        /// <returns></returns>
        string GetCondition(ConditionParameter[] list, out DbParameter[] commandParameters);

        /// <summary>
        /// 根据传入一参数获取出条件语句
        /// </summary>
        /// <param name="entity">实体.</param>
        /// <param name="commandParameters">输出数据库参数组.</param>
        /// <param name="prefixParameter">参数前缀.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string GetCondition(IEntity entity, out DbParameter[] commandParameters, string prefixParameter = "");

        /// <summary>
        /// 根据传入一参数获取出条件语句
        /// </summary>
        /// <param name="dic">数据字典对.</param>
        /// <param name="commandParameters">输出数据库参数组.</param>
        /// <param name="prefixParameter">参数前缀.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string GetCondition(Dictionary<string, object> dic, out DbParameter[] commandParameters, string prefixParameter = "");

        /// <summary>
        /// 根据传入一参数获取出条件语句.
        /// </summary>
        /// <param name="xmlNode">Xml条件参数节点.</param>
        /// <param name="commandParameters">输出命令参数.</param>
        /// <param name="prefixParameter">参数前缀.</param>
        /// <returns></returns>
        string GetCondition(System.Xml.XmlNode xmlNode, out DbParameter[] commandParameters, string prefixParameter = "");
        #endregion

        #region DateFormatString 日期时间格式配置
        /// <summary>
        /// 日期时间格式字符串
        /// </summary>
        string DateTimeFormatString { get; set; }

        /// <summary>
        /// 日期格式字符串
        /// </summary>
        string DateFormatString { get; set; }

        #endregion

        /// <summary>
        /// 数据库分页Sql
        /// </summary>
        string PagingSql { get; }

        #region 数据库相关的处理Sql语句
        ///// <summary>
        ///// 收缩数据库语句
        ///// </summary>
        //string ShrinkDataBase { get; }

        ///// <summary>
        ///// 查看数据库文件及相关信息
        ///// </summary>
        //string DataBaseFile { get; }

        ///// <summary>
        ///// 清空数据库日志
        ///// </summary>
        //string ClearLog { get; }

        ///// <summary>
        ///// 备份数据库语句
        ///// </summary>
        //string BackupDataBase { get; }

        ///// <summary>
        ///// 还原数据库语句
        ///// </summary>
        //string RestoreDatabase { get; }
        #endregion
    }
}
