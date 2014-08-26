using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Metis.ClientSdkConsole
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
            return result;
        }
    }
}