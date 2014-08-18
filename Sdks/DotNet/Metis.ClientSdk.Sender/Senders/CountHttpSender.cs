using Metis.ClientSdk.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Metis.ClientSdk.Sender
{
    /// <summary>
    /// 按照队列中数据的数量发送
    /// </summary>
    internal class CountHttpSender : BaseHttpSender
    {
        CancellationTokenSource cancelSrc;
        TaskFactory taskFactory;

        public CountHttpSender() 
        {
            cancelSrc = new CancellationTokenSource();
            taskFactory = new TaskFactory(cancelSrc.Token);
        }
        /// <summary>
        /// 添加新的Entry
        /// </summary>
        public override void DoAppend(LogEntity entry)
        {
            if (logList.Count < maxLogEntry)
            {
                logList.Enqueue(entry);
            }
            else
            {
                logList.Enqueue(entry);
                IList<LogEntity> sendDatas = new List<LogEntity>(this.sendBatchSize);
                for (int i = this.interval; i >= 0; i--)
                {
                    // 如果列表已经被清空, 强制退出循环
                    if (logList.IsEmpty)
                        break;
                    LogEntity et = null;
                    if (logList.TryDequeue(out et))
                        sendDatas.Add(et);
                }
                //启动一个任务进行发送
                this.taskFactory.StartNew(SendData, sendDatas);
            }
        }
        /// <summary>
        /// 这个方法用于停止所有发送任务
        /// </summary>
        public override void Clear()
        {
            if (taskFactory != null && cancelSrc != null)
            {
                cancelSrc.Cancel();
            }
        }
        private void SendData(object datas)
        {
            IList<LogEntity> postList = (List<LogEntity>)datas;
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
        }
    }
}
