using System;

namespace ECF
{
    /// <summary>
    ///   <see cref="ECF.Utils"/>
    /// 数据格式转换
    /// Author:  XP
    /// Created: 2011/11/21
    /// </summary>
    public partial class Utils
    {
        #region 将数据转换为指定类型

        #region 重载1
        /// <summary>
        /// 将数据转换为指定类型
        /// </summary>
        /// <param name="data">转换的数据</param>
        /// <param name="targetType">转换的目标类型</param>
        public static object ConvertTo(object data, Type targetType)
        {
            //如果数据为空，则返回
            if (Utils.IsNullOrEmpty(data))
            {
                return null;
            }

            try
            {
                //如果数据实现了IConvertible接口，则转换类型
                if (data is IConvertible)
                {
                    return Convert.ChangeType(data, targetType);
                }
                else
                {
                    return data;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #region 重载2
        /// <summary>
        /// 将对象转换为指定类型
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="obj">类型转换.</param>
        /// <returns>
        /// T
        /// </returns>
        public static T ConvertTo<T>(object obj) where T : class
        {
            //如果数据为空，则返回
            if (Utils.IsNullOrEmpty(obj))
            {
                return null;
            }

            try
            {
                //如果数据是T类型，则直接转换
                if (obj is T)
                {
                    return (T)obj;
                }

                //如果数据实现了IConvertible接口，则转换类型
                if (obj is IConvertible)
                {
                    return (T)Convert.ChangeType(obj, typeof(T));
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        #endregion

        #region 将数据转换为指定类型的值
        /// <summary>
        /// 将数据转换为指定类型的值，可以是值类型或对象
        /// </summary>
        /// <typeparam name="T">转换的目标类型</typeparam>
        /// <param name="data">转换的数据</param>
        /// <returns>
        /// T
        /// </returns>
        public static T ConvertToValue<T>(object data)
        {
            //如果数据为空，则返回
            if (Utils.IsNullOrEmpty(data))
            {
                return default(T);
            }

            try
            {
                //如果数据是T类型，则直接转换
                if (data is T)
                {
                    return (T)data;
                }

                //如果数据实现了IConvertible接口，则转换类型
                if (data is IConvertible)
                {
                    return (T)Convert.ChangeType(data, typeof(T));
                }
                else
                {
                    return default(T);
                }
            }
            catch
            {
                return default(T);
            }
        }
        #endregion

        /// <summary>
        /// 获取带有空类型的准确数据类型.
        /// </summary>
        /// <param name="type">类型.</param>
        /// <returns>
        /// System.Type
        /// </returns>
        public static Type GetNullableType(Type type)
        {
            try
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                {
                    Type[] typeArray = type.GetGenericArguments();
                    return typeArray[0];
                }
            }
            catch (System.Exception ex)
            {
                //new ECFException(ex);
            }
            return type;

        }
    }
}
