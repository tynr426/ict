using System;
using System.Xml;
using ECF.Xml;
using System.Xml.Serialization;
using System.IO;
using ECF.Caching;

namespace ECF.Data
{
    /// <summary>
    ///   <see cref="ECF.Data.ConnectionProvider"/>
    /// 数据库连接提供者
    /// Author:  XP
    /// Created: 2011/9/15
    /// </summary>
    [Serializable]
    [XmlRoot("dbase")]
    public  class ConnectionProvider : IConnectionProvider
    {
        #region Property
        /// <summary>
        /// SqlServer数据库连接驱动
        /// </summary>
        const string SqlServerDriver = "Data Source={0};initial catalog={1};uid={2};pwd={3};";

        /// <summary>
        /// MySql数据库连接驱动
        /// </summary>
        const string MySqlDriver = "Server={0};Database={1};Uid={2};Pwd={3};allow user variables=true;";

        /// <summary>
        /// Sqlite数据库驱动
        /// </summary>
        const string SqliteDriver = "Data Source={0};";

        /// <summary>
        /// Access数据库驱动
        /// </summary>
        const string AccessDriver = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};";

        /// <summary>
        /// 数据库服务器或数据库的物理路径(Access)
        /// </summary>
        public string DBServer { get; set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否采用安全连接
        /// </summary>
        public bool Trusted { get; set; }

        /// <summary>
        /// 数据库id
        /// </summary>
        public int DBId { get; set; }

        /// <summary>
        /// 文件型数据库地址.
        /// </summary>
        public string DataSource { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public DatabaseType DBType { get; set; }

        /// <summary>
        /// 是否加密
        /// </summary>
        public bool Encrypt { get; set; }

        /// <summary>
        /// 密钥
        /// </summary>
        public string SecretKey { get; set; }

        /// <summary>
        /// 加密类型
        /// AES
        /// DES
        /// Base64
        /// </summary>
        public string EncryptType { get; set; }

        /// <summary>
        /// 配置路径.
        /// </summary>
        public string ConfigPath { get; set; }

        /// <summary>
        /// 连接关键字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 连接超时时间
        /// </summary>
        public int TimeOut { get; set; }

        private CacheType queryCacheType = CacheType.None;
        /// <summary>
        /// 数据查询缓存类型.
        /// </summary>
        public CacheType QueryCache { get { return queryCacheType; } set { queryCacheType = value; } }

        /// <summary>
        /// 缓存的数据库连接Key.
        /// </summary>
        public string CacheKey { get; set; }

        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnString { get; set; }


        string _alias = string.Empty;

        /// <summary>
        /// 别名
        /// </summary>
        public string Alias
        {
            get
            {
                return _alias;
            }
            set
            {
                _alias = value;
            }
        }


        string _Charset = string.Empty;

        /// <summary>
        /// 编码方式
        /// </summary>
        public string Charset
        {
            get
            {
                return _Charset;
            }
            set
            {
                _Charset = value;
            }
        }

        /// <summary>
        /// 是否使用连接池,如果使用连接池程序打开数据库后需要把程序关闭或30分钟后数据库自动关闭连接
        /// 设为True则使用完全立即关闭所以连接
        /// </summary>
        public bool NoPooling { get; set; }

        ExecuteType _ExecutionType = ExecuteType.Write;

        /// <summary>
        /// 执行操作类型
        /// </summary>
        public ExecuteType ExecutionType { get { return _ExecutionType; } set { _ExecutionType = value; } }

        #endregion

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// 在没有给定数据库类型时默认返回SqlServer数据库
        /// </summary>
        public ConnectionProvider()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionProvider"/> class.
        /// </summary>
        /// <param name="dbType">Type of the db.</param>
        public ConnectionProvider(DatabaseType dbType)
        {
            DBType = dbType;
        }

        /// <summary>
        /// 根据XmlNode获取数据库连接提供者
        /// </summary>
        /// <param name="xmlNode"></param>
        public ConnectionProvider(XmlNode xmlNode)
        {
            SetProperty(xmlNode);
            Parse();
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="dbType">给定数据库类型.</param>
        /// <param name="connString">数据库连接字符串.</param>
        public ConnectionProvider(DatabaseType dbType, string connString)
        {
            DBType = dbType;
            ConnString = connString;
        }

        #endregion

        #region ToString

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (String.IsNullOrEmpty(ConnString))
            {
                Parse();
            }
            return ConnString;
        }

        #endregion

        #region 私有方法

        /// <summary>
        /// 根据XmlNode设置属性
        /// </summary>
        /// <param name="xNode">XmlNode</param>
        void SetProperty(XmlNode xNode)
        {
            try
            {
                string dtype = XmlUtils.GetXmlNodeAttribute(xNode, "dbtype");
                switch (dtype.ToLower())
                {
                    case "mysql":
                        DBType = DatabaseType.MySql;
                        break;

                    case "access":
                        DBType = DatabaseType.Access;
                        break;
                    //case "excel":
                    //    DBType = DatabaseType.Excel;
                    //    break;
                    case "oracle":
                        DBType = DatabaseType.Oracle;
                        break;

                    case "sqlite":
                        DBType = DatabaseType.SQLite;
                        break;

                    default:
                        DBType = DatabaseType.SqlServer;
                        break;
                }

                Key = XmlUtils.GetXmlNodeAttribute(xNode, "key");
                DBServer = XmlUtils.GetXmlNodeAttribute(xNode, "dbserver");
                Trusted = Utils.ToBool(XmlUtils.GetXmlNodeAttribute(xNode, "trusted"));
                UserName = XmlUtils.GetXmlNodeAttribute(xNode, "username");
                Password = XmlUtils.GetXmlNodeAttribute(xNode, "password");
                DBName = XmlUtils.GetXmlNodeAttribute(xNode, "dbname");
                Alias = XmlUtils.GetXmlNodeAttribute(xNode, "alias");
                NoPooling = Utils.ToBool(XmlUtils.GetXmlNodeAttribute(xNode, "nopooling"));
                DataSource = XmlUtils.GetXmlNodeAttribute(xNode, "datasource"); // 只对文件型数据库有效
                Charset = XmlUtils.GetXmlNodeAttribute(xNode, "charset");
                DBId = Utils.ToInt(XmlUtils.GetXmlNodeAttribute(xNode, "dbid"));
                CacheKey = XmlUtils.GetXmlNodeAttribute(xNode, "cachekey"); // 缓存数据库连接key


                // 想对路径处理
                if (!String.IsNullOrEmpty(DataSource) && DataSource.IndexOf(":") < 1)
                {
                    DataSource = Path.Combine(Utils.RootPath, DataSource);
                }

                if (String.IsNullOrEmpty(XmlUtils.GetXmlNodeAttribute(xNode, "encrypt")))
                {
                    //Encrypt = DBConfig.Instance.IsEncrypt;
                    //EncryptType = DBConfig.Instance.EncryptType;
                    //SecretKey = DBConfig.Instance.SecretKey;
                }
                else
                {
                    Encrypt = Utils.ToBool(XmlUtils.GetXmlNodeAttribute(xNode, "encrypt"));
                    EncryptType = XmlUtils.GetXmlNodeAttribute(xNode, "encrypttype");
                    SecretKey = XmlUtils.GetXmlNodeAttribute(xNode, "secretkey");
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }

        /// <summary>
        /// 根据属性生成数据库连接字符串
        /// </summary>
        public void Parse()
        {
            try
            {
                switch (DBType)
                {
                    case DatabaseType.MySql:    //MySql
                        if (String.IsNullOrEmpty(DBName)) DBName = "MySql";
                        //if (string.IsNullOrEmpty(Charset)) Charset = "uft8";

                        ConnString = String.Format(MySqlDriver, DBServer, DBName, UserName, Password) + (string.IsNullOrEmpty(Charset) ? "" : "Charset=" + Charset + ";") + "Pooling=" + (NoPooling ? "false;" : "true;");

                        break;

                    case DatabaseType.Access: //Access数据库
                        ConnString = String.Format(AccessDriver, DataSource);
                        if (!String.IsNullOrEmpty(Password))
                        {
                            ConnString += "Jet OLEDB:Database Password=" + Password;
                        }
                        break;

                    case DatabaseType.SQLite: // Sqlite数据库
                        ConnString = String.Format(SqliteDriver, DataSource);
                        if (!String.IsNullOrEmpty(UserName) && !String.IsNullOrEmpty(Password))
                        {
                            ConnString += "Jet OLEDB:Database Password=" + Password;
                        }
                        break;

                    case DatabaseType.Redis:
                        ConnString = DBServer;
                        break;

                    default:    //Sql数据库
                        if (String.IsNullOrEmpty(DBName)) DBName = "master";
                        if (Trusted)
                        {
                            ConnString = "integrated security=SSPI;data source=" + DBServer + ";" + "Initial Catalog=" + DBName + ";" + "persist security info=true;" + (NoPooling ? "Pooling=false;" : "");
                        }
                        else
                        {
                            ConnString = String.Format(SqlServerDriver, DBServer, DBName, UserName, Password) + (NoPooling ? "Pooling=false;" : "");
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }


        /// <summary>
        /// 解密字符串
        /// </summary>
        /// <param name="etype">解密类型</param>
        /// <param name="str">需要解密的字符串</param>
        /// <returns>返回解密的数据</returns>
        string DeEnctypt(string etype, string str)
        {
            //switch (etype.ToLower())
            //{
            //    case "des":
            //        return DES.Decode(str);
            //    default:
            //        return AES.Decode(str);
            //}
            return string.Empty;
        }

        #endregion

        /// <summary>
        /// To XML node
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <returns>
        /// XmlNode
        /// </returns>
        public XmlNode ToXmlNode(XmlNode xmlNode)
        {
            xmlNode.Attributes["key"].Value = Key;

            return xmlNode;
        }


    }


    /// <summary>
    ///  返回一个空的链接对象.
    /// </summary>
    [Serializable]
    sealed class EmptyConnectionProvider : ConnectionProvider
    {
    }
}
