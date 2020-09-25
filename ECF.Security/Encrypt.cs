using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ECF.Security
{
    /// <summary>
    ///   <see cref="ECF.Security.Encrypt"/>
    /// encrypt 多种加解密算法
    /// Author:  XP
    /// Created: 2011/9/9
    /// </summary>
    public class Encrypt
    {
        #region Base64加密

        /// <summary>
        /// Base64加密
        /// </summary>
        /// <param name="data">待加密数据.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>
        /// System.String
        /// </returns>
        /// <exception cref="Exception">Error in base64Encode + e.Message</exception>
        public static string EncodeBase64(string data, string encoding = "UTF-8")
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;
            try
            {
                byte[] encData_byte = new byte[data.Length];
                Encoding encoder = Encoding.GetEncoding(encoding);
                encData_byte = encoder.GetBytes(data);
                string encodedData = Convert.ToBase64String(encData_byte);
                return encodedData;
            }
            catch (Exception e)
            {
                //new ECFException("Error in base64Encode" + e.Message);
                return data;
            }
        }
        #endregion

        #region Base64解密
        /// <summary>
        /// Base64解密
        /// </summary>
        /// <param name="data">等解密数据.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>
        /// System.String
        /// </returns>
        /// <exception cref="Exception">Error in base64Decode + e.Message</exception>
        public static string DecodeBase64(string data, string encoding = "UTF-8")
        {
            if (string.IsNullOrEmpty(data)) return string.Empty;
            try
            {
                Encoding encoder = Encoding.GetEncoding(encoding);
                System.Text.Decoder uDecode = encoder.GetDecoder();

                byte[] todecode_byte = Convert.FromBase64String(data);
                int charCount = uDecode.GetCharCount(todecode_byte, 0, todecode_byte.Length);
                char[] decoded_char = new char[charCount];
                uDecode.GetChars(todecode_byte, 0, todecode_byte.Length, decoded_char, 0);
                string result = new String(decoded_char);
                return result;
            }
            catch (Exception e)
            {
                //new ECFException("Error in base64Decode" + e.Message);
                return data;
            }
        }

        #endregion

        #region sha1加密

        /// <summary>
        /// SHA1加密
        /// </summary>
        /// <param name="intput">输入字符串</param>
        /// <returns>加密后的字符串</returns>
        public static string Sha1(string intput)
        {
            try
            {
                byte[] StrRes = Encoding.Default.GetBytes(intput);
                HashAlgorithm mySHA = new SHA1CryptoServiceProvider();
                StrRes = mySHA.ComputeHash(StrRes);
                StringBuilder EnText = new StringBuilder();
                foreach (byte Byte in StrRes)
                {
                    EnText.AppendFormat("{0:x2}", Byte);
                }
                return EnText.ToString();
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return "";
            }
        }


        #endregion

        #region MD5函数

        /// <summary>
        /// MD5Hash函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5Hash(string str)
        {
            try
            {
                byte[] b = Encoding.Default.GetBytes(str);
                b = new MD5CryptoServiceProvider().ComputeHash(b);
                string ret = "";
                for (int i = 0; i < b.Length; i++)
                    ret += b[i].ToString("x").PadLeft(2, '0');
                return ret;
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return "";
            }
        }

        /// <summary>
        /// MD5 16位加密
        /// </summary>
        /// <param name="str">加密前的字符串</param>
        /// <returns>16位加密后的大写字符串</returns>
        public static string MD516(string str)
        {
            try
            {
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                string t2 = BitConverter.ToString(md5.ComputeHash(UTF8Encoding.Default.GetBytes(str)), 4, 8);
                t2 = t2.Replace("-", "");
                return t2;
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return "";
            }
        }

        /// <summary>
        /// MD5 32位加密
        /// </summary>
        /// <param name="str">加密前的字符串</param>
        /// <returns>32位加密后的大写字符串</returns>
        public static string MD532(string str)
        {
            try
            {
                string cl = str;
                string pwd = "";
                //实例化一个md5对像
                System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
                // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择
                byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
                //通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
                for (int i = 0; i < s.Length; i++)
                {
                    // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符
                    pwd = pwd + s[i].ToString("x2");

                }
                return pwd;
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return "";
            }
        }

        /// <summary>
        /// 得到文件的 MD5 验证码
        /// </summary>
        /// <param name="FPath">文件的路径</param>
        /// <returns>文件的MD5验证码，如果文件打开失败，则抛出异常。<br />如果获取md5码失败，则返回String.Empty值</returns>
        public static String GetFileMd5Code(String FPath)
        {
            String returnValue = String.Empty;
            MD5CryptoServiceProvider myMd5 = new MD5CryptoServiceProvider();
            FileStream myFileStream;
            myFileStream = new FileStream(FPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                returnValue = BitConverter.ToString((myMd5.ComputeHash(myFileStream)));
                returnValue = returnValue.Replace("-", "");
            }
            catch
            {
            }
            finally
            {
                myFileStream.Close();
            }
            return returnValue;
        }

        #endregion

        #region SHA256函数

        /// <summary>
        /// SHA256函数
        /// </summary>
        /// /// <param name="str">原始字符串</param>
        /// <returns>SHA256结果</returns>
        public static string SHA256(string str)
        {
            byte[] SHA256Data = Encoding.UTF8.GetBytes(str);
            SHA256Managed Sha256 = new SHA256Managed();
            byte[] Result = Sha256.ComputeHash(SHA256Data);
            return Convert.ToBase64String(Result);  //返回长度为44字节的字符串
        }

        #endregion

        #region UTF-8操作


        /// <summary>
        /// This method is used to convert the string from "%E6%9D%B1" to UTF8 Chinese Chracter
        /// </summary>
        /// <param name="theInput">The input.</param>
        /// <returns>System.String</returns>
        public static string ConvertGB2UTF8(string theInput)
        {
            try
            {
                //Only the string whhich starts with % will be handled
                if (theInput.StartsWith("%"))
                {
                    UTF8Encoding utf8 = new UTF8Encoding();

                    String theString = theInput;

                    Byte[] bytes = new byte[theString.Length * 3 / 9];

                    string[] split = theString.Split(Convert.ToChar("%"));
                    int i = 0;
                    foreach (string s in split)
                    {
                        string inputStr = s.Trim();
                        int theValue = 0;
                        if (inputStr.Length == 2)
                        {
                            //Get the high position
                            theValue = theValue + ConvertHexToDec(inputStr.Substring(0, 1)) * 16;

                            //Get the low position
                            theValue = theValue + ConvertHexToDec(inputStr.Substring(1, 1));
                            //Only the index of bytes array is less than the length
                            if (i < bytes.Length)
                            {
                                //bytes = (byte)theValue;
                            }
                            i++;
                        }
                    }

                    //Use the UTF8Encoding object to convert the bytes to string
                    string theResult = utf8.GetString(bytes);
                    return theResult;
                }
                else
                {
                    return theInput;
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return "";
            }
        }

        //Convert the Hex Code to Dec. A-15, E-14....
        /// <summary>
        /// 转换为十六进制 Convert hex to dec<br/>
        /// 2008-2-14 by Shaipe
        /// </summary>
        /// <param name="theValue">The value.</param>
        /// <returns>System.Int32</returns>
        public static int ConvertHexToDec(string theValue)
        {
            string myValue = theValue.Trim().ToUpper();

            switch (myValue)
            {
                case "1":
                    return 1;
                case "2":
                    return 2;
                case "3":
                    return 3;
                case "4":
                    return 4;
                case "5":
                    return 5;
                case "6":
                    return 6;
                case "7":
                    return 7;
                case "8":
                    return 8;
                case "9":
                    return 9;
                case "A":
                    return 10;
                case "B":
                    return 11;
                case "C":
                    return 12;
                case "D":
                    return 13;
                case "E":
                    return 14;
                case "F":
                    return 15;
                default:
                    return 0;
            }
        }

        #endregion

        
    }
}
