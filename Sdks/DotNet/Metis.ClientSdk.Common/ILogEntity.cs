using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Entities
{
    public interface ILogEntity
    {
        string LogType { get; set; }
        string AccessToken { get; set; }
        int TerminalCode { get; set; }
        long IpAddress { get; }
    }
}
