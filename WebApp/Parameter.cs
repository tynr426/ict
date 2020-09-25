using ECF;
using ECF.Data;
using ECF.Data.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Controller
{
    /// <summary>
    /// FullName： <see cref="Vast.Op.Parameter"/>
    /// Summary ： 基础参数基类 
    /// Version： 1.0
    /// DateTime： 2014/1/14
    /// CopyRight (c) shaipe
    /// </summary>
    public class SysParameter
    {
        //sign=4CB5E563FCC6E102DD7AD2DD178BC7BF&timestamp=2014-01-14+15%3A15%3A22&v=2.0&app_key=1012129701
        //&method=taobao.user.seller.get&partner_id=top-apitools&format=xml


        /// <summary>
        /// 分配给应用的AppKey
        /// </summary>
        public String AppId { get; set; }


        /// <summary>
        /// API协议版本，可选值:1.0
        /// </summary>
        public String Version { get; set; }

        /// <summary>
        /// API输入参数签名结果
        /// </summary>
        public String Sign { get; set; }

        /// <summary>
        /// 时间戳，格式为yyyy-MM-dd HH:mm:ss，例如：2008-01-25 20:23:30。淘宝API服务端允许客户端请求时间误差为10分钟。
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// 授权访问Token.
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// API接口名称
        /// </summary>
        public String Method { get; set; }


        /// <summary>
        /// 当前运营主体.
        /// </summary>
        public int Proprietor { get; set; }

        /// <summary>
        /// 请求内容.
        /// </summary>
        public string RequestContent { get; set; }


        public IDapperFactory dapperFactory { get; set; }

        /// <summary>
        /// 外键Id.
        /// </summary>
        public int FKId { get; set; }

        /// <summary>
        /// 访问者角色标识.
        /// </summary>
        //public PlatformRole VisitorFlag { get; set; }

        /// <summary>
        /// 访问者角色Id.
        /// </summary>
        public int VisitorId { get; set; }

        /// <summary>
        /// 访问者管理员Id
        /// </summary>
        public int ManagerId { get; set; }

        /// <summary>
        /// 合作者Id
        /// </summary>
        public String PartnerId { get; set; }

        /// <summary>
        /// 商家Id.
        /// </summary>
        public int StoreId { get; set; }

        /// <summary>
        /// 可选，指定响应格式。默认xml,目前支持格式为xml,json
        /// </summary>
        public FormatType Format { get; set; }

        /// <summary>
        /// 特殊属性
        /// </summary>
        public Dictionary<string, object> Properties { get; set; }
    }


    /// <summary>
    /// FullName： <see cref="ParamersExtension"/>
    /// Summary ： 参数扩展方法
    /// Version： 1.0
    /// DateTime：2014/6/4
    /// CopyRight (c)GUQIANG-PC
    /// </summary>
    public static class ParamersExtension
    {
        #region 参数格式化为分页查询条件对象 +static PagingQuery ToPagingQuery(this SysParameter para)
        /// <summary>
        /// 参数格式化为分页查询条件对象（解析的节点只包含：Fields、SumFields、PageIndex、PageSize、Condition、Orderby、Groupby） by hfs 20160909
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static PagingQuery ToPagingQuery(this SysParameter para)
        {
            XmlDocument orderby = new XmlDocument();
            orderby.LoadXml("<Orderby>" + (!string.IsNullOrEmpty(para.ToValue("Orderby")) ? para.ToValue("Orderby") : "") + "</Orderby>");
            XmlDocument condition = new XmlDocument();
            condition.LoadXml("<Condition>" + (!string.IsNullOrEmpty(para.ToValue("Condition")) ? para.ToValue("Condition") : "") + "</Condition>");
            PagingQuery query = new PagingQuery
            {
                Fields = para.ToValue("Fields"),
                SumFields = para.ToValue("SumFields"),
                PageIndex = para.ToInt("PageIndex", 1),
                PageSize = para.ToInt("PageSize", 10),
                Condition = Condition.Collection(condition),
                Orderby = Orderby.Collection(orderby),
                Groupby = para.ToValue("Groupby")
            };

            return query;
        }
        #endregion

        #region 参数格式化为列表查询条件对象 +static ListQuery ToListQuery(this SysParameter para)
        /// <summary>
        /// 参数格式化为列表查询条件对象（解析的节点只包含：Fields、PageIndex、PageSize、Condition、Orderby、Groupby） by hfs 20160909
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public static ListQuery ToListQuery(this SysParameter para)
        {
            XmlDocument orderby = new XmlDocument();
            orderby.LoadXml("<Orderby>" + (!string.IsNullOrEmpty(para.ToValue("Orderby")) ? para.ToValue("Orderby") : "") + "</Orderby>");
            XmlDocument condition = new XmlDocument();
            condition.LoadXml("<Condition>" + (!string.IsNullOrEmpty(para.ToValue("Condition")) ? para.ToValue("Condition") : "") + "</Condition>");
            ListQuery query = new ListQuery
            {
                Fields = para.ToValue("Fields"),
                Condition = Condition.Collection(condition),
                Orderby = Orderby.Collection(orderby),
                Groupby = para.ToValue("Groupby")
            };

            return query;
        }
        #endregion

        #region ToParmersValue 参数格式化 by gqiang 20140605
        /// <summary>
        /// 参数格式化.
        /// </summary>
        /// <param name="para">参数.</param>
        /// <param name="key">关键字.</param>
        /// <returns></returns>
        public static string ToValue(this SysParameter para, string key)
        {
            string value = string.Empty;

            if (!para.IsNullOrEmpty(key))
            {
                value = para.Properties[key].ToString();
            }

            return value;
        }
        #endregion

        #region ToDecimal 参数转为数字类型 by gqiang 20140605
        /// <summary>
        /// 参数转为数字类型.
        /// </summary>
        /// <param name="para">参数.</param>
        /// <param name="key">关键字.</param>
        /// <returns></returns>
        public static decimal ToDecimal(this SysParameter para, string key)
        {
            decimal value = 0.00m;

            if (!para.IsNullOrEmpty(key))
            {
                decimal.TryParse(para.Properties[key].ToString(), out value);
            }

            return value;
        }
        #endregion

        #region ToInt 参数转为整数类型 by gqiang 20140605
        /// <summary>
        /// Converts the string representation of a number to an integer.
        /// </summary>
        /// <param name="para">The para.</param>
        /// <param name="key">The key.</param>
        /// <returns>
        /// Int32
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>说明原因 added by xiepeng 2018/9/20</description></item>
        ///   </list>
        /// </remarks>
        public static int ToInt(this SysParameter para, string key)
        {
            return ToInt(para, key, 0);
        }
        /// <summary>
        /// 参数转为整数类型.
        /// </summary>
        /// <param name="para">参数.</param>
        /// <param name="key">关键字.</param>
        /// <returns></returns>
        public static int ToInt(this SysParameter para, string key, int defaultValue)
        {
            int value = defaultValue;

            if (!para.IsNullOrEmpty(key))
            {
                int.TryParse(para.Properties[key].ToString(), out value);
            }

            return value;
        }
        #endregion

        #region ToBool 参数转换为布尔型 by gqiang 20140605
        /// <summary>
        /// 参数转换为布尔型.
        /// </summary>
        /// <param name="para">参数.</param>
        /// <param name="key">关键字.</param>
        /// <returns></returns>
        public static bool ToBool(this SysParameter para, string key)
        {
            if (para.Properties == null || !para.Properties.ContainsKey(key))
                return false;
            return Utils.ToBool(para.Properties[key]);
            //bool value = false;
            //if (!para.IsNullOrEmpty(key))
            //{
            //    bool.TryParse(para.Properties[key], out value);
            //}

            //return value;
        }
        #endregion

        #region ToDateTime 转换为时间格式 by gqiang 20151120
        /// <summary>
        /// 转换为时间格式.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="key"></param>
        /// <returns>转换失败则为系统默认最小日期</returns>
        public static DateTime ToDateTime(this SysParameter para, string key)
        {
            DateTime dateTime = DateTime.MinValue;

            string dateTimeString = string.Empty;

            if (!para.IsNullOrEmpty(key))
            {
                dateTimeString = para.Properties[key].ToString();
            }

            DateTime.TryParse(HttpUtility.HtmlDecode(dateTimeString), out dateTime);

            return dateTime;
        }
        #endregion

        #region ToXml 参数转换为XML by gqiang 20150814
        /// <summary>
        /// 参数转换为XML.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public static XmlDocument ToXml(this SysParameter parameter)
        {
            if (parameter.Properties == null)
                parameter.Properties = new Dictionary<string, object>();

            var xmlDoc = new XmlDocument();
            var sb = new StringBuilder();

            sb.Append("<root>");
            foreach (var property in parameter.Properties)
            {
                sb.Append("<" + property.Key + ">" + property.Value + "</" + property.Key + ">");
            }
            sb.Append("</root>");
            xmlDoc.LoadXml(sb.ToString());

            return xmlDoc;
        }
        #endregion

        #region ToDictionary 转换为键值对 by gqiang 20150908
        /// <summary>
        /// 转换为键值对.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns></returns>
        public static Dictionary<string, object> ToDictionary(this SysParameter parameter)
        {
            Dictionary<string, object> dictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            if (parameter.Properties == null) return dictionary;

            foreach (var property in parameter.Properties)
            {
                if (!dictionary.ContainsKey(property.Key))
                {
                    dictionary.Add(property.Key, property.Value);
                }
                else
                {
                    dictionary[property.Key] = property.Value;
                }
            }

            return dictionary;
        }
        #endregion

        #region Split 返回的字符串数组包含此实例中的子字符串 by gqiang 20140716
        /// <summary> 
        /// 返回的字符串数组包含此实例中的子字符串（由指定 Unicode 字符数组的元素分隔）。
        /// </summary>
        /// <param name="para">查询参数.</param>
        /// <param name="key">关键字.</param>
        /// <param name="oparet">操作符.</param>
        /// <returns></returns>
        public static string[] Split(this SysParameter para, string key, char oparet)
        {
            string[] values = { };

            if (!para.IsNullOrEmpty(key))
            {
                values = para.Properties[key].ToString().Split(oparet);
            }

            return values;
        }
        #endregion

        #region IsNullOrEmpty 判断参数是否为空或者是否包含关键字 by gqiang 20140605
        /// <summary>
        /// 判断参数是否为空或者是否包含关键字.
        /// </summary>
        /// <param name="para">参数.</param>
        /// <param name="key">关键字.</param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this SysParameter para, string key)
        {
            bool result = true;

            if (para.Properties != null && !string.IsNullOrEmpty(key))  // by xp 添加对Key的空值进行判断
            {
                if (para.Properties.ContainsKey(key) && !String.IsNullOrEmpty(para.Properties[key].ToString()))
                {
                    result = false;
                }
            }

            return result;
        }
        #endregion
    }
}
