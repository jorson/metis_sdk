using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;

namespace Metis.ClientSdk
{
    internal class GathererLogger
    {
        static GathererLogger logger;

        int maxCount = 100;
        ConcurrentQueue<string> logList = new ConcurrentQueue<string>();

        public static GathererLogger Instance
        {
            get
            {
                if (logger == null)
                {
                    logger = new GathererLogger();
                }
                return logger;
            }
        }

        private GathererLogger()
        {
        }

        public void Write(string info)
        {
            string tmpString = String.Empty;
            while(logList.Count >= maxCount)
            {
                logList.TryDequeue(out tmpString);
            }
            logList.Enqueue(info);
        }

        public string Read()
        {
            StringBuilder builder = new StringBuilder();
            var list = logList.ToList();

            foreach (var info in list)
            {
                builder.AppendLine(info);
            }
            return builder.ToString();
        }
    }
}
