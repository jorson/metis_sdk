using Metis.ClientSdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Web
{
    internal static class HttpContextExtend
    {
        static Func<object, object> handler;

        static HttpContextExtend()
        {
            TypeAccessor ta = TypeAccessor.GetAccessor(typeof(HttpContext));
            handler = ta.GetFieldGetter("HideRequestResponse");
        }

        public static bool IsAvailable(this HttpContext context)
        {
            return context != null && !((bool)handler(context));
        }
    }
}
