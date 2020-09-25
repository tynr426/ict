using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;

namespace ECF
{
    /// <summary>
    /// FullName: <see cref="ECF.XmlExtend"/>
    /// Summary : Xml对象操作扩展
    /// Version: 2.1
    /// DateTime: 2015/5/15 
    /// CopyRight (c) Shaipe
    /// </summary>
    public static class XmlExtend
    {

        #region RemoveChild 移除子节点
        /// <summary>
        /// 移除子节点.
        /// </summary>
        /// <param name="filterNode">The filter node.</param>
        /// <param name="xmlNodeName">Name of the XML node.</param>
        /// <returns></returns>
        public static XmlNode RemoveChild(this XmlNode filterNode, string xmlNodeName)
        {
            try
            {
                XmlNodeList nodeList = filterNode.ChildNodes;
                foreach (XmlNode xn in nodeList)
                {
                    if (xn.Name == xmlNodeName)
                    {
                        filterNode.RemoveChild(xn);
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return filterNode;
        }
        #endregion

        #region AppendChild 添加子节点

        /// <summary>
        /// 添加子节点.
        /// </summary>
        /// <param name="xmlNode">The XML node.</param>
        /// <param name="extensionNode">The extension node.</param>
        /// <param name="xmlDocument">The XML document.</param>
        /// <param name="newNodeName"></param>
        /// <returns></returns>
        public static XmlNode AppendChild(this XmlNode xmlNode, XmlNode extensionNode, XmlDocument xmlDocument, string newNodeName)
        {
            if (extensionNode != null)
            {
                try
                {
                    if (xmlNode == null)
                    {
                        var newNode = xmlDocument.CreateElement(newNodeName);
                        var rooNode = xmlDocument.SelectSingleNode("//root");
                        if (rooNode != null) rooNode.AppendChild(newNode);
                        xmlNode = xmlDocument.SelectSingleNode("//" + newNodeName);
                    }

                    foreach (XmlNode childNode in extensionNode.ChildNodes)
                    {
                        if (xmlNode != null) xmlNode.AppendChild(childNode);
                    }
                }
                catch (Exception ex)
                {
                    new ECFException(ex);
                }
            }

            return xmlNode;
        }

        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="xmlNode"></param>
        /// <param name="xmlDocument"></param>
        /// <param name="parentNodeName"></param>
        /// <param name="childNodeName"></param>
        /// <returns></returns>
        public static XmlNode AppendChild(this XmlNode xmlNode, XmlDocument xmlDocument, string parentNodeName, string childNodeName)
        {
            XmlNode extensionNode = xmlDocument.SelectSingleNode("//" + childNodeName);
            if (extensionNode != null)
            {
                try
                {
                    if (xmlNode == null)
                    {
                        var newNode = xmlDocument.CreateElement(parentNodeName);
                        var rooNode = xmlDocument.SelectSingleNode("//root");
                        if (rooNode != null) rooNode.AppendChild(newNode);
                        xmlNode = xmlDocument.SelectSingleNode("//" + parentNodeName);
                    }

                    foreach (XmlNode childNode in extensionNode.ChildNodes)
                    {
                        if (xmlNode != null) xmlNode.AppendChild(childNode);
                    }
                }
                catch (Exception ex)
                {
                    new ECFException(ex);
                }
            }

            return xmlNode;
        }

        #endregion

        #region AppendChild 添加子节点
        /// <summary>
        /// 添加子节点
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="nodeValue">The node value.</param>
        /// <returns>
        /// XmlDocument
        /// </returns>
        public static XmlDocument AppendChild(this XmlDocument xmlDocument, string nodeName, object nodeValue)
        {
            XmlElement elem = xmlDocument.CreateElement(nodeName);
            XmlText text = xmlDocument.CreateTextNode(nodeValue.ToString());

            if (xmlDocument.DocumentElement == null)
            {
                XmlElement xmlelem = xmlDocument.CreateElement("root");
                xmlDocument.AppendChild(xmlelem);
            }

            if (xmlDocument.DocumentElement != null)
            {
                xmlDocument.DocumentElement.AppendChild(elem);
                xmlDocument.DocumentElement.LastChild.AppendChild(text);
            }


            return xmlDocument;
        }

        /// <summary>
        /// 添加到节点
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <param name="childXml">The child XML.</param>
        /// <returns>
        /// XmlDocument
        /// </returns>
        public static XmlDocument AppendChild(this XmlDocument xmlDocument, string nodeName, XmlDocument childXml)
        {

            try
            {
                XmlElement elem = xmlDocument.CreateElement(nodeName);
                var selectSingleNode = childXml.SelectSingleNode("root");

                if (selectSingleNode != null)
                {
                    if (xmlDocument.DocumentElement == null)
                    {
                        XmlElement xmlelem = xmlDocument.CreateElement("root");
                        xmlDocument.AppendChild(xmlelem);
                    }

                    if (xmlDocument.DocumentElement != null)
                    {
                        XmlNodeList xmlNodeList = selectSingleNode.ChildNodes;

                        foreach (XmlNode xmlNode in xmlNodeList)
                        {
                            XmlNode childNode = xmlDocument.CreateElement(xmlNode.Name);
                            childNode.InnerText = xmlNode.InnerText;
                            elem.AppendChild(childNode);
                        }

                        if (xmlDocument.DocumentElement != null) xmlDocument.DocumentElement.AppendChild(elem);
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return xmlDocument;
        }

        #endregion

        #region GetValue 獲取XML節點值

        /// <summary> 
        /// 獲取XML節點值
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <param name="nodeName"></param>
        /// <returns></returns>
        public static string GetValue(this XmlDocument xmlDocument, string nodeName)
        {
            string value = string.Empty;

            try
            {
                var elementById = xmlDocument.GetElementsByTagName(nodeName);
                if (elementById.Count > 0)
                {
                    var node = elementById[0];
                    if (node.ChildNodes.Count == 1)
                    {
                        switch (node.FirstChild.NodeType)
                        {
                            case XmlNodeType.CDATA:
                                value = node.FirstChild.Value;
                                break;
                            case XmlNodeType.Text:
                                value = node.InnerText;
                                break;
                            default:
                                value = node.InnerXml;
                                break;
                        }
                    }
                    else
                    {
                        value = node.InnerXml;
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return value;
        }

        /// <summary>
        /// 獲取XML節點值.
        /// </summary>
        /// <typeparam name="T">结果类型</typeparam>
        /// <param name="xmlDocument">The XML document.</param>
        /// <param name="nodeName">Name of the node.</param>
        /// <returns></returns>
        public static T GetValue<T>(this XmlDocument xmlDocument, string nodeName)
        {
            T result = default(T);
            try
            {
                var value = xmlDocument.GetValue(nodeName);

                if (value == null || string.IsNullOrEmpty(value))
                {
                    return result;
                }

                result = (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return result;
        }

        /// <summary>
        /// Get value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlNode">The XML node.</param>
        /// <returns>
        /// T
        /// </returns>
        public static T GetValue<T>(this XmlNode xmlNode)
        {
            T result = (T)Convert.ChangeType(xmlNode.InnerText, typeof(T));

            return result;
        }

        /// <summary>
        /// 获取XmlNode子节点的InnerText
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xmlNode"></param>
        /// <param name="nodeName">子节点名称</param>
        /// <returns></returns>
        public static T GetSingleNodeValue<T>(this XmlNode xmlNode, string nodeName)
        {
            if (xmlNode != null && xmlNode.SelectSingleNode(nodeName) != null)
                return (T)Convert.ChangeType(xmlNode.SelectSingleNode(nodeName).InnerText, typeof(T));
            else
                return default(T);
        }

        #endregion

        #region GetAttribute 获取属性值
        /// <summary>
        /// 获取属性值.
        /// </summary>
        /// <param name="xmldNode">The XMLD node.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns></returns>
        public static string GetAttribute(this XmlNode xmldNode, string attributeName)
        {
            string result = string.Empty;
            if (xmldNode != null && xmldNode.Attributes != null)
                result = xmldNode.Attributes[attributeName].Value;
            return result;
        }
        #endregion

        #region 获取XMLNode的XMLElement的值
        /// <summary>
        /// 获取XMLNode的XMLElement的值.
        /// </summary>
        /// <param name="xmldNode"></param>
        /// <param name="xmlElementName">Name of the XML element.</param>
        /// <returns></returns>
        public static T GetElementValue<T>(this XmlNode xmldNode, string xmlElementName)
        {
            XmlElement xmlElement = xmldNode[xmlElementName];

            object xmlElementValue = new object();

            if (xmlElement != null)
                xmlElementValue = xmlElement.InnerText;

            T result = (T)Convert.ChangeType(xmlElementValue, typeof(T));

            return result;
        }
        #endregion

        #region SetValue 設置XML節點值
        /// <summary>
        /// 設置XML節點值.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <param name="sourceName">Name of the source.</param>
        /// <param name="destinationName">Name of the destination.</param>
        /// <returns></returns>
        public static XmlDocument SetValue(this XmlDocument xmlDocument, string sourceName, string destinationName)
        {
            var destination = xmlDocument.GetElementsByTagName(destinationName);

            if (destination.Count == 0)
            {
                xmlDocument = xmlDocument.AppendChild(destinationName, xmlDocument.GetValue(sourceName));
            }
            else
            {
                destination[0].InnerText = xmlDocument.GetValue(sourceName);
            }

            return xmlDocument;
        }
        #endregion
    }
}
