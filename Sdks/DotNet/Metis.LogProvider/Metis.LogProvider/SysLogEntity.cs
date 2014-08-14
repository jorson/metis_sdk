using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Core;
using System.Web;
using System.Collections.Specialized;
using System.Web.Script.Serialization;

namespace Metis.ClientSdk.LogProvider
{
    internal class SysLogEntity : Metis.ClientSdk.Entities.LogEntity
    {
        /// <summary>
        /// 序列化器
        /// </summary>
        private static readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        //日志等级
        private LogLevel logLevel = LogLevel.INFO;
        //日志记录者
        private string logger = String.Empty;
        //日志消息
        private string logMessage = String.Empty;
        //调用堆栈
        private string callInfo = String.Empty;
        public SysLogEntity()
        {
            
        }
        public LogLevel LogLevel { get { return logLevel; } protected set { logLevel = value; } }
        public string Logger { get { return logger; } protected set { logger = value; } }
        public string LogMessage { get { return logMessage; } protected set { logMessage = value; } }
        public string CallInfo { get { return callInfo; } protected set { callInfo = value; } }
        /// <summary>
        /// 将log4j的日志事件转换为SysLogEntity对象
        /// </summary>
        public void TryParseLoggingEvent(LoggingEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException("logEvent");
            }
            LoggingEventData data = logEvent.GetLoggingEventData();
            //日志事件的消息
            this.logMessage = data.Message;
            //日志事件级别
            TransLogLevel(logEvent.Level);
            //记录者名称
            this.logger = data.LoggerName;
            CallStack callStack = new CallStack();
            GetContextInfo(callStack);
            //设置调用信息
            if (logEvent.ExceptionObject != null)
            {
                GetExceptionInfo(callStack, logEvent.ExceptionObject);
            }
            //序列化对象
            this.callInfo = serializer.Serialize(callStack);
        }
        /// <summary>
        /// 转换日志级别
        /// </summary>
        private void TransLogLevel(log4net.Core.Level level)
        {
            if (level == log4net.Core.Level.Debug)
                this.logLevel = LogProvider.LogLevel.DEBUG;
            else if (level == log4net.Core.Level.Info)
                this.logLevel = LogProvider.LogLevel.INFO;
            else if (level == log4net.Core.Level.Warn)
                this.logLevel = LogProvider.LogLevel.WARN;
            else if (level == log4net.Core.Level.Error)
                this.logLevel = LogProvider.LogLevel.ERROR;
            else if (level == log4net.Core.Level.Fatal)
                this.logLevel = LogProvider.LogLevel.FATAL;
        }
        /// <summary>
        /// 构建调用信息
        /// </summary>
        private void GetContextInfo(CallStack callStack)
        {
            var context = HttpContext.Current;

            callStack.AbsolutePath = context.Request.Url.AbsolutePath;
            callStack.ReferrerUrl = context.Request.UrlReferrer == null ?
                String.Empty : context.Request.UrlReferrer.AbsolutePath;
            callStack.QueryData = context.Request.Url.Query;
            if (!context.IsLargeRequest())
            {
                callStack.FormData = ConvertNameValueCollection(context.Request.Form);
            }
            callStack.User = new CallStack.UserIdentity()
            {
                IsAuthenticated = context.User.Identity.IsAuthenticated,
                Name = context.User.Identity.Name
            };
        }
        /// <summary>
        /// 构建异常信息
        /// </summary>
        private void GetExceptionInfo(CallStack callStack, Exception ex)
        {
            callStack.ExData = new CallStack.ExceptionData();
            callStack.ExData.ExceptionType = ex.GetType().FullName;
            callStack.ExData.CauseSource = ex.Source;
            if (ex.TargetSite != null)
                callStack.ExData.CauseMethod = ex.TargetSite.ToString();
            callStack.ExData.ErrorMessage = ex.Message;
            callStack.ExData.TraceStack = ex.StackTrace;
        }
        /// <summary>
        /// 根据键值对集合获取指定格式字符串
        /// </summary>
        /// <param name="value">键值对集合</param>
        /// <returns>格式如:key1=value1&key2=value2的字符串</returns>
        private static string ConvertNameValueCollection(NameValueCollection nvc)
        {
            var list = new List<string>();
            foreach (var key in nvc.AllKeys)
            {
                list.Add(string.Concat(key, "=", nvc[key]));
            }
            return string.Join("&", list);
        }
    }

    [Serializable]
    internal class CallStack
    {
        public string AbsolutePath { get; set; }
        public string ReferrerUrl { get; set; }
        public string QueryData { get; set; }
        public string FormData { get; set; }
        public UserIdentity User { get; set; }
        public ExceptionData ExData { get; set; }
        [Serializable]
        public class ExceptionData
        {
            public string ExtendMessage { get; set; }
            public string ExceptionType { get; set; }
            public string CauseMethod { get; set; }
            public string CauseSource { get; set; }
            public string ErrorMessage { get; set; }
            public string TraceStack { get; set; }
        }
        [Serializable]
        public class UserIdentity
        {
            public string Name { get; set; }
            public bool IsAuthenticated { get; set; }
        }
    }
    
}
