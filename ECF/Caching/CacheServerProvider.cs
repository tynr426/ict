using ECF.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ECF.Caching
{
    /// <summary>
    /// 版权信息：CopyRight (c) 2018 任我行科技研发中心
    /// 文件名称：<see cref="ECF.Caching.CacheServerProvider"/>
    /// 作者：XP-COMPANY-Shaipe
    /// 日期：2018/10/18
    /// 摘要：缓存提供者
    /// 版本：3.1.9
    /// </summary>
    public class CacheServerProvider
    {
        #region properties
        /// <summary>
        /// 连接关键字
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 连接别名
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// 服务
        /// </summary>
        public string Server { get; set; }

        /// <summary>
        /// 数据 库类型
        /// </summary>
        public CacheType Type { get; set; }

        /// <summary>
        /// 连接密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 数据库名
        /// </summary>
        public string DBName { get; set; }

        /// <summary>
        /// 编码方式
        /// </summary>
        public string Charset { get; set; }

        /// <summary>
        /// 缓存前缀.
        /// </summary>
        public string Prefix { get; set; }

        /// <summary>
        /// 是否为Saas模式.
        /// </summary>
        public bool IsSaas { get; set; }

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheServerProvider"/> class.
        /// </summary>
        /// <param name="xNode">The x node.</param>
        public CacheServerProvider(XmlNode xNode)
        {
            Parse(xNode);
        }

        void Parse(XmlNode xmlNode)
        {
            try
            {
                Key = XmlUtils.GetXmlNodeAttribute(xmlNode, "key");
                Server = XmlUtils.GetXmlNodeAttribute(xmlNode, "server");
                //UserName = XmlUtils.GetXmlNodeAttribute(xNode, "username");
                Password = XmlUtils.GetXmlNodeAttribute(xmlNode, "password");
                DBName = XmlUtils.GetXmlNodeAttribute(xmlNode, "dbname");
                Alias = XmlUtils.GetXmlNodeAttribute(xmlNode, "alias");
                Charset = XmlUtils.GetXmlNodeAttribute(xmlNode, "charset");
                // 是否为Saas
                IsSaas = Utils.ToBool(XmlUtils.GetXmlNodeAttribute(xmlNode, "saas"));
                // 获取缓存配置
                Type = CacheType.None;
                int type = Utils.ToInt(XmlUtils.GetXmlNodeAttribute(xmlNode, "type"));
                switch (type)
                {
                    case 1:
                        Type = CacheType.Memory;
                        break;
                    case 2:
                        Type = CacheType.Redis;
                        break;
                    case 3:
                        Type = CacheType.Memcached;
                        break;
                    default:
                        Type = CacheType.None;
                        break;
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }
    }


    /// <summary>
	/// FullName： <see cref="ECF.Data.ProviderCollection"/>
	/// Summary ： 数据库连接提供集体 
	/// Version： 1.0.0.0 
	/// DateTime： 2012/4/14 13:56 
	/// CopyRight (c) by shaipe
	/// </summary>
	public class CacheServerProviderCollection : IEnumerable<CacheServerProvider>
    {
        /// <summary>
        /// 存放容器
        /// </summary>
        private Dictionary<string, CacheServerProvider> _Dictionary;

        #region 构造函数

        /// <summary>
        /// 构造默认的集合
        /// </summary>
        public CacheServerProviderCollection()
        {
            _Dictionary = new Dictionary<string, CacheServerProvider>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 构造初始含有一定数量的集合
        /// </summary>
        /// <param name="capacity">初始容量</param>
        public CacheServerProviderCollection(int capacity)
        {
            _Dictionary = new Dictionary<string, CacheServerProvider>(capacity, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 构造集合
        /// </summary>
        /// <param name="collection">要复制的集合列表</param>
        public CacheServerProviderCollection(IDictionary<string, CacheServerProvider> collection)
        {
            _Dictionary = new Dictionary<string, CacheServerProvider>(collection);
        }

        #endregion

        #region 返回数据

        /// <summary>
        /// 返回某个索引位置的元素
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>如果存在此索引位置值,则返回其元素值否则返回null</returns>
        public CacheServerProvider this[int index]
        {
            get
            {
                try
                {
                    if (index >= 0 && index < _Dictionary.Count)
                    {
                        IEnumerator<CacheServerProvider> enumer = _Dictionary.Values.GetEnumerator();
                        int i = 0;

                        while (enumer.MoveNext())
                        {
                            if (i == index)
                            {
                                return enumer.Current;
                            }

                            i++;
                        }
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    new ECFException(ex);
                    return null;
                }
            }
            set
            {
                this[index] = value;
            }
        }

        /// <summary>
        /// 返回某个名称的元素
        /// </summary>
        /// <param name="name">元素名称</param>
        /// <returns>如果存在此量,则返回其量否则返回null</returns>
        public CacheServerProvider this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                if (!name.StartsWith("e_"))
                    name = "e_" + name;
                
                CacheServerProvider value = null;

                bool success = _Dictionary.TryGetValue(name, out value);

                if (!success)
                    value = null;

                return value;
            }
            set
            {
                this[name] = value;
            }
        }

        #endregion

        /// <summary>
        /// 添加某个元素
        /// </summary>
        /// <param name="item">元素</param>
        public void Add(CacheServerProvider item)
        {
            if (item == null)
                return;

            try
            {
                string elemName = item.Key;// + item.ExecutionType

                if (!elemName.StartsWith("e_"))
                    elemName = "e_" + elemName;

                // 判断是否存在某个值,如果存在则更新
                if (Contains(elemName))
                {
                    _Dictionary[elemName] = item;
                }
                else
                {
                    _Dictionary.Add(elemName, item);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }

        /// <summary>
        /// 清空所有属性值
        /// </summary>
        internal void Clear()
        {
            if (_Dictionary !=null)
                _Dictionary.Clear();
        }

        /// <summary>
        /// 判断是否存在某个属性
        /// </summary>
        /// <param name="name">要判断的元素名称</param>
        /// <returns>存在则返回true否则返回false</returns>
        public bool Contains(string name)
        {
            if (!name.StartsWith("e_"))
                name = "e_" + name;

            return _Dictionary.ContainsKey(name);
        }

        /// <summary>
        /// 返回属性数目
        /// </summary>
        public int Count
        {
            get
            {
                return _Dictionary.Count;
            }
        }

        /// <summary>
        /// 将集合转换为数组.
        /// </summary>
        /// <returns></returns>
        public CacheServerProvider[] CloneTo()
        {
            try
            {
                CacheServerProvider[] dbps = new CacheServerProvider[this.Count];
                for (int i = 0; i < this.Count; i++)
                {
                    dbps[i] = this[i];
                }
                return dbps;
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return null;
            }
        }

        /// <summary>
        /// 从列表中删除元素.
        /// </summary>
        /// <param name="elem">模板元素.</param>
        internal void Remove(CacheServerProvider elem)
        {
            string elemName = elem.Key;

            if (!elemName.StartsWith("e_"))
                elemName = "e_" + elemName;

            if (this.Contains(elemName))
                _Dictionary.Remove(elemName);
        }

        #region IEnumerable<ConnectionProvider> 成员

        /// <summary>
        /// 返回当前对象的迭代器
        /// </summary>
        /// <returns></returns>
        public IEnumerator<CacheServerProvider> GetEnumerator()
        {
            return this._Dictionary.Values.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        /// <summary>
        /// 返回当前对象的迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this._Dictionary.Values.GetEnumerator();
        }

        #endregion
    }
}
