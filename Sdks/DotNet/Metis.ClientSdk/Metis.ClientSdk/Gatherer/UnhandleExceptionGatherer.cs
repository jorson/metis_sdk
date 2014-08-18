using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Gatherer
{
    internal class UnhandleExceptionGatherer : BaseGatherer
    {
        public override string Name
        {
            get { return "全局未捕获异常采集者"; }
        }
        public override string Description
        {
            get { return "处理HttpApplication中的OnException对象"; }
        }
        public override void BeginRequest()
        {
        }
        public override void EndRequest()
        {
        }
        public override void ExceptionOccur()
        {
        }
        public override void Dispose()
        {
        }
        internal static object GetCurrentSetting()
        {
            return null;
        }
    }
}
