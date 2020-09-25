using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;
using System.Web;
using ECF.IO;

namespace ECF.Web.Http
{

    /// <summary>
    /// FullName： <see cref="ECF.Web.Http.SyncHttp"/>
    /// Summary ： 同步Http处理类
    /// Version： 1.0.0.0 
    /// DateTime： 2012/5/15 10:07 
    /// CopyRight (c) by shaipe
    /// </summary>
    public class SyncHttp
    {
        /// <summary>
        /// http请求的超时时间
        /// </summary>
        public static int Timeout = 300000;

        #region HttpGet 同步方式发起http get请求
        /// <summary>
        /// 同步方式发起http get请求
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string HttpGet(string url)
        {
            string errorMsg = string.Empty;
            return HttpGet(url, out errorMsg);
        }
        /// <summary>
        /// 同步方式发起http get请求
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="errorMsg">获取错误信息.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string HttpGet(string url, out string errorMsg)
        {
            return HttpGet(url, "", out errorMsg);
        }

        /// <summary>
        /// 同步方式发起http get请求.
        /// </summary>
        /// <param name="url">请求URL.</param>
        /// <param name="pararmeters">请求参数列表.</param>
        /// <param name="errorMsg">错误信息.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="encode">编码格式.</param>
        /// <returns>
        /// String
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>说明原因 added by Shaipe 2018/10/23</description></item>
        ///   </list>
        /// </remarks>
        public static string HttpGet(string url, QueryParameter[] pararmeters, out string errorMsg, NameValueCollection headers = null, string encode = "UTF-8")
        {
            string querystring = pararmeters.ToLinkString();
            return HttpGet(url, querystring, out errorMsg, null, encode);
        }


        /// <summary>
        /// 同步方式发起http get请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="queryString">参数字符串</param>
        /// <param name="errorMsg">错误信息.</param>
        /// <param name="encode">编码格式.</param>
        /// <param name="headers">添加网络请求的头部.</param>
        /// <returns>
        /// 请求返回值
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>扩展网络请求头部对外参数 modify by xiepeng 2018/9/21</description></item>
        ///   </list>
        /// </remarks>
        public static string HttpGet(string url, string queryString, out string errorMsg, NameValueCollection headers = null, string encode = "UTF-8")
        {
            errorMsg = string.Empty;

            if (!string.IsNullOrEmpty(queryString))
            {
                if (url.IndexOf("?") > -1)
                {
                    url += "&" + queryString;
                }
                else
                {
                    url += "?" + queryString;
                }

            }
            string referer = "";

            //if (HttpContext.Current != null && HttpContext.Current.Request != null)
            //{
            //    referer = HttpContext.Current.Request.Url.AbsoluteUri;
            //}
            NetRequest request = new NetRequest();
            if (headers != null)
            {
                foreach (string key in headers)
                {
                    request.AddHeader(key, headers[key]);
                }
            }
            NetResponse response = request.Get(url, referer, out errorMsg);

            return response.Content;

        }


        #endregion

        #region HttpPost 同步方式发起http post请求

        /// <summary>
        /// 同步方式发起http post请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <returns>请求返回值</returns>
        public static string HttpPost(string url)
        {
            string errorMsg = string.Empty;
            return HttpPost(url, "", out errorMsg);
        }

        /// <summary>
        /// 同步方式发起http post请求
        /// </summary>
        /// <param name="url">请求URL.</param>
        /// <param name="errorMsg">请求返回值.</param>
        /// <param name="headers">需要传输的Header信息.</param>
        /// <param name="encode">编码格式.</param>
        /// <returns>
        /// System.String
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加Headers参数 modify by Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        public static string HttpPost(string url, out string errorMsg, NameValueCollection headers = null, string encode = "UTF-8")
        {
            return HttpPost(url, "", out errorMsg, headers, encode);
        }

        /// <summary>
        /// HTTP post
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="paras">The paras.</param>
        /// <param name="headers">需要传输的Header信息.</param>
        /// <param name="encode">编码格式.</param>
        /// <returns>
        /// System.String
        /// </returns>
        /// <remarks>
        /// <list>
        ///   <item>
        ///     <description>添加Headers参数 modify by Shaipe 2018/9/25</description>
        ///   </item>
        /// </list>
        /// </remarks>
        public static string HttpPost(string url, QueryParameter[] paras, NameValueCollection headers = null, string encode = "UTF-8")
        {
            string querystring = paras.ToLinkString(), errorMsg = string.Empty;
            return HttpPost(url, querystring, out errorMsg, headers, encode);
        }

        /// <summary>
        /// 同步方式发起http post请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="paras">请求参数列表</param>
        /// <param name="errorMsg">返回错误信息.</param>
        /// <param name="headers">需要传输的Header信息.</param>
        /// <param name="encode">编码格式.</param>
        /// <returns>
        /// 请求返回值
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加Headers参数 modify by Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        public static string HttpPost(string url, QueryParameter[] paras, out string errorMsg, NameValueCollection headers = null, string encode = "UTF-8")
        {
            string querystring = paras.ToLinkString();
            return HttpPost(url, querystring, out errorMsg, headers, encode);
        }

        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <param name="queryParams">请求参数</param>
        /// <param name="errorMsg">返回错误信息.</param>
        /// <param name="headers">需要传输的Header信息.</param>
        /// <param name="encode">编码格式.</param>
        /// <returns>
        /// 返回的请求内容
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加Headers参数 modify by Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        public static string HttpPost(string url, Dictionary<string, object> queryParams, out string errorMsg, NameValueCollection headers = null, string encode = "UTF-8")
        {

            return HttpPost(url, queryParams.ToLinkString(), out errorMsg, headers, encode);
        }

        /// <summary>
        /// 发送POST请求
        /// </summary>
        /// <param name="url">请求的url</param>
        /// <param name="queryParams">请求参数</param>
        /// <param name="errorMsg">返回错误信息.</param>
        /// <param name="headers">需要传输的Header信息.</param>
        /// <param name="encode">编码格式.</param>
        /// <returns>
        /// 返回的请求内容
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加Headers参数 modify by Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        public static string HttpPost(string url, NameValueCollection queryParams, out string errorMsg, NameValueCollection headers = null, string encode = "UTF-8")
        {

            string data = "";
            if (queryParams.Count > 0)
            {
                foreach (string k in queryParams)
                {
                    data += k + "=" + queryParams[k];
                    data += "&";
                }
                data = data.TrimEnd('&');
            }
            return HttpPost(url, data, out errorMsg);
        }

        /// <summary>
        /// 同步方式发起http post请求
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="queryString">参数字符串</param>
        /// <param name="errorMsg">返回错误信息.</param>
        /// <param name="headers">需要传输的Header信息.</param>
        /// <param name="encode">编码格式.</param>
        /// <returns>
        /// 请求返回值
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加Headers参数 modifyby Shaipe 2018/9/25</description></item>
        ///   </list>
        /// </remarks>
        public static string HttpPost(string url, string queryString, out string errorMsg, NameValueCollection headers = null, string encode = "UTF-8")
        {
            errorMsg = string.Empty;

            string referer = "";

            //if (HttpContext.Current != null && HttpContext.Current.Request != null)
            //{
            //    referer = HttpContext.Current.Request.Url.AbsoluteUri;
            //}

            NetResponse response = new NetRequest().Post(url, queryString, referer, out errorMsg);

            return response.Content;

        }



        /// <summary>
        /// 同步方式发起http post请求.
        /// </summary>
        /// <param name="url">请求Url地址.</param>
        /// <param name="format">Post数据格式.</param>
        /// <param name="data">发送的数据.</param>
        /// <param name="encode">数据编码格式.</param>
        /// <param name="errorMsg">返回错误信息.</param>
        /// <param name="headers">需要传输的Header信息.</param>
        /// <returns>
        /// System.String
        /// </returns>
        /// <remarks>
        ///   <list>
        ///    <item><description>添加请求的Header参数 modify by xiepeng 2018/9/20</description></item>
        ///   </list>
        /// </remarks>
        public static string HttpPost(string url, string format, object data, Encoding encode, out string errorMsg, NameValueCollection headers = null)
        {
            string referer = "";

            //if (HttpContext.Current != null && HttpContext.Current.Request != null)
            //{
            //    referer = HttpContext.Current.Request.Url.AbsoluteUri;
            //}
            if (data != null)
            {
                NetRequest request = new NetRequest();
                if (headers != null)
                {
                    foreach (string key in headers)
                    {
                        request.AddHeader(key, headers[key]);
                    }
                }
                NetResponse response = request.Post(url, data.ToString(), referer, out errorMsg, format, encode.BodyName);

                return response.Content;
            }
            else
            {
                errorMsg = "post数据为空";
                return string.Empty;
            }

        }

        #endregion

        #region HttpPostWithFile 同步方式发起http post请求，可以同时上传文件

        /// <summary>
        /// 同步方式发起http post请求，可以同时上传文件
        /// </summary>
        /// <param name="url">请求URL</param>
        /// <param name="listParams">请求参数字符串</param>
        /// <param name="files">上传文件列表</param>
        /// <returns>请求返回值</returns>
        public static string HttpPostWithFile(string url, QueryParameter[] listParams, QueryParameter[] files)
        {
            Stream requestStream = null;
            StreamReader responseReader = null;
            string responseData = null;
            string boundary = DateTime.Now.Ticks.ToString("x");
            HttpWebRequest webRequest = null;
            Stream responseStream = null;
            try
            {
                webRequest = WebRequest.Create(url) as HttpWebRequest;
                webRequest.ServicePoint.Expect100Continue = false;
                webRequest.Timeout = Timeout;
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                webRequest.Method = "POST";
                webRequest.KeepAlive = true;
                webRequest.Credentials = CredentialCache.DefaultCredentials;

                string referer = "http://www.shaipe.cn";
                //if (HttpContext.Current != null && HttpContext.Current.Request != null)
                //{
                //    referer = HttpContext.Current.Request.Url.ToString();
                //}

                webRequest.Referer = referer;


                Stream memStream = new MemoryStream();

                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");
                string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";

                foreach (QueryParameter param in listParams)
                {
                    string formitem = string.Format(formdataTemplate, param.Name, param.Value);
                    byte[] formitembytes = Encoding.UTF8.GetBytes(formitem);
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }

                memStream.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: \"{2}\"\r\n\r\n";

                foreach (QueryParameter param in files)
                {
                    string name = param.Name;
                    string filePath = param.Value;
                    string file = Path.GetFileName(filePath);
                    string contentType = FileUtils.GetContentType(file);

                    string header = string.Format(headerTemplate, name, file, contentType);
                    byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);

                    memStream.Write(headerbytes, 0, headerbytes.Length);

                    FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                    byte[] buffer = new byte[1024];
                    int bytesRead = 0;

                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                    {
                        memStream.Write(buffer, 0, bytesRead);
                    }

                    memStream.Write(boundarybytes, 0, boundarybytes.Length);
                    fileStream.Close();
                }

                webRequest.ContentLength = memStream.Length;

                requestStream = webRequest.GetRequestStream();

                memStream.Position = 0;
                byte[] tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();
                requestStream = null;

                responseStream = webRequest.GetResponse().GetResponseStream();
                responseReader = new StreamReader(responseStream);
                responseData = responseReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new ECFException(ex.Message, ex);
            }
            finally
            {
                if (requestStream != null)
                {
                    requestStream.Close();
                    requestStream = null;
                }

                if (responseStream != null)
                {
                    responseStream.Close();
                    responseStream = null;
                }

                if (responseReader != null)
                {
                    responseReader.Close();
                    responseReader = null;
                }

                webRequest = null;
            }

            return responseData;
        }


        #endregion

        #region HttpPost 请求带文件
        /// <summary>
        /// HTTP post 请求带文件
        /// </summary>
        /// <param name="url">请求地址.</param>
        /// <param name="param">请求参数.</param>
        /// <param name="filePath">请求文件路径.</param>
        /// <param name="fieldName">请求文件路径.</param>
        /// <returns>
        /// System.String
        /// </returns>
        public static string HttpPost(string url, IDictionary<object, object> param, string filePath, string fieldName = "pic")
        {
            try
            {
                string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
                byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

                HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
                wr.ContentType = "multipart/form-data; boundary=" + boundary;
                wr.Method = "POST";
                wr.KeepAlive = true;
                wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

                string referer = "http://www.shaipe.cn";
                //if (HttpContext.Current != null && HttpContext.Current.Request != null)
                //{
                //    referer = HttpContext.Current.Request.Url.ToString();
                //}

                wr.Referer = referer;

                Stream rs = wr.GetRequestStream();
                string responseStr = null;

                string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
                foreach (string key in param.Keys)
                {
                    rs.Write(boundarybytes, 0, boundarybytes.Length);
                    string formitem = string.Format(formdataTemplate, key, param[key]);
                    byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                    rs.Write(formitembytes, 0, formitembytes.Length);
                }
                rs.Write(boundarybytes, 0, boundarybytes.Length);

                string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
                string header = string.Format(headerTemplate, fieldName, filePath, "text/plain");
                byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
                rs.Write(headerbytes, 0, headerbytes.Length);

                FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                byte[] buffer = new byte[4096];
                int bytesRead = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    rs.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
                rs.Write(trailer, 0, trailer.Length);
                rs.Close();

                WebResponse wresp = null;
                try
                {
                    wresp = wr.GetResponse();
                    Stream stream2 = wresp.GetResponseStream();
                    StreamReader reader2 = new StreamReader(stream2);
                    responseStr = reader2.ReadToEnd();
                    //logger.Debug(string.Format("File uploaded, server response is: {0}", responseStr));
                }
                catch (Exception ex)
                {
                    new ECFException("Error uploading file", ex);
                    if (wresp != null)
                    {
                        wresp.Close();
                        wresp = null;
                    }
                }
                finally
                {
                    wr = null;
                }
                return responseStr;
            }
            catch (Exception e1)
            {
                new ECFException("URL:" + url + "\r\n" + e1.Message, e1);
                return null;
            }

        }
        #endregion

        #region HttpPost 请求数据(带图片)
        /// <summary>
        /// HTTP POST方式请求数据(带图片)
        /// </summary>
        /// <param name="url">URL</param>        
        /// <param name="param">POST的数据</param>
        /// <param name="fileByte">图片</param>
        /// <param name="fieldName">字段名,用于上传时在Files里取的字段名</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public static string HttpPost(string url, IDictionary<object, object> param, byte[] fileByte, string fieldName = "pic", string fileName = "xxx.png")
        {
            string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

            HttpWebRequest wr = (HttpWebRequest)WebRequest.Create(url);
            wr.ContentType = "multipart/form-data; boundary=" + boundary;
            wr.Method = "POST";
            wr.KeepAlive = true;
            wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

            string referer = "http://www.shaipe.cn";
            //if (HttpContext.Current != null && HttpContext.Current.Request != null)
            //{
            //    referer = HttpContext.Current.Request.Url.ToString();
            //}


            wr.Referer = referer;


            Stream rs = wr.GetRequestStream();
            string responseStr = null;

            string formdataTemplate = "Content-Disposition: form-data; name=\"{0}\"\r\n\r\n{1}";
            foreach (string key in param.Keys)
            {
                rs.Write(boundarybytes, 0, boundarybytes.Length);
                string formitem = string.Format(formdataTemplate, key, param[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                rs.Write(formitembytes, 0, formitembytes.Length);
            }
            rs.Write(boundarybytes, 0, boundarybytes.Length);

            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
            string header = string.Format(headerTemplate, fieldName, fileName, "text/plain");//image/jpeg
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            rs.Write(headerbytes, 0, headerbytes.Length);

            rs.Write(fileByte, 0, fileByte.Length);

            byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
            rs.Write(trailer, 0, trailer.Length);
            rs.Close();

            WebResponse wresp = null;
            try
            {
                wresp = wr.GetResponse();
                Stream stream2 = wresp.GetResponseStream();
                StreamReader reader2 = new StreamReader(stream2);
                responseStr = reader2.ReadToEnd();
                new ECFException(string.Format("File uploaded, server response is: {0}", responseStr));
            }
            catch (Exception ex)
            {
                new ECFException("Error uploading file  URL: " + url + "\r\n" + ex.Message, ex);
                if (wresp != null)
                {
                    wresp.Close();
                    wresp = null;
                }
            }
            finally
            {
                wr = null;
            }
            return responseStr;
        }
        #endregion

    }
}
