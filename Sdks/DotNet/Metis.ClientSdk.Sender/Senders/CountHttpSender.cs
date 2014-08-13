using Metis.ClientSdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Sender
{
    /// <summary>
    /// 按照队列中数据的数量发送
    /// </summary>
    internal class CountHttpSender : ISingleSender
    {
        public void Prepare(IDictionary<string, object> config)
        {
            
            throw new NotImplementedException();
        }

        public void DoAppend(LogEntity entry)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
