using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Core;

namespace Metis.ClientSdk.LogProvider
{
    internal class SysLogEntity : Metis.ClientSdk.Entities.LogEntity
    {
        //日志等级
        private LogLevel logLevel = LogLevel.INFO;
        //日志记录者
        private string logger = String.Empty;
        //日志消息
        private string logMessage = String.Empty;
        //调用堆栈
        private string callStack = String.Empty;
        public SysLogEntity(LoggingEvent logEvent)
        {
            logEvent.ExceptionObject
        }
        public LogLevel LogLevel { get { return logLevel; } protected set { logLevel = value; } }
        public string Logger { get { return logger; } protected set { logger = value; } }
        public string LogMessage { get { return logMessage; } protected set { logMessage = value; } }
        public string CallStack { get { return callStack; } protected set { callStack = value; } }
    }
    [Serializable]
    internal class CallStack
    {
        public string AbsolutePath { get; set; }
        public string ReferrerUrl { get; set; }
        public string ParamData { get; set; }
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
