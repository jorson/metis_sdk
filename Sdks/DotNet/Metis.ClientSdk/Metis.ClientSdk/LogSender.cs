using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Metis.ClientSdk.Entities;

namespace Metis.ClientSdk
{
    internal class LogSender
    {
        private const int MAX_NUMBER_LOGENTRY = 5000;

        private static readonly int batchSize = 200;
        private static readonly int sendBatchSize = 500;
        private static readonly int interval = 3;
        private static readonly CsvSerializer serializer = new CsvSerializer();

        private static Timer timer = null;
        private static bool running = false;

        private static ConcurrentQueue<LogEntity> logList = new ConcurrentQueue<LogEntity>();
        private static GaeaHttpClient httpClient = new GaeaHttpClient(2000);

        static LogSender()
        {
            //设置HttpClient的参数
            httpClient.Host = GathererContext.Current.GathererHost;
            httpClient.Headers.Add("charset", "utf-8");
            //初始化Timer
            timer = new Timer(new TimerCallback(SendLogs), null, interval * 1000, interval * 1000);
        }
        /// <summary>
        /// 将日志数据实体加入到待发送队列 
        /// </summary>
        /// <param name="entry">日志实体对象</param>
        public static void Append(LogEntity entry)
        {
            //当队列中的对象数量小于最大数量时
            if (logList.Count < MAX_NUMBER_LOGENTRY)
            {
                logList.Enqueue(entry);
            }
        }

        internal static void SendLogs(object sender)
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
            if(postList.Count > 0)
            {
                //将要发送的日志, 按照日志类型进行分类
                var entityGroup = postList.GroupBy<LogEntity, string>(o => o.LogType);
                foreach(var group in entityGroup)
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

        private static void PostData(List<LogEntity> logs)
        {
            try
            {
                if (logs.Count == 0)
                    return;

                NameValueCollection data = new NameValueCollection();
                var firstLog = logs.FirstOrDefault();
                //添加需要发送的数据
                data.Add("TerminalCode", firstLog.TerminalCode.ToString());
                data.Add("MultiData", serializer.SerializeMutil(firstLog.GetType(), logs));
                //获取需要添加的路径
                var postUrl = GetRequestUrl(firstLog.LogType.ToLower());
                if(httpClient.Headers.AllKeys.Contains("AccessToken"))
                {
                    httpClient.Headers["AccessToken"] = firstLog.AccessToken;
                }
                else
                {
                    httpClient.Headers.Add("AccessToken", firstLog.AccessToken);
                }                
                //向服务器POST数据
                httpClient.HttpPost(postUrl, data);
            }
            catch
            {
            }
            
        }

        private static string GetRequestUrl(params string[] args)
        {
            var frames = new List<string>();
            frames.Add(GathererContext.Current.GathererPath);
            frames.AddRange(args);
            return UriPath.Combine(frames.ToArray());
        }
    }
}
