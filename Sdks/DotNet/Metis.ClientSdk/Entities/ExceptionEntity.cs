using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Entities
{
    internal class ExceptionEntity : LogEntity
    {
        public ExceptionEntity(string message, Exception ex)
        {
            this.ExtendMessage = message;
            this.LogType = "Exception";
            this.TryParseEx(ex);
        }

        public ExceptionEntity(Exception ex)
            : this(String.Empty, ex)
        {

        }

        public string ExtendMessage { get; protected set; }
        public string ExceptionType { get; protected set; }
        public string CauseMethod { get; protected set; }
        public string CauseSource { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public string TraceStack { get; protected set; }

        private void TryParseEx(Exception ex)
        {
            this.ExceptionType = ex.GetType().FullName;
            this.CauseSource = ex.Source;
            if (ex.TargetSite != null)
                this.CauseMethod = ex.TargetSite.ToString();
            this.ErrorMessage = ex.Message;
            this.TraceStack = ex.StackTrace;
        }
    }
}
