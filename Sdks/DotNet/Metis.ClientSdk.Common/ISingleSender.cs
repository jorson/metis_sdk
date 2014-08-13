using Metis.ClientSdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk
{
    /// <summary>
    /// 日志发送者接口
    /// </summary>
    public interface ISingleSender
    {
        void Prepare(IDictionary<string, object> config);
        void DoAppend(LogEntity entry);
        void Clear();
    }
}
