using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ECF;
using ECF.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Controller.Controllers
{
    [ApiController]
    public class RouterController : ControllerBase
    {
        private IHttpContextAccessor _accessor;
        private IDapperFactory _dapperFactory;
        public RouterController(IDapperFactory dapperFactory, IHttpContextAccessor accessor)
        {
            _dapperFactory = dapperFactory;
            _accessor = accessor;
        }
        [HttpPost]
        [Route("/route.axd")]//根目录
        public async Task<string> ProcessRequest()
        {
            var context = _accessor.HttpContext;

            var requestString =await GetRequestString(context);

            if (string.IsNullOrEmpty(requestString))
            {
                return ResultResponse.ExceptionResult("请求参数错误,缺少接口必须参数。", "", 4001).ToJson();
            }

            //解析请求参数 sign=4CB5E563FCC6E102DD7AD2DD178BC7BF&timestamp=2014-01-14+15%3A15%3A22&v=2.0&app_key=1012129701&method=taobao.user.seller.get&partner_id=top-apitools&format=xml
            SysParameter para = OpsUtil.ParseOpenApiParameter(requestString);
            para.RequestContent = requestString;
            // RequestParameter = para;
            // 参数有效性校验
            string msg = "";
            if (!ValidateRequest.ValidParameter(para, out msg))
            {
                return ResultResponse.ExceptionResult(msg, "", 4001).ToJson();

            }

            // 对时间戳进行验证
            //if (!ValidateRequest.ValidTimestamp(para))
            //{
            //    return ResultResponse.ExceptionResult("接口调用时间戳有误。", "", 4002);
            //}

            ////验证签名，防止参数被修改
            //if (!ValidateRequest.ValidSign(para))
            //{
            //    return ResultResponse.ExceptionResult("签名信息不正确。", "", 4003);
            //}
            para.dapperFactory = _dapperFactory;
            //分发API请求
            return Newtonsoft.Json.JsonConvert.SerializeObject(Distribute(para));
        }
        /// <summary>
        /// 分发API请求
        /// </summary>
        /// <param name="para">开发平台查询参数</param>
        private IResultResponse Distribute(SysParameter para)
        {
            if (string.IsNullOrEmpty(para.Method))
            {
                return ResultResponse.ExceptionResult("提交参数不正确，Method参数不正确.");
            }
            IDistribute distributeSystem = null;
            string module = "";

            Regex reSysDistribute = new Regex(@"^(?<module>[^\.]+)\.(?<other>[\S\s]*?)",
                RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Match mSys = reSysDistribute.Match(para.Method);
            if (!mSys.Success)
            {
                return ResultResponse.ExceptionResult("action 参数错误");
            }
            //string platform = mSys.Groups["platform"].Value.ToLower();
            module = mSys.Groups["module"].Value.ToLower();

            #region 版本分发处理
            string version = para.Version ?? "1.0";
            switch (version)
            {
                case "1.0":
                    distributeSystem = V1.API.Distribute(module);
                    break;
                default://默认API 1.0通道 如果在所有版本中都找不到，使用默认的
                    distributeSystem = V1.API.Distribute(module);
                    break;
            }
            #endregion


            if (distributeSystem == null)
                return ResultResponse.ExceptionResult("提交参数不正确，没有找到名称为：“" + module + "”的接口处理模块");
            else
                return distributeSystem.Process(para);
        }
        /// <summary>
        /// 获取接口请求的所有参数
        /// </summary>
        /// <param name="context">网络请求上下文.</param>
        /// <returns>String</returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>将原ProcessRequest中的部分代码抽取 added by xiepeng 2018/9/20</description></item>
        ///   </list>
        /// </remarks>
        private async Task<string> GetRequestString(HttpContext context)
        {
            string requestString = string.Empty;
            var req = context.Request;
            // 为当前请求对象赋值，方便后续使用
          
            string method = req.Method.ToUpper();
            if (string.Compare(method, "GET", StringComparison.OrdinalIgnoreCase) == 0)
            {
                requestString = req.QueryString.ToString();
            }
            else
            {
                requestString =await ReadStream1(req.Body);
            }
            return requestString;
        }

        #region　private Method 私有方法

        /// <summary>
        /// 读取http请求的流
        /// </summary>
        /// <param name="stream">http请求的流</param>
        /// <returns></returns>
        private async Task<string> ReadStream1(Stream stream)
        {
            string reqString = string.Empty;

            if (stream.CanRead)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        reqString = await reader.ReadToEndAsync();
                    }                       
                }
                catch
                {
                    reqString = string.Empty;
                }
            }

            return reqString;
        }
        private string ReadStream(Stream stream)
        {
            string reqString = string.Empty;

            if (stream.CanRead)
            {
                try
                {
                    using (StreamReader reader = new StreamReader(stream))
                    {
                        reqString = reader.ReadToEnd();
                    }
                }
                catch
                {
                    reqString = string.Empty;
                }
            }

            return reqString;
        }
        #endregion
    }
}