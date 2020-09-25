using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ECF
{
    /// <summary>
    /// 字节序列化
    /// 2017-05-08 by shaipe
    /// </summary>
    public static class ByteSerializer
    {
        /// <summary>
        /// 序列化为二进制
        /// </summary>
        /// <param name="model">实体</param>
        /// <returns></returns>
        public static byte[] Serialize(object model)
        {

            if (model == null)
            {
                return null;
            }
            try
            {
                // 判断对象是否可以序列化
                if (model.GetType().IsSerializable)
                {
                    using (MemoryStream mStream = new MemoryStream())
                    {
                        BinaryFormatter formatter = new BinaryFormatter();

                        formatter.Serialize(mStream, model);

                        byte[] serializeBuffer = mStream.GetBuffer();
                        mStream.Flush();
                        return serializeBuffer;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex.Message, ex);
                return new byte[] { };
            }
        }

        /// <summary>
        /// 反序列化为实体
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <returns>
        /// System.Object
        /// </returns>
        public static object Deserialize(byte[] bytes)
        {
            try
            {
                if (bytes == null || bytes.Length == 0)
                {
                    return null;
                }

                using (MemoryStream mStream = new MemoryStream(bytes, 0, bytes.Length))
                {

                    BinaryFormatter formatter = new BinaryFormatter();

                    return formatter.Deserialize(mStream);
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// Deserialize
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="bytes">The bytes.</param>
        /// <returns>
        /// T
        /// </returns>
        public static T Deserialize<T>(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0)
            {
                return default(T);
            }

            try
            {
                using (MemoryStream mStream = new MemoryStream(bytes, 0, bytes.Length))
                {
                    BinaryFormatter formatter = new BinaryFormatter();

                    object o = formatter.Deserialize(mStream);
                    if (o != null)
                    {
                        return (T)o;
                    }
                }
            }
            catch (Exception ex)
            {
                ////new ECFException(ex.Message, ex);
            }

            return default(T);
        }
    }
}
