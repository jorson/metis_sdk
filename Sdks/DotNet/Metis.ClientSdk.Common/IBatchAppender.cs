using Metis.ClientSdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk
{
    public interface IBatchSender : ISingleSender
    {
        void DoAppend(IList<LogEntity> logEntities);
    }
}
