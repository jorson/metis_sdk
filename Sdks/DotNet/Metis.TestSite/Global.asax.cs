using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace Metis.DemoSite
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        static bool hasInit = false;

        public override void Init()
        {
            if (!hasInit)
            {
                hasInit = true;
                base.Init();
                base.Error += MvcApplication_Error;
            }
        }

        void MvcApplication_Error(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.WriteLine("Exception handled by Error");
        }

        protected void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();

            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Metis.ClientSdk.GathererContext.Setup();
            //string configPath =
            //    Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["log4net.config"]);
            //log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(configPath));

            
        }
    }
}