using System;
using System.Collections;
using System.Reflection;
using System.Xml;
using System.Text;
using System.Data;
using System.Collections.Generic;
using Newtonsoft.Json;
using ECF.Json;
using ECF.Data;
using ECF.Caching;


#if (!WINDOWS)
using System.Runtime.Serialization;
using System.Collections.Specialized;
using ECF.Utilities;
#endif
namespace ECF
{
    /// <summary>
    /// 实体接口,方便后期通过继承类接口而实现的数据实体接口
    /// 系统中所有的实体类都需要强制继续此接口，主要用于后期进行跨模块调用
    /// </summary>
    public interface IEntity
    {

        /// <summary>
        /// 实体的类的全名，用于类型缓存提高反射效率
        /// </summary>
        string EntityFullName { get; }

        #region 给实体附值

        /// <summary>
        /// 给实体附值.
        /// </summary>
        /// <param name="json">json格式数据.</param>
        void SetValues(string json);

        /// <summary>
        /// 给实体附值
        /// </summary>
        /// <param name="nvc">NameValueCollection格式数据.</param>
        void SetValues(NameValueCollection nvc);

        /// <summary>
        /// 给实体附值.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument格式数据.</param>
        void SetValues(XmlDocument xmlDoc);

        /// <summary>
        /// 给实体附值.
        /// </summary>
        /// <param name="xnl">The XNL.</param>
        void SetValues(XmlNodeList xnl);

        /// <summary>
        /// 给实体附值.
        /// </summary>
        /// <param name="ht">Hashtable格式数据.</param>
        void SetValues(Hashtable ht);

        /// <summary>
        /// 给实体附值.
        /// </summary>
        /// <param name="dr">DataRow格式的数据</param>
        void SetValues(DataRow dr);

        /// <summary>
        /// 给实体附值
        /// </summary>
        /// <param name="dic">Dictionary格式数据.</param>
        void SetValues(Dictionary<string, object> dic);

        /// <summary>
        /// 给实体的单个对象赋值
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="value">字段值</param>
        void SetValue(string name, object value);

        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="name">属性名.</param>
        /// <returns>
        /// 返回实体指字属性名的值
        /// </returns>
        object GetValue(string name);


        #endregion

        #region 把实体数据转换为相应格式的字符串

        /// <summary>
        /// 将实体转换成Json格式的数据.
        /// </summary>
        /// <returns></returns>
        string ToJson();

        /// <summary>
        ///  将实体转换成Json格式的数据
        ///  Author :   XP-PC/Shaipe
        ///  Created:  10-17-2014
        /// </summary>
        /// <param name="unicode">是否需要对值进行Unicode编码 .</param>
        /// <returns>System.String.</returns>
        string ToJson(bool unicode);

        /// <summary>
        ///  将实体转换成Json格式的数据.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  2016-06-27
        /// </summary>
        /// <param name="unicode">是否需要对值进行Unicode编码 .</param>
        /// <param name="beNull">是否需要空值字段.</param>
        /// <returns>System.String.</returns>
        string ToJson(bool unicode, bool beNull);

        /// <summary>
        /// 将实体转换成Json格式的数据.
        /// </summary>
        /// <param name="unicode">是否对值进行Unicode编码.</param>
        /// <param name="beNull">是否需要空值字段.</param>
        /// <param name="filter">需要要过滤的字段.</param>
        /// <returns>
        /// System.String.
        /// </returns>
        string ToJson(bool unicode, bool beNull, string[] filter);

        /// <summary>
        ///  将实体转换成Json格式的数据
        ///  Author :   XP-PC/Shaipe
        ///  Created:  10-17-2014
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>System.String.</returns>
        string ToJson(List<string> properties);

        /// <summary>
        ///  将实体转换成Json格式的数据.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  10-17-2014
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="unicode">是否需要对值进行Unicode编码 .</param>
        /// <returns>System.String.</returns>
        string ToJson(List<string> properties, bool unicode);

        /// <summary>
        ///  将实体转换成Json格式的数据.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  10-17-2014
        /// </summary>
        /// <param name="properties">需要的属性字段名.</param>
        /// <param name="unicode">是否需要对值进行Unicode编码 .</param>
        /// <param name="beNull">是否需要空值字段.</param>
        /// <returns>System.String.</returns>
        string ToJson(List<string> properties, bool unicode, bool beNull);

        /// <summary>
        /// 将实体转换成Xml模式的数据.
        /// </summary>
        /// <returns></returns>
        string ToXml();

        /// <summary>
        /// 将实体转换成Xml模式的数据
        /// 添加对List和实体的深入转化处理 by xp 20140611.
        /// </summary>
        /// <param name="filter">需要过滤的字段.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string ToXml(string[] filter);

        /// <summary>
        /// 将实体转换成Xml模式的数据. by xp 20130829
        /// 添加对List和实体的深入转化处理 by xp 20140611.
        /// </summary>
        /// <param name="properties">需要转换的属性列表.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string ToXml(List<string> properties);

        /// <summary>
        /// 将实体数据转换为Hashtable数据进行输出.
        /// </summary>
        /// <returns></returns>
        Hashtable ToHashtable();

        /// <summary>
        /// 将实体数据转换为Dictionary
        /// </summary>
        /// <returns>
        /// Dictionary&lt;System.String, System.Object&gt;
        /// </returns>
        Dictionary<string, object> ToDictionary();

        /// <summary>
        /// 将实体数据转换为Dictionary
        /// </summary>
        /// <param name="filter">需要过滤的字段，这里的字段不会被转出.</param>
        /// <returns>
        /// Dictionary&lt;System.String, System.Object&gt;
        /// </returns>
        Dictionary<string, object> ToDictionary(string[] filter);

        /// <summary>
        /// 序列化为Xml属性
        /// </summary>
        /// <param name="nodeName">节点名.</param>
        /// <returns>
        /// System.String
        /// </returns>
        string ToXmlAttribute(string nodeName);


        #endregion

    }



    /// <summary>
    ///   <see cref="ECF.Entity"/>
    /// entity 实体转换处理类
    /// Author:  XP
    /// Created: 2011/9/19
    /// </summary>
    [Serializable]
    public abstract class Entity : IEntity
    {
        /// <summary>
        /// 获取属性的限制条件
        /// </summary>
        const BindingFlags PropertiesBindings = BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance;

        /// <summary>
        /// 实体的全名
        /// </summary>
        public abstract string EntityFullName { get; }

        #region SetValues给实体填值

#if(!WINDOWS)
        #region SetValues 附Json格式数据
        /// <summary>
        /// 给实体附值.
        /// </summary>
        /// <param name="json">json格式数据.</param>
        public void SetValues(string json)
        {
            try
            {
                Dictionary<string, object> dic = (Dictionary<string, object>)JsonConvert.DeserializeObject(json);
                SetValues(dic);
            }
            catch (Exception ex)
            {
                //new ECFException(ex);
            }
        }

        #endregion
#endif

        #region SetValues 附Xml格式数据

        /// <summary>
        /// Set object values
        /// </summary>
        /// <param name="t">The t.</param>
        /// <param name="xac">The xac.</param>
        /// <returns>
        /// System.Object
        /// </returns>
        private object SetObjValues(Type t, XmlAttributeCollection xac)
        {
            try
            {
                object p = Activator.CreateInstance(t);
                PropertyInfo pi;
                foreach (XmlAttribute xa in xac)
                {
                    pi = t.GetProperty(xa.Name, PropertiesBindings);
                    //判断属性是否为空和是否可写
                    if (!Utils.IsNullOrEmpty(pi) && pi.CanWrite)
                    {

                        if (pi.PropertyType.BaseType.Name == "Enum")
                        {
                            pi.SetValue(p, Enum.Parse(pi.PropertyType, xa.Value, true), null);
                        }
                        else
                        {
                            pi.SetValue(p, ReflectionUtil.ChangeType(xa.Value, pi), null);
                        }
                    }
                }
                return p;
            }
            catch (Exception ex)
            {
                //new ECFException(ex);
                return null;
            }
        }

        /// <summary>
        /// 给实体附值.
        /// </summary>
        /// <param name="xmlDoc">XmlDocument格式数据.</param>
        public void SetValues(XmlDocument xmlDoc)
        {
            try
            {
                Type type = GetCacheType();
                XmlNode root = xmlDoc.DocumentElement;
                XmlNodeList xnl = root.ChildNodes;
                XmlNode node;
                PropertyInfo pi;
                for (int i = 0; i < xnl.Count; i++)
                {
                    node = xnl[i];
                    //if (Utils.IsNullOrEmpty(node.InnerText))
                    //    continue;
                    //else
                    //{
                    //获取属性
                    pi = type.GetProperty(node.Name, PropertiesBindings);
                    //判断属性是否为空和是否可写
                    if (!Utils.IsNullOrEmpty(pi) && pi.CanWrite)
                    {

                        if (pi.PropertyType.BaseType.Name == "Enum")
                        {
                            if (!string.IsNullOrEmpty(node.InnerText))
                                pi.SetValue(this, Enum.Parse(pi.PropertyType, node.InnerText, true), null);
                        }
                        else if (pi.PropertyType.Name.IndexOf("List") > -1)
                        {
                            if (node.ChildNodes.Count > 0)
                            {
                                Type[] tys = pi.PropertyType.GetGenericArguments();

                                if (tys.Length > 0)
                                {
                                    Type tl = typeof(List<>);
                                    tl = tl.MakeGenericType(tys[0]);
                                    var ol = Activator.CreateInstance(tl);

                                    foreach (XmlNode ln in node.ChildNodes)
                                    {
                                        if (ln is XmlElement)
                                        {
                                            MethodInfo mi = tl.GetMethod("Add");
                                            if (mi != null)
                                            {
                                                mi.Invoke(ol, new object[] { SetObjValues(tys[0], ln.Attributes) });
                                            }
                                        }
                                    }

                                    pi.SetValue(this, ol, null);
                                }
                            }

                        }
                        else
                            try
                            {
                                pi.SetValue(this, ReflectionUtil.ChangeType(node.InnerText, pi), null);
                            }
                            catch
                            {
                                continue;
                            }
                        //pi.SetValue(this, Convert.ChangeType(node.InnerText, pi.PropertyType, CultureInfo.CurrentCulture), null);
                    }
                    //}
                }

            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
            }
        }

        /// <summary>
        /// 给实体附值
        /// </summary>
        /// <param name="xnl">Xml节点列表.</param>
        public void SetValues(XmlNodeList xnl)
        {
            try
            {
                Type type = GetCacheType();
                XmlNode node;
                PropertyInfo pi;
                for (int i = 0; i < xnl.Count; i++)
                {
                    node = xnl[i];
                    //获取属性
                    pi = type.GetProperty(node.Name, PropertiesBindings);
                    //判断属性是否为空和是否可写
                    if (!Utils.IsNullOrEmpty(pi) && pi.CanWrite)
                    {
                        if (pi.PropertyType.BaseType.Name == "Enum")
                        {
                            if (!string.IsNullOrEmpty(node.InnerText))
                                pi.SetValue(this, Enum.Parse(pi.PropertyType, node.InnerText, true), null);
                        }
                        else
                            try
                            {
                                pi.SetValue(this, ReflectionUtil.ChangeType(node.InnerText, pi), null);
                            }
                            catch
                            {
                                continue;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
            }
        }

        #endregion

        #region SetValues 附NameValueCollection 给实体赋值 by 20130826
        /// <summary>
        /// 给实体赋值 by 20130826
        /// </summary>
        /// <param name="nvc">NameValueCollection格式数据.</param>
        public void SetValues(NameValueCollection nvc)
        {
            try
            {
                Type type = GetCacheType();
                if (nvc == null) return;
                PropertyInfo pi;
                foreach (string key in nvc.AllKeys)
                {
                    pi = type.GetProperty(key, PropertiesBindings);
                    if (!Utils.IsNullOrEmpty(pi) && pi.CanWrite)
                    {
                        if (pi.PropertyType.BaseType.Name == "Enum")
                        {
                            if (!string.IsNullOrEmpty(nvc[key]))
                                pi.SetValue(this, Enum.Parse(pi.PropertyType, nvc[key], true), null);
                        }
                        else
                            try
                            {
                                pi.SetValue(this, ReflectionUtil.ChangeType(nvc[key], pi), null);
                            }
                            catch
                            {
                                continue;
                            }
                    }
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
            }

        }
        #endregion

        #region SetValues 附Hashtable格式数据
        /// <summary>
        /// 给实体附值.
        /// </summary>
        /// <param name="ht">Hashtable格式数据.</param>
        public void SetValues(Hashtable ht)
        {
            try
            {
                Type type = GetCacheType();
                if (ht == null) return;
                PropertyInfo pi;
                //将hashtable进行枚举化
                System.Collections.IDictionaryEnumerator enumerator = ht.GetEnumerator();
                //遍历hashtable中的所有元素
                while (enumerator.MoveNext())
                {
                    //if (Utils.IsNullOrEmpty(enumerator.Value))
                    //    continue;
                    //else
                    //{
                    pi = type.GetProperty(enumerator.Key.ToString(), PropertiesBindings);
                    if (!Utils.IsNullOrEmpty(pi) && pi.CanWrite)
                    {
                        if (pi.PropertyType.BaseType.Name == "Enum")
                        {
                            if (enumerator.Value != null && !string.IsNullOrEmpty(enumerator.Value.ToString()))
                                pi.SetValue(this, Enum.Parse(pi.PropertyType, enumerator.Value.ToString(), true), null);
                        }
                        else
                            try
                            {
                                pi.SetValue(this, ReflectionUtil.ChangeType(enumerator.Value, pi), null);
                            }
                            catch
                            {
                                continue;
                            }
                        //pi.SetValue(this, Convert.ChangeType(enumerator.Value, pi.PropertyType, CultureInfo.CurrentCulture), null);
                    }
                    //}
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
            }
        }

        #endregion

        #region SetValues 附Dictionary<string,object>格式数据
        /// <summary>
        /// 给实体附值.
        /// </summary>
        /// <param name="dic">Hashtable格式数据.</param>
        public void SetValues(Dictionary<string, object> dic)
        {
            try
            {
                Type type = GetCacheType();
                PropertyInfo pi;
                if (dic == null) return;
                foreach (KeyValuePair<string, object> d in dic)
                {
                    //if (Utils.IsNullOrEmpty(d.Value))
                    //    continue;
                    //else
                    //{
                    pi = type.GetProperty(d.Key.Trim(), PropertiesBindings);
                    if (!Utils.IsNullOrEmpty(pi) && pi.CanWrite)
                    {
                        if (pi.PropertyType.BaseType.Name == "Enum")
                        {
                            if (d.Value != null && !string.IsNullOrEmpty(d.Value.ToString()))
                                pi.SetValue(this, Enum.Parse(pi.PropertyType, d.Value.ToString(), true), null);
                        }
                        else
                        {
                            try
                            {
                                pi.SetValue(this, ReflectionUtil.ChangeType(d.Value, pi), null);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    //}
                }

            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
            }
        }

        #endregion

        #region SetValues 附DataRow格式数据
        /// <summary>
        /// 给实体附值.
        /// </summary>
        /// <param name="dr">DataRow格式的数据</param>
        public void SetValues(DataRow dr)
        {
            try
            {
                Type type = GetCacheType();
                PropertyInfo pi;
                foreach (DataColumn col in dr.Table.Columns)
                {
                    try
                    {
                        // 当数据库值为DbNull时跳过
                        if (dr[col.ColumnName] is DBNull) continue;
                        //if (Utils.IsNullOrEmpty(dr[col.ColumnName]))
                        //    continue;
                        //else
                        //{

                        pi = type.GetProperty(col.ColumnName, PropertiesBindings);
                        if (!Utils.IsNullOrEmpty(pi) && pi.CanWrite)
                        {
                            if (pi.PropertyType.BaseType.Name == "Enum")
                            {
                                if (!string.IsNullOrEmpty(dr[col.ColumnName].ToString()))
                                    pi.SetValue(this, Enum.Parse(pi.PropertyType, dr[col.ColumnName].ToString(), true), null);
                            }
                            else
                                try
                                {
                                    pi.SetValue(this, ReflectionUtil.ChangeType(dr[col.ColumnName], pi), null);
                                }
                                catch
                                {
                                    continue;
                                }
                            //pi.SetValue(this, Convert.ChangeType(dr[col.ColumnName], pi.PropertyType, CultureInfo.CurrentCulture), null);
                        }
                        //}
                    }
                    catch
                    {
                        continue;
                    }

                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
            }
        }

        #endregion

        #region SetValues 附IEntity格式数据 by xp 20130829
        /// <summary>
        /// 给实体附值.
        /// </summary>
        /// <param name="ent">实体值进行对拷.</param>
        public void SetValues(IEntity ent)
        {
            try
            {
                Type type = GetCacheType();
                PropertyInfo[] pis = type.GetProperties(PropertiesBindings);

                foreach (PropertyInfo pi in pis)
                {
                    if (!Utils.IsNullOrEmpty(pi) && pi.CanWrite)
                    {
                        object val = ent.GetValue(pi.Name);
                        if (Utils.IsNullOrEmpty(val))
                            continue;
                        if (pi.PropertyType.BaseType.Name == "Enum")
                        {
                            if (!string.IsNullOrEmpty(val.ToString()))
                                pi.SetValue(this, Enum.Parse(pi.PropertyType, val.ToString(), true), null);
                        }
                        else
                            try
                            {
                                pi.SetValue(this, ReflectionUtil.ChangeType(val, pi), null);
                            }
                            catch
                            {
                                continue;
                            }
                    }
                }

            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
            }
        }

        /// <summary>
        /// 给实体的单个对象赋值
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="value">字段值</param>
        public void SetValue(string name, object value)
        {
            if (!Utils.IsNullOrEmpty(name) && !Utils.IsNullOrEmpty(value))
            {
                try
                {
                    Type type = GetCacheType();
                    PropertyInfo pi = type.GetProperty(name, PropertiesBindings);
                    if (!Utils.IsNullOrEmpty(pi) && pi.CanWrite)
                    {
                        if (pi.PropertyType.BaseType.Name == "Enum")
                        {
                            if (!string.IsNullOrEmpty(value.ToString()))
                                pi.SetValue(this, Enum.Parse(pi.PropertyType, value.ToString(), true), null);
                        }
                        else
                        {
                            try
                            {
                                pi.SetValue(this, ReflectionUtil.ChangeType(value, pi), null);
                            }
                            catch
                            {

                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    //new ECFException(ex);
                }
            }
        }
        #endregion

        #endregion

        #region 格式转换输出

        #region ToJson

        /// <summary>
        ///  将实体转换成Json格式的数据.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  10-17-2014
        /// </summary>
        /// <returns>System.String.</returns>
        public virtual string ToJson()
        {
            return ToJson(false);
        }



        /// <summary>
        /// 将实体转换成Json格式的数据.
        /// 添加对List和实体的深入转化处理 by xp 20140611.
        /// </summary>
        /// <returns>
        /// System.String
        /// </returns>
        public virtual string ToJson(bool unicode)
        {
            return ToJson(unicode, false);
        }

        /// <summary>
        /// 将实体转换成Json格式的数据
        /// </summary>
        /// <param name="unicode">是否对值进行Unicode编码.</param>
        /// <param name="beNull">是否需要空值字段.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public virtual string ToJson(bool unicode, bool beNull)
        {
            return ToJson(unicode, beNull, null);
        }

        /// <summary>
        /// 将实体转换成Json格式的数据.
        /// </summary>
        /// <param name="unicode">是否对值进行Unicode编码.</param>
        /// <param name="beNull">是否需要空值字段.</param>
        /// <param name="filter">需要要过滤的字段.</param>
        /// <returns>
        /// System.String.
        /// </returns>
        public virtual string ToJson(bool unicode, bool beNull, string[] filter)
        {
            try
            {
                Type type = GetCacheType();
                PropertyInfo[] pis = type.GetProperties(PropertiesBindings);
                List<string> items = new List<string>();
                foreach (PropertyInfo pi in pis)
                {
                    // 判断是否需要过滤字段
                    if (filter != null && filter.Length > 0)
                    {
                        // 判断当前字段名是否在需要过滤的列表中
                        if (((IList)filter).Contains(pi.Name)) continue;
                    }

                    string json = ConvertJson(pi, unicode, beNull);
                    if (!string.IsNullOrEmpty(json))
                    {
                        items.Add(json);
                    }
                }
                return "{" + string.Join(",", items) + "}";
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
                return "";
            }
        }

        /// <summary>
        ///  对反射属性进行转换.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  10-17-2014
        /// </summary>
        /// <param name="pi">The pi.</param>
        /// <param name="unicode">是否需要对值进行Unicode编码 .</param>
        /// <param name="beNull">是否需要空值字段.</param>
        /// <returns>System.String.</returns>
        string ConvertJson(PropertyInfo pi, bool unicode, bool beNull)
        {
            string result = string.Empty;
            try
            {
                object val = pi.GetValue(this, null);

                if (val == null)
                {
                    if (beNull)
                        return "\"" + pi.Name + "\":null";
                    else
                        return result;
                }
                else
                {
                    if (val is String)
                    {
                        if (pi.Name.ToLower() != "entityfullname")
                        {
                            return ConvertJsonData(pi.Name, val, typeof(string), unicode);
                        }
                    }
                    else if (val is DataSet)
                    {
                        List<string> tables = new List<string>();
                        foreach (DataTable dt in ((DataSet)val).Tables)
                        {
                            tables.Add("\"" + dt.TableName + "\":" + DBHelper.TableToJson(dt, unicode));
                        }
                        result = "\"" + pi.Name + "\":{" + string.Join(",", tables) + "}";
                    }
                    else if (val is DataTable)
                    {
                        result = ("\"" + pi.Name + "\":" + DBHelper.TableToJson((DataTable)val));
                    }
                    else if (val is DataRow)
                    {
                        result = "\"" + pi.Name + "\":" + DBHelper.RowToJson((DataRow)val, beNull, null);
                    }
                    else if (val is IEntity)
                    {
                        result = ("\"" + pi.Name + "\":" + ((IEntity)val).ToJson());
                    }
                    else if (val is IDictionary)
                    {
                        IDictionary dictionary = val as IDictionary;
                        if (dictionary != null)
                        {
                            result = ("\"" + pi.Name + "\":" + JsonUtils.ConvertJson(dictionary, unicode));
                        }
                    }
                    else if (val is IEnumerable)
                    {
                        result = ("\"" + pi.Name + "\":" + JsonUtils.ConvertJson((IEnumerable)val, unicode));
                    }
                    else
                    {
                        result = (ConvertJsonData(pi.Name, pi.GetValue(this, null), pi.PropertyType, unicode));
                    }
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex);
            }

            return result;
        }



        /// <summary>
        ///  To the json.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  10-17-2014
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <returns>System.String.</returns>
        public virtual string ToJson(List<string> properties)
        {
            return ToJson(properties, false);
        }

        /// <summary>
        ///  将实体转换成Json格式的数据.
        /// Author :   XP-PC/Shaipe
        /// Created:  10-17-2014
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="unicode">是否需要对值进行Unicode编码 .</param>
        /// <returns>System.String.</returns>
        public virtual string ToJson(List<string> properties, bool unicode)
        {
            return ToJson(properties, unicode, false);
        }


        /// <summary>
        ///  将实体转换成Json模式的数据. by xp 20140611
        /// 添加对List和实体的深入转化处理 by xp 20140611.
        /// Author :   XP-PC/Shaipe
        /// Created:  10-17-2014
        /// </summary>
        /// <param name="properties">需要转换的属性列表.</param>
        /// <param name="unicode">是否需要对值进行Unicode编码 .</param>
        /// <param name="beNull">是否需要空值字段.</param>
        /// <returns>System.String</returns>
        public virtual string ToJson(List<string> properties, bool unicode, bool beNull)
        {
            try
            {
                List<string> items = new List<string>();
                Type type = GetCacheType();
                foreach (string pname in properties)
                {
                    PropertyInfo pi = type.GetProperty(pname, PropertiesBindings);
                    if (pi != null)
                    {
                        items.Add(ConvertJson(pi, unicode, beNull));
                    }
                }
                return "{" + string.Join(",", items) + "}";
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
                return "";
            }
        }
        #endregion

        #region ToXml

        /// <summary>
        /// 将实体转换成Xml模式的数据 
        /// </summary>
        /// <returns>
        /// System.String
        /// </returns>
        public virtual string ToXml()
        {
            return ToXml(new string[] { });
        }
        /// <summary>
        /// 将实体转换成Xml模式的数据
        /// 添加对List和实体的深入转化处理 by xp 20140611.
        /// </summary>
        /// <param name="filter">需要过滤的字段.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public virtual string ToXml(string[] filter)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                Type type = GetCacheType();
                PropertyInfo[] pis = type.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    // 判断是否需要过滤字段
                    if (filter != null && filter.Length > 0)
                    {
                        // 判断当前字段名是否在需要过滤的列表中
                        if (((IList)filter).Contains(pi.Name)) continue;
                    }

                    // 判断是否为IEntity实体
                    #region 判断是否为IEntity实体
                    if (pi.PropertyType.GetInterface("IEntity", false) != null)
                    {
                        IEntity ent = pi.GetValue(this, null) as IEntity;
                        string nodeName = pi.Name;

                        object[] attrs = pi.GetCustomAttributes(typeof(EntityClassAttribute), true);
                        if (attrs != null && attrs.Length > 0)
                        {
                            EntityClassAttribute pa = attrs.GetValue(0) as EntityClassAttribute;
                            if (pa != null && !String.IsNullOrEmpty(pa.NodeName))
                            {
                                nodeName = pa.NodeName;
                            }
                        }

                        if (ent != null)
                            sb.Append("<" + nodeName + ">" + ent.ToXml() + "</" + nodeName + ">");
                        else
                            sb.Append("<" + nodeName + "></" + nodeName + ">");
                    }
                    #endregion

                    // 判断是否为List对象
                    #region 判断是否为List对象
                    else if (pi.PropertyType.Name.IndexOf("List") > -1)
                    {
                        Type[] tys = pi.PropertyType.GetGenericArguments();
                        object val = pi.GetValue(this, null);
                        if (tys.Length > 0 && val != null)
                        {
                            int itemCount = Utils.ToInt(pi.PropertyType.GetProperty("Count").GetValue(val, null));

                            sb.Append("<" + pi.Name + ">");
                            for (int i = 0; i < itemCount; i++)
                            {
                                object o = pi.PropertyType.GetProperty("Item").GetValue(val, new object[] { i });
                                Type t = o.GetType();

                                string nodeName = t.Name;
                                bool isAttribute = false;

                                object[] attrs = t.GetCustomAttributes(typeof(EntityClassAttribute), true);
                                if (attrs != null && attrs.Length > 0)
                                {
                                    EntityClassAttribute pa = attrs.GetValue(0) as EntityClassAttribute;
                                    if (pa != null && !String.IsNullOrEmpty(pa.NodeName))
                                    {
                                        nodeName = pa.NodeName;

                                        isAttribute = Utils.ToBool(pa.IsXmlAttribute);
                                    }

                                }

                                if (tys[0].GetInterface("IEntity", false) != null)
                                {
                                    if (isAttribute)
                                    {
                                        sb.Append(((IEntity)o).ToXmlAttribute(nodeName));
                                    }
                                    else
                                        sb.Append("<" + nodeName + ">" + ((IEntity)o).ToXml() + "</" + nodeName + ">");
                                }
                                else
                                {
                                    sb.Append("<" + nodeName + ">" + o.ToString() + "</" + nodeName + ">");
                                }
                            }
                            sb.Append("</" + pi.Name + ">");

                        }
                    }
                    #endregion
                    else // 其他类型
                    {
                        if (pi.Name.ToLower() != "entityfullname")
                        {
                            object val = pi.GetValue(this, null);
                            if (val != null)
                            {
                                if (val is DataTable)
                                {
                                    sb.Append("<" + pi.Name + ">" + DBHelper.Table2Xml((DataTable)val) + "</" + pi.Name + ">");
                                }
                                else if (val is DataSet)
                                {
                                    List<string> tables = new List<string>();
                                    foreach (DataTable dt in ((DataSet)val).Tables)
                                    {
                                        tables.Add("<" + dt.TableName + ">" + DBHelper.Table2Xml(dt) + "</" + dt.TableName + ">");
                                    }
                                    sb.Append("<" + pi.Name + ">" + string.Join("", tables) + "</" + pi.Name + " > ");
                                }
                                else if (val is DataRow)
                                {
                                    sb.Append("<" + pi.Name + ">" + DBHelper.DataRow2Xml((DataRow)val, "", null) + "</" + pi.Name + ">");
                                }
                                else
                                {
                                    sb.Append(ConvertXmlData(pi.Name, ReflectionUtil.TypeToString(val, pi)));
                                }
                            }
                        }
                    }

                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
                return "";
            }
        }

        /// <summary>
        /// 把对像序列化为
        /// </summary>
        /// <param name="nodeName">节点名称.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public virtual string ToXmlAttribute(string nodeName)
        {

            try
            {
                StringBuilder sb = new StringBuilder();
                Type type = GetCacheType();
                PropertyInfo[] pis = type.GetProperties();
                sb.Append("<" + nodeName + " ");
                foreach (PropertyInfo pi in pis)
                {
                    if (pi.Name.ToLower() != "entityfullname")
                    {
                        object val = pi.GetValue(this, null);
                        if (val != null)
                        {
                            sb.Append(pi.Name + "=\"" + val + "\" ");
                        }
                    }
                }
                sb.Append("/>");

                return sb.ToString();
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
                return "";
            }
        }

        /// <summary>
        /// 将实体转换成Xml模式的数据. by xp 20130829
        /// 添加对List和实体的深入转化处理 by xp 20140611.
        /// </summary>
        /// <param name="properties">需要转换的属性列表.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public virtual string ToXml(List<string> properties)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                Type type = GetCacheType();

                foreach (string pname in properties)
                {
                    PropertyInfo pi = type.GetProperty(pname, PropertiesBindings);
                    if (pi != null)
                    {
                        // 判断是否为IEntity实体
                        if (pi.PropertyType.GetInterface("IEntity", false) != null)
                        {
                            IEntity ent = pi.GetValue(this, null) as IEntity;
                            string nodeName = pi.Name;

                            object[] attrs = pi.GetCustomAttributes(typeof(EntityClassAttribute), true);
                            if (attrs != null && attrs.Length > 0)
                            {
                                EntityClassAttribute pa = attrs.GetValue(0) as EntityClassAttribute;
                                if (pa != null && !String.IsNullOrEmpty(pa.NodeName))
                                {
                                    nodeName = pa.NodeName;
                                }
                            }

                            if (ent != null)
                                sb.Append("<" + nodeName + ">" + ent.ToXml() + "</" + nodeName + ">");
                            else
                                sb.Append("<" + nodeName + "></" + nodeName + ">");
                        }
                        // 判断是否为List对象
                        else if (pi.PropertyType.Name.IndexOf("List") > -1)
                        {
                            Type[] tys = pi.PropertyType.GetGenericArguments();

                            object val = pi.GetValue(this, null);
                            if (tys.Length > 0 && val != null)
                            {
                                int itemCount = Utils.ToInt(pi.PropertyType.GetProperty("Count").GetValue(val, null));

                                sb.Append("<" + pi.Name + ">");
                                for (int i = 0; i < itemCount; i++)
                                {
                                    object o = pi.PropertyType.GetProperty("Item").GetValue(val, new object[] { i });
                                    if (tys[0].GetInterface("IEntity", false) != null)
                                    {
                                        Type t = o.GetType();
                                        string nodeName = t.Name;

                                        object[] attrs = t.GetCustomAttributes(typeof(EntityClassAttribute), true);
                                        if (attrs != null && attrs.Length > 0)
                                        {
                                            EntityClassAttribute pa = attrs.GetValue(0) as EntityClassAttribute;
                                            if (pa != null && !String.IsNullOrEmpty(pa.NodeName))
                                            {
                                                nodeName = pa.NodeName;
                                            }
                                        }

                                        sb.Append("<" + nodeName + ">" + ((IEntity)o).ToXml() + "</" + nodeName + ">");
                                    }
                                    else
                                    {
                                        sb.Append("<Text>" + o.ToString() + "</Text>");
                                    }
                                }
                                sb.Append("</" + pi.Name + ">");

                            }
                        }
                        else // 其他类型
                        {
                            if (pi.Name.ToLower() != "entityfullname")
                            {
                                object val = pi.GetValue(this, null);
                                if (val != null)
                                {
                                    if (val is DataTable)
                                    {
                                        sb.Append("<" + pi.Name + ">" + DBHelper.Table2Xml((DataTable)val, true) + "</" + pi.Name + ">");
                                    }
                                    else if (val is DataSet)
                                    {
                                        List<string> tables = new List<string>();
                                        foreach (DataTable dt in ((DataSet)val).Tables)
                                        {
                                            tables.Add("<" + dt.TableName + ">" + DBHelper.Table2Xml(dt, true) + "</" + dt.TableName + ">");
                                        }
                                        sb.Append("<" + pi.Name + ">" + string.Join("", tables) + "</" + pi.Name + " > ");
                                    }
                                    else
                                    {
                                        sb.Append(ConvertXmlData(pi.Name, ReflectionUtil.TypeToString(val, pi)));
                                    }


                                }
                            }
                        }
                    }

                }

                return sb.ToString();
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
                return "";
            }
        }

        #endregion

        #region ToHashtable
        /// <summary>
        /// 将实体数据转换为Hashtable数据进行输出.
        /// </summary>
        /// <returns></returns>
        public virtual Hashtable ToHashtable()
        {
            try
            {
                Hashtable th = new Hashtable();
                Type type = GetCacheType();
                PropertyInfo[] pis = type.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    if (pi.Name.ToLower() != "entityfullname")
                    {
                        if (pi.GetValue(this, null) != null)
                        {
                            th.Add(pi.Name, pi.GetValue(this, null));
                        }
                    }
                }
                return th;
            }
            catch
            {
                return new Hashtable();
            }
        }


        #endregion

        /// <summary>
        /// 将实体数据转换为Dictionary
        /// </summary>
        /// <returns>
        /// Dictionary&lt;System.String, System.Object&gt;
        /// </returns>
        public virtual Dictionary<string, object> ToDictionary()
        {
            return ToDictionary(new string[] { });
        }

        /// <summary>
        /// 将实体数据转换为Dictionary
        /// </summary>
        /// <param name="filter">需要过滤的字段，这里的字段不会被转出.</param>
        /// <returns>
        /// Dictionary&lt;System.String, System.Object&gt;
        /// </returns>
        public virtual Dictionary<string, object> ToDictionary(string[] filter)
        {
            Dictionary<string, object> dic = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            try
            {
                Type type = GetCacheType();
                PropertyInfo[] pis = type.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    // 判断是否需要过滤字段
                    if (filter != null && filter.Length > 0)
                    {
                        // 判断当前字段名是否在需要过滤的列表中
                        if (((IList)filter).Contains(pi.Name)) continue;
                    }

                    if (pi.Name.ToLower() != "entityfullname")
                    {
                        if (pi.GetValue(this, null) != null)
                        {
                            object val = pi.GetValue(this, null);
                            if (val is IEntity) // 判断值是否为继承接口实体
                            {
                                val = ((IEntity)val).ToDictionary();
                            }

                            dic.Add(pi.Name, val);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex);
            }
            return dic;
        }

        #endregion

        #region ConvertXmlData 转换成Xml格式的格式的字符串





        /// <summary>
        /// 转换成Xml格式的格式的字符串
        /// </summary>
        /// <param name="name">节点名称.</param>
        /// <param name="val">节点值.</param>
        /// <returns>返回的值字段名全部为小写</returns>
        string ConvertXmlData(string name, string val)
        {

            if (name != string.Empty && val != string.Empty)
            {
                if (val.IndexOf("<") != -1 || val.IndexOf(">") != -1)
                {
                    return "<" + name + "><![CDATA[" + (val) + "]]></" + name + ">";
                }
                else
                {
                    return "<" + name + ">" + val + "</" + name + ">";
                }
            }
            return "";
        }


        /// <summary>
        ///  以Json的格式输出数据.
        /// Author :   XP-PC/Shaipe
        /// Created:  10-17-2014
        /// </summary>
        /// <param name="name">属性名称.</param>
        /// <param name="val">属性值.</param>
        /// <param name="type">类型.</param>
        /// <param name="unicode">是否需要对值进行Unicode编码 .</param>
        /// <returns>System.String.</returns>
        private string ConvertJsonData(string name, object val, Type type, bool unicode)
        {
            if (!String.IsNullOrEmpty(name))
            {
                return "\"" + name + "\":" + Utils.JsonValueOfType(val, type, unicode);
            }
            return "";
        }




        #endregion

        #region GetCacheType
        /// <summary>
        /// 从缓存中获取类型,如果缓存中没有此类型则向缓存中写入最新的实体类型.
        /// </summary>
        /// <returns>
        /// System.Type
        /// </returns>
        Type GetCacheType()
        {
            try
            {
                Type o = DefaultCache.Get<Type>(EntityFullName);
                if (Utils.IsNullOrEmpty(o))
                {
                    Type type = this.GetType();
                    DefaultCache.Max(EntityFullName, type);
                    return type;
                }
                return o;
            }
            catch (Exception ex)
            {
                //new ECFException(ex);
                return this.GetType();
            }
        }


        #endregion

        #region GetObjectData
        /// <summary>
        /// 获取对象数据
        /// </summary>
        /// <param name="info">序列化信息.</param>
        /// <param name="context">字符串流对象.</param>
        /// <exception cref="T:System.Security.SecurityException">调用方没有所要求的权限。</exception>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            try
            {
                Type type = GetCacheType();
                PropertyInfo[] pis = type.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    if (pi.Name.ToLower() != "entityfullname")
                    {
                        if (pi.GetValue(this, null) != null)
                        {
                            info.AddValue(pi.Name.ToLower(), pi.GetValue(this, null));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //new ECFException(ex);
            }
        }
        #endregion

        #region GetValue 获取属性值
        /// <summary>
        /// 获取属性值
        /// </summary>
        /// <param name="name">属性名称.</param>
        /// <returns>
        /// System.Object
        /// </returns>
        public object GetValue(string name)
        {
            if (Utils.IsNullOrEmpty(name)) return null;
            try
            {
                Type type = GetCacheType();
                PropertyInfo pi = type.GetProperty(name, PropertiesBindings);
                if (pi == null)
                {
                    FieldInfo fi = type.GetField(name, PropertiesBindings);
                    if (fi != null)
                    {
                        return fi.GetValue(this);
                    }
                    return null;
                }
                return pi.GetValue(this, null);
            }
            catch (Exception ex)
            {
                //new ECFException(ex);
                return null;
            }
        }
        #endregion

    }



}
