using Metis.ClientSdk.ConfigSection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Gatherer
{
    internal abstract class BaseGathererConfig
    {
        //当前采集器是否可用
        protected bool isEnabled = false;
        //扩展数据提供者 
        protected static IGathererDataProvider extendDataPrivoder = null;
        private string configSection;

        public BaseGathererConfig(string configSection)
        {
            Arguments.NotNull(configSection, "configSection");
            this.configSection = configSection;
            var configNode = GathererSection.Instance.TryGetNode(configSection);
            LoadConfig(configNode);
        }

        /// <summary>
        /// 当前采集器是否可用
        /// </summary>
        public bool IsEnabled { get { return isEnabled; } }
        /// <summary>
        /// 扩展数据提供者 
        /// </summary>
        public IGathererDataProvider ExtendDataPrivoder { get { return extendDataPrivoder; } }

        protected virtual void LoadConfig(GathererConfigNode configNode)
        {
            //如果不存在配置节点,在模块也就不需要启用
            if (configNode == null)
                return;

            //获取当前Processor是否可用
            if (configNode.Attributes["enabled"] != null &&
                configNode.Attributes["enabled"].Equals("true", StringComparison.CurrentCultureIgnoreCase))
            {
                isEnabled = true;
            }
            //如果模块没有被启用, 后续也不需要加载
            if (!isEnabled)
                return;

            var privoder = configNode.TryGetNode("extendDataPrivoder").Attributes["value"];
            if (!String.IsNullOrEmpty(privoder))
            {
                object objPrivoder = FastActivator.Create(privoder);
                if (objPrivoder is IGathererDataProvider)
                    extendDataPrivoder = (IGathererDataProvider)objPrivoder;
                else
                    throw new ArgumentException("配置中的extendDataPrivoder对象没有实现IGathererDataPrivoder接口");
            }
        }
    }
}
