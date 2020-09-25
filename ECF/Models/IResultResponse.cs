using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ECF
{
    /// <summary>
    ///  获取接口
    /// </summary>
    public interface IResultResponse : IEntity
    {
        /// <summary>
        /// 输出类型
        /// </summary>
        Type ContentType { get; set; }

        /// <summary>
        /// 输出数据格式
        /// </summary>
        FormatType Format { get; set; }

        /// <summary>
        /// 返回的具体内容.
        /// </summary>
        object Content { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        bool Success { get; set; }

        /// <summary>
        /// 返回状态.
        /// </summary>
        int Code { get; set; }

        /// <summary>
        /// 返回状态.
        /// </summary>
        string Message { get; set; }

        /// <summary>
        /// 异常信息跟踪.
        /// </summary>
        string StackTrace { get; set; }

        /// <summary>
        /// 还原.
        /// </summary>
        /// <typeparam name="T">具体类型</typeparam>
        /// <returns>T.</returns>
        T RestoreType<T>();

        /// <summary>
        /// 设置状态码
        /// </summary>
        /// <param name="code">状态码.</param>
        /// <returns>
        /// IResultResponse
        /// </returns>
        IResultResponse SetCode(int code);
    }

   
    /// <summary>
    /// 开放平台输出类型
    /// </summary>
    [Serializable]
    public enum FormatType
    {
        /// <summary>
        /// json格式
        /// </summary>
        Json,
        /// <summary>
        /// XML格式
        /// </summary>
        Xml,
        /// <summary>
        /// 二进制流
        /// </summary>
        Binary,
        /// <summary>
        /// 纯文本
        /// </summary>
        Text
    }

   

    
}
