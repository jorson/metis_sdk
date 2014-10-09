using Metis.ClientSdk.Entities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;

namespace Metis.ClientSdk.Sender
{
    internal abstract class BaseHttpSender : ISingleSender
    {
        protected int maxLogEntry = 5000;
        protected int interval = 3;

        protected readonly int batchSize = 200;
        protected readonly int sendBatchSize = 500;

        protected readonly CsvSerializer serializer = new CsvSerializer();
        protected GaeaHttpClient httpClient = new GaeaHttpClient(2000);
        protected string gathererPath = String.Empty;

        public abstract void DoAppend(LogEntity entry);
        public virtual void Prepare(IDictionary<string, object> config)
        {
            if (!config.Keys.Contains("GathererPath"))
                throw new ArgumentNullException("必须存在GathererPath的配置");

            this.gathererPath = config["GathererPath"].ToString();
            //设置HttpClient的参数
            object gathererHost = null;
            if (config.TryGetValue("GathererHost", out gathererHost))
                httpClient.Host = gathererHost.ToString();
            httpClient.Headers.Add("charset", "utf-8");
            //设置其他变量
            object objMaxEntry = null;
            if (config.TryGetValue("MaxQueueSize", out objMaxEntry))
                Int32.TryParse(objMaxEntry.ToString(), out maxLogEntry);
            object objInterval = null;
            if (config.TryGetValue("SendInterval", out objInterval))
                Int32.TryParse(objInterval.ToString(), out interval);
        }
        public virtual void Clear()
        {
        }
        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="logs"></param>
        protected void PostData(List<LogEntity> logs)
        {
            try
            {
                if (logs.Count == 0)
                    return;

                NameValueCollection data = new NameValueCollection();
                var firstLog = logs.FirstOrDefault();
                //添加需要发送的数据
                data.Add("TerminalCode", firstLog.TerminalCode.ToString());
                var sendData = serializer.SerializeMutil(firstLog.GetType(), logs);
                data.Add("MultiData", HttpUtility.UrlEncode(sendData));
                //获取需要添加的路径
                var postUrl = GetRequestUrl(firstLog.LogType.ToLower());
                if (httpClient.Headers.AllKeys.Contains("AccessToken"))
                {
                    httpClient.Headers["AccessToken"] = firstLog.AccessToken;
                }
                else
                {
                    httpClient.Headers.Add("AccessToken", firstLog.AccessToken);
                }
                //向服务器POST数据
                httpClient.HttpPost(postUrl, data);
                //响应正确
                logs.ForEach(o => o.Ack());
            }
            catch
            {
                //响应失败
                logs.ForEach(o => o.Fail());
            }
        }
        private string GetRequestUrl(params string[] args)
        {
            var frames = new List<string>();
            frames.Add(gathererPath);
            frames.AddRange(args);
            return UriPath.Combine(frames.ToArray());
        }
    }
}
