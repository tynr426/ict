using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace ECF.Xml
{
    /// <summary>
    /// FullName： <see cref="ECF.Xml.XmlHelper"/>
    /// Summary ： XML helper Class 
    /// Verssion： 1.0.0.0 
    /// DateTime： 2012/6/5 11:51 
    /// CopyRight (c) by shaipe
    /// </summary>
    public class XmlHelper
    {
        #region XML→T
        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns>
        /// T
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
        /// T
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
        /// System.Collections.Generic.IList&lt;T&gt;
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

        /// <summary>
        /// Deserialize to entity list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns>
        /// T
        /// </returns>
        private static T DeserializeToEntityList<T>(string xml)
        {
            var method = typeof(XmlHelper).GetMethod("DeserializeToList", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static).MakeGenericMethod(typeof(T).GetGenericArguments()[0]);
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
    }
}
