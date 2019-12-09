using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace CC.Helper
{
    public class CCHttpRequest
    {
        private static readonly string DefaultUserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; SV1; .NET CLR 1.1.4322; .NET CLR 2.0.50727)";
        private static readonly Encoding DefaultEncoding = Encoding.Default;

        /// <summary> 
        /// 创建GET方式的HTTP请求 
        /// </summary> 
        /// <param name="url">请求的URL</param> 
        /// <param name="timeout">请求的超时时间</param> 
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param> 
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param> 
        /// <returns></returns> 
        public static async Task<HttpWebResponse> CreateGetHttpResponseAsync(string url, IDictionary<string, string> parameters = null, int? timeout = 5000, string userAgent = null, CookieCollection cookies = null, Dictionary<string, string> header = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }

            HttpWebRequest request = WebRequest.Create(UrlSplicing(url, parameters)) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = DefaultUserAgent;
            if (!string.IsNullOrEmpty(userAgent))
            {
                request.UserAgent = userAgent;
            }
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
                request.ReadWriteTimeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            if (header != null)
            {
                foreach (var item in header)
                {
                    Console.WriteLine(item.Key);
                    request.Headers.Add(item.Key, item.Value);
                }
            }
            return (await request.GetResponseAsync()) as HttpWebResponse;
        }

        /// <summary> 
        /// 创建POST方式的HTTP请求 
        /// </summary> 
        /// <param name="url">请求的URL</param> 
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param> 
        /// <param name="timeout">请求的超时时间</param> 
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param> 
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param> 
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param> 
        /// <returns></returns> 
        public static async Task<HttpWebResponse> CreatePostHttpResponseAsync(string url, IDictionary<string, string> parameters = null, int? timeout = 5000, string userAgent = null, Encoding requestEncoding = null, CookieCollection cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            requestEncoding = requestEncoding ?? DefaultEncoding;
            HttpWebRequest request = null;
            //如果是发送HTTPS请求 
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = userAgent ?? DefaultUserAgent;
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据 
            if (!(parameters == null || parameters.Count == 0))
            {
                byte[] data = requestEncoding.GetBytes(DicCombination(parameters));
                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }
            }
            HttpWebResponse res = (await request.GetResponseAsync()) as HttpWebResponse;
            return res;
        }

        /// <summary> 
        /// 创建POST方式的HTTP请求 
        /// </summary> 
        /// <param name="url">请求的URL</param> 
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param> 
        /// <param name="timeout">请求的超时时间</param> 
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param> 
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param> 
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param> 
        /// <returns></returns> 
        public static async Task<HttpWebResponse> CreatePostHttpResponseAsync(string url, string parameters, int? timeout = 5000, string userAgent = null, Encoding requestEncoding = null, CookieCollection cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            requestEncoding = requestEncoding ?? DefaultEncoding;
            HttpWebRequest request = null;
            //如果是发送HTTPS请求 
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = userAgent ?? DefaultUserAgent;
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据 
            if (!string.IsNullOrEmpty(parameters))
            {
                byte[] data = requestEncoding.GetBytes(parameters);
                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }
            }
            HttpWebResponse res = (await request.GetResponseAsync()) as HttpWebResponse;
            return res;
        }

        /// <summary> 
        /// 创建POST方式的HTTP请求(数据为JSON格式)
        /// </summary> 
        /// <param name="url">请求的URL</param> 
        /// <param name="parameters">随同请求POST的参数名称及参数值字典</param> 
        /// <param name="timeout">请求的超时时间</param> 
        /// <param name="userAgent">请求的客户端浏览器信息，可以为空</param> 
        /// <param name="requestEncoding">发送HTTP请求时所用的编码</param> 
        /// <param name="cookies">随同HTTP请求发送的Cookie信息，如果不需要身份验证可以为空</param> 
        /// <returns></returns> 
        public static async Task<HttpWebResponse> CreatePostHttpResponseJsonAsync(string url, IDictionary<string, string> parameters = null, int? timeout = 5000, string userAgent = null, Encoding requestEncoding = null, CookieCollection cookies = null)
        {
            if (string.IsNullOrEmpty(url))
            {
                throw new ArgumentNullException("url");
            }
            requestEncoding = requestEncoding ?? DefaultEncoding;
            HttpWebRequest request = null;
            //如果是发送HTTPS请求 
            if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = userAgent ?? DefaultUserAgent;
            if (timeout.HasValue)
            {
                request.Timeout = timeout.Value;
            }
            if (cookies != null)
            {
                request.CookieContainer = new CookieContainer();
                request.CookieContainer.Add(cookies);
            }
            //如果需要POST数据 
            if (!(parameters == null || parameters.Count == 0))
            {
                byte[] data = requestEncoding.GetBytes(JsonConvert.SerializeObject(parameters));
                using (Stream stream = await request.GetRequestStreamAsync())
                {
                    await stream.WriteAsync(data, 0, data.Length);
                }
            }
            HttpWebResponse res = (await request.GetResponseAsync()) as HttpWebResponse;
            return res;
        }


        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受 
        }

        public static string gb2312Toutf8(string gb2312Str)
        {

            System.Text.Encoding GB2312 = System.Text.Encoding.GetEncoding("GB2312");
            System.Text.Encoding UTF8 = System.Text.Encoding.UTF8;
            return UTF8.GetString(GB2312.GetBytes(gb2312Str));
        }

        public static string utf8Togb2312(string utf8Str)
        {
            System.Text.Encoding GB2312 = System.Text.Encoding.GetEncoding("GB2312");
            System.Text.Encoding UTF8 = System.Text.Encoding.UTF8;
            return GB2312.GetString(UTF8.GetBytes(utf8Str));
        }


        /// <summary>
        /// 参数组合
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string DicCombination(IDictionary<string, string> parameters)
        {
            StringBuilder buffer = new StringBuilder();
            //如果需要POST数据 
            if (!(parameters == null || parameters.Count == 0))
            {
                foreach (string key in parameters.Keys)
                {
                    buffer.AppendFormat("&{0}={1}", key, parameters[key]);

                }
            }
            return buffer.ToString();
        }

        /// <summary>
        /// 地址拼接
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private static string UrlSplicing(string url, IDictionary<string, string> parameters)
        {
            if (url.Contains("?"))
                return $"{url}{DicCombination(parameters)}";
            else
                return $"{url}?{DicCombination(parameters)}";
        }
    }
}
