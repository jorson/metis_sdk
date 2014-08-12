using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Xml.Linq;

namespace Metis.ClientSdk.ConfigSection
{
    public class GathererSection : ConfigurationSection
    {
        const string sectionName = "metis.gatherer";
        GathererConfigNode rootNode;
        static FileSystemWatcher watcher;

        public static EventHandler GaeaSectionChanged;

        static GathererSection()
        {
            if (HostingEnvironment.IsHosted)
            {
                var file = HostingEnvironment.MapPath("~/web.config");
                XDocument doc = XDocument.Load(file);
                var configSourceAttr = doc.Descendants("metis.gatherer").Attributes("configSource").FirstOrDefault();

                if (configSourceAttr != null)
                {
                    var configSource = configSourceAttr.Value;
                    if (!configSource.StartsWith("~"))
                        configSource = Path.Combine("~/", configSource);
                    var path = HostingEnvironment.MapPath(configSource);

                    watcher = new FileSystemWatcher(Path.GetDirectoryName(path), Path.GetFileName(path));
                    watcher.NotifyFilter = NotifyFilters.LastWrite;
                    watcher.Changed += ConfigChanged;
                    watcher.EnableRaisingEvents = true;
                }
            }
        }

        public GathererConfigNode RootNode { get { return rootNode; } }

        public static GathererSection Instance
        {
            get
            {
                return ConfigurationManager.GetSection(sectionName) as GathererSection;
            }
        }

        protected override void DeserializeElement(System.Xml.XmlReader reader, bool serializeCollectionKey)
        {
            string xml = reader.ReadOuterXml();
            XDocument doc = XDocument.Parse(xml);
            rootNode = GathererConfigNode.ConvertFrom((XElement)doc.FirstNode, null);
        }

        public IEnumerable<GathererConfigNode> TryGetNodes(string path)
        {
            return rootNode.TryGetNodes(path);
        }

        public GathererConfigNode TryGetNode(string path)
        {
            return rootNode.TryGetNode(path);
        }

        public string TryGetValue(string path)
        {
            return rootNode.TryGetValue(path);
        }

        public Type TryGetType(string path)
        {
            string type = TryGetValue(path);
            if (type == null)
                return null;
            return Type.GetType(type);
        }

        static void ConfigChanged(object sender, FileSystemEventArgs e)
        {
            ConfigurationManager.RefreshSection(sectionName);
            if (GaeaSectionChanged != null)
                GaeaSectionChanged(sender, e);
        }
    }
}
