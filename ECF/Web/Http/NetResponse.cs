using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace ECF.Web.Http
{
    /// <summary>
    /// 网络请求
    /// </summary>
    public class NetResponse

    {
        string _Cookie = string.Empty;
        /// <summary>
        /// Http请求返回的Cookie
        /// </summary>
        public string Cookie
        {
            get { return _Cookie; }
            set { _Cookie = value; }
        }


        CookieCollection cookiecollection = new CookieCollection();
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection
        {
            get { return cookiecollection; }
            set { cookiecollection = value; }
        }


        private string content = string.Empty;
        /// <summary>
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空
        /// </summary>
        public string Content
        {
            get { return content; }
            set { content = value; }
        }


        private byte[] resultbyte = null;
        /// <summary>
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空
        /// </summary>
        public byte[] ResultByte
        {
            get { return resultbyte; }
            set { resultbyte = value; }
        }


        private WebHeaderCollection header = new WebHeaderCollection();

        /// <summary>
        /// Header
        /// </summary>
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }


        private string statusDescription = "";
        /// <summary>
        /// 返回状态说明
        /// </summary>
        public string StatusDescription
        {
            get { return statusDescription; }
            set { statusDescription = value; }
        }


        private HttpStatusCode statusCode = HttpStatusCode.OK;
        /// <summary>
        /// 返回状态码,默认为OK
        /// </summary>
        public HttpStatusCode StatusCode
        {
            get { return statusCode; }
            set { statusCode = value; }
        }
    }
}
