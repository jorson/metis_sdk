using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Metis.DemoSite.Areas.Visit.Controllers
{
    public class VisitController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HasAccesstoken()
        {
            return View();
        }

        public ActionResult NoAccesstoken()
        {
            return View();
        }
    }
}
