using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk
{
    internal class WebServer
    {
        static internal WebServerType GetIISServerType()
        {
            try
            {
                RegistryKey parameters = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Services\\W3SVC\\Parameters");
                int majorVersion = (int)parameters.GetValue("MajorVersion");
                switch (majorVersion)
                {
                    case 5:
                        return WebServerType.IIS5;
                    case 6:
                        return WebServerType.IIS6;
                    case 7:
                        return WebServerType.IIS7;
                    case 8:
                        return WebServerType.IIS8;
                    default:
                        return WebServerType.Unknown;
                }
            }
            catch
            {
                return WebServerType.Unknown;
            }
        }
    }

    internal enum WebServerType
    {
        Unknown,
        IIS5,
        IIS6,
        IIS7,
        IIS8
    }
}
