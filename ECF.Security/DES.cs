using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ECF.Security
{
    /// <summary>
    ///   <see cref="ECF.Security.DES"/>
    /// DES加解密算法
    /// Author:  XP
    /// Created: 2011/9/9
    /// </summary>
    public class DES
    {
        #region 定义密钥
        /// <summary>
        /// 默认密钥
        /// </summary>
        private static string _key = "shaipexp";
        /// <summary>
        /// 密码钥匙，必须为8位
        /// </summary>
        public static string Key
        {
            get { return _key; }
            set { _key = value; }
        }
        #endregion

        #region Des加解密

        /// <summary>
        ///  Encode 
        ///  2008-12-11 10:47 by XP
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>System.String</returns>
        public static string Encode(string str)
        {
            return Encode(str, null);
        }

        /// <summary>
        /// Des加密
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <param name="pwdKey">密钥.</param>
        /// <returns>
        /// 加密后的的字符串
        /// </returns>
        public static string Encode(string str, string pwdKey)
        {
            try
            {
                if (!String.IsNullOrEmpty(pwdKey))
                {
                    Key = pwdKey;
                }
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.Default.GetBytes(str);
                des.Key = ASCIIEncoding.ASCII.GetBytes(_key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(_key);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                StringBuilder ret = new StringBuilder();
                foreach (byte b in ms.ToArray())
                {
                    ret.AppendFormat("{0:X2}", b);
                }
                ret.ToString();
                if (!String.IsNullOrEmpty(pwdKey))
                {
                    Key = "shaipexp";
                }
                return ret.ToString();
            }
            catch
            {
                return "";
            }
        }

        /// <summary>
        ///  Decode 
        ///  2008-12-11 10:47 by XP
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns>System.String</returns>
        public static string Decode(string str)
        {
            return Decode(str, null);
        }

        /// <summary>
        /// Des解密算法
        /// </summary>
        /// <param name="str">要解密的字符串</param>
        /// <param name="pwdKey">The PWD key.</param>
        /// <returns>解密后的字符串</returns>
        public static string Decode(string str, string pwdKey)
        {
            try
            {
                if (!String.IsNullOrEmpty(pwdKey))
                {
                    Key = pwdKey;
                }

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = new byte[str.Length / 2];
                for (int x = 0; x < str.Length / 2; x++)
                {
                    int i = (Convert.ToInt32(str.Substring(x * 2, 2), 16));
                    inputByteArray[x] = (byte)i;
                }

                des.Key = ASCIIEncoding.ASCII.GetBytes(_key);
                des.IV = ASCIIEncoding.ASCII.GetBytes(_key);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();

                StringBuilder ret = new StringBuilder();
                if (!String.IsNullOrEmpty(pwdKey))
                {
                    Key = "shaipexp";
                }
                return Encoding.Default.GetString(ms.ToArray());
            }
            catch
            {
                return "";
            }
        }
        #endregion
    }
}
