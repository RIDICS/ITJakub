using System.Collections.Generic;
using System.Text;
using System.Xml;
using ITJakub.Contracts.Searching;

namespace ITJakub.Core.Database.Exist
{
    /// <summary>
    /// Utility class with useful methods used for manipulation with XML documents.
    /// </summary>
    public class XmlTool
    {
        private readonly TeiP5Descriptor m_descriptor;

        public XmlTool(TeiP5Descriptor descriptor)
        {
            m_descriptor = descriptor;
        }

        /// <summary>
        /// Removes all XML comments.
        /// </summary>
        /// <param name="xmlFilepath">Filepath to the XML</param>
        public static void RemoveXmlComments(string xmlFilepath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFilepath);
            doc.PreserveWhitespace = false;
            XmlNodeList list = doc.SelectNodes("//comment()");

            if (list != null)
                foreach (XmlNode node in list)
                {
                    if (node.ParentNode != null)
                        node.ParentNode.RemoveChild(node);
                }
            doc.Save(xmlFilepath);
        }

        /// <summary>
        /// Cuts text contents specified by element name from specified XML.
        /// </summary>
        /// <param name="xml">source XML</param>
        /// <param name="elementName">name of element</param>
        /// <returns>text content of specified elements</returns>
        public static string[] CutElementsText(string xml, string elementName)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            doc.PreserveWhitespace = false;
            XmlNodeList list = doc.SelectNodes("//" + elementName);
            List<string> strList = new List<string>();

            if (list != null)
                foreach (XmlNode node in list)
                {
                    strList.Add(node.InnerText);
                }
            return strList.ToArray();
        }


        /// <summary>
        /// parse kwic result
        /// </summary>
        /// <param name="rootNodeForKwic">source xmlNode</param>
        /// <param name="kwicNode">kwic node root</param>
        /// <param name="nManager"></param>
        /// <returns></returns>
        public static KwicStructure ParseKwicStructure(XmlNode rootNodeForKwic, string kwicNode, XmlNamespaceManager nManager)
        {
            XmlNode node = rootNodeForKwic.SelectSingleNode(string.Format(".//{0}", kwicNode), nManager);

            if (node != null)
            {
                KwicStructure kwic = new KwicStructure
                    {
                        Before = GetKwicSubstringFromXmlNode(node, "previous"),
                        Match = GetKwicSubstringFromXmlNode(node, "hi"),
                        After = GetKwicSubstringFromXmlNode(node, "following")
                    };
                return kwic;
            }
            return null;
        }


        private static string GetKwicSubstringFromXmlNode(XmlNode kwicNode, string classSpecification)
        {
            XmlNodeList selectedNodes = kwicNode.SelectNodes(string.Format("span[@class='{0}']", classSpecification));
            //XmlNodeList selectedNodes = kwicNode.SelectNodes(string.Format("/span[@class='{0}']", classSpecification));
            if (selectedNodes != null)
            {
                StringBuilder builder = new StringBuilder();
                foreach (XmlNode selectedFilteredNode in selectedNodes)
                {
                    builder.Append(selectedFilteredNode.InnerText);
                }
                return builder.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Removes the root element (all children are preserved) from the XML.
        /// </summary>
        /// <param name="xml">XML to be processed</param>
        /// <returns>XML without its root element</returns>
        public static string RemoveRootElement(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            doc.PreserveWhitespace = false;
            XmlNode parent = doc.SelectSingleNode("/*");

            StringBuilder sb = new StringBuilder();
            if (parent != null)
                foreach (XmlNode node in parent.ChildNodes)
                {
                    sb.Append(node.OuterXml);
                }
            return sb.ToString();
        }

        public static string ParseTeiAuthor(XmlNode authorNode, string authorNodeName, XmlNamespaceManager nManager)
        {
            XmlNode teiAuthorNode = authorNode.SelectSingleNode(string.Format(".//tei:{0}", authorNodeName), nManager);//TODO multiple authors possible ?
            if (teiAuthorNode != null)
            {
                var childNodes = teiAuthorNode.ChildNodes;
                StringBuilder result = new StringBuilder();

                foreach (XmlNode node in childNodes)
                {
                    if (node.Name == TeiP5Descriptor.WordTagName)
                        result.Append(node.InnerText);
                    if (node.Name == TeiP5Descriptor.SpaceTagName)
                        result.Append(" ");
                }

                return result.ToString();
            }
            return null;
        }


        public static string ParseTeiTitle(XmlNode titleNode, string titleNodeName, XmlNamespaceManager nManager)
        {
            XmlNode teiTitleNod = titleNode.SelectSingleNode(string.Format(".//tei:{0}", titleNodeName), nManager);//TODO multiple titles possible ?
            if (teiTitleNod != null)
            {
                var childNodes = teiTitleNod.ChildNodes;
                StringBuilder result = new StringBuilder();

                foreach (XmlNode node in childNodes)
                {
                    if (node.Name == TeiP5Descriptor.WordTagName)
                        result.Append(node.InnerText);
                    if (node.Name == TeiP5Descriptor.SpaceTagName)
                        result.Append(" ");
                }

                return result.ToString();
            }
            return null;
        }

        public static List<string> ParseTeiCategoriesIds(XmlNode categoriesNode, string categoriesNodeName, string categoryTargetAttName, XmlNamespaceManager namespaceManager)
        {
            XmlNodeList catList = categoriesNode.SelectNodes(string.Format(".//tei:{0}", categoriesNodeName), namespaceManager);
            var result = new List<string>();
            if (catList != null)
                foreach (XmlNode catNode in catList)
                {
                    if (catNode.Attributes != null)
                        result.Add(catNode.Attributes[categoryTargetAttName].Value);
                }

            return result;
        }

        public static string ParseId(XmlNode selectedNode, string idAttributeName)
        {
            if (selectedNode.Attributes != null)
            {
                XmlAttribute item = selectedNode.Attributes[idAttributeName];
                return item.Value;
            }
            return null;
        }

        public static string ParseXmlContext(XmlNode contextNode)
        {
            return contextNode.InnerXml;
        }
    }
}