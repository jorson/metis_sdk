using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Filter
{
    /// <summary>
    /// 内置的URL过滤器,当用户配置需要过滤器,但是没有配置过滤器类型时,默认采用
    /// </summary>
    internal class AllPassUrlFilter : IUrlFilter
    {
        public bool IsAllowed(string url)
        {
            return true;
        }
    }
}
