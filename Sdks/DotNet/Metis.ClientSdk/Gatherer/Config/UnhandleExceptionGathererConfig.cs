using Metis.ClientSdk.ConfigSection;
using Metis.ClientSdk.Entities;
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
        static Dictionary<string, LogLevel> specialExType = new Dictionary<string, LogLevel>();
        static bool exceptAll = false;
        public UnhandleExceptionGathererConfig()
            : base(CONFIG_SECTION)
        {
        }
        /// <summary>
        /// 需要排查的异常类型
        /// </summary>
        public List<string> ExceptExceptionType { get { return exceptExType; } }
        /// <summary>
        /// 需要拦截的特定异常类型
        /// </summary>
        public Dictionary<string, LogLevel> specialExceptionType { get { return specialExType; } }
        /// <summary>
        /// 排除所有的异常类型, 既模块不进行任何异常的拦截, 但仍旧会拦截特别声明的异常
        /// </summary>
        public bool ExceptAllException { get { return exceptAll; } }
        protected override void LoadConfig(GathererConfigNode configNode)
        {
            //执行通用的配置读取
            base.LoadConfig(configNode);
            if (!IsEnabled)
                return;
            //个性化配置读取
            //所有需要排除的Exception, 既不通过这个模块进行拦截的异常
            var excepts = configNode.TryGetNodes("excepts/except");
            exceptExType = excepts.Select(o => o.Attributes["type"].ToLower()).ToList();
            //如果存在"*", 则表示所有异常都忽略掉
            if (exceptExType.Contains("*"))
                exceptAll = true;
            //所有需要进行特殊处理的Exception, 既当拦截到这个异常时, 按照设定的级别上报
            var specials = configNode.TryGetNodes("specials/special");
            specialExType = specials.ToDictionary(o => o.Attributes["type"].ToLower(),
                o => 
                {
                    var level = o.Attributes["level"].ToLower();
                    switch (level)
                    {
                        case "debug":
                            return LogLevel.DEBUG;
                        case "info":
                            return LogLevel.INFO;
                        case "warn":
                            return LogLevel.WARN;
                        case "error":
                            return LogLevel.ERROR;
                        case "fatal":
                            return LogLevel.FATAL;
                        default:
                            return LogLevel.ERROR;
                    }
                });
        }
    }
}
