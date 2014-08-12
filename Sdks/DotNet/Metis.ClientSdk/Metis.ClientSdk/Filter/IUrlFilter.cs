using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Filter
{
    public interface IUrlFilter
    {
        bool IsAllowed(string url);
    }
}
