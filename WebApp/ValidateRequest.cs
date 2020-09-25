using ECF;
using Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Controller
{
    public class ValidateRequest
    {
        /// <summary>
        /// 参数有效性判断
        /// </summary>
        /// <param name="parameter">系统参数.</param>
        /// <param name="messaage">消息.</param>
        /// <returns>
        /// Boolean
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加参数校验方法 added by Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        public static bool ValidParameter(SysParameter parameter, out string messaage)
        {
            if (string.IsNullOrEmpty(parameter.AppId))
            {
                messaage = "缺少必须的Appid参数";
                return false;
            }

            if (!Constant.AppSecret.ContainsKey(parameter.AppId))
            {
                messaage = "请求的参数错误AppId不正确";
                return false;
            }

            if (parameter.Timestamp == 0)
            {
                messaage = "缺少必须的Timestamp参数";
                return false;
            }

            if (string.IsNullOrEmpty(parameter.Sign))
            {
                messaage = "缺少必须的Sign参数";
                return false;
            }

            if (string.IsNullOrEmpty(parameter.Method))
            {
                messaage = "缺少必须的Method参数";
                return false;
            }

            messaage = "";
            return true;
        }

        /// <summary>
        /// 验证签名，防止参数被修改
        /// </summary>
        /// <returns>Boolean</returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>代码梳理 added by xiepeng 2018/9/20</description></item>
        ///   </list>
        /// </remarks>
        public static bool ValidSign(SysParameter parameter)
        {
            bool ret = false;
            // 1. 根据appid获取出对应的Secret

            string secret = Constant.AppSecret[parameter.AppId];

            Dictionary<string, string> dic = parameter.RequestContent.ToDictionary();

            if (dic != null)
            {
                Dictionary<string, string> sortDic = dic.SortFilter(new string[] { "sign", "secret" });

                string sign = ECF.Security.Encrypt.MD532(sortDic.ToLinkString() + "&secret=" + secret);
                // 3. 对比签名有效性
                ret = (sign == parameter.Sign);
            }
            return ret;
        }

        /// <summary>
        /// 对时间戳参数进行验证
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>
        /// Boolean
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加时间戳校验方法 added by xiepeng 2018/9/20</description></item>
        ///   </list>
        /// </remarks>
        public static bool ValidTimestamp(SysParameter parameter)
        {
            bool ret = false;
            long currentStamp = Utils.GetTimstamp();
            long absDiff = System.Math.Abs(currentStamp - parameter.Timestamp);
            // 请求的时间差为上下180秒以内
            ret = (absDiff < 180);
            return ret;
        }
    }
}
