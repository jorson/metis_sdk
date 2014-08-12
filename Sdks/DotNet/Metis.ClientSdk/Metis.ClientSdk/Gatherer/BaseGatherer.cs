using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Metis.ClientSdk.Gatherer
{
    /// <summary>
    /// 采集器基础上下文对象
    /// </summary>
    internal abstract class BaseGatherer
    {
        //当前IIS的部署环境
        static WebServerType iisVersion = WebServerType.Unknown;
        bool debugRequest = false;
        //当前请求的上下文
        protected HttpContext context;

        static BaseGatherer()
        {
            iisVersion = WebServer.GetIISServerType(); 
        }
        /// <summary>
        /// 当前IIS的部署环境
        /// </summary>
        protected static WebServerType IISVersion { get { return iisVersion; } }
        /// <summary>
        /// 是否为Debug的请求
        /// </summary>
        protected bool DebugRequest { get { return debugRequest; } }
        public abstract void BeginRequest();
        public abstract void EndRequest();
        public abstract void Dispose();
        /// <summary>
        /// 获取URL中?之前的字符串
        /// </summary>
        /// <param name="url">原始的URL字符串</param>
        /// <returns>处理后的字符串</returns>
        protected string GetPureUrl(string url)
        {
            int count = url.IndexOf('?');
            if (count == -1)
                return url;
            return url.Substring(0, count);
        }
        /// <summary>
        /// 处理Debug页面的请求
        /// </summary>
        protected void HandleDebugInfo()
        {
            if (context.Request.RawUrl.Equals("/gatherer/debug"))
            {
                debugRequest = true;
                if (!GathererContext.Current.IsDebug)
                {
                    context.Response.Write("Gatherer DEBUG mode is off");
                    context.Response.End();
                }
                else
                {
                    string logInfos = GathererLogger.Instance.Read();
                    context.Response.Write(logInfos);
                    context.Response.End();
                }
            }
        }
    }
}
