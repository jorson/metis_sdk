using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Metis.ClientSdk.Gatherer
{
    internal class UnhandleExceptionGatherer : BaseGatherer
    {
        public const string GATHERER_KEY = "__UNHANDLE_EXCEPTION_GATHERER__";
        private static string gathererName = "全局未捕获异常采集者";
        private static string gathererDesc = "处理HttpApplication中的OnException对象";

        protected static UnhandleExceptionGathererConfig config;

        static UnhandleExceptionGatherer()
        {
            config = new UnhandleExceptionGathererConfig();
        }

        public static UnhandleExceptionGatherer Current
        {
            get
            {
                if (HttpContext.Current.Items[GATHERER_KEY] == null)
                {
                    var gatherer = new UnhandleExceptionGatherer();
                    if (gatherer.IsEnabled)
                    {
                        HttpContext.Current.Items.Add(GATHERER_KEY, gatherer);
                    }
                    return gatherer;
                }
                else
                {
                    return (UnhandleExceptionGatherer)HttpContext.Current.Items[GATHERER_KEY];
                }
            } 
        }

        public override string Name
        {
            get { return gathererName; }
        }
        public override string Description
        {
            get { return gathererDesc; }
        }
        /// <summary>
        /// 当前模块是否可用
        /// </summary>
        public bool IsEnabled { get { return config.IsEnabled; } }
        protected UnhandleExceptionGatherer()
        {
            if (!HttpContext.Current.IsAvailable())
                throw new ArgumentNullException("application");
            this.application = HttpContext.Current.ApplicationInstance;
        }
        public override void ExceptionOccur()
        {
            //获取最近的一个异常
            var lastEx = application.Server.GetLastError();
            //如果异常为空
            if (lastEx == null)
                return;
            //获取异常的类型 
            var exName = lastEx.GetType().FullName.Split('.').Last();
            var isExcept = config.ExceptExceptionType.Exists((o) => 
            {
                return o.Equals(exName, StringComparison.CurrentCultureIgnoreCase); 
            });
            //如果是需要排除的异常
            if (isExcept)
                return;
            //获取accesstoken
            string accesstoken = config.ExtendDataPrivoder.GetAccesstoken();
            //推送数据
            GathererContext.Current.AppendException(accesstoken, lastEx, 
                "this is message from unhandle exception gatherer");
        }
        public override void Dispose()
        {
        }
        /// <summary>
        /// 获取当前采集者的配置(这个方法存在大量的重复问题,后续需要进行优化)
        /// </summary>
        internal static object GetCurrentSetting()
        {
            return null;
        }
    }
}
