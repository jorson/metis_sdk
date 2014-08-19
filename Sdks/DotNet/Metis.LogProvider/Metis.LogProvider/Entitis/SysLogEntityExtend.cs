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
            entry.SetContextInfo(callStack);
            //设置调用信息
            if (logEvent.ExceptionObject != null)
            {
                entry.SetExceptionInfo(callStack, logEvent.ExceptionObject);
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
    }
}
