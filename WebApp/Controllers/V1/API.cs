using ECF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sdk;

namespace Controller.V1
{
    /// <summary>
    /// FullName： <see cref="Vast.Ops.V2.API"/>
    /// Summary ： 开放平台接口 
    /// Version： 2.0
    /// DateTime： 2017/5/9
    /// CopyRight (c) hfshan
    /// </summary>
    public abstract class API : IDistribute
    {
        /// <summary>
        /// 执行接口
        /// </summary>
        /// <param name="para"></param>
        public IResultResponse Process(SysParameter para)
        {
            IResultResponse result = GetResultResponse(para);

            // 如果使用给定结果的方式返回可以直接输出二进制流
            return(result != null ? result : ResultResponse.ExceptionResult("提交参数不正确", "", 4001));
        }

        /// <summary>
        /// 获得接口请求结果
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        public abstract IResultResponse GetResultResponse(SysParameter para);

        /// <summary>
        /// 分发API请求
        /// </summary>
        /// <param name="para">开发平台查询参数</param>
        public static IDistribute Distribute(string module)
        {

            IDistribute distributeSystem = null;

            switch (module.ToLower())
            {
                //系统配置信息模块
                case "core":
                    distributeSystem = new V1.Core.Distribute();
                    break;
                default:
                    break;
            }
            return distributeSystem;
        }

    }
}
