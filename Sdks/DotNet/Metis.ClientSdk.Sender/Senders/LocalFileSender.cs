using Metis.ClientSdk.Counter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;

namespace Metis.ClientSdk.Sender
{
    /// <summary>
    /// 本地日志文件发送者
    /// </summary>
    internal class LocalFileSender : ISingleSender
    {
        private string logPrefix = "LocalLog_";
        private string logSuffix = ".log";
        private string rootPath = String.Empty;

        private static string currentLogFile = String.Empty;
        private object syncObj = new object();
        private FileStream currentStream;

        private static readonly JavaScriptSerializer serializer = new JavaScriptSerializer();

        public LocalFileSender()
        {
        }
        public void Prepare(IDictionary<string, object> map)
        {
            //初始化配置项目
            object objLocalUrl = null, objPrefix = null;
            if (map.TryGetValue("LocalUrl", out objLocalUrl))
            {
                this.rootPath = objLocalUrl.ToString();
                if (!this.rootPath.EndsWith(@"\"))
                    this.rootPath = this.rootPath + @"\";
            }
            if (map.TryGetValue("LogPrefix", out objPrefix))
                this.logPrefix = objPrefix.ToString();
            //初始化最初开始的文件
            string startFile = String.Format("{0}{1}{2}{3}", 
                rootPath, logPrefix, DateTime.Now.ToString("yyyyMMdd"), logSuffix);
            //如果最初的文件不存在
            currentLogFile = startFile;
            this.currentStream = new FileStream(currentLogFile, FileMode.Append, 
                FileAccess.Write, FileShare.Write, 4096, true);
        }
        public void DoAppend(Entities.LogEntity entry)
        {
            string currFile = String.Format("{0}{1}{2}{3}",
                rootPath, logPrefix, DateTime.Now.ToString("yyyyMMdd"), logSuffix);
            if (currentLogFile != currFile)
            {
                lock (syncObj)
                {
                    currentLogFile = currFile;
                    if (this.currentStream != null)
                    {
                        //关闭当前文件流 
                        this.currentStream.Flush();
                        this.currentStream.Close();
                        this.currentStream.Dispose();
                        //重新初始化新的文件流
                        this.currentStream = new FileStream(currentLogFile, FileMode.Append,
                            FileAccess.Write, FileShare.ReadWrite, 4096, true);
                    }
                }
            }
            WriteFile(entry);
        }
        private void WriteFile(Entities.LogEntity entry)
        {
            string record = serializer.Serialize(entry);
            record = String.Format("{0}\t{1}\r\n", entry.CallTimestamp.ToString("yyyy-MM-dd HH:mm:ss"), record);
            byte[] inputBytes = Encoding.Default.GetBytes(record);
            try
            {
                IAsyncResult result = this.currentStream.BeginWrite(inputBytes, 0, inputBytes.Length,
                                        new AsyncCallback((rt) =>
                                        {
                                            if (rt.IsCompleted)
                                                entry.Ack();
                                        }), null);
                this.currentStream.EndWrite(result);
            }
            catch
            {
                entry.Fail();
            }

        }
        public void Clear()
        {
            if (this.currentStream != null)
            {
                this.currentStream.Flush();
                this.currentStream.Close();
                this.currentStream.Dispose();
            }
        }
    }
}