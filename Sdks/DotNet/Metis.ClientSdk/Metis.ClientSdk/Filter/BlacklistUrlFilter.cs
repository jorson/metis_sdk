using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Metis.ClientSdk.Filter
{
    /// <summary>
    /// 黑名单的筛选
    /// </summary>
    internal class BlacklistUrlFilter : IUrlFilter
    {
        static Regex blacklistUrlFilter;

        public BlacklistUrlFilter(string blacklist)
        {
            if (!String.IsNullOrWhiteSpace(blacklist))
            {
                blacklistUrlFilter = new Regex(blacklist, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            }
        }

        public bool IsAllowed(string url)
        {
            if (blacklistUrlFilter == null)
                return true;
            return !blacklistUrlFilter.IsMatch(url);
        }
    }
}
