using log4net.Core;
using Metis.ClientSdk.Entities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Metis.ClientSdk.Entities
{
    public static class SysLogEntityExtend
    {
        /// <summary>
        /// 序列化器
        /// </summary>
        private static readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        /// <summary>
        /// 将log4j的日志事件转换为SysLogEntity对象
        /// </summary>
        public static void TryParseLoggingEvent(this SysLogEntity entry, LoggingEvent logEvent)
        {
            if (logEvent == null)
            {
                throw new ArgumentNullException("logEvent");
            }
            LoggingEventData data = logEvent.GetLoggingEventData();
            //日志事件的消息
            entry.LogMessage = data.Message;
            //日志事件级别
            TransLogLevel(entry, logEvent.Level);
            //记录者名称
            entry.Logger = data.LoggerName;
            CallStack callStack = new CallStack();
            GetContextInfo(callStack);
            //设置调用信息
            if (logEvent.ExceptionObject != null)
            {
                GetExceptionInfo(callStack, logEvent.ExceptionObject);
            }
            //序列化对象
            entry.CallInfo = serializer.Serialize(callStack);
        }

        /// <summary>
        /// 转换日志级别
        /// </summary>
        private static void TransLogLevel(SysLogEntity entry, log4net.Core.Level level)
        {
            if (level == log4net.Core.Level.Debug)
                entry.LogLevel = LogLevel.DEBUG;
            else if (level == log4net.Core.Level.Info)
                entry.LogLevel = LogLevel.INFO;
            else if (level == log4net.Core.Level.Warn)
                entry.LogLevel = LogLevel.WARN;
            else if (level == log4net.Core.Level.Error)
                entry.LogLevel = LogLevel.ERROR;
            else if (level == log4net.Core.Level.Fatal)
                entry.LogLevel = LogLevel.FATAL;
        }
        /// <summary>
        /// 构建调用信息
        /// </summary>
        private static void GetContextInfo(CallStack callStack)
        {
            var context = HttpContext.Current;
            if (!context.IsAvailable())
                return;

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
        private static void GetExceptionInfo(CallStack callStack, Exception ex)
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
}
