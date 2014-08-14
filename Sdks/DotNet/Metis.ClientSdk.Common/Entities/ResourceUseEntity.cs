using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Entities
{
    internal class ResourceUseEntity : LogEntity
    {
        public ResourceUseEntity()
        {
            this.LogType = "Resource";
        }

        public int FromApp { get; set; }

        public int ResourceId { get; set; }

        public int ResourceType { get; set; }

        public long UserId { get; set; }
    }
}
