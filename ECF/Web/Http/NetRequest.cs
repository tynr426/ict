using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace ECF.Web.Http
{
    /// <summary>
    /// 支持代理的Http请求
    /// </summary>
    public class NetRequest
    {
        #region properties
        /// <summary>
        /// 默认用户信息
        /// </summary>
        const string DefaultUserAgent = "Mozilla/5.0 (Windows NT 6.3; Trident/8.0; rv:11.0) like Gecko";

        /// <summary>
        /// 默认内容类型
        /// </summary>
        const string DefaultContentType = "application/x-www-form-urlencoded;";

        /// <summary>
        /// The proxy
        /// </summary>
        private WebProxy _proxy = null;

        /// <summary>
        /// The cc
        /// </summary>
        private CookieContainer _cookieContainer = new CookieContainer();

        // 设置Header
        private NameValueCollection _header = new NameValueCollection();

        private int _delayTime;
        private int _timeout = 0; // The default is 120000 milliseconds (120 seconds).
        private int _tryTimes = 3; // 默认重试3次
        private string _lastUrl = string.Empty;

        private string _certPath = string.Empty;
        private string _certPassword = string.Empty;

        /// <summary>
        /// 超时时间.
        /// </summary>
        public int TimeOut { get { return _timeout; } }

        /// <summary>
        /// 间隔时间，s.
        /// </summary>
        public int DelayTime { get { return _delayTime; } }

        /// <summary>
        /// 尝试次数.
        /// </summary>
        public int TryTimes { get { return _tryTimes; } }
        #endregion

        #region public method
        /// <summary>
        /// 设置尝试次数.
        /// </summary>
        /// <param name="times">次数.</param>
        public void SetTryTimes(int times)
        {
            if (times > 0)
            {
                _tryTimes = times;
            }
        }

        /// <summary>
        /// 设置每次请求的超时时间
        /// </summary>
        /// <param name="timeout">时间.</param>
        public void SetTimeOut(int timeout)
        {
            if (timeout > 0)
            {
                _timeout = timeout;
            }
        }

        /// <summary>
        /// 设置推迟时间
        /// </summary>
        /// <param name="delayTime">推迟时间.</param>
        public void SetDelayTime(int delayTime)
        {
            _delayTime = delayTime;
        }

        /// <summary>
        /// 设置代理
        /// </summary>
        /// <param name="server">代理服务.</param>
        /// <param name="port">端口.</param>
        /// <param name="username">用户名.</param>
        /// <param name="password">密码.</param>
        public void SetProxy(string server, int port, string username, string password)
        {
            if (null != server && port > 0)
            {
                _proxy = new WebProxy(server, port);
                if (null != username && null != password)
                {
                    _proxy.Credentials = new NetworkCredential(username, password);
                    // 对本地地址不使用代理
                    _proxy.BypassProxyOnLocal = true;
                }
            }
        }

        /// <summary>
        /// 添加Cookie.
        /// </summary>
        /// <param name="name">Cookie名称.</param>
        /// <param name="value">值.</param>
        public void AddCookie(string name, string value)
        {
            _cookieContainer.Add(new Cookie(name, value));
        }

        /// <summary>
        /// 添加Cookie集.
        /// </summary>
        /// <param name="cookies">The cookies.</param>
        public void AddCookie(CookieCollection cookies)
        {
            _cookieContainer.Add(cookies);
        }

        /// <summary>
        /// 添加自定义请求头部
        /// </summary>
        /// <param name="name">头部名称.</param>
        /// <param name="value">头部值.</param>
        public void AddHeader(string name, string value)
        {
            _header.Add(name, value);
        }

        /// <summary>
        /// 设置证书
        /// </summary>
        /// <param name="certPath">证书的路径.</param>
        /// <param name="certPassword">客户端证书密码.</param>
        public void SetClientCert(string certPath, string certPassword)
        {
            _certPath = certPath;
            _certPassword = certPassword;
        }

        /// <summary>
        /// POST方式发送数据到远程地址.
        /// </summary>
        /// <param name="url">远程地址.</param>
        /// <param name="content">内容.</param>
        /// <param name="referer">引用地址.</param>
        /// <param name="contentType">发送内容.</param>
        /// <param name="errorMsg">错误消息.</param>
        /// <param name="encode">编码方式.</param>
        /// <returns></returns>
        public NetResponse Post(string url, byte[] content, string referer, out string errorMsg, string contentType = DefaultContentType, string encode = "UTF-8")
        {
            int failedTimes = _tryTimes;
            errorMsg = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.OK;
            string statusDes = "";

            while (failedTimes-- > 0)
            {
                HttpWebRequest request = null;
                try
                {
                    if (failedTimes < _tryTimes - 1) // 推迟处理,第一次请求不延迟                        
                        Delay();

                    // 获取网络请求对象
                    request = GetWebRequest(url, referer);
                    request.Method = "POST";
                    request.ContentType = contentType;
                    if (_timeout > 0)
                    {
                        request.Timeout = _timeout;
                    }

                    Encoding encoding = Encoding.GetEncoding(encode);
                    request.ContentLength = content.Length;

                    using (Stream steam = request.GetRequestStream())
                    {
                        steam.Write(content, 0, content.Length);
                    }

                    using (WebResponse response = request.GetResponse())
                    {
                        return GetNetResponse((HttpWebResponse)response, encoding);
                    }
                }
                catch (WebException ex)
                {
                    errorMsg = ex.Message;
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse webResponse = (HttpWebResponse)ex.Response;
                        statusCode = webResponse.StatusCode;
                        statusDes = webResponse.StatusDescription;
                    }
                    else
                    {
                        statusCode = HttpStatusCode.BadGateway;
                        statusDes = ex.Message;
                    }

                    new ECFException("HTTP POST Error: " + ex.Message + "\nUrl: " + url + "\nPost:" + content);

                }
                finally
                {
                    request = null;
                }
            }

            NetResponse Result = new NetResponse()
            {
                Cookie = "",
                Header = null,
                Content = errorMsg,
                StatusDescription = statusDes,
                StatusCode = statusCode
            };
            return Result;
        }


        /// <summary>
        /// POST方式发送数据到远程地址.
        /// </summary>
        /// <param name="url">远程地址.</param>
        /// <param name="content">内容.</param>
        /// <param name="referer">引用地址.</param>
        /// <param name="contentType">发送内容.</param>
        /// <param name="errorMsg">错误消息.</param>
        /// <param name="encode">编码方式.</param>
        /// <returns></returns>
        public NetResponse Post(string url, string content, string referer, out string errorMsg, string contentType = DefaultContentType, string encode = "UTF-8")
        {
            Encoding encoding = Encoding.GetEncoding(encode);
            Byte[] bs = encoding.GetBytes(content);
            return Post(url, bs, referer, out errorMsg, contentType, encode);
        }

        /// <summary>
        /// POST方式发送数据到远程地址.
        /// </summary>
        /// <param name="url">远程地址.</param>
        /// <param name="content">内容.</param>
        /// <param name="referer">引用地址.</param>
        /// <param name="errorMsg">错误消息.</param>
        /// <returns>
        /// NetResponse
        /// </returns>
        public NetResponse Post(string url, IRequestData content, string referer, out string errorMsg)
        {
            byte[] bs = ByteSerializer.Serialize(content);
            return Post(url, bs, referer, out errorMsg, "application/x-binary-irequestdata");
        }


        /// <summary>
        /// 通过HTTPGet方式获取远程页面数据.
        /// </summary>
        /// <param name="url">URL地址.</param>
        /// <param name="referer">引用地址.</param>
        /// <param name="errorMsg">输出错误消息.</param>
        /// <param name="encode">编码格式.</param>
        /// <returns>
        /// NetResponse
        /// </returns>
        public NetResponse Get(string url, string referer, out string errorMsg, string encode = "utf-8")
        {
            int failedTimes = _tryTimes;
            errorMsg = string.Empty;
            HttpStatusCode statusCode = HttpStatusCode.OK;
            string statusDes = "";

            while (failedTimes-- > 0)
            {
                HttpWebRequest request = null;
                try
                {
                    if (failedTimes < _tryTimes - 1) // 推迟处理,第一次请求不延迟    
                        Delay();

                    // 获取网络请求对象
                    request = GetWebRequest(url, referer);
                    request.Method = "GET";
                    if (_timeout > 0)
                    {
                        request.Timeout = _timeout;
                    }

                    using (WebResponse response = request.GetResponse())
                    {
                        return GetNetResponse((HttpWebResponse)response, Encoding.GetEncoding(encode));
                    }
                }
                catch (WebException ex)
                {
                    errorMsg = ex.Message;
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        HttpWebResponse webResponse = (HttpWebResponse)ex.Response;
                        statusCode = webResponse.StatusCode;
                        statusDes = webResponse.StatusDescription;
                    }
                    else
                    {
                        statusCode = HttpStatusCode.BadGateway;
                        statusDes = ex.Message;
                    }

                    new ECFException("HTTP GET Error: " + ex.Message + "\nUrl: " + url);
                }
                finally
                {
                    request = null;
                }
            }

            NetResponse Result = new NetResponse()
            {
                Cookie = "",
                Header = null,
                Content = errorMsg,
                StatusDescription = statusDes,
                StatusCode = statusCode
            };
            return Result;
        }

        #endregion

        #region private method
        /// <summary>
        /// 推迟处理.
        /// </summary>
        private void Delay()
        {
            if (_delayTime > 0)
            {
                Random rd = new Random();
                int delayTime = _delayTime * 1000 + rd.Next(1000);
                Thread.Sleep(delayTime);
            }
        }

        /// <summary>
        /// 获取网络响应.
        /// </summary>
        /// <param name="webResponse">Web响应.</param>
        /// <param name="encoding">The encoding.</param>
        /// <returns>
        /// NetResponse
        /// </returns>
        private NetResponse GetNetResponse(HttpWebResponse webResponse, Encoding encoding)

        {
            //返回参数
            NetResponse netResponse = new NetResponse();
            try
            {
                netResponse.StatusCode = webResponse.StatusCode;
                netResponse.StatusDescription = webResponse.StatusDescription;
                netResponse.Header = webResponse.Headers;

                // 设置Cookies
                if (webResponse.Cookies != null)
                    netResponse.CookieCollection = webResponse.Cookies;

                if (webResponse.Headers["set-cookie"] != null)
                    netResponse.Cookie = webResponse.Headers["set-cookie"];

                // 内存数据流处理
                MemoryStream _stream = new MemoryStream();

                //GZIIP处理
                if (webResponse.ContentEncoding != null && webResponse.ContentEncoding.Equals("gzip", StringComparison.InvariantCultureIgnoreCase))
                {
                    //开始读取流并设置编码方式
                    //new GZipStream(response.GetResponseStream(), CompressionMode.Decompress).CopyTo(_stream, 10240);
                    //.net4.0以下写法
                    _stream = GetMemoryStream(new GZipStream(webResponse.GetResponseStream(), CompressionMode.Decompress));
                }
                else
                {
                    //开始读取流并设置编码方式
                    //response.GetResponseStream().CopyTo(_stream, 10240);
                    //.net4.0以下写法
                    _stream = GetMemoryStream(webResponse.GetResponseStream());
                }

                //获取Byte
                byte[] RawResponse = _stream.ToArray();
                _stream.Close();

                //是否返回Byte类型数据
                netResponse.ResultByte = RawResponse;

                //从这里开始我们要无视编码了
                if (encoding == null)
                {
                    Match meta = Regex.Match(Encoding.Default.GetString(RawResponse), "<meta([^<]*)charset=([^<]*)[\"']", RegexOptions.IgnoreCase);
                    string charter = (meta.Groups.Count > 1) ? meta.Groups[2].Value.ToLower() : string.Empty;
                    charter = charter.Replace("\"", "").Replace("'", "").Replace(";", "").Replace("iso-8859-1", "gbk");
                    if (charter.Length > 2)
                        encoding = Encoding.GetEncoding(charter.Trim());
                    else
                    {
                        if (string.IsNullOrEmpty(webResponse.CharacterSet))
                            encoding = Encoding.UTF8;
                        else
                            encoding = Encoding.GetEncoding(webResponse.CharacterSet);
                    }
                }
                //得到返回的HTML
                netResponse.Content = encoding.GetString(RawResponse);

            }
            catch (WebException ex)
            {
                //这里是在发生异常时返回的错误信息
                webResponse = (HttpWebResponse)ex.Response;
                netResponse.Content = ex.Message;

                if (webResponse != null)
                {
                    netResponse.StatusCode = webResponse.StatusCode;
                    netResponse.StatusDescription = webResponse.StatusDescription;
                }
            }
            catch (Exception ex)
            {
                netResponse.Content = ex.Message;
            }

            return netResponse;
        }

        /// <summary>
        /// 4.0以下.net版本取数据使用
        /// </summary>
        /// <param name="streamResponse">流</param>
        private MemoryStream GetMemoryStream(Stream streamResponse)
        {
            MemoryStream _stream = new MemoryStream();
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = streamResponse.Read(buffer, 0, Length);
            // 读取数据流  
            while (bytesRead > 0)
            {
                _stream.Write(buffer, 0, bytesRead);
                bytesRead = streamResponse.Read(buffer, 0, Length);
            }
            return _stream;
        }

        /// <summary>
        /// 获取http请求对象.
        /// </summary>
        /// <param name="url">远程地址.</param>
        /// <param name="referer">引用地址.</param>
        /// <returns></returns>
        private HttpWebRequest GetWebRequest(string url, string referer)
        {
            HttpWebRequest httpRequest = null;

            //如果是发送HTTPS请求  
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback +=
                delegate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
                {
                    return true; // 总是接受  
                };

                httpRequest = WebRequest.Create(url) as HttpWebRequest;
                // 设置客户端证书
                if (!string.IsNullOrEmpty(_certPath) && !string.IsNullOrEmpty(_certPassword))
                {
                    X509Certificate2 cer = new X509Certificate2(_certPath, _certPassword);

                    httpRequest.ClientCertificates.Add(cer);
                }
                httpRequest.ProtocolVersion = HttpVersion.Version11;
            }
            else
            {
                httpRequest = WebRequest.Create(url) as HttpWebRequest;
            }

            // 用户代理
            httpRequest.UserAgent = DefaultUserAgent;

            // 设置Cookie
            if (_cookieContainer.Count > 0)
            {
                httpRequest.CookieContainer = _cookieContainer;
            }
            // 引用路径
            httpRequest.Referer = referer;
            if (_timeout > 0)
            {
                httpRequest.Timeout = _timeout;
            }

            httpRequest.ServicePoint.Expect100Continue = false;
            httpRequest.AllowAutoRedirect = false;
            httpRequest.Accept = "*/*";

            if (_header.Count > 0)
            {
                httpRequest.Headers.Add(_header);
            }

            if (_proxy != null)
            {
                if (_proxy.Credentials != null)
                {
                    httpRequest.UseDefaultCredentials = true;
                }
                httpRequest.Proxy = _proxy;
            }

            return httpRequest;
        }
        #endregion
    }
}
