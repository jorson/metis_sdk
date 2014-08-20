using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Metis.DemoSite;

namespace Metis.DemoSite.Privoder
{
    public class DemoModule : IHttpModule
    {
        StreamWatcher watcher;

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += context_BeginRequest;
            context.EndRequest += context_EndRequest;
        }

        void context_EndRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;

            System.Reflection.FieldInfo fi = context.Response.Output.GetType().GetField("_charBufferLength", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField);

            System.Reflection.FieldInfo fi2 = context.Response.Output.GetType().GetField("_charBufferFree", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.GetField);

            int charBuffer = (int)fi.GetValue(context.Response.Output);
            int charBufferFree = (int)fi2.GetValue(context.Response.Output);

            System.Diagnostics.Debug.WriteLine("charBuffer:" + charBuffer);
            System.Diagnostics.Debug.WriteLine("charBufferFree:" + charBufferFree);

            Type responseType = typeof(HttpResponse);
            System.Reflection.MethodInfo method = responseType.GetMethod("GetBufferedLength", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            if (method != null)
            {
                var result = method.Invoke(context.Response, null);
                System.Diagnostics.Debug.WriteLine("Response-Size2:" + result);
            }
            System.Diagnostics.Debug.WriteLine("Response-Size2:" + watcher.Length);
            
        }

        void context_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = application.Context;

            if (context.Request.RequestType.Equals("post", StringComparison.CurrentCultureIgnoreCase))
            {
                System.Diagnostics.Debug.WriteLine("Total-Bytes:" + context.Request.TotalBytes);
                System.Diagnostics.Debug.WriteLine("Input-Bytes:" + context.Request.InputStream.Length);
                System.Diagnostics.Debug.WriteLine("Count-Request-Size:" + CountRequestSize(context.Request));
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Total-Bytes:" + context.Request.TotalBytes);
                System.Diagnostics.Debug.WriteLine("Count-Request-Size:" + CountRequestSize(context.Request));
            }

            watcher = new StreamWatcher(context.Response.Filter);
            context.Response.Filter = watcher;
        }

        long CountRequestSize(HttpRequest request)
        {
            MemoryStream ms = new MemoryStream();
            try
            {
                TextWriter writer = new StreamWriter(ms);
                writer.Write(request.HttpMethod + " " + request.Path);
                string queryStringText = request.GetQueryStringText();
                if (!String.IsNullOrWhiteSpace(queryStringText))
                {
                    writer.Write("?" + queryStringText);
                }
                string headerCombine = request.GetCombineHeaders(true);
                if (!String.IsNullOrWhiteSpace(headerCombine))
                {
                    writer.Write(headerCombine);
                }
                writer.Write("\r\n");
                writer.Flush();

                return request.InputStream.Length + ms.Length;
            }
            finally
            {
                ms.Close();
            }            
        }
    }
}