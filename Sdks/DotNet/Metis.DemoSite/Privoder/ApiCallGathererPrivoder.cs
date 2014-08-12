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
            return "a0f0651d8c4c4318972eb55a440209ff";
        }

        public IDictionary<string, object> GetExtendData(HttpContext context)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            result.Add(Metis.ClientSdk.ConstVariables.CALL_ACCESS_TOKEN, "a0f0651d8c4c4318972eb55a440209ff");
            return result;
        }
    }
}