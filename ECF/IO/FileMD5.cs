using System;
namespace ECF.IO
{
    /// <summary>
    ///   <see cref="ECF.IO.FileMD5"/>
    /// 给文件一个Md5值,以方便验证文件是否是相同的
    /// Author:  XP
    /// Created: 2011/9/9
    /// </summary>
    public class FileMD5
    {
        #region MD5File 计算文件的 MD5 值 
        /// <summary>        
        /// 计算文件的 MD5 值        
        /// </summary>        
        /// <param name="fileName">要计算 MD5 值的文件名和路径</param>        
        /// <returns>MD5 值16进制字符串</returns>        
        public static string MD5File(string fileName)
        {
            return HashFile(fileName, "md5");
        } 
        #endregion

        #region SHA1File 计算文件的 sha1 值
        /// <summary>        
        /// 计算文件的 sha1 值       
        /// </summary>        
        /// <param name="fileName">要计算 sha1 值的文件名和路径</param>        
        /// <returns>sha1 值16进制字符串</returns>        
        public static string SHA1File(string fileName)
        {
            return HashFile(fileName, "sha1");
        } 
        #endregion

        #region HashFile 计算文件的哈希值
        /// <summary>        
        /// 计算文件的哈希值        
        /// </summary>        
        /// <param name="fileName">要计算哈希值的文件名和路径</param>        
        /// <param name="algName">算法:sha1,md5</param>        
        /// <returns>哈希值16进制字符串</returns>        
        private static string HashFile(string fileName, string algName)
        {
            try
            {
                if (!System.IO.File.Exists(fileName))
                    return string.Empty;
                System.IO.FileStream fs = new System.IO.FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                byte[] hashBytes = HashData(fs, algName);
                fs.Close();
                return ByteArrayToHexString(hashBytes);
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return string.Empty;
            }
        }
        #endregion

        #region HashData 计算哈希值
        /// <summary>        
        /// 计算哈希值        
        /// </summary>        
        /// <param name="stream">要计算哈希值的 Stream</param>        
        /// <param name="algName">算法:sha1,md5</param>        
        /// <returns>哈希值字节数组</returns>        
        private static byte[] HashData(System.IO.Stream stream, string algName)
        {
            try
            {
                System.Security.Cryptography.HashAlgorithm algorithm;
                if (algName == null)
                {
                    throw new ArgumentNullException("algName 不能为 null");
                }
                if (string.Compare(algName, "sha1", true) == 0)
                {
                    algorithm = System.Security.Cryptography.SHA1.Create();
                }
                else
                {
                    if (string.Compare(algName, "md5", true) != 0)
                    {
                        throw new Exception("algName 只能使用 sha1 或 md5");
                    }
                    algorithm = System.Security.Cryptography.MD5.Create();
                }
                return algorithm.ComputeHash(stream);
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return new byte[0];
            }
        }
        #endregion

        #region ByteArrayToHexString
        /// <summary>
        /// 字节数组转换为16进制表示的字符串 
        /// </summary>
        /// <param name="buf">字节</param>
        /// <returns></returns>
        private static string ByteArrayToHexString(byte[] buf)
        {
            return BitConverter.ToString(buf).Replace("-", "");
        }

        #endregion
    }
}
