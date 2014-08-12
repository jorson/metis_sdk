using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Metis.DemoSite.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public String Index(string account, string password)
        {
            return "Return String:" + account + ":" + password;
        }

    }
}
