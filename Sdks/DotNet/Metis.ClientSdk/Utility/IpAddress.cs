using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Metis.ClientSdk
{
    public static class IpAddress
    {
        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static string GetClientIP()
        {
            string result = String.Empty;
            if (HttpContext.Current == null || !HttpContext.Current.IsAvailable())
                return "0.0.0.0";

            result = HttpContext.Current.Request.Headers["X-Real-IP"];

            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(result))
                result = HttpContext.Current.Request.UserHostAddress;

            if (string.IsNullOrEmpty(result))
                result = "0.0.0.0";

            if (result == "::1")
                result = "127.0.0.1";

            return result;
        }

        public static string GetServerIP()
        {
            string result = String.Empty;
            if (HttpContext.Current == null || !HttpContext.Current.IsAvailable())
                return "0.0.0.0";

            result = HttpContext.Current.Request.ServerVariables["Local_Addr"].ToString();
            if (String.IsNullOrEmpty(result))
            {
                result = HttpContext.Current.Request.ServerVariables["LOCAL_ADDR"].ToString();
            }
            return result;
        }

        /// <summary>
        /// 获得当前页面客户端的IP
        /// </summary>
        /// <returns>当前页面客户端的IP</returns>
        public static long GetIPNumber(bool isServer)
        {
            string ipaddress = isServer ? GetServerIP() : GetClientIP();
            return IPToNumber(ipaddress);
        }

        private static long IPToNumber(string strIPAddress)
        {
            //将目标IP地址字符串strIPAddress转换为数字
            string[] arrayIP = strIPAddress.Split('.');
            UInt32 sip1 = UInt32.Parse(arrayIP[0]);
            UInt32 sip2 = UInt32.Parse(arrayIP[1]);
            UInt32 sip3 = UInt32.Parse(arrayIP[2]);
            UInt32 sip4 = UInt32.Parse(arrayIP[3]);
            long tmpIpNumber;
            tmpIpNumber = sip1 * 256 * 256 * 256 + sip2 * 256 * 256 + sip3 * 256 + sip4;
            return tmpIpNumber;
        }
    }
}
