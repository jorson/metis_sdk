using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.ConfigSection
{
    public class GathererConfigurationException : Exception
    {
                public GathererConfigurationException()
        {
        }

                public GathererConfigurationException(string message)
            : base(message)
        {
        }
    }

    public class MissConfigurationException : GathererConfigurationException
    {
        string message;

        public MissConfigurationException(GathererConfigNode settingNode, string path)
            : this(new List<GathererConfigNode>() { settingNode }, path)
        {
        }

        public MissConfigurationException(IEnumerable<GathererConfigNode> settingNodes, string path)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("在以下路径找未找到需要的数据:");
            foreach (GathererConfigNode node in settingNodes)
            {
                builder.AppendLine(node.Path + path + ";");
            }
            message = builder.ToString();
        }

        public override string Message
        {
            get
            {
                return message;
            }
        }
    }
}
