using System;
using System.Collections.Generic;
using System.Collections;

namespace ECF.Data
{
	/// <summary>
	/// FullName： <see cref="ECF.Data.ProviderCollection"/>
	/// Summary ： 数据库连接提供集体 
	/// Version： 1.0.0.0 
	/// DateTime： 2012/4/14 13:56 
	/// CopyRight (c) by shaipe
	/// </summary>
	public class ProviderCollection : IEnumerable<IConnectionProvider>
    {
        /// <summary>
        /// 存放容器
        /// </summary>
        private Dictionary<string, IConnectionProvider> _Dictionary;

        #region 构造函数

        /// <summary>
        /// 构造默认的集合
        /// </summary>
        public ProviderCollection()
        {
            _Dictionary = new Dictionary<string, IConnectionProvider>(StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 构造初始含有一定数量的集合
        /// </summary>
        /// <param name="capacity">初始容量</param>
        public ProviderCollection(int capacity)
        {
            _Dictionary = new Dictionary<string, IConnectionProvider>(capacity, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 构造集合
        /// </summary>
        /// <param name="collection">要复制的集合列表</param>
        public ProviderCollection(IDictionary<string, IConnectionProvider> collection)
        {
            _Dictionary = new Dictionary<string, IConnectionProvider>(collection);
        }

        #endregion

        #region 返回数据

        /// <summary>
        /// 返回某个索引位置的元素
        /// </summary>
        /// <param name="index">索引值</param>
        /// <returns>如果存在此索引位置值,则返回其元素值否则返回null</returns>
        public IConnectionProvider this[int index]
        {
            get
            {
                try
                {
                    if (index >= 0 && index < _Dictionary.Count)
                    {
                        IEnumerator<IConnectionProvider> enumer = _Dictionary.Values.GetEnumerator();
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
                }
                catch (Exception ex)
                {
                    new ECFException(ex);
                }
                return null;
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
        public IConnectionProvider this[string name]
        {
            get
            {
                if (string.IsNullOrEmpty(name))
                    return null;

                if (!name.StartsWith("e_"))
                    name = "e_" + name;

                //if (!this.Contains(name))
                //    throw new ECFException("元素集合中不包含名为\"" + name + "\"的元素");

                IConnectionProvider value = null;

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
        public void Add(IConnectionProvider item)
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
        public IConnectionProvider[] CloneTo()
        {
            try
            {
                IConnectionProvider[] dbps = new ConnectionProvider[this.Count];
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
        internal void Remove(IConnectionProvider elem)
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
        public IEnumerator<IConnectionProvider> GetEnumerator()
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
