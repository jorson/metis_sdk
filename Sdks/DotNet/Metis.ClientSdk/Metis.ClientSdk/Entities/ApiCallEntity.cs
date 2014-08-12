using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Entities
{
    internal class ApiCallEntity : LogEntity
    {
        public ApiCallEntity()
        {
            this.LogType = "api";
        }
        /// <summary>
        /// 调用AppId
        /// </summary>
        public int CallAppId { get; set; }
        /// <summary>
        /// 调用API的APP的AccessToken
        /// </summary>
        public string CallAccessToken { get; set; }
        /// <summary>
        /// 请求状态码
        /// </summary>
        public int StatusCode { get; set; }
        /// <summary>
        /// 调用地址
        /// </summary>
        public string CallUrl { get; set; }
        /// <summary>
        /// 响应时间(ms)
        /// </summary>
        public long ResponseTime { get; set; }
        /// <summary>
        /// 请求大小(byte)
        /// </summary>
        public long RequestSize { get; set; }
        /// <summary>
        /// 响应大小(byte)
        /// </summary>
        public long ResponseSize { get; set; }
    }
}
