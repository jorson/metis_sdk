using Metis.ClientSdk.Gatherer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;
using System.Reflection;
using System.Web;
using System.Web.Script.Serialization;

namespace Metis.ClientSdk.Console
{
    /// <summary>
    /// 控制台采集者, 这个类用于响应控制台的请求
    /// </summary>
    internal class ConsoleGatherer : BaseGatherer
    {
        private const string CONSOLE_PREFIX = "__gatherer__";
        private Dictionary<string, Action> consoleRequests = new Dictionary<string, Action>();
        private ResourceManager resourceManager = 
            new ResourceManager("Metis.ClientSdk.Properties.Resources", 
                Assembly.GetExecutingAssembly());
        private readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public ConsoleGatherer()
        {
            if (HttpContext.Current.IsAvailable())
                throw new ArgumentNullException("application");
            this.application = HttpContext.Current.ApplicationInstance;
            //添加处理过程
            consoleRequests.Add(String.Format("/{0}/index", CONSOLE_PREFIX),
                HandleIndexRequest);
            consoleRequests.Add(String.Format("/{0}/sysinfo", CONSOLE_PREFIX),
                HandleSysInfoRequest);
            consoleRequests.Add(String.Format("/{0}/gatherers", CONSOLE_PREFIX),
                HandleGathererListRequest);
            consoleRequests.Add(String.Format("/{0}/counters", CONSOLE_PREFIX),
                HandleCounterListRequest);
        }
        public override string Name
        {
            get { return "采集者控制台"; }
        }
        public override string Description
        {
            get { return "用于显示采集者SDK中的各种信息"; }
        }
        public override void BeginRequest()
        {
            //根据输入请求决定响应
            string rawUrl = application.Request.RawUrl;
            //如果存在可以处理的请求
            if (consoleRequests.ContainsKey(rawUrl))
            {
                //统一处理控制台没有被开启时的提示
                if (!GathererContext.Current.ConsoleEnabled)
                    WriteContent("meitis console is disabled! enable console please set 'true' to config section 'console_enable' in config file");
                else
                    consoleRequests[rawUrl].Invoke();
                application.CompleteRequest();
            }
            //不存在则直接放行
        }
        public override void EndRequest()
        {
            //DO NOTHING
        }
        public override void ExceptionOccur()
        {
            //DO NOTHING
        }
        public override void Dispose()
        {
        }
        /// <summary>
        /// 处理主页的请求
        /// </summary>
        private void HandleIndexRequest()
        {
            string indexHtml = LoadHtmlFileFromRes("index");
            WriteContent(indexHtml);
        }
        /// <summary>
        /// 处理获取系统消息的请求 
        /// </summary>
        private void HandleSysInfoRequest()
        {
            string sysinfo = serializer.Serialize(new 
            {
                IP = application.Request.ServerVariables.Get("Local_Addr").ToString(),                
                ServerName = Environment.MachineName,
                Host = application.Request.Url.Host.ToString(),
                Is64Bit = Environment.Is64BitOperatingSystem.ToString(),
                OsVersion = Environment.OSVersion.ToString(),
                ProcessorCount = Environment.ProcessorCount
            });
            WriteContent(sysinfo, true);
        }
        /// <summary>
        /// 处理获取Gatherer列表的请求
        /// </summary>
        private void HandleGathererListRequest()
        {
            object apiCallSetting = ApiCallGatherer.GetCurrentSetting();
            object pageVisitSetting = PageVisitGatherer.GetCurrentSetting();
            object unhandleExSetting = UnhandleExceptionGatherer.GetCurrentSetting();

            List<object> settingList = new List<object>();
            if (apiCallSetting != null)
                settingList.Add(apiCallSetting);
            if (pageVisitSetting != null)
                settingList.Add(pageVisitSetting);
            if (unhandleExSetting != null)
                settingList.Add(unhandleExSetting);
            WriteContent(serializer.Serialize(settingList), true);
        }
        /// <summary>
        /// 处理获取Counter列表的请求
        /// </summary>
        private void HandleCounterListRequest()
        {
            var counterList = Counter.AtomicCounter.Instance.GetAllWithGrouping();
            //这里只取开头是log_entry的计数器
            var entityGroup = counterList.Where(o => o.Item2.StartsWith("log_entry_"))
                .GroupBy(o => o.Item2, o => new KeyValuePair<string, Int64>(o.Item1, o.Item3));

            var result = serializer.Serialize(entityGroup.Select(eg =>
            {
                var ackCount = eg.FirstOrDefault(o => o.Key.EndsWith("ack"));
                var failCount = eg.FirstOrDefault(o => o.Key.EndsWith("fail"));
                return new
                {
                    GroupKey = eg.Key,
                    AckCount = ackCount.Value,
                    FailCount = failCount.Value
                };
            }));
            WriteContent(result, true);
        }
        private string LoadHtmlFileFromRes(string resName)
        {
            object resObj = resourceManager.GetObject(resName);
            if (resObj == null)
                return "can't found resource! resource name is " + resName;
            return resObj.ToString();
        }
        private void WriteContent(string content, bool isJson = false)
        {
            application.Response.ContentType = isJson 
                ? "application/joson; charset=UTF-8"
                : "text/html; charset=UTF-8";
            application.Response.Write(content);
        }
    }
}
