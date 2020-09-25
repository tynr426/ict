using ECF;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Controller
{
    /// <summary>
    /// API请求分发处理接口
    /// </summary>
    public interface IDistribute
    {
        /// <summary>
        /// 处理分发请求
        /// </summary>
        /// <param name="route">请求的路由对象.</param>
        /// <param name="para">开发平台查询参数</param>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加参数 modify by Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        IResultResponse Process(SysParameter para);
    }
}
