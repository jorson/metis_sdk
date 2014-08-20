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
        public PageVisitGathererConfig()
            : base(CONFIG_SECTION)
        {
        }
        public IUrlFilter UrlFilter { get { return urlFilter; } }
        public bool UseFilter { get { return useFilter; } }
        public string BlackList { get { return blackList; } }

        protected override void LoadConfig(GathererConfigNode configNode)
        {
            //执行通用的配置读取
            base.LoadConfig(configNode);
            if (!IsEnabled)
                return;
            //执行个性化的配置读取
            var strUseFilter = configNode.TryGetNode("useFilter").Attributes["value"];
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
