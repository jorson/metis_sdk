using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Metis.SimulateWeb
{
    public class ApiCallGathererPrivoder : Metis.ClientSdk.IGathererDataPrivoder
    {
        Nd.OAuthClient.AccessGrant accessGrant = null;
        string clientId = System.Configuration.ConfigurationManager.AppSettings["oauth.client.id"];
        string clientSecret = System.Configuration.ConfigurationManager.AppSettings["oauth.client.secret"];

        public string GetAccesstoken()
        {
            if (accessGrant == null || accessGrant.IsExpire())
            {   
                int id = 70;
                Int32.TryParse(clientId, out id);
                Nd.OAuthClient.TokenData data = new Nd.OAuthClient.ClientCredentialsTokenData(id, clientSecret);
                var result = Nd.OAuthClient.OAuthService.Authorize(data);
                if (result.Data != null)
                {
                    return result.Data.AccessToken;
                }
                else
                {
                    return "69d63fadaee842a4a5cc575f8eeff707";
                }
            }
            return accessGrant.AccessToken;
        }

        public IDictionary<string, object> GetExtendData(HttpContext context)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            return result;
        }
    }
}