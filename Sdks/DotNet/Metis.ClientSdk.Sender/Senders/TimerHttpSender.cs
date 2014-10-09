using Metis.ClientSdk.Counter;
using Metis.ClientSdk.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading;

namespace Metis.ClientSdk.Sender
{
    /// <summary>
    /// 通过HTTP定时发送日志的发送者
    /// </summary>
    internal class TimerHttpSender : BaseHttpSender
    {
        private Timer timer = null;
        private bool running = false;
        protected static ConcurrentQueue<LogEntity> logList = new ConcurrentQueue<LogEntity>();

        public TimerHttpSender(){ }
        public override void Prepare(IDictionary<string, object> config)
        {
            base.Prepare(config);
            //初始化Timer
            timer = new Timer(new TimerCallback(SendLogs), null, interval * 1000, interval * 1000);
        }
        public override void DoAppend(LogEntity entry)
        {
            //当队列中的对象数量小于最大数量时
            if (logList.Count < maxLogEntry)
            {
                logList.Enqueue(entry);
            }
        }
        internal void SendLogs(object sender)
        {
            if (running)
                return;
            running = true;

            List<LogEntity> postList = new List<LogEntity>(100);
            LogEntity entry;
            int counter = 0;

            while (logList.TryDequeue(out entry) && counter < sendBatchSize)
            {
                postList.Add(entry);
                counter++;
            }

            //如果有需要发送的日志
            if (postList.Count > 0)
            {
                //将要发送的日志, 按照日志类型进行分类
                var entityGroup = postList.GroupBy<LogEntity, string>(o => o.LogType);
                foreach (var group in entityGroup)
                {
                    //每次将某种类型的日志进行发送
                    int repeatCount = (int)Math.Ceiling(group.Count() * 1.0D / batchSize);
                    for (int i = 0; i < repeatCount; i++)
                    {
                        var data = group.Skip(i * batchSize).Take(batchSize).ToList();
                        PostData(data);
                    }
                }
            }
            running = false;
        }
    }
}
