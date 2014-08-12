using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace Metis.ClientSdk
{
    internal class InnerWebClient : WebClient
    {
        int timeout;
        Action<WebRequest> onGetWebRequestHandler;
        Action<WebResponse> onGetWebResponseHandler;
        Func<Uri, Uri> onCreateWebRequestUriHandler;

        public InnerWebClient(int timeout, 
            Func<Uri, Uri> onCreateWebRequestUriHandler,
            Action<WebRequest> onGetWebRequestHandler, 
            Action<WebResponse> onGetWebResponseHandler)
        {
            this.timeout = timeout;
            this.onCreateWebRequestUriHandler = onCreateWebRequestUriHandler;
            this.onGetWebRequestHandler = onGetWebRequestHandler;
            this.onGetWebResponseHandler = onGetWebResponseHandler;

            Encoding = Encoding.UTF8;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = base.GetWebRequest(address) as HttpWebRequest;
            request.Timeout = timeout;
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;
            onGetWebRequestHandler(request);
            return request;
        }

        

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            var response = base.GetWebResponse(request);
            onGetWebResponseHandler(response);
            return response;
        }
    }
}
