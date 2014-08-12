using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Specialized;
using System.Web;

namespace Metis.ClientSdk
{
    internal class GaeaHttpClient 
    {
        bool isAsync = false;

        public GaeaHttpClient()
        {
            this.Timeout = 2000;
        }

        public GaeaHttpClient(int timeout, WebHeaderCollection headers = null, 
            Encoding encoding = null, string host = null, bool async = false)
            : this()
        {
            this.Timeout = timeout;
            this.isAsync = async;
            this.Headers = new WebHeaderCollection();
            if (encoding == null)
                this.Encoding = Encoding.UTF8;
        }

        public int Timeout { get; set; }
        public Encoding Encoding { get; set; }
        public WebHeaderCollection Headers { get; set; }
        public string Host { get; set; }
        public bool Async { get { return isAsync; } set { isAsync = value; } }

        string ConvertToJson(object instance)
        {
            Arguments.NotNull(instance, "instance");
            JavaScriptSerializer jss = new JavaScriptSerializer();
            return jss.Serialize(instance);
        }

        protected virtual WebClient CreateWebClient()
        {
            var client = new InnerWebClient(Timeout, OnCreateWebRequestUri, OnGetWebRequest, OnGetWebResponse);
            if (Headers != null)
            {
                for (var i = 0; i < Headers.Count; i++)
                {
                    client.Headers.Add(Headers.GetKey(i), Headers.Get(i));
                }
            }
            if (Encoding != null)
            {
                client.Encoding = Encoding;
            }
            if (this.Async)
            {
                client.DownloadStringCompleted += DownloadStringCompleted;
                client.UploadValuesCompleted += UploadValueCompleted;
                client.UploadStringCompleted += UploadStringCompleted;
            }
            return client;
        }

        void DownloadStringCompleted(object sender, DownloadStringCompletedEventArgs e)
        {
        }

        void UploadValueCompleted(object sender, UploadValuesCompletedEventArgs e)
        {
        }

        void UploadStringCompleted(object sender, UploadStringCompletedEventArgs e)
        {
        }

        protected virtual Uri OnCreateWebRequestUri(Uri uri)
        {
            return uri;
        }

        protected virtual void OnGetWebRequest(WebRequest request)
        {
            if (request is HttpWebRequest && !String.IsNullOrEmpty(Host))
            {
                ((HttpWebRequest)request).Host = Host;
            }
        }

        protected virtual void OnGetWebResponse(WebResponse response)
        {
        }

        protected virtual string Get(Uri url)
        {
            using (var client = CreateWebClient())
            {
                try
                {
                    return client.DownloadString(url);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("请求地址 {0} 发生错误。", url), ex);
                }
            }
        }

        protected virtual string Post(string url, NameValueCollection data)
        {
            using (var client = CreateWebClient())
            {
                try
                {
                    var buffer = client.UploadValues(url, data);
                    return client.Encoding.GetString(buffer);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("请求地址 {0} 发生错误。", url), ex);
                }
            }
        }

        protected virtual string PostJson(string url, object instance)
        {
            var json = ConvertToJson(instance);
            using (var client = CreateWebClient())
            {
                try
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    return client.UploadString(url, json);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("请求地址 {0} 发生错误。", url), ex);
                }
            }
        }

        protected virtual void GetAsync(Uri uri)
        {
            if (!this.Async)
                throw new ArgumentException("Please initialize attribute 'Async' to 'TRUE'");

            using (var client = CreateWebClient())
            {
                client.DownloadStringAsync(uri);
            }
        }

        protected virtual void PostAsync(Uri uri, NameValueCollection data)
        {
            if (!this.Async)
                throw new ArgumentException("Please initialize attribute 'Async' to 'TRUE'");

            using (var client = CreateWebClient())
            {
                client.UploadValuesAsync(uri, data);
            }
        }

        protected virtual void PostJsonAsync(Uri uri, object instance)
        {
            if (!this.Async)
                throw new ArgumentException("Please initialize attribute 'Async' to 'TRUE'");

            var json = ConvertToJson(instance);
            using (var client = CreateWebClient())
            {
                try
                {
                    client.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                    client.UploadStringAsync(uri, json);
                }
                catch (Exception ex)
                {
                    throw new Exception(String.Format("请求地址 {0} 发生错误。", uri.AbsoluteUri), ex);
                }
            }
        }

        public string HttpGet(string url, string key, string value)
        {
            url = UriPath.AppendArguments(url, key, value);
            if (this.Async)
            {
                GetAsync(new Uri(url));
                return String.Empty;
            }
            else
                return Get(new Uri(url));
        }

        public string HttpGet(string url, NameValueCollection data)
        {
            url = UriPath.AppendArguments(url, data);
            if(this.Async)
            {
                GetAsync(new Uri(url));
                return String.Empty;
            }
            else
                return Get(new Uri(url));
        }

        public string HttpGet(string url, object values)
        {
            url = UriPath.AppendArguments(url, values);
            if(this.Async)
            {
                GetAsync(new Uri(url));
                return String.Empty;
            }
            else
                return Get(new Uri(url));
            
        }

        public string HttpPost(string url, string key, string value)
        {
            var data = new NameValueCollection();
            data.Add(key, value);

            if (this.Async)
            {
                PostAsync(new Uri(url), data);
                return String.Empty;
            }
            else
                return Post(url, data);
        }

        public string HttpPost(string url, object values)
        {
            var data = UriPath.ToNameValueCollection(values);
            if (this.Async)
            {
                PostAsync(new Uri(url), data);
                return String.Empty;
            }
            else
                return Post(url, data);
        }

        public virtual string HttpPost(string url, NameValueCollection data)
        {
            if (this.Async)
            {
                PostAsync(new Uri(url), data);
                return String.Empty;
            }
            else
                return Post(url, data);
        }

        public string HttpPostJson(string url, object instance, object values)
        {
            url = UriPath.AppendArguments(url, values);

            if (this.Async)
            {
                PostJsonAsync(new Uri(url), instance);
                return String.Empty;
            }
            else
                return PostJson(url, instance);
        }

        public string HttpPostJson(string url, object instance, NameValueCollection data)
        {
            url = UriPath.AppendArguments(url, data);
            if (this.Async)
            {
                PostJsonAsync(new Uri(url), instance);
                return String.Empty;
            }
            else
                return PostJson(url, instance);
        }
    }
}
