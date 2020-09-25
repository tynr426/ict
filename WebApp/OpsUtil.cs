using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Web;
using ECF;
using Sdk;
using System.Data;
using ECF.Data;

namespace Controller
{
    public class OpsUtil
    {
        /// <summary>
        /// 解析开放平台参数
        /// </summary>
        /// <param name="requestString">查询字符串</param>
        /// <returns></returns>
        public static SysParameter ParseOpenApiParameter(string requestString)
        {
            SysParameter sysPara = new SysParameter();

            Regex reParameter = new Regex(@"(?<paraKey>[^=|^&]+)=(?<paraValue>[^\&|$]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

            Regex reSpace = new Regex(@"[\s]*", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);

            requestString = reSpace.Replace(requestString, String.Empty);

            string paraKey = null, paraValue = null;

            foreach (Match m in reParameter.Matches(requestString))
            {
                paraKey = HttpUtility.UrlDecode(m.Groups["paraKey"].Value);
                paraValue = HttpUtility.UrlDecode(m.Groups["paraValue"].Value);

                // 签名数据
                if (String.Compare("sign", paraKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sysPara.Sign = paraValue;
                }
                // 请求时间戳
                else if (String.Compare("timestamp", paraKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    long tstamp = 0;
                    long.TryParse(paraValue, out tstamp);
                    sysPara.Timestamp = tstamp;

                }
                // 版本号
                else if (String.Compare("v", paraKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sysPara.Version = paraValue;
                }
                // 应用key
                else if (String.Compare("appid", paraKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sysPara.AppId = paraValue;
                }
                // 请求的方法
                else if (String.Compare("method", paraKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sysPara.Method = paraValue.ToLower();
                }
                // 商户id
                else if (String.Compare("partnerid", paraKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sysPara.PartnerId = paraValue;
                }
                // 入驻商家Id
                else if (String.Compare("storeid", paraKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sysPara.StoreId = Utils.ToInt(paraValue);
                }
                // 获取外键id
                else if (String.Compare("fkid", paraKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sysPara.FKId = Utils.ToInt(paraValue);
                }

                else if (String.Compare("accessToken", paraKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    sysPara.AccessToken = Utils.ToString(paraValue);
                }
                // 返回数据格式
                else if (String.Compare("format", paraKey, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (String.Compare(paraValue, "json", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        sysPara.Format = FormatType.Json;
                    }
                    else if (String.Compare(paraValue, "xml", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        sysPara.Format = FormatType.Xml;
                    }
                    else if (String.Compare(paraValue, "binary", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        sysPara.Format = FormatType.Binary;
                    }
                }
                //else
                //{
                if (sysPara.Properties == null)
                    sysPara.Properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);


                if (!sysPara.Properties.ContainsKey(paraKey))
                {
                    sysPara.Properties.Add(paraKey, paraValue);
                }
                else
                {
                    new Exceptions("接收的参数中有相同的参数，参数名为：" + paraKey, "Ops");
                }

                //}
            }

            return sysPara;
        }


        public static IResultResponse VerifyAccessToken(SysParameter para)
        {
            string token = para.ToValue("AccessToken");

            //解析Token
            AccessToken tokenEnt = AccessToken.ParseToken(token);


            if (tokenEnt == null || tokenEnt.SellerId <= 0 || tokenEnt.DBId <= 0
                ) return ResultResponse.ExceptionResult("Token无效", "", 4004);


            // 判断token有效期
            if (Utils.ToUnixTime(DateTime.Now) > tokenEnt.Expired) return ResultResponse.ExceptionResult("Token已过期", "", 4004);


            return ResultResponse.GetSuccessResult(tokenEnt);

        }

        /// <summary>
        /// 獲取最后數據
        /// </summary>
        /// <param name="para"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string GetContent(SysParameter para, object result)
        {
            string content = string.Empty;
            if (result is DataTable)
            {
                if (para.Format == FormatType.Json)
                {
                    content = DBHelper.TableToJson(result as DataTable);
                }
                else
                {
                    content = DBHelper.Table2Xml(result as DataTable);
                }
            }
            else if (result is IEntity)
            {
                if (para.Format == FormatType.Json)
                {
                    content = (result as IEntity).ToJson();
                }
                else
                {
                    content = "<entity>" + (result as IEntity).ToXml() + "</entity>";
                }
            }
            else
            {
                if (result == null) result = "";

                if (para.Format == FormatType.Json)
                {
                    content = "{\"result\":\"" + result.ToString() + "\"}";
                }
                else
                {
                    content = "<result>" + result.ToString() + "</result>";
                }
            }

            return content;
        }

        /// <summary>
        /// 格式化键值对中的日期
        /// </summary>
        /// <param name="stafferDic"></param>
        public static void DictionayFormateDate(Dictionary<string, object> stafferDic)
        {
            for (int i = 0; i < stafferDic.Count; i++)
            {
                KeyValuePair<string, object> kv = stafferDic.ElementAt(i);
                if (kv.Value.GetType() == typeof(System.DateTime))
                {
                    stafferDic[kv.Key] = kv.Value.ToString();
                }
            }
        }
     
    }
}
