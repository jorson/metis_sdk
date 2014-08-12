using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Entities
{
    public class RegisterEntity : LogEntity
    {
        string ucCode = "auc";

        public RegisterEntity()
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
    }
}
