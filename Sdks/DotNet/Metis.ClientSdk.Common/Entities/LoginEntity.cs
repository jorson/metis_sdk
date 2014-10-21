using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Entities
{
    /// <summary>
    /// 用户登录日志实体
    /// </summary>
    public class LoginEntity : LogEntity
    {
        string ucCode = "auc";

        public LoginEntity(long ipAddress, DateTime logTime)
            : base(ipAddress, logTime)
        {
            this.LogType = "login";
        }

        /// <summary>
        /// 表示账号体系编码, 默认为auc
        /// </summary>
        public string UcCode { get { return this.ucCode; } set { this.ucCode = value; } }
        /// <summary>
        /// 登陆用户ID
        /// </summary>
        public long UserId { get; set; }
        /// <summary>
        /// 唯一标识类型，UserName=1,Mobile=2,Email=3,IDCard=4,Solution=5
        /// </summary>
        public int IdentityType { get; set; }
        /// <summary>
        /// 账号方案标识
        /// </summary>
        public int SolutionId { get; set; }
        /// <summary>
        /// 来源应用ID
        /// </summary>
        public int SourceAppId { get; set; }
        /// <summary>
        /// 来源终端编码
        /// </summary>
        public int SourceTerminalCode { get; set; }

        public override string ToString()
        {
            return String.Format("[Login]UserId:{0},LogTime:{1}", this.UserId, this.CallTimestamp);
        }
    }
}
