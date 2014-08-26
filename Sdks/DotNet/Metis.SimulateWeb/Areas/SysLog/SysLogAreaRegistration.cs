using System.Web.Mvc;

namespace Metis.SimulateWeb.Areas.SysLog
{
    public class SysLogAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "SysLog";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "SysLog_default",
                "SysLog/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
