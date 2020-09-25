using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF.Data.Query
{
    /// <summary>
    /// 数据字段
    /// 2017/8/30 by shaipe
    /// </summary>
    [Serializable]
    public partial class Field : Entity
    {
        /// <summary>
        /// 字段别名
        /// </summary>
        public string Alias { get; set; }
        /// <summary>
        /// 字段本名或者自定义字段类型（本名例如：a.Name包含表别名，也可以是一个子查询或者case子句，类型例如：DataTable）
        /// </summary>
        public object Autonym { get; set; }

        /// <summary>
        /// Autonym中包含的所有表别名都需要放入此集合
        /// </summary>
        public IList<string> TableAlias { get; set; }

        /// <summary>
        /// 是否自定义字段
        /// </summary>
        public bool IsCustom { get; set; } = false;

        /// <summary>
        /// 实体的全名
        /// </summary>
        public override string EntityFullName => "ECF.DATA.Query.Field_ECF";

        #region 构造函数
        /// <summary>
        /// 构造函数
        /// </summary>
        private Field() { }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="alias">字段别名</param>
        /// <param name="autonym">字段本名或者自定义字段类型（本名例如：a.Name包含表别名，也可以是一个子查询或者case子句，类型例如：DataTable）</param>
        /// <param name="tableAlias">Autonym中包含的所有表别名都需要放入此集合</param>
        /// <param name="isCustom">是否自定义字段</param>
        private Field(string alias, object autonym, IList<string> tableAlias, bool isCustom)
        {
            Alias = alias;
            Autonym = autonym;
            TableAlias = tableAlias;
            IsCustom = isCustom;
        }

        /// <summary>
        /// 自定义字段构造函数
        /// </summary>
        /// <param name="alias">字段别名</param>
        /// <param name="autonym">自定义字段类型(例如：DataTable)</param>
        public Field(string alias, Type autonym) : this(alias, autonym, null, true)
        {

        }
        /// <summary>
        /// 普通字段构造函数
        /// </summary>
        /// <param name="alias">字段别名</param>
        /// <param name="autonym">字段本名（本名例如：a.Name包含表别名，也可以是一个子查询或者case子句)</param>
        /// <param name="tableAlias">Autonym中包含的所有表别名都需要放入此集合</param>
        public Field(string alias, string autonym, params string[] tableAlias) : this(alias, autonym, tableAlias.ToList(), false)
        {

        }
        #endregion
    }
}
