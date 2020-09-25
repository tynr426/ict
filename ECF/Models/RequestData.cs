using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ECF
{
    /// <summary>
    /// 网络请求数据 
    /// 2017-06-05 by shaipe
    /// </summary>
    /// <seealso cref="ECF.IRequestData" />
    [Serializable]
    public class RequestData : IRequestData
    {
        /// <summary>
        /// 内容类型
        /// </summary>
        public Type ContentType { get; set; }

        object _Content = null;

        /// <summary>
        /// 请求内容
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// 请求执行动作
        /// </summary>
        public string Action { get; set; }

        long _TimeSpan = 0;
        /// <summary>
        /// 请求时间戳.
        /// </summary>
        public long TimeSpan
        {
            get { return _TimeSpan; }
        }

        string _sign = string.Empty;
        /// <summary>
        /// 签名信息.
        /// </summary>
        public string Sign
        {
            get { return _sign; }
        }

        /// <summary>
        /// 还原类型
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>
        /// T
        /// </returns>
        public T RestoreType<T>()
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

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestData"/> class.
        /// </summary>
        public RequestData() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RequestData"/> class.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="content">The content.</param>
        public RequestData(string action, object content)
        {
            Content = content;
            if (content != null)
                ContentType = content.GetType();
            Action = action;
        }

        /// <summary>
        /// 获取请求数据
        /// </summary>
        /// <param name="action">动作.</param>
        /// <param name="content">内容.</param>
        /// <returns>
        /// IRequestData
        /// </returns>
        public static IRequestData GetData(string action, object content)
        {
            IRequestData rdata = new RequestData();
            rdata.Content = content;
            rdata.Action = action;
            rdata.ContentType = content.GetType();
            return rdata;
        }

        private void AddSign()
        {

        }

    }
}
