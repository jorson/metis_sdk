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
        protected static IGathererDataPrivoder extendDataPrivoder = null;

        public BaseGathererConfig()
        {
            LoadConfig();
        }

        /// <summary>
        /// 当前采集器是否可用
        /// </summary>
        public bool IsEnabled { get { return isEnabled; } }
        /// <summary>
        /// 扩展数据提供者 
        /// </summary>
        public IGathererDataPrivoder ExtendDataPrivoder { get { return extendDataPrivoder; } }

        protected abstract void LoadConfig();
    }
}
