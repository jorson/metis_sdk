using System.Web.Mvc;

namespace Metis.DemoSite.Areas.Visit
{
    public class VisitAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Visit";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Visit_default",
                "Visit/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
