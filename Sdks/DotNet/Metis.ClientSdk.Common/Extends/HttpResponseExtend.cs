using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Web
{
    internal static class HttpResponseExtend
    {
        public static string GetCombineHeaders(this HttpResponse response, bool asRaw)
        {
            StringBuilder builder = new StringBuilder();
            try
            {
                foreach (string key in response.Headers.AllKeys)
                {
                    builder.Append(key);
                    builder.Append(asRaw ? ": " : ":");
                    builder.Append(response.Headers[key]);
                    builder.Append("\r\n");
                }
            }
            catch
            {
                builder.Append("\r\n");
            }
            return builder.ToString();
        }
    }
}
