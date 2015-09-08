using System.Diagnostics;
using System.Xml;

namespace Daliboris.Pomucky.Xml
{
    [DebuggerDisplay("{Prefix}, {Name}, {Depth}")]
    internal class XmlElementInfo
    {
        public XmlElementInfo(string name, int depth)
        {
            Name = name;
            Depth = depth;
        }

        public XmlElementInfo(string name, int depth, XmlElementInfo parent)
        {
            Name = name;
            Depth = depth;
            Parent = parent;
        }

        public XmlElementInfo(string ns, string prefix, string name, int depth, XmlElementInfo parent)
        {
            Namespace = ns;
            Prefix = prefix;
            Name = name;
            Depth = depth;
            Parent = parent;
        }

        public XmlElementInfo(string ns, string prefix, string name, int depth)
        {
            Namespace = ns;
            Prefix = prefix;
            Name = name;
            Depth = depth;
        }

        public string Namespace { get; set; }
        public string Prefix { get; set; }
        public string Name { get; set; }
        public int Depth { get; set; }
        public XmlElementInfo Parent { get; set; }



        public string GetXPath()
        {
            return GetXPath(false);
        }

        public string GetXPath(bool includingPrefix)
        {
            if (Parent == null)
                return Name;
            return Parent.GetXPath(includingPrefix) + "/" + Name;
        }



        public static XmlElementInfo GetInfo(XmlReader xmlReader)
        {
            XmlElementInfo info = new XmlElementInfo(xmlReader.NamespaceURI, xmlReader.Prefix, xmlReader.LocalName,
                xmlReader.Depth);
            return info;
        }

        public static XmlElementInfo GetInfo(XmlReader xmlReader, XmlElementInfo parent)
        {
            XmlElementInfo info = GetInfo(xmlReader);
            info.Parent = parent;
            return info;
        }
    }
}