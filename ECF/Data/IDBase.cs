namespace ECF.Data
{
	/// <summary>
	/// FullName： <see cref="ECF.Data.IDBase"/>
	/// Summary ： 数据库相关的操作
	/// Version： 1.0
	/// DateTime： 2013/12/2
	/// CopyRight (c) shaipe
	/// </summary>
	public interface IDBase
    {
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        string ConnectionString { get; set; }

        /// <summary>
        /// 测试数据库连接
        /// </summary>
        bool TestConnection(out string msg);

        /// <summary>
        /// 判断数据库是否存在
        /// </summary>
        /// <param name="dbName">数据库名称.</param>
        /// <returns>
        /// System.Boolean
        /// </returns>
        bool ExistDBase(string dbName);

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbName">数据库名称.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        int CreateDBase(string dbName);

        /// <summary>
        /// 创建数据库
        /// </summary>
        /// <param name="dbName">数据库名称.</param>
        /// <param name="dbFileDir">数据库文件存放目录.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        int CreateDBase(string dbName, string dbFileDir);

        /// <summary>
        /// 删除指定的数据库
        /// </summary>
        /// <param name="dbName">数据库名称.</param>
        void DeleteDBase(string dbName);

        /// <summary>
        /// 备份数据
        /// </summary>
        /// <param name="dbName">数据库名称.</param>
        /// <param name="dbFileName">Name of the database file.</param>
        /// <returns>
        /// System.Boolean
        /// </returns>
        bool BackupDataBase(string dbName,string dbFileName);

        /// <summary>
        /// 还原数据库
        /// </summary>
        /// <param name="dbName">数据库名称.</param>
        /// <param name="backFile">The back file.</param>
        /// <returns>
        /// System.Boolean
        /// </returns>
        bool RestoreDataBase(string dbName,string backFile);

        /// <summary>
        /// 收缩数据库
        /// </summary>
        /// <param name="dbName">数据库名称.</param>
        /// <returns>
        /// System.Boolean
        /// </returns>
        bool ShrinkDataBase(string dbName);


        /// <summary>
        /// 在指定的数据库中执行Sql文件
        /// </summary>
        /// <param name="dbName">数据库名称.</param>
        /// <param name="filePath">Sql文件路径.</param>
        /// <param name="isTran">如果sql代码中带有go语句不能使用事务.</param>
        /// <returns>
        /// System.Int32
        /// </returns>
        int ExecuteSqlFile(string dbName, string filePath, bool isTran);

    }
}
