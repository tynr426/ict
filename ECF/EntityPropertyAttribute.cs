using System;
using System.Reflection;

namespace ECF
{

    /// <summary>
    /// 属性数据格式
    /// </summary>
    public enum PropertyFormat
    {
        /// <summary>
        /// 支持HTML
        /// </summary>
        Html,
        /// <summary>
        /// 普通字符串
        /// </summary>
        String,
        /// <summary>
        /// UnixTime为长整型时间格式
        /// </summary>
        UnixTime
    }

    /// <summary>
    /// FullName： <see cref="ECF.EntityPropertyAttribute"/>
    /// Summary ： 数据格式属性
    /// Version： 1.0.0.0 
    /// DateTime： 2013/4/16 10:56
    /// Author  ： XP-WIN7
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class EntityPropertyAttribute : Attribute
    {
        private PropertyFormat _Format;

        /// <summary>
        /// 属性格式.
        /// </summary>
        public PropertyFormat Format
        {
            get { return _Format; }
            set { _Format = value; }
        }

        ///// <summary>
        ///// 添加属性的节点名
        ///// </summary>
        //public string NodeName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPropertyAttribute"/> class.
        /// </summary>
        public EntityPropertyAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPropertyAttribute" /> class.
        /// </summary>
        /// <param name="format">The format.</param>
        public EntityPropertyAttribute(PropertyFormat format)
        {
            _Format = format;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPropertyAttribute"/> class.
        /// </summary>
        /// <param name="noDataField">if set to <c>true</c> [no data field].</param>
        public EntityPropertyAttribute(bool noDataField)
        {
            _NoDataField = noDataField;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityPropertyAttribute"/> class.
        /// </summary>
        /// <param name="noDataField">if set to <c>true</c> [no data field].</param>
        /// <param name="format">The format.</param>
        public EntityPropertyAttribute(bool noDataField, PropertyFormat format)
        {
            _Format = format;
            _NoDataField = noDataField;
        }

        ///// <summary>
        ///// 实体在转换为Xml或Json时的节点名称
        ///// </summary>
        ///// <param name="nodeName">节点名称.</param>
        //public EntityPropertyAttribute(string nodeName)
        //{
        //    NodeName = nodeName;
        //}

        #region NoDataFeild 指定是否为数据库字段
        bool _NoDataField;

        /// <summary>
        /// 指定是否为数据库字段,默认不需加,只是针对一个实体中非数据库字段的才加.
        /// </summary>
        public bool NoDataField
        {
            get { return _NoDataField; }
            set { _NoDataField = value; }
        }
        #endregion

        /// <summary>
        /// 判断属性是否为数据库字段.
        /// </summary>
        /// <param name="pi">属性信息.</param>
        public static bool IsNoDataField(PropertyInfo pi)
        {
            try
            {
                object[] cas = pi.GetCustomAttributes(typeof(EntityPropertyAttribute), true);
                if (cas.Length > 0)
                {
                    EntityPropertyAttribute pa = cas.GetValue(0) as EntityPropertyAttribute;
                    if (pa != null)
                    {
                        return pa.NoDataField;
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return false;
        }

    }


    /// <summary>
    /// FullName： <see cref="ECF.EntityClassAttribute"/>
    /// Summary ： 实体类自定义属性
    /// Version： 1.0
    /// DateTime： 2014/6/16
    /// CopyRight (c) shaipe
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class EntityClassAttribute : Attribute
    {
        /// <summary>
        /// 节点名
        /// </summary>
        public string NodeName { get; set; }

        /// <summary>
        /// 序列化为属性
        /// </summary>
        public bool IsXmlAttribute { get; set; }

        /// <summary>
        /// 是否验证字段.
        /// </summary>
        public bool ValidateField { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityClassAttribute"/> class.
        /// </summary>
        public EntityClassAttribute()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityClassAttribute"/> class.
        /// </summary>
        /// <param name="validateField">if set to <c>true</c> [validate field].</param>
        public EntityClassAttribute(bool validateField)
        {
            ValidateField = validateField;
        }

        /// <summary>
        ///  Determines whether the specified information is validate.
        ///  Author :   XP-PC/Shaipe
        ///  Created:  02-16-2017
        /// </summary>
        /// <param name="info">The information.</param>
        /// <returns><c>true</c> if the specified information is validate; otherwise, <c>false</c>.</returns>
        public static bool IsValidateField(MemberInfo info)
        {
            try
            {
                object[] attributes = info.GetCustomAttributes(typeof(EntityClassAttribute), true);
                if (attributes.Length > 0)
                {
                    EntityClassAttribute pa = attributes.GetValue(0) as EntityClassAttribute;
                    if (pa != null)
                    {
                        return pa.ValidateField;
                    }
                }
            }
            catch (Exception ex)
            {
                new ECFException(ex);
            }
            return false;
        }


    }
}
