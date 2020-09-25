using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF
{
    /// <summary>
    /// i request data Class 
    /// 2017-06-05 by shaipe
    /// </summary>
    public interface IRequestData
    {
        /// <summary>
        /// 输出类型
        /// </summary>
        Type ContentType { get; set; }
        
        /// <summary>
        /// 返回的具体内容.
        /// </summary>
        object Content { get; set; }

        /// <summary>
        /// 需要执行的动作
        /// </summary>
        string Action { get; set; }

        /// <summary>
        /// 请求时间戳.
        /// </summary>
        long TimeSpan { get; }

        /// <summary>
        /// 签名信息.
        /// </summary>
        string Sign { get; }

        /// <summary>
        /// 还原.
        /// </summary>
        /// <typeparam name="T">具体类型</typeparam>
        /// <returns>T.</returns>
        T RestoreType<T>();
    }
}
