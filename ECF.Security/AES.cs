using System;
using System.Security.Cryptography;
using System.Text;

namespace ECF.Security
{
    /// <summary>
    ///   <see cref="ECF.Security.AES"/>
    /// AES加解密算法
    /// Author:  XP
    /// Created: 2011/9/9
    /// </summary>
    public class AES
    {
        #region AES加解密字符处理
        private static byte[] Keys = { 0x41, 0x72, 0x65, 0x79, 0x6F, 0x75, 0x6D, 0x79, 0x53, 0x6E, 0x6F, 0x77, 0x6D, 0x61, 0x6E, 0x3F };

        private static string key = "ABCDEFGHIJKLMNOPQRWTUVWXYZSHAIPE";
        /// <summary>
        /// AES加密处理
        /// </summary>
        /// <param name="encryptString">The encrypt string.</param>
        /// <returns>System.String</returns>
        public static string Encode(string encryptString)
        {
            try
            {
                string encryptKey = Utils.GetSubString(key, 32, "");
                encryptKey = encryptKey.PadRight(32, ' ');

                RijndaelManaged rijndaelProvider = new RijndaelManaged();
                rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32));
                rijndaelProvider.IV = Keys;
                ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

                byte[] inputData = Encoding.UTF8.GetBytes(encryptString);
                byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                return Convert.ToBase64String(encryptedData);
            }
            catch (Exception ex)
            {
                ////new ECFException(ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="decryptString">The decrypt string.</param>
        /// <returns>System.String</returns>
        public static string Decode(string decryptString)
        {
            try
            {
                string decryptKey = Utils.GetSubString(key, 32, "");
                decryptKey = decryptKey.PadRight(32, ' ');

                RijndaelManaged rijndaelProvider = new RijndaelManaged();
                rijndaelProvider.Key = Encoding.UTF8.GetBytes(decryptKey);
                rijndaelProvider.IV = Keys;
                ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

                byte[] inputData = Convert.FromBase64String(decryptString);
                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                return Encoding.UTF8.GetString(decryptedData);
            }
            catch
            {
                return "";
            }

        }
        #endregion


        #region AES 加解密数据流
        /// <summary>
        /// AES加密处理
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static byte[] EncodeBytes(byte[] inputData)
        {
            try
            {
                string encryptKey = Utils.GetSubString(key, 32, "");
                encryptKey = encryptKey.PadRight(32, ' ');

                RijndaelManaged rijndaelProvider = new RijndaelManaged();
                rijndaelProvider.Key = Encoding.UTF8.GetBytes(encryptKey.Substring(0, 32));
                rijndaelProvider.IV = Keys;
                ICryptoTransform rijndaelEncrypt = rijndaelProvider.CreateEncryptor();

                //byte[] inputData = Encoding.UTF8.GetBytes(encryptString);
                byte[] encryptedData = rijndaelEncrypt.TransformFinalBlock(inputData, 0, inputData.Length);
                return encryptedData;
            }
            catch
            {
                return new byte[] { };
            }
        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="inputData">The input data.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static byte[] DecodeBytes(byte[] inputData)
        {
            try
            {
                string decryptKey = Utils.GetSubString(key, 32, "");
                decryptKey = decryptKey.PadRight(32, ' ');

                RijndaelManaged rijndaelProvider = new RijndaelManaged();
                rijndaelProvider.Key = Encoding.UTF8.GetBytes(decryptKey);
                rijndaelProvider.IV = Keys;
                ICryptoTransform rijndaelDecrypt = rijndaelProvider.CreateDecryptor();

                //byte[] inputData = Convert.FromBase64String(decryptString);
                byte[] decryptedData = rijndaelDecrypt.TransformFinalBlock(inputData, 0, inputData.Length);

                return decryptedData;
            }
            catch
            {
                return new byte[] { };
            }

        }
        #endregion


    }
}
