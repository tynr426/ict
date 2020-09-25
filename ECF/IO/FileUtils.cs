using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
namespace ECF.IO
{
    /// <summary>
    /// FullName： <see cref="ECF.IO.FileUtils"/>
    /// Summary ： 文件操作类 
    /// Version： 1.0.0.0 
    /// DateTime： 2011/12/3 20:18 
    /// CopyRight (c) by shaipe
    /// </summary>
    public class FileUtils
    {
        #region ReadFile 获得模板中的内容

        /// <summary>
        /// 获得模板中的内容
        /// </summary>
        /// <param name="filePath">模板路径</param>
        /// <returns></returns>
        public static string ReadFile(string filePath)
        {
            FileStream fs = null;
            try
            {
                fs = new FileStream(filePath, FileMode.Open);
                Encoding code = GetEncoding(fs);
                StreamReader sr = new StreamReader(fs, code);
                string str = sr.ReadToEnd(); // 读取文件
                sr.Close();
                sr.Dispose();
                fs.Close();
                fs.Dispose();
                return str;
            }
            catch
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                return "";
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="code">The code.</param>
        /// <returns>System.String</returns>
        public static string ReadFile(string filePath, Encoding code)
        {
            try
            {
                StreamReader sr = null;
                string str = "";
                sr = new StreamReader(filePath, code);
                str = sr.ReadToEnd(); // 一次性读取文件
                sr.Close();
                return str;
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region GetContentType 根据文件名获取文件类型
        /// <summary>
        /// 根据文件名获取文件类型
        /// </summary>
        /// <param name="fileName">文件名.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string GetContentType(string fileName)
        {
            string contentType = "application/octetstream";
            try
            {
                string ext = Path.GetExtension(fileName).ToLower();
                RegistryKey registryKey = Registry.ClassesRoot.OpenSubKey(ext);

                if (registryKey != null && registryKey.GetValue("Content Type") != null)
                {
                    contentType = registryKey.GetValue("Content Type").ToString();
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return contentType;
        }
        #endregion

        #region GetEncoding 获取文件的编码类型

        /// <summary>
        /// 获取文件编码
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public static Encoding GetEncoding(string FilePath)
        {
            // 默认编码
            Encoding targetEncoding = Encoding.Default;

            try
            {
                if (String.IsNullOrEmpty(FilePath))
                    throw new ArgumentNullException("参数文件路径不能为空");

                if (!File.Exists(FilePath))
                    throw new FileNotFoundException("文件\"" + FilePath + "\"不存在");

                //保存文件流的前4个字节
                byte byte1 = 0, byte2 = 0, byte3 = 0, byte4 = 0;

                using (Stream stream = new FileInfo(FilePath).Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (stream != null && stream.Length >= 2)
                    {
                        // 保存当前Seek位置
                        long origPos = stream.Seek(0, SeekOrigin.Begin);

                        stream.Seek(0L, SeekOrigin.Begin);

                        byte1 = Convert.ToByte(stream.ReadByte());
                        byte2 = Convert.ToByte(stream.ReadByte());

                        if (stream.Length >= 3)
                        {
                            byte3 = Convert.ToByte(stream.ReadByte());
                        }
                        if (stream.Length >= 4)
                        {
                            byte4 = Convert.ToByte(stream.ReadByte());
                        }

                        // 根据文件流的前4个字节判断Encoding Unicode  {0xFF,  0xFE}; BE-Unicode  {0xFE,  0xFF}; UTF8  =  {0xEF,  0xBB,  0xBF};
                        if (byte1 == 0xFE && byte2 == 0xFF)
                        {
                            targetEncoding = Encoding.BigEndianUnicode;
                        }
                        if (byte1 == 0xFF && byte2 == 0xFE && byte3 != 0xFF)
                        {
                            targetEncoding = Encoding.Unicode;
                        }
                        if (byte1 == 0xEF && byte2 == 0xBB && byte3 == 0xBF)
                        {
                            targetEncoding = Encoding.UTF8;
                        }

                        //恢复Seek位置              
                        stream.Seek(origPos, SeekOrigin.Begin);
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return targetEncoding;
        }


        /// <summary>
        /// 获取文件流的编码格式
        /// </summary>
        /// <param name="fs">文件流</param>
        /// <returns></returns>
        public static System.Text.Encoding GetEncoding(FileStream fs)
        {
            /*byte[] Unicode=new byte[]{0xFF,0xFE};  
            byte[] UnicodeBIG=new byte[]{0xFE,0xFF};  
            byte[] UTF8=new byte[]{0xEF,0xBB,0xBF};*/

            try
            {
                BinaryReader r = new BinaryReader(fs, System.Text.Encoding.Default);
                byte[] ss = r.ReadBytes(3);
                r.Close();
                //编码类型 Coding=编码类型.ASCII;   
                if (ss[0] >= 0xEF)
                {
                    if (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF)
                    {
                        return System.Text.Encoding.UTF8;
                    }
                    else if (ss[0] == 0xFE && ss[1] == 0xFF)
                    {
                        return System.Text.Encoding.BigEndianUnicode;
                    }
                    else if (ss[0] == 0xFF && ss[1] == 0xFE)
                    {
                        return System.Text.Encoding.Unicode;
                        //Encoding.
                    }
                    else
                    {
                        return System.Text.Encoding.Default;
                    }
                }
                else
                {
                    return System.Text.Encoding.Default;
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
                return System.Text.Encoding.Default;
            }
        }

        #endregion

        #region SaveFile 保存文件 Saves the file.

        /// <summary>
        /// 保存内容到文件.
        /// </summary>
        /// <param name="filePath">文件路径.</param>
        /// <param name="content">需要保存的内容.</param>
        /// <returns></returns>
        public static bool SaveFile(string filePath, string content)
        {
            Encoding encode = System.Text.Encoding.GetEncoding("gb2312");
            return SaveFile(filePath, content, encode);
        }
        /// <summary>
        /// 保存内容到文件.
        /// </summary>
        /// <param name="filePath">文件路径.</param>
        /// <param name="content">需要保存的内容.</param>
        /// <param name="code">文件的编码方式.</param>
        /// <returns></returns>
        public static bool SaveFile(string filePath, string content, Encoding code)
        {

            StreamWriter sw = null;
            try
            {
                if (!Directory.Exists(CatalogPath(filePath)))
                {
                    Directory.CreateDirectory(CatalogPath(filePath));
                }
                if (File.Exists(filePath))
                {
                    File.WriteAllText(filePath, content, code);
                }
                else
                {
                    sw = new StreamWriter(filePath, false, code);
                    sw.Write(content);
                    sw.Flush();
                    sw.Close();
                }
                return true;
            }
            catch (Exception ex)
            {
                new ECFException(ex.Message, ex);
                return false;
            }
        }


        #endregion

        #region CatalogPath 转换上一层路径 Returnlpathes the specified STR.
        /// <summary>
        /// 转换上一层路径 Returnlpathes the specified STR.
        /// </summary>
        /// <param name="str">The STR.</param>
        /// <returns></returns>
        public static string CatalogPath(string str)
        {
            string code = "";
            try
            {
                if (str.IndexOf("\\") > 0)
                {
                    str = str.Replace("\\", "/");
                }
                string[] sArray = str.Split('/');
                for (int ii = 0; ii < sArray.Length - 1; ii++)
                    code += sArray[ii] + "/";
                if (code == "/")
                    code = "";
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return code;
        }
        #endregion


        #region ExistDirectory 检测指定目录是否存在
        /// <summary>
        /// 检测指定目录是否存在
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>        
        public static bool ExistDirectory(string directoryPath)
        {
            return Directory.Exists(directoryPath);
        }
        #endregion

        #region ExistFile 检测指定文件是否存在
        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>        
        public static bool ExistFile(string filePath)
        {
            return File.Exists(filePath);
        }
        #endregion

        #region GetFileNames 获取指定目录中的文件列表
        /// <summary>
        /// 获取指定目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static string[] GetFilePaths(string directoryPath)
        {
            //如果目录不存在，则抛出异常
            if (!ExistDirectory(directoryPath))
            {
                new ECFException("目录不存在");
                return new string[] { };
            }

            //获取文件列表
            return Directory.GetFiles(directoryPath);
        }


        /// <summary>
        /// 获取文件名.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径 .</param>
        /// <returns></returns>
        public static string[] GetFileNames(string directoryPath)
        {
            List<string> names = new List<string>();
            try
            {
                string[] temps = GetFilePaths(directoryPath);

                foreach (string t in temps)
                {
                    names.Add(Path.GetFileName(t));
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }

            return names.ToArray();
        }

        /// <summary>
        /// 获取指定目录及子目录中所有文件列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetFileNames(string directoryPath, string searchPattern, bool isSearchChild)
        {
            //如果目录不存在，则抛出异常
            if (!ExistDirectory(directoryPath))
            {
                throw new FileNotFoundException();
            }

            try
            {
                if (isSearchChild)
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetFiles(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region GetDirectories 获取指定目录中的子目录列表
        /// <summary>
        /// 获取指定目录中所有子目录列表,若要搜索嵌套的子目录列表,请使用重载方法.
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>        
        public static string[] GetDirectories(string directoryPath)
        {
            try
            {
                return Directory.GetDirectories(directoryPath);
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取指定目录及子目录中所有子目录列表
        /// </summary>
        /// <param name="directoryPath">指定目录的绝对路径</param>
        /// <param name="searchPattern">模式字符串，"*"代表0或N个字符，"?"代表1个字符。
        /// 范例："Log*.xml"表示搜索所有以Log开头的Xml文件。</param>
        /// <param name="isSearchChild">是否搜索子目录</param>
        public static string[] GetDirectories(string directoryPath, string searchPattern, bool isSearchChild)
        {
            try
            {
                if (isSearchChild)
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.AllDirectories);
                }
                else
                {
                    return Directory.GetDirectories(directoryPath, searchPattern, SearchOption.TopDirectoryOnly);
                }
            }
            catch (IOException ex)
            {
                throw ex;
            }
        }
        #endregion

        #region CreateDirectory  创建一个目录
        /// <summary>
        /// 创建一个目录
        /// </summary>
        /// <param name="directoryPath">目录的绝对路径</param>
        public static void CreateDirectory(string directoryPath)
        {
            //有效性验证
            if (Utils.IsNullOrEmpty(directoryPath))
            {
                return;
            }

            try
            {
                //如果目录不存在则创建该目录
                if (!ExistDirectory(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }
        #endregion

        #region CopyDirectory 文件夹复制
        /// <summary>
        /// 文件夹复制 by xp 20120724
        /// </summary>
        /// <param name="sourceDirName">原始路径</param>
        /// <param name="destDirName">目标路径</param>
        /// <returns></returns>
        public static void CopyDirectory(string sourceDirName, string destDirName)
        {
            try
            {
                if (sourceDirName.Substring(sourceDirName.Length - 1) != "\\")
                {
                    sourceDirName = sourceDirName + "\\";
                }
                if (destDirName.Substring(destDirName.Length - 1) != "\\")
                {
                    destDirName = destDirName + "\\";
                }
                if (Directory.Exists(sourceDirName))
                {
                    if (!Directory.Exists(destDirName))
                    {
                        Directory.CreateDirectory(destDirName);
                    }
                    foreach (string item in Directory.GetFiles(sourceDirName))
                    {
                        File.Copy(item, destDirName + Path.GetFileName(item), true);
                    }
                    foreach (string item in Directory.GetDirectories(sourceDirName))
                    {
                        CopyDirectory(item, destDirName + item.Substring(item.LastIndexOf("\\") + 1));
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
        }
        #endregion

        #region GetFileLastWriteTime 获取文件最新写入新内容的时间
        /// <summary>
        /// 获取文件最新写入新内容的时间
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public static DateTime GetFileLastWriteTime(string FilePath)
        {
            DateTime dt = DateTime.Now;

            try
            {
                if (String.IsNullOrEmpty(FilePath))
                {
                    new ECFException("参数文件路径不能为空");
                }

                if (!File.Exists(FilePath))
                {
                    new ECFException("文件\"" + FilePath + "\"不存在");
                }

                dt = new FileInfo(FilePath).LastWriteTime;
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return dt;
        }

        #endregion

        #region TryReadFile 获得模板中的内容

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <param name="FileEncode">输出当前文件的文件编码</param>
        /// <param name="FileContent">输出文件的内容</param>
        /// <param name="ErrorMessage">读取文件的错误信息</param>
        /// <returns>文件内容</returns>
        public static bool TryReadFile(string FilePath, out Encoding FileEncode, out string FileContent, out string ErrorMessage)
        {
            FileEncode = Encoding.Default;
            FileContent = String.Empty;
            ErrorMessage = String.Empty;

            if (!File.Exists(FilePath))
            {
                ErrorMessage = "文件\"" + FilePath + "\"不存在";
                return false;
            }

            StreamReader sr = null;

            try
            {
                FileEncode = GetEncoding(FilePath);
                sr = new StreamReader(FilePath, FileEncode);
                FileContent = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            finally
            {
                if (sr != null)
                    sr.Dispose();

                sr = null;
            }

            return true;
        }



        #endregion

        /// <summary>
        /// 读取二进制文件流
        /// </summary>
        /// <param name="filePath">文件路径.</param>
        /// <returns>
        /// System.Byte[]
        /// </returns>
        public static byte[] ReadFileBytes(string filePath)

        {
            FileStream pFileStream = null;
            byte[] pReadByte = new byte[0];
            try
            {
                pFileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                BinaryReader r = new BinaryReader(pFileStream);
                //将文件指针设置到文件开
                r.BaseStream.Seek(0, SeekOrigin.Begin);
                pReadByte = r.ReadBytes((int)r.BaseStream.Length);
                return pReadByte;
            }
            catch
            {
                return pReadByte;
            }
            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
        }

        /// <summary>
        /// 写byte[]到fileName
        /// </summary>
        /// <param name="bytes">The p read byte.</param>
        /// <param name="filePath">Name of the file.</param>
        /// <returns>
        /// System.Boolean
        /// </returns>
        public static bool WriteFileBytes(byte[] bytes, string filePath)
        {
            FileStream pFileStream = null;

            try
            {
                pFileStream = new FileStream(filePath, FileMode.OpenOrCreate);
                pFileStream.Write(bytes, 0, bytes.Length);
            }
            catch
            {
                return false;
            }
            finally
            {
                if (pFileStream != null)
                    pFileStream.Close();
            }
            return true;
        }
    }
}
