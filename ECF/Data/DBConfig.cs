using System;
using System.IO;
using System.Xml;
using ECF.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace ECF.Data
{
    /// <summary>
    ///   <see cref="ECF.Data.DBConfig"/>
    /// DB config 数据库连接配置类
    /// Author:  XP
    /// Created: 2011/9/1
    /// </summary>
    [Serializable]
    [XmlRoot("configuration", Namespace = "", IsNullable = false)]
    public class DBConfig
    {
        #region 静态化方法

        #region Instance 获取实例化对象

        private static DBConfig _instance = null;

        /// <summary>
        /// Instance
        /// </summary>
        public static DBConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DBConfig();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 清除DBConfig的缓存
        /// </summary>
        public static void Clear()
        {
            _instance = null;
        }
        #endregion

        #endregion

        const string DEFAULT_CONFIG_PATH = "Config\\DB.Config";

        #region 构造化

        /// <summary>
        /// Initializes a new instance of the <see cref="DBConfig"/> class.
        /// </summary>
        public DBConfig()
        {
            //默认情况下对数据库的配置不进行加密
            IsEncrypt = false;

            //开始初始化数据库配置
            Initialize();
        }

        /// <summary>
        /// 指定配置文件路径的构造化.
        /// </summary>
        /// <param name="filePath">文件路径.</param>
        public DBConfig(string filePath)
        {
            if (File.Exists(filePath))
            {
                ConfigPath = filePath;
            }
            Initialize();
        }


        #endregion

        #region Property

        /// <summary>
        /// 配置文档
        /// </summary>
        private XmlDocument ConfigDocument;

        #region ConfigPath

        string _configPath = Utils.RootPath + DEFAULT_CONFIG_PATH;

        /// <summary>
        /// 配置文件的路径
        /// </summary>

        public string ConfigPath
        {
            get { return _configPath; }
            set { _configPath = value; }

        }

        #region 数据库配置的加密配置

        /// <summary>
        /// 是否加密的公共配置
        /// </summary>

        public bool IsEncrypt { get; set; }

        /// <summary>
        /// 加密类型
        /// </summary>
        public string EncryptType { get; set; }

        /// <summary>
        /// 密钥.
        /// </summary>
        public string SecretKey { get; set; }
        
        /// <summary>
        /// 是否开启数据访问监控.
        /// </summary>
        public bool IsMonitor { get; set; }
        
        #endregion

        #region Connections 配置文件中已配置的所有数据库连接

        ProviderCollection _connections;

        /// <summary>
        /// 配置文件中已配置的所有数据库连接
        /// </summary>
        [XmlAnyElement]
        public ProviderCollection Connections
        {
            get { return _connections; }
            set { _connections = value; }
        }
        
        #endregion


        #region PagingSql 获取分页脚本

        string _PagingSql;

        /// <summary>
        /// 获取分页脚本
        /// </summary>
        public string PagingSql
        {
            get { return _PagingSql; }
            set { _PagingSql = value; }
        }
        #endregion

        #endregion

        #endregion

        #region private

        /// <summary>
        /// 数据库连接配置初始化
        /// by XP-PC 2012/4/14
        /// </summary>
        private void Initialize()
        {
            ConfigDocument = XmlUtils.GetXmlDocument(ConfigPath);

            if (ConfigDocument != null)
            {
                Parser();
            }

        }

        /// <summary>
        /// 数据库连接解析
        /// </summary>
        private void Parser()
        {
            try
            {
                //解析基本设置
                XmlNode setting = ConfigDocument.SelectSingleNode("//settings");
                if (setting != null)
                {
                    foreach (XmlNode node in setting.ChildNodes)
                    {
                        if (node.NodeType == XmlNodeType.Element)
                        {
                            string key = XmlUtils.GetXmlNodeAttribute(node, "key");
                            string val = XmlUtils.GetXmlNodeAttribute(node, "value");
                            switch (key.ToLower())
                            {
                                case "encrypt":
                                    IsEncrypt = Utils.ToBool(val);
                                    break;

                                case "encrypteype":
                                    EncryptType = val;
                                    break;
                                case "secretkey":
                                    SecretKey = val;
                                    break;
                                case "ismonitor":
                                    IsMonitor = Utils.ToBool(val);
                                    break;
                            }
                        }
                    }
                }


                if (Connections == null)
                    Connections = new ProviderCollection();

                //解析所有的数据库连接
                XmlNode dbases = ConfigDocument.SelectSingleNode("//dbases");
                if (dbases != null)
                {
                    Connections.Clear();
                    for (int n = 0; n < dbases.ChildNodes.Count; n++)
                    {
                        XmlNode node = dbases.ChildNodes[n];
                        if (node.NodeType == XmlNodeType.Element)
                        {
                            IConnectionProvider provider = new ConnectionProvider(node);
                            if (provider != null)
                            {
                                Connections.Add(provider);
                            }
                            else
                            {
                                throw new ECFException("获取数据库连接提供者错误");
                            }
                        }
                    }
                }               
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }
     
        #endregion

        /// <summary>
        /// 配置是否存在.
        /// </summary>
        /// <value><c>true</c> if exists; otherwise, <c>false</c>.</value>
        public static bool Exists
        {
            get
            {
                return File.Exists(Utils.RootPath + DEFAULT_CONFIG_PATH);
            }
        }

        /// <summary>
        /// 创建数据库配置
        /// </summary>
        /// <param name="configDir">数据库配置存放目录.</param>
        /// <param name="key">配置关键字.</param>
        /// <param name="dbServer">数据库服务器.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="dbType">Type of the database.</param>
        /// <param name="alias">The alias.</param>
        /// <returns>
        /// System.Boolean
        /// </returns>
        public static bool CreateConfig(string configDir, string key, string dbServer, string userName, string password, string dbName, string dbType = "MySql", string alias = "")
        {
            Dictionary<string, object> dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                {"key", key }
                ,{"alias", string.IsNullOrEmpty(alias) ? key : alias }
                ,{"dbtype", dbType }
                ,{"dbserver", dbServer }
                ,{"username", userName }
                ,{"password", password }
                ,{"dbname", dbName }
                ,{"encrypt", "False" }
                ,{"charset", "utf8" }
            };

            return CreateConfig(configDir, dic);

        }
        /// <summary>
        /// 保存dbconfig
        /// </summary>
        /// <param name="configDir">数据库配置存放目录.</param>
        /// <param name="key">配置关键字.</param>
        /// <param name="dbServer">数据库服务器.</param>
        /// <param name="userName">Name of the user.</param>
        /// <param name="password">The password.</param>
        /// <param name="dbName">Name of the database.</param>
        /// <param name="dbType">Type of the database.</param>
        /// <param name="alias">The alias.</param>
        /// <param name="dbid">数据库id</param>
        /// <returns>
        /// System.Boolean
        /// </returns>
        public bool SaveDBConfig(string configDir, string key, string dbServer, string userName, string password, string dbName, string dbType, string alias, int dbid)
        {
            try
            {
                Dictionary<string, object> dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                {"key", key }
                ,{"alias", string.IsNullOrEmpty(alias) ? key : alias }
                ,{"dbtype", dbType }
                ,{"dbserver", dbServer }
                ,{"username", userName }
                ,{"password", password }
                ,{"dbname", dbName }
                ,{"encrypt", "False" }
                ,{"charset", "utf8" }
            };
                if (dbid > 0) dic.Add("dbid", dbid);
                //解析所有的数据库连接
                XmlNode dbases = ConfigDocument.SelectSingleNode("//dbases");
                if (dbases != null)
                {

                    XmlElement dbase = XmlUtils.SetAttributes(ConfigDocument.CreateElement("add"), dic);
                    dbases.InnerXml = "";
                    dbases.AppendChild(dbase);
                }
                ConfigDocument.Save(configDir + "\\db.config");
                return true;
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return false;
            }
        }

        /// <summary>
        /// 保存数据库连接配置
        /// </summary>
        public static bool CreateConfig(string configDir, Dictionary<string, object> dic)
        {

            try
            {
                XmlDocument document = new XmlDocument();
                XmlDeclaration decl = document.CreateXmlDeclaration("1.0", "UTF-8", "yes");
                document.AppendChild(decl);

                XmlElement config = document.CreateElement("configuration");
                document.AppendChild(config);

                #region Settings
                XmlElement settings = document.CreateElement("settings");
                config.AppendChild(settings);

                //XmlElement key = document.CreateElement("add");
                //key.SetAttribute("key", "secretKey");
                //key.SetAttribute("value", "shaipe");
                //settings.AppendChild(key);

                XmlElement moitor = document.CreateElement("add");
                moitor.SetAttribute("key", "isMonitor");
                moitor.SetAttribute("value", "false");
                settings.AppendChild(moitor);
                #endregion

                #region 数据库连接

                XmlElement dbases = document.CreateElement("dbases");
                config.AppendChild(dbases);

                XmlElement dbase = XmlUtils.SetAttributes(document.CreateElement("add"), dic);
                dbases.AppendChild(dbase);
                #endregion

                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                document.Save(configDir + "\\db.config");

                return true;
            }
            catch (Exception ex)
            {
                new ECFException(ex.Message, ex);
                return false;
            }

        }


    }
}
