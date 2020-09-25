using ECF.Caching;
using System;
namespace ECF.Data
{
    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum DatabaseType
    {
        /// <summary>
        /// Access数据库类型
        /// </summary>
        Access,
        ///// <summary>
        ///// 微软Excel表格
        ///// </summary>
        //Excel,
        /// <summary>
        /// MySql数据库
        /// </summary>
        MySql,

        /// <summary>
        /// 微软SQL数据库类型
        /// </summary>
        SqlServer,

        /// <summary>
        /// 甲古文数据库
        /// </summary>
        Oracle,

        /// <summary>
        /// SQLite数据库
        /// </summary>
        SQLite,

        /// <summary>
        /// 芒果Db
        /// </summary>
        MongoDb,

        /// <summary>
        /// The postgre SQL
        /// </summary>
        PostgreSql,

        /// <summary>
        /// The redis
        /// </summary>
        Redis
    }

    /// <summary>
    /// 数据库链接的执行类型
    /// </summary>
    public enum ExecuteType
    {
        /// <summary>
        /// 混合型
        /// </summary>
        Mix = 0,

        /// <summary>
        /// 写数据
        /// </summary>
        Write = 1,

        /// <summary>
        /// 读数据
        /// </summary>
        Read = 2
    }


    /// <summary>
    /// 数据库连接提供者接口
    /// </summary>
    public interface IConnectionProvider 
    {

        /// <summary>
        /// 连接关键字
        /// </summary>
        string Key { get; set; }

        /// <summary>
        /// 数据库服务器或数据库的物理路径(Access)
        /// </summary>
        string DBServer { get; set; }

        /// <summary>
        /// 数据源，主要用于文件型数据库.
        /// </summary>
        /// <value>The data source.</value>
        string DataSource { get; set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        string DBName { get; set; }

        /// <summary>
        /// 别名
        /// </summary>
        string Alias { get; set; }

        /// <summary>
        /// 字符编码.
        /// </summary>
        string Charset { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        string Password { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        DatabaseType DBType { get; set; }

        /// <summary>
        /// 是否采用安全连接
        /// </summary>
        bool Trusted { get; set; }

        /// <summary>
        /// 数据库id
        /// </summary>
        int DBId { get; set; }

        /// <summary>
        /// 是否加密
        /// </summary>
        bool Encrypt { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        string SecretKey { get; set; }

        /// <summary>
        /// 加密类型
        /// AES
        /// DES
        /// Base64
        /// </summary>
        string EncryptType { get; set; }
        
        /// <summary>
        /// 连接字符串
        /// </summary>
        string ConnString { get; set; }

        /// <summary>
        /// 是否使用连接池,如果使用连接池程序打开数据库后需要把程序关闭或30分钟后数据库自动关闭连接
        /// 设为True则使用完全立即关闭所以连接
        /// </summary>
        bool NoPooling { get; set; }

        /// <summary>
        /// 链接执行类型
        /// </summary>
        ExecuteType ExecutionType { get; set; }

        /// <summary>
        /// 源数据缓存，此处将会在数据处理底层对源数据进行缓存，查询时将数据加入缓存，更新修改时把对应的数据缓存进行清理.
        /// </summary>
        CacheType QueryCache { get; set; }

        /// <summary>
        ///0: 不开启查询缓存(默认值)，1：使用系统Memory作为缓存容器，2： Redis缓存， 3： Memercache(此还为实现）
        /// </summary>
        string CacheKey { get; set; }

        /// <summary>
        /// 重新对赋值进行解析.
        /// </summary>
        void Parse();


    }
}
