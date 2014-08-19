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

        public void Direct()
        {
            throw new ArgumentNullException("EX");
        }

        public int Unhandled()
        {
            int i = 0;
            int j = 1;

            return j / i;
        }

        public int TryCatch()
        {
            try
            {
                int i = 0;
                int j = 1;

                return j / i;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            System.Diagnostics.Trace.WriteLine("Handled By OnException");
        }
    }
}
