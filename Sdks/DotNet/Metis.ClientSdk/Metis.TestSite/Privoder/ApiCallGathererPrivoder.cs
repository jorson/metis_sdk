using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Metis.DemoSite.Privoder
{
    public class ApiCallGathererPrivoder : Metis.ClientSdk.IGathererDataPrivoder
    {
        public string GetAccesstoken()
        {
            return "69d63fadaee842a4a5cc575f8eeff707";
        }

        public IDictionary<string, object> GetExtendData(HttpContext context)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add(Metis.ClientSdk.ConstVariables.CALL_ACCESS_TOKEN, "69d63fadaee842a4a5cc575f8eeff707");
            return result;
        }
    }
}