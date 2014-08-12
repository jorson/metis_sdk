using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Metis.DemoSite.Areas.V1.Controllers
{
    public class OAuthController : Controller
    {
        public string Index(string account, string password)
        {
            Random rand = new Random();
            System.Threading.Thread.Sleep(rand.Next(100, 200));

            return String.Format("{0}.{1}", account, password);
        }
    }
}
