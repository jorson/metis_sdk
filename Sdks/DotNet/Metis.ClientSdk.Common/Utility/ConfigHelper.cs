using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk
{
    internal static class ConfigHelper
    {
        /// <summary>
        /// 获取设置
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetAppSetting(string name)
        {
            return ConfigurationManager.AppSettings[name] ?? string.Empty;
        }

        public static int GetAppSettingForInt(string name, int defaultValue)
        {
            int v;
            if (Int32.TryParse(ConfigurationManager.AppSettings[name], out v))
                return v;
            return defaultValue;
        }

        public static bool GetAppSettingForBool(string name, bool defaultVaule)
        {
            bool v;
            if (bool.TryParse(ConfigurationManager.AppSettings[name], out v))
                return v;
            else
                return defaultVaule;
        }
    }
}
