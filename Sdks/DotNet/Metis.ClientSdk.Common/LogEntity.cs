using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Metis.ClientSdk.Entities
{
    public abstract class LogEntity
    {
        long ipAddress = 0;
        DateTime logDate = new DateTime();

        protected LogEntity()
        {
            CheckPropertyType();
            logDate = DateTime.Now;
            ipAddress = Metis.ClientSdk.IpAddress.GetIPNumber(true);
        }

        public string LogType { get; set; }
        /// <summary>
        /// 应用的AccessToken
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// 客户端ID
        /// </summary>
        public int TerminalCode { get; set; }
        /// <summary>
        /// 调用端IP地址
        /// </summary>
        public long IpAddress { get { return ipAddress; } protected set { ipAddress = value; } }

        public DateTime CallTimestamp { get { return logDate; } protected set { logDate = value; } }

        protected void CheckPropertyType()
        {
            var allProperty = this.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach(var property in allProperty)
            {
                if (property.PropertyType != typeof(String) &&
                    property.PropertyType != typeof(Int32) &&
                    property.PropertyType != typeof(Int64) &&
                    property.PropertyType != typeof(DateTime))
                    throw new NotSupportedException("日志实体中的属性只能是String, Int32, Int64, Datetime");
            }
        }
    }
}
