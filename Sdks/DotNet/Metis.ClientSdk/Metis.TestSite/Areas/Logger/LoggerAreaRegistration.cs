using System.Web.Mvc;

namespace Metis.DemoSite.Areas.Logger
{
    public class LoggerAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Logger";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Logger_default",
                "Logger/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
