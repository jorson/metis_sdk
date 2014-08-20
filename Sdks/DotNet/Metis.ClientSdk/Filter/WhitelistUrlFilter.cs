using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Configuration;

namespace Metis.ClientSdk.Filter
{
    /// <summary>
    /// 白名单的筛选
    /// </summary>
    internal class WhitelistUrlFilter : IUrlFilter
    {
        static Regex whitelistRegex;

        public WhitelistUrlFilter(string whitelist)
        {
            if (!String.IsNullOrWhiteSpace(whitelist))
            {
                whitelistRegex = new Regex(whitelist, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }
        }


        public bool IsAllowed(string url)
        {
            if (whitelistRegex == null)
                return true;
            return whitelistRegex.IsMatch(url);
        }
    }
}
