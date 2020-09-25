using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace ECF.Utilities
{
    /// <summary>
    ///   <see cref="ECF.Utilities.ReflectionUtil"/>
    /// 公用反射处理类
    /// Author:  XP
    /// Created: 2011/9/24
    /// </summary>
    public class ReflectionUtil
    {
        #region 获取程序集中的类型
        /// <summary>
        /// 获取本地程序集中的类型
        /// </summary>
        /// <param name="typeName">类型名称，范例格式："命名空间.类名",类型名称必须在本地程序集中</param>        
        public static Type GetType(string typeName)
        {
            try
            {
                return Type.GetType(typeName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取指定程序集中的类型
        /// </summary>
        /// <param name="assembly">指定的程序集</param>
        /// <param name="typeName">类型名称，范例格式："命名空间.类名",类型名称必须在assembly程序集中</param>
        /// <returns></returns>
        public static Type GetType(Assembly assembly, string typeName)
        {
            try
            {
                return assembly.GetType(typeName);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region 动态创建对象实例

        /// <summary>
        /// 动态创建对象实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="parameters">传递给构造函数的参数</param>        
        public static object CreateInstance(Type type, params object[] parameters)
        {
            try
            {
                //类型为空则返回
                if (Utils.IsNullOrEmpty(type))
                {
                    return null;
                }

                return Activator.CreateInstance(type, parameters);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 动态创建对象实例
        /// </summary>
        /// <param name="className">类名，格式:"命名空间.类名"</param>
        /// <param name="parameters">传递给构造函数的参数</param>        
        public static object CreateInstance(string className, params object[] parameters)
        {
            //获取类型
            Type type = GetType(className);

            return CreateInstance(type, parameters);
        }

        /// <summary>
        /// 创建类的实例
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="type">类型</param>
        /// <param name="parameters">传递给构造函数的参数</param>        
        public static T CreateInstance<T>(Type type, params object[] parameters) where T : class
        {
            return Utils.ConvertTo<T>(CreateInstance(type, parameters));
        }

        /// <summary>
        /// 创建类的实例
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="className">类名，格式:"命名空间.类名"</param>
        /// <param name="parameters">传递给构造函数的参数</param>        
        public static T CreateInstance<T>(string className, params object[] parameters) where T : class
        {
            return Utils.ConvertTo<T>(CreateInstance(className, parameters));
        }

        /// <summary>
        /// 动态创建对象实例
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="className">类名，格式:"命名空间.类名"</param>
        /// <param name="parameters">传递给构造函数的参数</param>        
        public static object CreateInstance(Assembly assembly, string className, params object[] parameters)
        {
            //获取类型
            Type type = assembly.GetType(className);

            return CreateInstance(type, parameters);
        }

        /// <summary>
        /// 动态创建对象实例
        /// </summary>
        /// <param name="assembly">程序集</param>
        /// <param name="className">类名，格式:"命名空间.类名"</param>
        /// <param name="parameters">传递给构造函数的参数</param>        
        public static T CreateInstance<T>(Assembly assembly, string className, params object[] parameters) where T : class
        {
            //获取类型
            Type type = assembly.GetType(className);

            return CreateInstance<T>(type, parameters);
        }


        #endregion

        /// <summary>
        /// 数据库类型转换
        /// </summary>
        /// <param name="val">待转换值.</param>
        /// <param name="pi">The pi.</param>
        /// <returns>
        /// System.Object
        /// </returns>
        public static object ChangeType(object val, PropertyInfo pi)
        {
            if (val == null) return null;
            Type type = pi.PropertyType;
            type = Utils.GetNullableType(type);

            if (val.ToString() == "" && (type.Name == "Int32" || type.Name == "Int64"))
            {
                return null;
            }

            if (type.Name == "bool" || type.Name == "Boolean")
            {
                if (val.ToString() == "1" || val.ToString().ToLower() == "true")
                {
                    val = true;
                }
                else
                {
                    val = false;
                }
            }
            else if (type.Name == "string" || type.Name == "String")
            {
                object[] attrs = pi.GetCustomAttributes(typeof(EntityPropertyAttribute), true);
                if (attrs.Length == 1)
                {
                    EntityPropertyAttribute pa = attrs.GetValue(0) as EntityPropertyAttribute;
                    if (pa != null)
                    {
                        if (pa.Format != PropertyFormat.Html)
                        {
                            val = Utils.EncodeHtml(val.ToString());
                        }
                        else if (pa.Format == PropertyFormat.UnixTime)
                        {
                            val = Utils.UnixTimeToTime(val.ToString());
                        }
                    }
                }
                else
                {
                    val = Utils.EncodeHtml(val.ToString());
                }
            }
            else if (type.Name.ToLower() == "int32")
            {
                val = Utils.ToInt(val);
            }
            else if(type.Name.ToLower() == "int64")//修改原因，是长整形转换成int会失败
            {
                val = Utils.ToInt64(val);
            }
            try
            {
                return Convert.ChangeType(val, type, CultureInfo.CurrentCulture);
            }
            catch //(Exception ex)
            {
                //new ECFException(ex.Message, ex);
                if (val != null)
                {
                    if (type.Name == "DateTime")
                        val = new DateTime(1900, 0, 0);
                    else if (type == typeof(int) || type == typeof(Decimal) || type == typeof(double))
                        val = 0;
                    return Convert.ChangeType(val, type, CultureInfo.CurrentCulture);
                }
                return null;
            }

        }

        /// <summary>
        /// 处理类型中的字定义属性
        /// </summary>
        /// <param name="val">The value.</param>
        /// <param name="pi">The pi.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string TypeToString(object val, PropertyInfo pi)
        {
            if (val == null) return null;
            string rval = val.ToString();
            object[] attrs = pi.GetCustomAttributes(typeof(EntityPropertyAttribute), true);
            if (attrs.Length > 0)
            {
                EntityPropertyAttribute pa = attrs.GetValue(0) as EntityPropertyAttribute;
                if (pa != null)
                {
                    if (pa.Format == PropertyFormat.UnixTime)
                    {
                        rval = Utils.ToUnixTime(Utils.ToDateTime(val)).ToString();
                    }
                    //else if (pa.Format == PropertyFormat.Html)
                    //{
                    //    rval = Utils.HtmlDecode(val.ToString());
                    //}
                }
            }
            return rval;

        }

        /// <summary>
        ///  通过反射获取程序集，不会对资源进行独占.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  09-30-2014
        /// </summary>
        /// <param name="path">程序集路径.</param>
        /// <returns>Assembly.</returns>
        public static Assembly GetAssembly(string path)
        {
            Assembly assembly = null;
            try
            {
                MemoryStream memStream;
                using (FileStream stream = new FileStream(path, FileMode.Open))
                {
                    using (memStream = new MemoryStream())
                    {
                        int res;
                        byte[] b = new byte[4096];
                        while ((res = stream.Read(b, 0, b.Length)) > 0)
                        {
                            memStream.Write(b, 0, b.Length);
                        }
                    }
                }
                assembly = Assembly.Load(memStream.ToArray());
            }
            catch (Exception ex)
            {
                //new ECFException(ex.Message, ex);
            }
            return assembly;
        }
    }
}
