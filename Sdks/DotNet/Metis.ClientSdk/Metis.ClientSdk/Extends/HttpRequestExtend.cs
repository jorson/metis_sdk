using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace System.Web
{
    internal static class HttpRequestExtend
    {
        /// <summary>
        /// 获取Request的Size
        /// </summary>
        /// <param name="request">HttpRequest对象</param>
        /// <returns>请求的大小</returns>
        public static long CountRequestSize(this HttpRequest request)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                TextWriter writer = new StreamWriter(ms);
                writer.Write(request.HttpMethod + " " + request.Path);

                //如果请求类型为GET,需要计算Querystring的长度
                if(request.RequestType.Equals("get", StringComparison.CurrentCultureIgnoreCase))
                {
                    string queryStringText = request.GetQueryStringText();
                    if (!String.IsNullOrWhiteSpace(queryStringText))
                    {
                        writer.Write("?" + queryStringText);
                    }
                }
                //计算Header的长度
                string headerCombine = request.GetCombineHeaders(true);
                if (!String.IsNullOrWhiteSpace(headerCombine))
                {
                    writer.Write(headerCombine);
                }
                writer.Write("\r\n");
                writer.Flush();
                //结果为InputStream的长度+之前流的长度
                return request.InputStream.Length + ms.Length;
            }
            finally
            {
                ms.Close();
            }
        }

        public static string GetQueryStringText(this HttpRequest request)
        {
            if (request.QueryString.Count == 0)
                return String.Empty;
            //找第一个"?"
            int questionMarkIndex = request.RawUrl.IndexOf('?');
            if (questionMarkIndex > 0)
                return request.RawUrl.Substring(questionMarkIndex + 1);
            return String.Empty;
        }

        public static string GetCombineHeaders(this HttpRequest request, bool asRaw)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string key in request.Headers.AllKeys)
            {
                builder.Append(key);
                builder.Append(asRaw ? ": " : ":");
                builder.Append(request.Headers[key]);
                builder.Append("\r\n");
            }
            return builder.ToString();
        }

    }
}
