using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF
{
    /// <summary>
    ///  输出结果.
    /// </summary>
    [Serializable]
    public class ResultResponse : Entity, IResultResponse
    {
        /// <summary>
        /// 实体的全名
        /// </summary>
        public override string EntityFullName => "ECF.ResultResponse,ECF";

        /// <summary>
        /// 输出类型
        /// </summary>
        public Type ContentType { get; set; }

        /// <summary>
        /// 输出数据格式
        /// </summary>
        public FormatType Format { get; set; }

        /// <summary>
        /// 返回的具体内容.
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// 是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 返回状态.
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 返回状态.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 异常信息跟踪.
        /// </summary>
        public string StackTrace { get; set; }

        /// <summary>
        ///  还原.
        /// </summary>
        /// <typeparam name="T">具体类型</typeparam>
        /// <returns>T.</returns>
        public virtual T RestoreType<T>()
        {
            try
            {
                return (T)this.Content;
            }
            catch (Exception ex)
            {
                new ECFException(ex.Message, ex);
                return default(T);
            }
        }


        #region 静态方法



        /// <summary>
        /// 获得成功结果
        /// </summary>
        /// <param name="content">The content.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// IResultResponse
        /// </returns>
        public static IResultResponse GetSuccessResult(object content, Type type = null)
        {
            return GetSuccessResult(content, "成功", type);
        }
        /// <summary>
        /// 获得成功结果
        /// </summary>
        /// <param name="content">内容数据.</param>
        /// <param name="message">The message.</param>
        /// <param name="type">The type.</param>
        /// <returns>
        /// IResultResponse
        /// </returns>
        public static IResultResponse GetSuccessResult(object content, string message, Type type = null)
        {
            if (type == null)
            {
                type = content.GetType();
            }

            return new ResultResponse()
            {
                ContentType = type,
                Content = content,
                Message = message,
                Code = 200,
                Success = true
            };
        }


        /// <summary>
        /// 获得失败结果
        /// </summary>
        /// <param name="content"></param>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static IResultResponse ExceptionResult(string message, object content = null, int code = 500)
        {
            return new ResultResponse()
            {
                Content = content,
                Message = message,
                Code = code,
                Success = false
            };
        }


        /// <summary>
        /// 返回异常结果.
        /// </summary>
        /// <param name="ex">异常对象.</param>
        /// <param name="content">返回内容.</param>
        /// <param name="code">错误状态码.</param>
        /// <returns>
        /// ECF.IResultResponse.
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>调整返回信息 modify by Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        public static IResultResponse ExceptionResult(Exception ex, object content = null, int code = 500)
        {
            IResultResponse response = new ResultResponse()
            {
                Content = content,
                Message = ex.Message,
                Code = code,
                Success = false
            };

            if (ex != null)
            {
                response.StackTrace = ex.StackTrace;
            }

            return response;
        }
        #endregion

        /// <summary>
        /// 设置状态码
        /// </summary>
        /// <param name="code">状态码.</param>
        /// <returns>
        /// IResultResponse
        /// </returns>
        public IResultResponse SetCode(int code)
        {
            this.Code = code;
            return this;
        }

        /// <summary>
        /// 转换为Json,默认情况
        /// </summary>
        /// <returns>
        /// System.String
        /// </returns>
        public override string ToJson()
        {
            return base.ToJson(false, false, new string[] { "ContentType" });
        }

        /// <summary>
        /// 转换为Xml格式
        /// </summary>
        /// <returns>
        /// System.String
        /// </returns>
        public override string ToXml()
        {
            return base.ToXml(new string[] { "ContentType" });
        }
    }
}
