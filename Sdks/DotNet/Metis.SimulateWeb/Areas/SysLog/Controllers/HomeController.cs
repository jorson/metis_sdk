using Metis.ClientSdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Metis.SimulateWeb.Areas.SysLog.Controllers
{
    public class HomeController : Controller
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger("comboAppender");

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Log(string message, int level)
        {
            if (String.IsNullOrEmpty(message))
            {
                return Json(new { Result = false, Message = "Log Error" });
            }

            switch (level)
            {
                case 1:
                    logger.Debug(message);
                    break;
                case 2:
                    logger.Debug(message);
                    break;
                case 3:
                    logger.Warn(message);
                    break;
                case 4:
                    logger.Error(message, new ArgumentNullException("Error_Info"));
                    break;
                case 5:
                    logger.Fatal(message, new ArgumentOutOfRangeException("Fatal_Info"));
                    break;
            }
            return Json(new { Result = true, Message = "Log OK" });
        }
    }
}
