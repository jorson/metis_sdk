using Metis.ClientSdk.Console;
using Metis.ClientSdk.Gatherer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Metis.ClientSdk
{
    /// <summary>
    /// 日志采集者的HTTPModule
    /// </summary>
    public class GathererModule : IHttpModule
    {
        private ConsoleGatherer consoleGatherer;
        
        public void Init(HttpApplication application)
        {
            //绑定事件
            application.BeginRequest += BeginRequestOccur;
            application.EndRequest += EndRequestOccur;
            //实例化ConsoleGatherer, 对象空判断,防止多次被实例化
            if(consoleGatherer == null)
                consoleGatherer = new ConsoleGatherer();
        }
        /// <summary>
        /// 开始请求时执行
        /// </summary>
        void BeginRequestOccur(object sender, EventArgs e)
        {
            //开始执行请求时,优先处理Console的请求 
            consoleGatherer.BeginRequest();
            //开始正常流程
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;
            //如果API Call采集器可用
            if (ApiCallGatherer.Current.IsEnabled)
            {
                //实例化采集器实例, 并调用其中的BeginRequest方法, 并将采集器对象加到当前上下文中
                ApiCallGatherer.Current.BeginRequest();
            }
            if (PageVisitGatherer.Current.IsEnabled)
            {
                //实例化采集器实例, 并调用其中的BeginRequest方法, 并将采集器对象加到当前上下文中
                PageVisitGatherer.Current.BeginRequest();
            }
        }
        /// <summary>
        /// 结束请求时执行
        /// </summary>
        void EndRequestOccur(object sender, EventArgs e)
        {
            HttpApplication app = (HttpApplication)sender;
            HttpContext context = app.Context;
            //如果HTTP上下文中包含特定的对象
            if (context.Items.Contains(ApiCallGatherer.GATHERER_KEY))
            {
                ((ApiCallGatherer)context.Items[ApiCallGatherer.GATHERER_KEY]).EndRequest();
            }
            //如果HTTP上下文中包含特定的对象
            if (context.Items.Contains(PageVisitGatherer.GATHERER_KEY))
            {
                ((PageVisitGatherer)context.Items[PageVisitGatherer.GATHERER_KEY]).EndRequest();
            }
        }
        /// <summary>
        /// 释放对象
        /// </summary>
        public void Dispose()
        {
        }
    }
}
