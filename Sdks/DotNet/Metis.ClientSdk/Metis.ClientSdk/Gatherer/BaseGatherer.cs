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
        private static WebServerType iisVersion = WebServerType.Unknown;
        private bool debugRequest = false;
        //当前请求的上下文
        protected HttpApplication application;

        static BaseGatherer()
        {
            iisVersion = WebServer.GetIISServerType(); 
        }
        /// <summary>
        /// 当前IIS的部署环境
        /// </summary>
        protected static WebServerType IISVersion { get { return iisVersion; } }
        /// <summary>
        /// 采集者名称
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// 采集者描述
        /// </summary>
        public abstract string Description { get; }
        /// <summary>
        /// 是否为Debug的请求
        /// </summary>
        protected bool DebugRequest { get { return debugRequest; } }
        public virtual void BeginRequest() { }
        public virtual void EndRequest() { }
        public virtual void ExceptionOccur() { }
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
            if (application.Request.RawUrl.Equals("/gatherer/debug"))
            {
                debugRequest = true;
                if (!GathererContext.Current.IsDebug)
                {
                    application.Response.Write("Gatherer DEBUG mode is off");
                    application.CompleteRequest();
                }
                else
                {
                    string logInfos = GathererLogger.Instance.Read();
                    application.Response.Write(logInfos);
                    application.CompleteRequest();
                }
            }
        }

        protected void GetCallAccessToken(BaseGathererConfig config, 
            out string callAccesstoken, out int callAppId)
        {
            //获取扩展数据
            var extendData = config.ExtendDataPrivoder.GetExtendData(HttpContext.Current);

            callAccesstoken = GetCallAppAccesstoken();
            //如果还是空的
            if (String.IsNullOrWhiteSpace(callAccesstoken))
            {
                //如果从上下文中获取不到调用端的AccessToken
                if (extendData.ContainsKey(ConstVariables.CALL_ACCESS_TOKEN))
                {
                    callAccesstoken = extendData[ConstVariables.CALL_ACCESS_TOKEN].ToString();
                }
                //如果还是空
                if (String.IsNullOrWhiteSpace(callAccesstoken))
                {
                    //检查是否存在CallAppId
                    if (extendData.ContainsKey(ConstVariables.CALL_APP_ID))
                    {
                        Int32.TryParse(extendData[ConstVariables.CALL_APP_ID].ToString(), out callAppId);
                    }
                }
            }
        }

        /// <summary>
        /// 从Request中获取调用APP的Accesstoken
        /// </summary>
        protected string GetCallAppAccesstoken()
        {
            string accessToken = String.Empty;
            //从QueryString里面获取
            if (!String.IsNullOrEmpty(application.Request.QueryString[ConstVariables.CALL_ACCESS_TOKEN]))
            {
                accessToken = application.Request.QueryString[ConstVariables.CALL_ACCESS_TOKEN];
            }
            else if (!String.IsNullOrEmpty(application.Request.QueryString[ConstVariables.CALL_ACCESS_TOKEN_OLD]))
            {
                accessToken = application.Request.QueryString[ConstVariables.CALL_ACCESS_TOKEN_OLD];
            }
            //如果从QueryString里面获取不到
            if (String.IsNullOrEmpty(accessToken))
            {
                //从Form里面获取
                if (!String.IsNullOrEmpty(application.Request.Form[ConstVariables.CALL_ACCESS_TOKEN]))
                {
                    accessToken = application.Request.Form[ConstVariables.CALL_ACCESS_TOKEN];
                }
                else if (!String.IsNullOrEmpty(application.Request.Form[ConstVariables.CALL_ACCESS_TOKEN_OLD]))
                {
                    accessToken = application.Request.Form[ConstVariables.CALL_ACCESS_TOKEN_OLD];
                }
            }
            return accessToken;
        }
    }
}
