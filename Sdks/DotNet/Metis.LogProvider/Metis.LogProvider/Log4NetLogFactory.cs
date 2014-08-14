using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Metis.ClientSdk.LogProvider
{
    public class Log4NetLogFactory : Nd.Tool.ILogFactory
    {
        static Log4NetLogFactory()
        {
            string configFile = System.Configuration.ConfigurationManager.AppSettings["log4net.config"];
            Nd.Tool.Arguments.NotNull(configFile, "configFile");
            if (System.Web.HttpContext.Current == null)
            {
                configFile = System.IO.Path.Combine(Environment.CurrentDirectory, configFile);
            }
            else
            {
                configFile = System.Web.HttpContext.Current.Server.MapPath(configFile);
            }
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(configFile));
        }
        public void Flush()
        {
        }

        public Nd.Tool.ILog GetLogger(Type type)
        {
            return new Log4NetLog(log4net.LogManager.GetLogger(type));
        }

        public Nd.Tool.ILog GetLogger(string name)
        {
            return new Log4NetLog(log4net.LogManager.GetLogger(name));
        }
    }
}
