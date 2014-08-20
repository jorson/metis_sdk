using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;
using System.IO;
using Metis.ClientSdk.ConfigSection;
using Metis.ClientSdk.Filter;

namespace Metis.ClientSdk
{
    public class VisitGathererModule : BaseGathererModule
    {
        const string HTML_CONTENT = "text/html";
        const string CONFIG_SECTION = "gatherer.pagevisit";

        bool useFilter = false;
        string blackList = String.Empty;
        IUrlFilter urlFilter = null;

        public VisitGathererModule()
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
            //绑定事件
            context.BeginRequest += BeginRequestOccur;
            context.EndRequest += EndRequestOccur;
        }

        public void BeginRequestOccur(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;

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
        }

        public void EndRequestOccur(object sender, EventArgs e)
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

            //判断请求的类型
            //如果请求方法是get,contenttype中包含"text/html"且反馈代码为200
            //这类的请求被认为是页面访问
            if(context.Request.RequestType.Equals("get", StringComparison.CurrentCultureIgnoreCase) &&   
               context.Response.ContentType.Contains(HTML_CONTENT) &&
               context.Response.StatusCode == 200)
            {
                if (base.extendDataPrivoder == null)
                    throw new ArgumentNullException("没有指定有效的ExtendDataPrivoder");

                string visitPage = base.GetPureUrl(context.Request.RawUrl);
                string referPage = context.Request.UrlReferrer == null ? "" :
                    base.GetPureUrl(context.Request.UrlReferrer.AbsolutePath);
                string accesstoken = base.extendDataPrivoder.GetAccesstoken();


                GathererContext.Current.AppendPageVisit(accesstoken, visitPage, referPage, context.Request.QueryString);
            }
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
                var node = configNode.TryGetNode("blackList");
                if (node != null)
                {
                    this.blackList = node.Attributes["value"];
                    this.urlFilter = new BlacklistUrlFilter(this.blackList);
                }
            }
        }
    }
}
