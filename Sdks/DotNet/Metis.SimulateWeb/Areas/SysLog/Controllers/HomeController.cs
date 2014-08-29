using Metis.ClientSdk.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Metis.SimulateWeb.Areas.SysLog.Controllers
{
    public class HomeController : Controller
    {
        static log4net.ILog logger = log4net.LogManager.GetLogger("SysLogHome");

        public ActionResult Index()
        {
            return View();
        }

        public string Accesstoken()
        {
            string clientId = System.Configuration.ConfigurationManager.AppSettings["oauth.client.id"];
            string clientSecret = System.Configuration.ConfigurationManager.AppSettings["oauth.client.secret"];
            int id = 70;
            Int32.TryParse(clientId, out id);
            var data = new Nd.OAuthClient.ClientCredentialsTokenData(id, clientSecret);
            var result = Nd.OAuthClient.OAuthService.Authorize(data);
            if (result.Code != 0)
                throw new Nd.OAuthClient.OAuthException("应用授权失败。" + result.Message);
            return result.Data.AccessToken;
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
