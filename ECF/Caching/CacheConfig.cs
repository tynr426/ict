using ECF.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ECF.Caching
{
    /// <summary>
    /// 版权信息：CopyRight (c) 2018 任我行科技研发中心
    /// 文件名称：<see cref="ECF.Caching.CacheConfig"/>
    /// 作者：XP-COMPANY-Shaipe
    /// 日期：2018/10/23
    /// 摘要：系统缓存配置文件
    /// 版本：3.1.9
    /// </summary>
    public class CacheConfig
    {
        const string DEFAULT_CONFIG_PATH = "config\\cache.config";

        #region Instance 获取实例化对象

        private static CacheConfig _instance = null;

        /// <summary>
        /// 
        /// </summary>
        public static CacheConfig Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new CacheConfig();
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

        /// <summary>
        /// 配置文档
        /// </summary>
        private XmlDocument ConfigDocument;

        #region properties
        string _configPath = Utils.RootPath + DEFAULT_CONFIG_PATH;

        /// <summary>
        /// 配置文件的路径
        /// </summary>

        public string ConfigPath
        {
            get { return _configPath; }
            set { _configPath = value; }

        }

        int _Expire = 7 * 24 * 3600;
        /// <summary>
        /// 緩存過期時長.
        /// </summary>
        public int Expire
        {
            get { return _Expire; }
            set { _Expire = value; }
        }

        CacheServerProviderCollection _caches;

        /// <summary>
        /// 配置文件中已配置的所有缓存信息
        /// </summary>
        [XmlAnyElement]
        public CacheServerProviderCollection Caches
        {
            get { return _caches; }
            set { _caches = value; }
        }
        #endregion



        /// <summary>
        /// Initializes a new instance of the <see cref="CacheConfig"/> class.
        /// </summary>
        public CacheConfig()
        {

            if (!File.Exists(ConfigPath))
            {
                CacheConfig.SaveConfig(ConfigPath, new Dictionary<string, object>()
                {
                    {"key","default" },
                    {"alias","缓存数据库" },
                    {"type",1 }
                });
            }
            ConfigDocument = XmlUtils.GetXmlDocument(ConfigPath);
            ConfigParser();

        }

        /// <summary>
        /// 配置文件解析
        /// </summary>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加新方法 added by Shaipe 2018/10/23</description></item>
        ///   </list>
        /// </remarks>
        public void ConfigParser()
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
                                case "expire":
                                    Expire = Utils.ToInt(val);
                                    break;

                                    //    case "encrypteype":
                                    //        EncryptType = val;
                                    //        break;
                                    //    case "secretkey":
                                    //        SecretKey = val;
                                    //        break;
                                    //    case "ismonitor":
                                    //        IsMonitor = Utils.ToBool(val);
                                    //        break;
                            }
                        }
                    }
                }

                if (Caches == null)
                    Caches = new CacheServerProviderCollection();

                // 解析缓存配置
                XmlNode caches = ConfigDocument.SelectSingleNode("//caches");
                if (caches != null)
                {
                    Caches.Clear();
                    for (int i = 0; i < caches.ChildNodes.Count; i++)
                    {
                        XmlNode node = caches.ChildNodes[i];
                        if (node.NodeType == XmlNodeType.Element)
                        {
                            CacheServerProvider provider = new CacheServerProvider(node);
                            if (provider != null)
                            {
                                Caches.Add(provider);
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

        /// <summary>
        /// 保存一配置
        /// </summary>
        /// <param name="configPath">配置路徑.</param>
        /// <param name="dic">The dic.</param>
        /// <returns>
        /// Boolean
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>说明原因 added by Shaipe 2018/10/23</description></item>
        ///   </list>
        /// </remarks>
        public static bool SaveConfig(string configPath, Dictionary<string, object> dic)
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

                #endregion

                #region 数据库连接

                XmlElement dbases = document.CreateElement("caches");
                config.AppendChild(dbases);

                XmlElement dbase = XmlUtils.SetAttributes(document.CreateElement("cache"), dic);
                dbases.AppendChild(dbase);
                #endregion

                string configDir = configPath.Replace(Path.GetFileName(configPath), "");
                if (!Directory.Exists(configDir))
                {
                    Directory.CreateDirectory(configDir);
                }

                document.Save(configPath);

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
