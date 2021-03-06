﻿using Metis.ClientSdk.ConfigSection;
using Metis.ClientSdk.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Metis.ClientSdk.Gatherer
{
    /// <summary>
    /// 处理页面访问日志的采集者
    /// </summary>
    internal class PageVisitGatherer : BaseGatherer
    {
        public const string GATHERER_KEY = "__PAGE_VIEW_GATHERER__";
        //用于判断页面请求类型的字符常量
        const string HTML_CONTENT = "text/html";
        //是否跳过请求
        bool skipRequest = false;
        //模块的配置
        static BaseGathererConfig config;

        /// <summary>
        /// API调用信息采集者的静态构造函数
        /// </summary>
        static PageVisitGatherer()
        {
            config = new PageVisitGathererConfig();
        }
        public static PageVisitGatherer Current
        {
            get
            {
                if (HttpContext.Current.Items[GATHERER_KEY] == null)
                {
                    var gatherer = new PageVisitGatherer();
                    if (gatherer.IsEnabled)
                    {
                        HttpContext.Current.Items.Add(GATHERER_KEY, gatherer);
                    }
                    return gatherer;
                }
                else
                {
                    return (PageVisitGatherer)HttpContext.Current.Items[GATHERER_KEY];
                }
            }
        }

        public PageVisitGatherer()
        {
            if (!HttpContext.Current.IsAvailable())
                throw new ArgumentNullException("context");
            this.context = HttpContext.Current;
        }
        /// <summary>
        /// 当前模块是否可用
        /// </summary>
        public bool IsEnabled { get { return config.IsEnabled; } }

        public override void BeginRequest()
        {
            if (!config.IsEnabled)
                return;

            //如果启用URL过滤器
            if (((PageVisitGathererConfig)config).UseFilter && ((PageVisitGathererConfig)config).UrlFilter != null)
            {
                //如果不是需要处理的URL
                if (!((PageVisitGathererConfig)config).UrlFilter.IsAllowed(context.Request.RawUrl))
                {
                    this.skipRequest = true;
                    return;
                }
            }
        }
        public override void EndRequest()
        {
            //当模块不被启用, DEBUG请求 或是需要跳过的请求时, 直接Return
            if (!config.IsEnabled || DebugRequest || this.skipRequest)
                return;

            //判断请求的类型
            //如果请求方法是get,contenttype中包含"text/html"且反馈代码为200
            //这类的请求被认为是页面访问
            if (context.Request.RequestType.Equals("get", StringComparison.CurrentCultureIgnoreCase) &&
               context.Response.ContentType.Contains(HTML_CONTENT) &&
               context.Response.StatusCode == 200)
            {
                if (config.ExtendDataPrivoder == null)
                    throw new ArgumentNullException("没有指定有效的ExtendDataPrivoder");

                string visitPage = base.GetPureUrl(context.Request.RawUrl);
                string referPage = context.Request.UrlReferrer == null ? "" :
                    base.GetPureUrl(context.Request.UrlReferrer.AbsolutePath);
                string accesstoken = config.ExtendDataPrivoder.GetAccesstoken();

                GathererContext.Current.AppendPageVisit(accesstoken, visitPage, referPage, context.Request.QueryString);
            }
        }
        public override void Dispose()
        {
        }
    }
}
