using System;
using System.Collections.Generic;
using System.Xml;

namespace ECF.Xml
{
    /// <summary>
    /// Xml解析器
    /// </summary>
    public class XmlParser
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="xml">Xml字符串</param>
        public XmlParser(string xml)
        {
            try
            {
                _xmlDoc.LoadXml(xml);
                _IsSuccess = true;
            }
            catch (Exception ex)
            {
                new ECFException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlParser" /> class.
        /// </summary>
        /// <param name="xml">The XML.</param>
        /// <param name="nms">The NMS.</param>
        public XmlParser(string xml, Dictionary<string, string> nms)
        {
            try
            {
                _xmlDoc.LoadXml(xml);

                _XNM = new XmlNamespaceManager(_xmlDoc.NameTable);

                foreach (string key in nms.Keys)
                {
                    _XNM.AddNamespace(key, nms[key]);
                }

                _IsSuccess = true;
            }
            catch (Exception ex)
            {
                new ECFException(ex.Message, ex);
            }
        }

        bool _IsSuccess = false;
        /// <summary>
        /// 判断Xml字符串解析是否成功
        /// </summary>
        public bool IsSuccess
        {
            get { return _IsSuccess; }
        }

        XmlNamespaceManager _XNM = null;
        /// <summary>
        /// 返回命名空间管理器
        /// </summary>
        public XmlNamespaceManager XNamespace
        {
            get
            {
                return _XNM;
            }
        }
        /// <summary>
        /// xml对象
        /// </summary>
        XmlDocument _xmlDoc = new XmlDocument();

        /// <summary>
        /// 返回解析好的XmlDocument对象
        /// </summary>
        public XmlDocument XmlDoc
        {
            get { return _xmlDoc; }
        }
        /// <summary>
        /// 获取文档元素
        /// </summary>
        public XmlElement DocumentElement
        {
            get
            {
                try
                {
                    return XmlDoc.DocumentElement;
                }
                catch
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取xmlPath的节点列表
        /// </summary>
        /// <param name="xmlPath">xmlPath</param>
        /// <returns></returns>
        public XmlNodeList XmlNodes(string xmlPath)
        {
            if (XNamespace != null)
            {
                return DocumentElement.SelectNodes(xmlPath, XNamespace);
            }
            return DocumentElement.SelectNodes(xmlPath);
        }

        /// <summary>
        /// 获取节点集
        /// </summary>
        /// <param name="xmlPath">xmlPath</param>
        /// <returns></returns>
        public XmlNode[] GetXmlNodes(string xmlPath)
        {
            List<XmlNode> list = new List<XmlNode>();
            try
            {
                XmlNodeList xnl;
                if (XNamespace != null)
                {
                    xnl = DocumentElement.SelectNodes(xmlPath, XNamespace);
                }
                else
                {
                    xnl = DocumentElement.SelectNodes(xmlPath);
                }

                if (xnl.Count < 1)
                {
                    foreach (XmlNode xn in DocumentElement.ChildNodes)
                    {
                        if (xn.Name.ToLower() == xmlPath.ToLower())
                        {
                            list.Add(xn);
                        }
                    }
                }
                else
                {
                    foreach (XmlNode xn1 in xnl)
                    {
                        list.Add(xn1);
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return list.ToArray();
        }



        /// <summary>
        /// 获取一个点
        /// </summary>
        /// <param name="xmlPath">xmlPath</param>
        /// <returns></returns>
        public XmlNode GetXmlNode(string xmlPath)
        {
            XmlNode xn = null;
            try
            {
                if (XNamespace != null)
                {
                    xn = DocumentElement.SelectSingleNode(xmlPath, XNamespace);
                }
                else
                {
                    xn = DocumentElement.SelectSingleNode(xmlPath);
                }
                if (xn == null)
                {
                    foreach (XmlNode xxn in DocumentElement.ChildNodes)
                    {
                        if (xxn.Name.ToLower() == xmlPath.ToLower())
                        {
                            return xxn;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return xn;
        }



        /// <summary>
        /// 获取Xml节点
        /// </summary>
        /// <param name="xmlPath">xmlPath</param>
        /// <param name="attrName">属性名</param>
        /// <param name="attrValue">属性值</param>
        /// <returns></returns>
        public XmlNode GetXmlNode(string xmlPath, string attrName, string attrValue)
        {
            XmlNode[] xnl = GetXmlNodes(xmlPath);
            foreach (XmlNode xn in xnl)
            {
                XmlAttribute xa = xn.Attributes[attrName];
                if (xa != null && xa.Value == attrValue)
                {
                    return xn;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据节点名获取节点的值
        /// </summary>
        /// <param name="xmlPath">xmlPath</param>
        /// <returns></returns>
        public string GetNodeValue(string xmlPath)
        {
            XmlNode node = GetXmlNode(xmlPath);
            if (node != null)
            {
                return node.InnerText;
            }
            return "";
        }

        /// <summary>
        /// 获取节点的值
        /// </summary>
        /// <param name="xmlPath">xmlPath</param>
        /// <param name="attrName">属性名</param>
        /// <returns></returns>
        public string GetNodeValue(string xmlPath, string attrName)
        {
            XmlNode node = GetXmlNode(xmlPath);
            if (node != null)
            {
                if (node.Attributes[attrName] != null)
                    return node.InnerText;
            }
            return "";
        }

        /// <summary>
        /// 获取节点属性
        /// </summary>
        /// <param name="xmlPath">xmlPath</param>
        /// <param name="attrName">属性名</param>
        /// <returns></returns>
        public string GetNodeAttribute(string xmlPath, string attrName)
        {
            XmlNode node = GetXmlNode(xmlPath);
            if (node != null)
            {
                XmlAttribute xa = node.Attributes[attrName];
                if (xa != null)
                {
                    return xa.Value;
                }
            }
            return "";
        }

        /// <summary>
        /// 根据节点属性名称和值获取另外属性的值
        /// </summary>
        /// <param name="xmlPath">XmlPath</param>
        /// <param name="attrName">属性名</param>
        /// <param name="conditionAttr">条件属性名</param>
        /// <param name="conditionValue">The condition value.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public string GetNodeAttribute(string xmlPath, string attrName, string conditionAttr, string conditionValue)
        {
            XmlNode node = GetXmlNode(xmlPath, conditionAttr, conditionValue);
            if (node != null)
            {
                XmlAttribute xa = node.Attributes[attrName];
                if (xa != null)
                {
                    XmlAttribute cxa = node.Attributes[conditionAttr];
                    if (cxa.Value.ToLower() == conditionValue.ToLower())
                    {
                        return xa.Value;
                    }
                }
            }
            return "";
        }

    }
}
