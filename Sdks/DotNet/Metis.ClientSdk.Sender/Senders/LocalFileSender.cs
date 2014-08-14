using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Sender
{
    internal class LocalAndHttpSender : ISingleSender
    {
        public void Prepare(IDictionary<string, object> map)
        {
            throw new NotImplementedException();
        }

        public void DoAppend(Entities.LogEntity entry)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }
    }
}
