using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Collections.Specialized;
using System.Web.Script.Serialization;

namespace Metis.ClientSdk.Entities
{
    public class SysLogEntity : Metis.ClientSdk.Entities.LogEntity
    {
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
            this.LogType = "syslog";
        }
        public LogLevel LogLevel { get { return logLevel; } set { logLevel = value; } }
        public string Logger { get { return logger; } set { logger = value; } }
        public string LogMessage { get { return logMessage; } set { logMessage = value; } }
        public string CallInfo { get { return callInfo; } set { callInfo = value; } }
    }

    [Serializable]
    public class CallStack
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

    /// <summary>
    /// 日志级别
    /// 优先级从高到低依次排列如下：FATAL，ERROR，WARN，INFO，DEBUG
    /// </summary>
    public enum LogLevel : byte
    {
        /// <summary>
        /// 程序内部的信息，用于调试
        /// </summary>
        DEBUG = 1,

        /// <summary>
        /// 强调应用的执行的进度，关键分支的记录，指明程序是否符合正确的业务逻辑
        /// </summary>
        INFO = 2,

        /// <summary>
        /// 潜在的有害状态
        /// </summary>
        WARN = 3,

        /// <summary>
        /// 错误事件发生，程序或许依然能够运行
        /// </summary>
        ERROR = 4,

        /// <summary>
        /// 错误可能会导致应用崩溃
        /// </summary>
        FATAL = 5
    }
}
