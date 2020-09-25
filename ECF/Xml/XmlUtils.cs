using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using ECF.Data;
using System.Security.Permissions;

namespace ECF.Xml
{
    /// <summary>
    ///   <see cref="ECF.Xml.XmlUtils"/>
    /// XML处理单元
    /// Author:  XP
    /// Created: 2011/9/1
    /// </summary>
    public class XmlUtils
    {

        /// <summary>
        /// The protoco l_ separator
        /// </summary>
        private const string PROTOCOL_SEPARATOR = "://";

        /// <summary>
        /// Get file system resource without protocol
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// System.String
        /// </returns>
        private static string GetFileSystemResourceWithoutProtocol(string filePath)
        {
            int pos = filePath.IndexOf(PROTOCOL_SEPARATOR);
            if (pos == -1)
            {
                return filePath;
            }
            else
            {
                // skip forward slashes after protocol name, if any
                if (filePath.Length > pos + PROTOCOL_SEPARATOR.Length)
                {
                    while (filePath[++pos] == '/')
                    {
                        ;
                    }
                }
                return filePath.Substring(pos);
            }
        }

        /// <summary>
        /// File exists
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>
        /// System.Boolean
        /// </returns>
        /// <exception cref="ECFException"></exception>
        private static bool FileExists(string filePath)
        {
            if (File.Exists(filePath))
            {
                // true if the caller has the required permissions and path contains the name of an existing file; 
                return true;
            }
            else
            {
                // This method also returns false if the caller does not have sufficient permissions 
                // to read the specified file, 
                // no exception is thrown and the method returns false regardless of the existence of path.
                // So we check permissiion and throw an exception if no permission
                FileIOPermission filePermission = null;
                try
                {
                    // filePath must be the absolute path of the file. 
                    filePermission = new FileIOPermission(FileIOPermissionAccess.Read, filePath);
                }
                catch
                {
                    return false;
                }
                try
                {
                    filePermission.Demand();
                }
                catch (Exception e)
                {
                    throw new ECFException(
                        string.Format("ECF.ORM在读写映射文件\"{0}\"时出错. Cause : {1}",
                        filePath,
                        e.Message), e);
                }

                return false;
            }
        }

        private static string _baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

        /// <summary>
        /// 获取XmlDocument对象
        /// by XP-PC 2012/4/18
        /// </summary>
        /// <param name="resourcePath">The resource path.</param>
        /// <returns>
        /// System.Xml.XmlDocument
        /// </returns>
        public static XmlDocument GetXmlDocument(string resourcePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            XmlTextReader reader = null;
            resourcePath = GetFileSystemResourceWithoutProtocol(resourcePath);

            if (!FileExists(resourcePath))
            {
                resourcePath = Path.Combine(_baseDirectory, resourcePath);
            }

            try
            {
                reader = new XmlTextReader(resourcePath);
                xmlDoc.Load(reader);
            }
            catch (Exception e)
            {
                throw new ECFException(
                    string.Format("不能加载数据库映射文件 \"{0}\". Cause : {1}",
                    resourcePath,
                    e.Message), e);
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                }
            }
            return xmlDoc;
        }


        /// <summary>
        /// 获取指定的xml节点
        /// </summary>
        /// <param name="xmlPath"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static XmlNode GetXmlNodeByName(string xmlPath, string name)
        {
            XmlDocument xDoc = new XmlDocument();
            try
            {
                xDoc.Load(xmlPath);
                XmlNode root = xDoc.DocumentElement;
                foreach (XmlNode xNode in root.ChildNodes)
                {
                    if (xNode.Attributes["key"] != null &&
                        xNode.Attributes["key"].ToString().ToLower() == name.ToLower())
                    {
                        return xNode;
                    }
                    continue;
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex.Message, ex);
            }
            return null;
        }

        /// <summary>
        /// 给Xml节点设置属性
        /// </summary>
        /// <param name="xNode">Xml节点.</param>
        /// <param name="attName">节点属性名称.</param>
        /// <param name="attValue">节点属性值.</param>
        /// <returns>Xml节点</returns>
        static public XmlNode SetAttribute(XmlNode xNode, string attName, string attValue)
        {
            XmlElement xe = (XmlElement)xNode;
            if (xNode != null)
            {
                xe.SetAttribute(attName, attValue);
            }
            return xe;
        }

        /// <summary>
        /// 根据Xml节点获取节点的值
        /// </summary>
        /// <param name="xnode">xml节点</param>
        /// <returns>返回String</returns>
        static public string GetXmlNodeValue(XmlNode xnode)
        {
            if (xnode != null)
            {
                return xnode.InnerText;
            }
            return null;
        }

        /// <summary>
        /// 获取xml节点的属性
        /// </summary>
        /// <param name="xNode"></param>
        /// <param name="attName"></param>
        /// <returns></returns>
        public static string GetXmlNodeAttribute(XmlNode xNode, string attName)
        {
            if (xNode.Attributes[attName] != null)
            {
                return xNode.Attributes[attName].Value;
            }
            return null;
        }

        /// <summary>
        /// 获取节点的值
        /// by XP-PC 2012/4/18
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <returns>
        /// System.String
        /// </returns>
        static public string GetNodeValue(XmlDocument doc, string nodeName)
        {
            try
            {
                return doc.DocumentElement[nodeName].InnerText;
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        /// 获取指定节点的指定属性
        /// by XP-PC 2012/4/18
        /// </summary>
        /// <param name="doc">The doc.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="attrName">Name of the attr.</param>
        /// <returns>
        /// System.String
        /// </returns>
        static public string GetNodeAttr(XmlDocument doc, string nodeName, string attrName)
        {
            XmlElement node = doc.DocumentElement[nodeName];
            if (node != null && node.NodeType == XmlNodeType.Element)
            {
                XmlAttribute xattr = node.Attributes[attrName];
                if (xattr != null)
                    return xattr.Value;
            }
            return "";
        }

        /// <summary>
        /// 获取
        /// </summary>
        /// <param name="xmlDoc">The XML doc.</param>
        /// <param name="eleName">Name of the ele.</param>
        /// <returns>System.String</returns>
        public static string GetXmlValue(XmlDocument xmlDoc, string eleName)
        {
            try
            {
                return xmlDoc.DocumentElement[eleName].InnerText;
            }
            catch
            {
                return "";
            }

        }

        /// <summary>
        /// 设置属性
        /// </summary>
        /// <param name="ele">元素.</param>
        /// <param name="dic">属性对.</param>
        /// <returns>
        /// XmlElement
        /// </returns>
        public static XmlElement SetAttributes(XmlElement ele, Dictionary<string, object> dic)
        {

            foreach (KeyValuePair<string, object> d in dic)
            {
                if (!string.IsNullOrEmpty(d.Key))
                {
                    ele.SetAttribute(d.Key, Utils.ToString(d.Value));
                }
            }
            return ele;
        }


        #region XML→T
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns>
        /// ``0
        /// </returns>
        public static T Deserialize<T>(string xml)
        {
            if (typeof(T).IsGenericType)
            {
                return DeserializeToEntityList<T>(xml);
            }
            else
            {
                return DeserializeToEntity<T>(xml);
            }
        }
        #endregion

        #region XML→T(单例)
        /// <summary>
        /// Deserialize to entity
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns>
        /// ``0
        /// </returns>
        private static T DeserializeToEntity<T>(string xml)
        {
            using (StringReader reader = new StringReader(xml))
            {
                XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                T obj = (T)xs.Deserialize(reader);
                return obj;
            }
        }
        #endregion

        #region XML→T(列表)
        /// <summary>
        /// Deserialize to list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns>
        /// IList{``0}
        /// </returns>
        private static IList<T> DeserializeToList<T>(string xml)
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(xml);
            string nodeName = typeof(T).Name.ToLower();
            IList<T> list = new List<T>();
            foreach (XmlNode node in document.GetElementsByTagName(nodeName))
            {
                list.Add(Deserialize<T>(node.OuterXml));
            }
            return list;
        }

        private static T DeserializeToEntityList<T>(string xml)
        {
            var method = typeof(XmlUtils).GetMethod("DeserializeToList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).MakeGenericMethod(typeof(T).GetGenericArguments()[0]);
            return (T)method.Invoke(null, new object[] { xml });
        }
        #endregion

        #region T→XML
        /// <summary>
        /// Serialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">The obj.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string Serialize<T>(T obj)
        {
            string xmlString = string.Empty;
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
            using (MemoryStream ms = new MemoryStream())
            {
                xmlSerializer.Serialize(ms, obj);
                xmlString = Encoding.UTF8.GetString(ms.ToArray());
            }
            return xmlString;
        }
        #endregion

        #region Methods

        /// <summary>
        /// XML Encode
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Encoded string</returns>
        public static string XmlEncode(string str)
        {
            if (str == null)
                return null;
            str = Regex.Replace(str, @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD]", "", RegexOptions.Compiled);
            return XmlEncodeAsIs(str);
        }

        /// <summary>
        /// XML Encode as is
        /// </summary>
        /// <param name="str">String</param>
        /// <returns>Encoded string</returns>
        public static string XmlEncodeAsIs(string str)
        {
            if (str == null)
                return null;
            using (var sw = new StringWriter())
            using (var xwr = new XmlTextWriter(sw))
            {
                xwr.WriteString(str);
                return sw.ToString();
            }
        }

        /// <summary>
        /// Encodes an attribute
        /// </summary>
        /// <param name="str">Attribute</param>
        /// <returns>Encoded attribute</returns>
        public static string XmlEncodeAttribute(string str)
        {
            if (str == null)
                return null;
            str = Regex.Replace(str, @"[^\u0009\u000A\u000D\u0020-\uD7FF\uE000-\uFFFD]", "", RegexOptions.Compiled);
            return XmlEncodeAttributeAsIs(str);
        }

        /// <summary>
        /// Encodes an attribute as is
        /// </summary>
        /// <param name="str">Attribute</param>
        /// <returns>Encoded attribute</returns>
        public static string XmlEncodeAttributeAsIs(string str)
        {
            return XmlEncodeAsIs(str).Replace("\"", "&quot;");
        }

        /// <summary>
        /// Decodes an attribute
        /// </summary>
        /// <param name="str">Attribute</param>
        /// <returns>Decoded attribute</returns>
        public static string XmlDecode(string str)
        {
            var sb = new StringBuilder(str);
            return sb.Replace("&quot;", "\"").Replace("&apos;", "'").Replace("&lt;", "<").Replace("&gt;", ">").Replace("&amp;", "&").ToString();
        }

        /// <summary>
        /// Serializes a datetime
        /// </summary>
        /// <param name="dateTime">Datetime</param>
        /// <returns>Serialized datetime</returns>
        public static string SerializeDateTime(DateTime dateTime)
        {
            var xmlS = new XmlSerializer(typeof(DateTime));
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                xmlS.Serialize(sw, dateTime);
                return sb.ToString();
            }
        }

        /// <summary>
        /// Deserializes a datetime
        /// </summary>
        /// <param name="dateTime">Datetime</param>
        /// <returns>Deserialized datetime</returns>
        public static DateTime DeserializeDateTime(string dateTime)
        {
            var xmlS = new XmlSerializer(typeof(DateTime));
            using (var sr = new StringReader(dateTime))
            {
                object test = xmlS.Deserialize(sr);
                return (DateTime)test;
            }
        }

        #endregion

        /// <summary>
        /// 转换为Xml字符串
        /// </summary>
        /// <param name="dic">IDictionary对象</param>
        /// <param name="nodeName">节点名称</param>
        /// <param name="isAttribute">是否以属性的形式进行显示</param>
        /// <returns></returns>
        public static string ConvertXml(System.Collections.IDictionary dic, string nodeName, bool isAttribute = false)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (string key in dic.Keys)
                {
                    object val = dic[key];

                    if (val == null)
                    {
                        sb.Append(Utils.ConvertXmlData(nodeName, ""));
                        continue;
                    }
                    // sting 也是 IEnumerable中的一种 情况 
                    if (val is string)
                    {
                        sb.Append(Utils.ConvertXmlData(key, val.ToString()));
                    }
                    else if (val is DataSet)
                    {
                        foreach (DataTable dt in ((DataSet)val).Tables)
                        {
                            sb.Append(DBHelper.Table2Xml(dt, dt.TableName, null));
                        }
                    }
                    else if (val is System.Collections.IDictionary)
                    {
                        sb.Append(ConvertXml((System.Collections.IDictionary)val, key, isAttribute));
                    }
                    else if (val is DataRow)
                    {
                        sb.Append(DBHelper.DataRow2Xml((DataRow)val, key, null));
                    }
                    else if (val is DataTable)
                    {
                        sb.Append(DBHelper.Table2Xml((DataTable)val, key, null));
                    }
                    else if (val is System.Collections.IEnumerable)
                    {
                        sb.Append(ConvertXml((System.Collections.IEnumerable)val, key, isAttribute));
                    }
                    else
                    {
                        sb.Append(Utils.ConvertXmlData(key, dic[key].ToString()));
                    }

                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return sb.ToString();
        }

        /// <summary>
        /// d
        /// </summary>
        /// <param name="enumerable"></param>
        /// <param name="nodeName"></param>
        /// <param name="isAttribute"></param>
        /// <returns></returns>
        public static string ConvertXml(System.Collections.IEnumerable enumerable, string nodeName, bool isAttribute = false)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                foreach (object val in enumerable)
                {
                    if (val == null)
                    {
                        sb.Append(Utils.ConvertXmlData(nodeName, ""));
                        continue;
                    }

                    // sting 也是 IEnumerable中的一种 情况 
                    if (val is string)
                    {
                        sb.Append(Utils.ConvertXmlData(nodeName, val.ToString()));
                    }
                    else if (val is DataSet)
                    {
                        foreach (DataTable dt in ((DataSet)val).Tables)
                        {
                            sb.Append(DBHelper.Table2Xml(dt, dt.TableName, null));
                        }
                    }
                    else if (val is System.Collections.IDictionary)
                    {
                        sb.Append(ConvertXml((System.Collections.IDictionary)val, "", isAttribute));
                    }
                    else if (val is DataRow)
                    {
                        sb.Append(DBHelper.DataRow2Xml((DataRow)val, "", null));
                    }
                    else if (val is DataTable)
                    {
                        sb.Append(DBHelper.Table2Xml((DataTable)val, "", null));
                    }
                    else if (val is System.Collections.IEnumerable)
                    {
                        sb.Append(ConvertXml((System.Collections.IEnumerable)val, "", isAttribute));
                    }
                    else
                    {
                        sb.Append(Utils.ConvertXmlData(nodeName, val.ToString()));
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return sb.ToString();
        }
    }
}
