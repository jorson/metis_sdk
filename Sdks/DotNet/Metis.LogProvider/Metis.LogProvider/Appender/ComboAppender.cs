using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Metis.ClientSdk;
using log4net.Core;
using log4net.Appender;

namespace Metis.ClientSdk.LogProvider
{
    public class ComboAppender : AppenderSkeleton
    {
        ISingleSender localSender, remoteSender;

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
        /// 缓冲队列最大消息数
        /// </summary>
        public int MaxQueueSize { get; set; }
        /// <summary>
        /// 发送间隔,当根据时间发送时,表示发送时间的间隔, 当根据数量发送是, 表示队列达到该数量后, 发送数据
        /// </summary>
        public int SendInterval { get; set; }

        protected override void Append(LoggingEvent loggingEvent)
        { 
            throw new NotImplementedException();
        }

        public virtual void Clear()
        {


        }

        private void Init()
        {
            Arguments.NotNullOrWhiteSpace(LocalSender, "LocalSender");
            Arguments.NotNullOrWhiteSpace(RemoteSender, "RemoteSender");
            //初始化Sender对象
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
            IDictionary<string, object> localSenderConfig = new Dictionary<string, object>();
            localSenderConfig.Add("LocalUrl", LocalUrl);
            localSender.Prepare(localSenderConfig);
        }
    }
}
