using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.Sender
{
    /// <summary>
    /// 发送者工厂
    /// </summary>
    public class SenderFactory
    {
        private static SenderFactory senderFactory;
        private SenderFactory()
        {
        }
        /// <summary>
        /// 发送者工厂单例
        /// </summary>
        public static SenderFactory Instance
        {
            get
            {
                if (senderFactory == null)
                {
                    senderFactory = new SenderFactory();
                }
                return senderFactory;
            }
        }

        public ISingleSender GetSender(string type)
        {
            try
            {
                Object obj = FastActivator.Create(type);
                if (!(obj is ISingleSender))
                    throw new ArgumentException("type:" + type + " not implement ISingleSender");
                return (ISingleSender)obj;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
