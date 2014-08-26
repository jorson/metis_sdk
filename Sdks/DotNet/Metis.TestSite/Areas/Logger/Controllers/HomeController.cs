using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Metis.DemoSite.Areas.Logger.Controllers
{
    public class HomeController : Controller
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger("comboAppender");

        public ActionResult Index()
        {
            return View();
        }

        public string Debug()
        {
            logger.Debug("this is debug level");
            return "debug";
        }
        public string Info()
        {
            logger.Info("this is info level");
            return "info";
        }
        public string Warn()
        {
            logger.Warn("this is info level");
            return "warn";
        }
        public string Error()
        {
            logger.Error("this is error level", new ArgumentException("Error_ArgumentInfo"));
            return "error";
        }
        public string Fatal()
        {
            logger.Error("this is error level", new ArgumentException("Fatal_ArgumentInfo"));
            return "fatal";
        }
    }
}
