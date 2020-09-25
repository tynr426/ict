using System;
using System.Xml;
using System.Collections.Specialized;

namespace ECF.Xml
{
    /// <summary>
    /// FullName： <see cref="ECF.Xml.NodeUtils"/>
    /// Summary ： Xml节点解析处理 
    /// Version： 1.0.0.0 
    /// DateTime： 2012/4/14 1:14 
    /// CopyRight (c) by shaipe
    /// </summary>
    public class NodeUtils
    {

        /// <summary>
        /// 将字符串转换为xml节点列表.
        /// </summary>
        /// <param name="xmlStr">xml字符串.</param>
        /// <returns></returns>
        public static XmlNodeList ToXmlNodeList(string xmlStr)
        {
            XmlNodeList xnl = null;
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.LoadXml(@"<?xml version=""1.0"" encoding=""utf-8"" ?><root>" + xmlStr + "</root>");
                XmlElement root = xmldoc.DocumentElement;
                xnl = root.ChildNodes;
            }
            catch (Exception ex)
            {
                new ECFException(ex.Message, ex);
            }
            return xnl;
        }

        /// <summary>
        /// Searches for the attribute with the specified name in this attributes list.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="name">The key</param>
        /// <returns></returns>
        public static string GetAttribute(NameValueCollection attributes, string name)
        {
            string value = attributes[name];
            if (value == null)
            {
                return string.Empty;
            }
            else
            {
                return value;
            }
        }

        /// <summary>
        /// Searches for the attribute with the specified name in this attributes list.
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="name">The key</param>
        /// <param name="def">The default value to be returned if the attribute is not found.</param>
        /// <returns></returns>
        public static string GetAttribute(NameValueCollection attributes, string name, string def)
        {
            string value = attributes[name];
            if (value == null)
            {
                return def;
            }
            else
            {
                return value;
            }
        }



        /// <summary>
        /// 将XmlNode节点转换为NameValue集合
        /// </summary>
        /// <param name="node">Xml节点</param>
        /// <returns></returns>
        public static NameValueCollection ParseAttributes(XmlNode node)
        {
            return ParseAttributes(node, null);
        }

        /// <summary>
        /// 将XmlNode节点转换为NameValue集合
        /// </summary>
        /// <param name="node">Xml节点</param>
        /// <param name="variables">变量集</param>
        /// <returns></returns>
        public static NameValueCollection ParseAttributes(XmlNode node, NameValueCollection variables)
        {
            NameValueCollection attributes = new NameValueCollection();
            try
            {
                int count = node.Attributes.Count;
                for (int i = 0; i < count; i++)
                {
                    XmlAttribute attribute = node.Attributes[i];
                    String value = ParsePropertyTokens(attribute.Value, variables);
                    attributes.Add(attribute.Name, value);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return attributes;
        }


        /// <summary>
        /// Replace properties by their values in the given string
        /// </summary>
        /// <param name="str"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static string ParsePropertyTokens(string str, NameValueCollection properties)
        {
            string OPEN = "{";
            string CLOSE = "}";

            string newString = str;
            try
            {
                if (newString != null && properties != null)
                {
                    int start = newString.IndexOf(OPEN);
                    int end = newString.IndexOf(CLOSE);

                    while (start > -1 && end > start)
                    {
                        string prepend = newString.Substring(0, start);
                        string append = newString.Substring(end + CLOSE.Length);

                        int index = start + OPEN.Length;
                        string propName = newString.Substring(index, end - index);
                        string propValue = properties.Get(propName);
                        if (propValue == null)
                        {
                            newString = prepend + propName + append;
                        }
                        else
                        {
                            newString = prepend + propValue + append;
                        }
                        start = newString.IndexOf(OPEN);
                        end = newString.IndexOf(CLOSE);
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return newString;
        }

    }
}
