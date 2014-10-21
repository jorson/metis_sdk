using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Entities
{
    public class RegisterEntity : LogEntity
    {
        string ucCode = "auc";

        public RegisterEntity(long ipAddress, DateTime logTime)
            : base(ipAddress, logTime)
        {
            this.LogType = "register";
        }

        /// <summary>
        /// 用户系统编码
        /// </summary>
        public string UcCode { get { return this.ucCode; } set { this.ucCode = value; } }
        /// <summary>
        /// 注册方式
        /// </summary>
        public int RegisterMode { get; set; }
        /// <summary>
        /// 客户端IP地址
        /// </summary>
        public long ClientIP { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 注册应用ID
        /// </summary>
        public long AppId { get; set; }

        public override string ToString()
        {
            return String.Format("[Register]UserId:{0},LogTime:{1}", this.UserId, this.CallTimestamp);
        }

    }
}
