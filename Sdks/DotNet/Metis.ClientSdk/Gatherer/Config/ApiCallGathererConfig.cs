using Metis.ClientSdk.ConfigSection;
using Metis.ClientSdk.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Gatherer
{
    internal class ApiCallGathererConfig : BaseGathererConfig
    {
        //处理器的配置节点
        const string CONFIG_SECTION = "gatherer.apicall";
        //URL筛选器对象
        static IUrlFilter urlFilter = null;
        //是否启用URL筛选器
        static bool useFilter = false;
        //URL白名单
        static string whiteList = String.Empty;

        public ApiCallGathererConfig()
            : base(CONFIG_SECTION)
        {
        }

        public IUrlFilter UrlFilter { get { return urlFilter; } }
        public bool UseFilter { get { return useFilter; } }
        public string WhiteList { get { return whiteList; } }

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
                var node = configNode.TryGetNode("whiteList");
                if (node != null)
                {
                    whiteList = node.Attributes["value"];
                    urlFilter = new WhitelistUrlFilter(whiteList);
                }
            }
        }
    }
}
