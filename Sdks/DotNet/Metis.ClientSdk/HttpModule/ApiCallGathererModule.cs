using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;
using System.IO;
using Metis.ClientSdk.Filter;
using Microsoft.Win32;
using Metis.ClientSdk.ConfigSection;

namespace Metis.ClientSdk
{
    public class ApiCallGathererModule : BaseGathererModule
    {
        const string CONFIG_SECTION = "gatherer.apicall";
        static WebServerType iisVersion = WebServerType.Unknown;

        IUrlFilter urlFilter = null;
        bool useFilter = false;
        string whiteList = String.Empty;

        public ApiCallGathererModule()
        {
        }

        public override void Dispose()
        {
        }

        public override void Init(HttpApplication context)
        {
            base.Init(context);
            //加载模块的配置
            LoadModuleConfig();
            //获取IIS版本
            iisVersion = WebServer.GetIISServerType();
            //绑定事件
            context.BeginRequest += BeginRequestOccur;
            context.EndRequest += EndRequestOccur;
        }

        void BeginRequestOccur(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;
            //处理DEBUG模式
            if (base.IsDebugRequest(context))
                return;

            //如果启用URL过滤器
            if (useFilter)
            {
                if (urlFilter != null)
                {
                    //如果不是需要处理的URL
                    if (!urlFilter.IsAllowed(context.Request.RawUrl))
                    {
                        context.Items.Add(SKIP_REQUEST, true);
                        return;
                    }
                }
            }

            //在Request的开始, 启动一个计时器
            Stopwatch watch = new Stopwatch();
            watch.Start();
            context.Items.Add("Watch", watch);
            StreamWatcher watcher = new StreamWatcher(context.Response.Filter);
            context.Response.Filter = watcher;
        }

        void EndRequestOccur(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;
            //如果是需要跳过或处于DEBUG页面时,直接结束掉
            if (context.Items.Contains(SKIP_REQUEST))
            {
                //如果存在, 且值为True, 那么直接返回
                if ((bool)context.Items[SKIP_REQUEST])
                    return;
            }

            if (context.Items.Contains(DEBUG))
                return;

            long duration = 0L;
            //获取执行时间
            if (context.Items.Contains("Watch"))
            {
                Stopwatch watch = (Stopwatch)context.Items["Watch"];
                if (watch.IsRunning)
                {
                    watch.Stop();
                    duration = watch.ElapsedMilliseconds;
                }
            }
            //获取请求地址
            string callUrl = base.GetPureUrl(context.Request.RawUrl);
            long requestSize = context.Request.CountRequestSize();
            long responseSize = 0;
            //当IIS版本为IIS7和IIS8时
            if(iisVersion == WebServerType.IIS7 || iisVersion == WebServerType.IIS8)
            {
                responseSize = context.Response.GetCombineHeaders(true).Length;
            }
            
            StreamWatcher watcher = (StreamWatcher)context.Response.Filter;
            if (context.Response.Filter is StreamWatcher)
            {
                responseSize += watcher.Length;
            }

            string accesstoken = "", callAccesstoken = "";
            int callAppId = 0;
            GetAccessTokenAndCallAccessToken(out accesstoken, out callAccesstoken, out callAppId);
            //推送数据
            GathererContext.Current.AppendApiCall(
                accesstoken, callAccesstoken, callAppId,
                callUrl, context.Response.StatusCode,
                responseTime: duration, requestSize: requestSize, responseSize: responseSize);
        }

        protected override void LoadModuleConfig()
        {
            var configNode = GathererSection.Instance.TryGetNode(CONFIG_SECTION);
            var strUseFilter = configNode.TryGetNode("useFilter").Attributes["value"];
            var privoder = configNode.TryGetNode("extendDataPrivoder").Attributes["value"];

            if (!String.IsNullOrEmpty(privoder))
            {
                object objPrivoder = FastActivator.Create(privoder);
                if (objPrivoder is IGathererDataPrivoder)
                {
                    this.extendDataPrivoder = (IGathererDataPrivoder)objPrivoder;
                }
                else
                {
                    throw new ArgumentException("配置中的extendDataPrivoder对象没有实现IGathererDataPrivoder接口");
                }
            }
            //获取是否使用Filter
            Boolean.TryParse(strUseFilter, out this.useFilter);
            if (useFilter)
            {
                var node = configNode.TryGetNode("whiteList");
                if (node != null)
                {
                    this.whiteList = node.Attributes["value"];
                    this.urlFilter = new WhitelistUrlFilter(this.whiteList);
                }
            }
        }

        void GetAccessTokenAndCallAccessToken(out string accesstoken, out string callAccesstoken, out int callAppId)
        {
            if (base.extendDataPrivoder == null)
                throw new ArgumentNullException("没有指定有效的ExtendDataPrivoder");

            callAppId = 0;
            accesstoken = extendDataPrivoder.GetAccesstoken();
            //获取扩展数据
            var extendData = extendDataPrivoder.GetExtendData(HttpContext.Current);

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
        /// <returns></returns>
        string GetCallAppAccesstoken()
        {
            string accessToken = String.Empty;
            if (HttpContext.Current.IsAvailable())
            {
                HttpContext context = HttpContext.Current;

                if (context.Request.RequestType.Equals("get", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!String.IsNullOrEmpty(context.Request.QueryString[ConstVariables.CALL_ACCESS_TOKEN]))
                    {
                        accessToken = context.Request.QueryString[ConstVariables.CALL_ACCESS_TOKEN];
                    }
                    else if (!String.IsNullOrEmpty(context.Request.QueryString[ConstVariables.CALL_ACCESS_TOKEN_OLD]))
                    {
                        accessToken = context.Request.QueryString[ConstVariables.CALL_ACCESS_TOKEN_OLD];
                    }
                }
                else if (context.Request.RequestType.Equals("post", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (!String.IsNullOrEmpty(context.Request.Form[ConstVariables.CALL_ACCESS_TOKEN]))
                    {
                        accessToken = context.Request.Form[ConstVariables.CALL_ACCESS_TOKEN];
                    }
                    else if (!String.IsNullOrEmpty(context.Request.Form[ConstVariables.CALL_ACCESS_TOKEN_OLD]))
                    {
                        accessToken = context.Request.Form[ConstVariables.CALL_ACCESS_TOKEN_OLD];
                    }
                }
            }
            return accessToken;
        }
    }
}
