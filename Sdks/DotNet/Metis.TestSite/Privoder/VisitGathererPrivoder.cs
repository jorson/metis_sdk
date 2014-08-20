using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Metis.DemoSite.Privoder
{
    public class VisitGathererPrivoder : Metis.ClientSdk.IGathererDataPrivoder
    {
        public IDictionary<string, object> GetExtendData(HttpContext context)
        {
            return new Dictionary<string, object>();
        }

        public string GetAccesstoken()
        {
            return "a99140efdb1849e387c13fda20ceb2d5";
        }
    }
}