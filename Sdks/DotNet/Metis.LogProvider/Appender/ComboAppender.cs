using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Metis.ClientSdk;
using Metis.ClientSdk.Entities;
using log4net.Core;
using log4net.Appender;
using System.Web;


namespace Metis.ClientSdk.LogProvider
{
    public class ComboAppender : AppenderSkeleton
    {
        ISingleSender localSender, remoteSender;
        IGathererDataProvider extendDataPrivoder;

        public ComboAppender()
            : base()
        {
        }
        /// <summary>
        /// 本地日志的发送者
        /// </summary>
        public string LocalSender { get; set; }
        /// <summary>
        /// 远程日志的发送者
        /// </summary>
        public string RemoteSender { get; set; }
        /// <summary>
        /// 远程日志接收地址
        /// </summary>
        public string RemoteUrl { get; set; }
        /// <summary>
        /// 远程日志主机
        /// </summary>
        public string RemoteHost { get; set; }
        /// <summary>
        /// 本地文件路径
        /// </summary>
        public string LocalUrl { get; set; }
        /// <summary>
        /// 本地文件的前缀
        /// </summary>
        public string LogPrefix { get; set; }
        /// <summary>
        /// 缓冲队列最大消息数
        /// </summary>
        public int MaxQueueSize { get; set; }
        /// <summary>
        /// 发送间隔,当根据时间发送时,表示发送时间的间隔, 当根据数量发送是, 表示队列达到该数量后, 发送数据
        /// </summary>
        public int SendInterval { get; set; }
        /// <summary>
        /// 额外数据获取者
        /// </summary>
        public string ExtendDataProvider { get; set; }

        protected override void Append(LoggingEvent loggingEvent)
        {
            try
            {
                SysLogEntity entry = new SysLogEntity();
                entry.TryParseLoggingEvent(loggingEvent);
                //远程的Sender
                if (extendDataPrivoder != null)
                {
                    entry.AccessToken = extendDataPrivoder.GetAccesstoken();
                    remoteSender.DoAppend(entry);
                }
                //本地Sender
                if (localSender != null)
                    localSender.DoAppend(entry);
            }
            catch(Exception ex) 
            {
                throw ex;
            }
        }
        public override void ActivateOptions()
        {
            Init();
            base.ActivateOptions();
        }

        public virtual void Clear()
        {
        }
        /// <summary>
        /// 初始化Appender对象
        /// </summary>
        private void Init()
        {
            //本地的Sender不是必须的
            //Arguments.NotNullOrWhiteSpace(LocalSender, "LocalSender");
            Nd.Tool.Arguments.NotNullOrWhiteSpace(RemoteSender, "RemoteSender");
            try
            {
                //初始化Sender对象
                if (!String.IsNullOrEmpty(this.LocalSender))
                    localSender = Sender.SenderFactory.Instance.GetSender(LocalSender);
                remoteSender = Sender.SenderFactory.Instance.GetSender(RemoteSender);
                //构建remote Sender的配置
                IDictionary<string, object> remoteSenderConfig = new Dictionary<string, object>();
                remoteSenderConfig.Add("GathererPath", RemoteUrl);
                remoteSenderConfig.Add("GathererHost", RemoteHost);
                remoteSenderConfig.Add("MaxQueueSize", MaxQueueSize);
                remoteSenderConfig.Add("SendInterval", SendInterval);
                remoteSender.Prepare(remoteSenderConfig);
                //构建local sender的配置
                if (localSender != null)
                {
                    IDictionary<string, object> localSenderConfig = new Dictionary<string, object>();
                    if (System.Web.HttpContext.Current != null)
                    {
                        if (LocalUrl.Contains("/"))
                        {
                            localSenderConfig.Add("LocalUrl",
                                System.Web.HttpContext.Current.Server.MapPath(LocalUrl));
                        }
                        else
                        {
                            localSenderConfig.Add("LocalUrl", LocalUrl);
                        }
                    }
                    else
                    {
                        localSenderConfig.Add("LocalUrl", LocalUrl);
                    }
                    localSenderConfig.Add("LogPrefix", LogPrefix);
                    localSender.Prepare(localSenderConfig);
                }
                //初始化额外数据提供者
                if (!String.IsNullOrEmpty(ExtendDataProvider))
                {
                    object objPrivoder = FastActivator.Create(ExtendDataProvider);
                    if (objPrivoder is IGathererDataProvider)
                    {
                        extendDataPrivoder = (IGathererDataProvider)objPrivoder;
                    }
                    else
                    {
                        throw new ArgumentException("配置中的extendDataPrivoder对象没有实现IGathererDataPrivoder接口");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
