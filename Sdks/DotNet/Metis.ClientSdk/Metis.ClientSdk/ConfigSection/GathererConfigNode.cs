using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Metis.ClientSdk.ConfigSection
{
    public class GathererConfigNode
    {

        public string Name { get; internal set; }

        public GathererConfigNode Parent { get; internal set; }

        public IEnumerable<GathererConfigNode> Nodes { get; internal set; }

        public IDictionary<string, string> Attributes { get; internal set; }

        internal bool IsRoot
        {
            get
            {
                return this == GathererSection.Instance.RootNode;
            }
        }

        public string Path
        {
            get
            {
                string path = "";
                GathererConfigNode node = this;
                while (node != null)
                {
                    path = node.Name + "/" + path;
                    node = node.Parent;
                }
                return path;
            }
        }

        public static GathererConfigNode ConvertFrom(XElement element, GathererConfigNode parent)
        {
            GathererConfigNode node = new GathererConfigNode();
            node.Name = element.Name.LocalName;
            node.Parent = parent;
            node.Attributes = element.Attributes().ToDictionary(o => o.Name.LocalName, o => o.Value);
            node.Nodes = element.Elements().Select(o => ConvertFrom(o, node));
            return node;
        }

        public bool Contains(string path)
        {
            path = path.Replace("\\", "/");
            int f = path.IndexOf("/");
            if (f == -1)
            {
                string v;
                bool hasAttr = Attributes.TryGetValue(path, out v);
                if (hasAttr)
                    return true;
                return Nodes.Any(o => o.Name == path);
            }

            string name = path.Substring(0, f);
            GathererConfigNode node = Nodes.FirstOrDefault(o => o.Name == name);
            if (node == null)
                return false;
            return node.Contains(path.Substring(f + 1));
        }

        public GathererConfigNode TryGetNode(string path)
        {
            path = path.Replace("\\", "/");
            int f = path.IndexOf("/");
            if (f == -1)
                return Nodes.FirstOrDefault(o => o.Name == path);

            string name = path.Substring(0, f);
            GathererConfigNode node = Nodes.FirstOrDefault(o => o.Name == name);
            if (node == null)
                return null;
            return node.TryGetNode(path.Substring(f + 1));
        }

        public IEnumerable<GathererConfigNode> TryGetNodes(string path)
        {
            path = path.Replace("\\", "/");
            int f = path.IndexOf("/");
            if (f == -1)
                return Nodes.Where(o => o.Name == path);

            string name = path.Substring(0, f);
            GathererConfigNode node = Nodes.FirstOrDefault(o => o.Name == name);
            if (node == null)
                return new List<GathererConfigNode>();
            return node.TryGetNodes(path.Substring(f + 1));
        }

        public string TryGetValue(string path)
        {
            path = path.Replace("\\", "/");
            int f = path.IndexOf("/");
            if (f == -1)
            {
                string v;
                if (Attributes.TryGetValue(path, out v))
                    return v;
                return null;
            }
            string name = path.Substring(0, f);

            GathererConfigNode node = Nodes.FirstOrDefault(o => o.Name == name);
            if (node == null)
                return null;

            return node.TryGetValue(path.Substring(f + 1));
        }
    }
}
