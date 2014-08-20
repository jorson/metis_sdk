using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Metis.DemoSite
{
    public static class HttpRequestExtend 
    {
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
            foreach(string key in request.Headers.AllKeys)
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