using Metis.ClientSdk.ConfigSection;
using Metis.ClientSdk.Filter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Web;

namespace Metis.ClientSdk.Gatherer
{
    internal class ApiCallGatherer : BaseGatherer
    {
        public const string GATHERER_KEY = "__API_CALL_GATHERER__";
        private static string gathererName = "API接口调用信息采集者";
        private static string gathererDesc = "用于记录API站点接口被调用的次数, 响应时间等请求信息";
        Stopwatch watch;
        //是否跳过请求
        bool skipRequest = false;
        //模块的配置
        protected static ApiCallGathererConfig config;
        List<int> writeBytes = new List<int>();

        /// <summary>
        /// API调用信息采集者的静态构造函数
        /// </summary>
        static ApiCallGatherer()
        {
            config = new ApiCallGathererConfig();
        }

        public static ApiCallGatherer Current 
        {
            get 
            {
                if (HttpContext.Current.Items[GATHERER_KEY] == null)
                {
                    var gatherer = new ApiCallGatherer();
                    if (gatherer.IsEnabled)
                    {
                        HttpContext.Current.Items.Add(GATHERER_KEY, gatherer);
                    }
                    return gatherer;
                }
                else
                {
                    return (ApiCallGatherer)HttpContext.Current.Items[GATHERER_KEY];
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
        protected ApiCallGatherer()
        {
            if (!HttpContext.Current.IsAvailable())
                throw new ArgumentNullException("application");
            this.application = HttpContext.Current.ApplicationInstance;
            this.watch = new Stopwatch();
        }
        /// <summary>
        /// 当前模块是否可用
        /// </summary>
        public bool IsEnabled { get { return config.IsEnabled; } }

        public override void BeginRequest()
        {
            //如果模块不可用直接就返回了
            if (!config.IsEnabled)
                return;
            //处理是否为Debug的请求
            base.HandleDebugInfo();
            if (base.DebugRequest)
                return;
           
            //如果启用URL过滤器
            if (config.UseFilter && config.UrlFilter != null)
            {
                //如果不是需要处理的URL
                if (!config.UrlFilter.IsAllowed(application.Request.RawUrl))
                {
                    this.skipRequest = true;
                    return;
                }
            }
            //启动定时器
            watch = new Stopwatch();
            watch.Start();
            ////添加输出流观察器
            StreamWatcher streamWatcher = new StreamWatcher(application.Response.Filter, writeBytes);
            application.Response.Filter = streamWatcher;
        }
        public override void EndRequest()
        {
            //当模块不被启用, DEBUG请求 或是需要跳过的请求时, 直接Return
            if (!config.IsEnabled || DebugRequest || this.skipRequest)
                return;

            long duration = 0L;
            //获取执行时间
            if (watch.IsRunning)
            {
                watch.Stop();
                duration = watch.ElapsedMilliseconds;
            }
            watch.Reset();

            //获取请求地址
            string callUrl = base.GetPureUrl(application.Request.RawUrl);
            long requestSize = application.Request.CountRequestSize();
            long responseSize = 0;
            //当IIS版本为IIS7和IIS8时
            if (IISVersion == WebServerType.IIS7 || IISVersion == WebServerType.IIS8)
            {
                responseSize = application.Response.GetCombineHeaders(true).Length;
            }

            if (application.Response.Filter is StreamWatcher)
            {
                StreamWatcher watcher = (StreamWatcher)application.Response.Filter;
                responseSize += watcher.Length;
            }
            else if (application.Response.Filter is GZipStream || application.Response.Filter is DeflateStream)
            {
                this.writeBytes.ForEach(o => { responseSize += o; });
            }

            string accesstoken = "", callAccesstoken = "";
            int callAppId = 0;
            accesstoken = config.ExtendDataPrivoder.GetAccesstoken();
            GetCallAccessToken(out callAccesstoken, out callAppId);

            //推送数据
            GathererContext.Current.AppendApiCall(
                accesstoken, callAccesstoken, callAppId,
                callUrl, application.Response.StatusCode,
                responseTime: duration, requestSize: requestSize, responseSize: responseSize);
        }
        public override void Dispose()
        {
            if (!config.IsEnabled)
                return;
        }
        /// <summary>
        /// 获取当前采集者的配置(这个方法存在大量的重复问题,后续需要进行优化)
        /// </summary>
        internal static object GetCurrentSetting()
        {
            if (config == null)
                return null;
            ApiCallGathererConfig realConfig = (ApiCallGathererConfig)config;
            if (realConfig.IsEnabled)
            {
                return new
                {
                    Name = gathererName,
                    Description = gathererDesc,
                    GathererEnabled = realConfig.IsEnabled,
                    FilterEnabled = realConfig.UseFilter,
                    FilterType = realConfig.UseFilter ? realConfig.UrlFilter.GetType().FullName : "",
                    WhiteList = realConfig.UseFilter ? realConfig.WhiteList : "",
                    ExtendDataPrivoder = realConfig.ExtendDataPrivoder.GetType().FullName
                };
            }
            else
            {
                return new
                {
                    Name = gathererName,
                    Description = gathererDesc,
                    GathererEnabled = realConfig.IsEnabled,
                };
            }
        }

        private void GetCallAccessToken(out string callAccesstoken, out int callAppId)
        {
            callAppId = 0;
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
        private string GetCallAccessToken()
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
