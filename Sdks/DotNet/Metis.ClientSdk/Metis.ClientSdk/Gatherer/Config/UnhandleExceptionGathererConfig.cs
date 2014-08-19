using Metis.ClientSdk.ConfigSection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Gatherer
{
    internal class UnhandleExceptionGathererConfig : BaseGathererConfig
    {
        //配置的节点 
        const string CONFIG_SECTION = "gatherer.unhandle";
        //不处理的Exception类型
        static List<string> exceptExType = new List<string>();
        public UnhandleExceptionGathererConfig()
            : base(CONFIG_SECTION)
        {
        }
        public List<string> ExceptExceptionType { get { return exceptExType; } }
        protected override void LoadConfig(GathererConfigNode configNode)
        {
            //执行通用的配置读取
            base.LoadConfig(configNode);
            if (!IsEnabled)
                return;
            //个性化配置读取
            var strExceptExType = configNode.TryGetNode("except").Attributes["value"];
            //将对象解析成数组
            try
            {
                var extypes = strExceptExType.Split('|');
                if (extypes.Length > 0)
                    exceptExType.AddRange(extypes);
            }
            catch
            {
            }
        }
    }
}
