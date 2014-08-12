using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Entities
{
    public class PageVisitEntity : LogEntity
    {
        string ucCode = "auc";

        public PageVisitEntity()
        {
            this.LogType = "visit";
        }
        /// <summary>
        /// 用户系统编码
        /// </summary>
        public string UcCode { get { return this.ucCode; } set { this.ucCode = value; } }
        /// <summary>
        /// 访问引用页面
        /// </summary>
        public string ReferPage { get; set; }
        /// <summary>
        /// 访问页面地址
        /// </summary>
        public string VisitPage { get; set; }
        /// <summary>
        /// 访问页面的参数
        /// </summary>
        public string VisitPageParam { get; set; }
        /// <summary>
        /// 将页面的参数转换为字符串格式
        /// </summary>
        /// <param name="pageParams"></param>
        public void ParsePageParams(NameValueCollection pageParams)
        {
            StringBuilder builder = new StringBuilder();
            foreach(string key in pageParams.AllKeys)
            {
                builder.AppendFormat("{0}={1}&", key, pageParams[key]);
            }
            builder.Append("__visit__=log");
            this.VisitPageParam = builder.ToString();
        }
    }
}
