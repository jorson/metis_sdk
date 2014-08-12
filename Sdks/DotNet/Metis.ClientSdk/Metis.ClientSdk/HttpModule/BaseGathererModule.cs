using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Diagnostics;
using System.IO;

namespace Metis.ClientSdk
{
    public abstract class BaseGathererModule : IHttpModule
    {
        protected const string DEBUG = "__debug__";
        protected const string SKIP_REQUEST = "__skip_request__";
        protected IGathererDataPrivoder extendDataPrivoder = null;

        public virtual void Dispose()
        {
        }

        public virtual void Init(HttpApplication context)
        {
            //实例化SDK对象
            Metis.ClientSdk.GathererContext.Setup();
        }

        protected string GetPureUrl(string url)
        {
            int count = url.IndexOf('?');
            if (count == -1)
                return url;
            return url.Substring(0, count);
        }

        protected void HandleDebugInfo(HttpContext context)
        {
            if (context.Request.RawUrl.Equals("/gatherer/debug"))
            {
                context.Items.Add(DEBUG, true);
                if (!GathererContext.Current.IsDebug)
                {
                    context.Response.Write("Gatherer DEBUG mode is off");
                    context.Response.End();
                }
                else
                {
                    string logInfos = GathererLogger.Instance.Read();
                    context.Response.Write(logInfos);
                    context.Response.End();
                }
            }
        }

        protected bool IsDebugRequest(HttpContext context)
        {
            //处理DEBUG模式
            HandleDebugInfo(context);
            return context.Items.Contains(DEBUG);
        }

        protected abstract void LoadModuleConfig();

    }
}
