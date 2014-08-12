using Metis.ClientSdk.ConfigSection;
using Metis.ClientSdk.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Gatherer
{
    internal class PageVisitGathererConfig : BaseGathererConfig
    {
        //模块配置节点
        const string CONFIG_SECTION = "gatherer.pagevisit";

        //是否启用URL筛选器
        static bool useFilter = false;
        //URL黑名单
        static string blackList = String.Empty;
        //URL筛选器对象
        static IUrlFilter urlFilter = null;

        public IUrlFilter UrlFilter { get { return urlFilter; } }
        public bool UseFilter { get { return useFilter; } }
        public string BlackList { get { return blackList; } }

        protected override void LoadConfig()
        {
            var configNode = GathererSection.Instance.TryGetNode(CONFIG_SECTION);
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

            var strUseFilter = configNode.TryGetNode("useFilter").Attributes["value"];
            var privoder = configNode.TryGetNode("extendDataPrivoder").Attributes["value"];

            if (!String.IsNullOrEmpty(privoder))
            {
                object objPrivoder = FastActivator.Create(privoder);
                if (objPrivoder is IGathererDataPrivoder)
                {
                    extendDataPrivoder = (IGathererDataPrivoder)objPrivoder;
                }
                else
                {
                    throw new ArgumentException("配置中的extendDataPrivoder对象没有实现IGathererDataPrivoder接口");
                }
            }
            //获取是否使用Filter
            Boolean.TryParse(strUseFilter, out useFilter);
            if (useFilter)
            {
                var node = configNode.TryGetNode("blackList");
                if (node != null)
                {
                    blackList = node.Attributes["value"];
                    urlFilter = new BlacklistUrlFilter(blackList);
                }
            }
        }
    }
}
