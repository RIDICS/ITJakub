using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Daliboris.Pomucky.Xml
{
    public class WordBreaker
    {
        private int _capacity;
        const string NsTei = "http://www.tei-c.org/ns/1.0";
        const string NsNlp = "http://vokabular.ujc.cas.cz/ns/tei-nlp/1.0";
        const string NsXml = "http://www.w3.org/XML/1998/namespace";

        public string Input { get; set; }
        public string Output { get; set; }
        public List<string> IgnoredElements { get; set; }
        public string Punctation { get; set; }
        /// <summary>
        /// Formát xml:id; pokud bude null, atribut se negeneruje. Formát by měl obsahovat místo pro jedinečnou hodnotu (pořadí slova v dokumentu).
        /// </summary>
        public string XmlIdFormat { get; set; }
        public void Run()
        {

            int id = 0;

            XmlElementInfo currentElement = null;

            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(new NameTable());
            namespaceManager.AddNamespace(String.Empty, NsTei);

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;

            List<string> ignoredElements = IgnoredElements ?? new List<string>();
            XmlElementInfo ignoredElement = null;

            string punctation = Punctation ?? String.Empty;

            bool ignoreText = false;
            bool? isRoot = null;

            using (XmlReader xmlReader = XmlReader.Create(Input))
            {
                using (XmlWriter xmlWriter = XmlWriter.Create(Output, xmlWriterSettings))
                {
                    xmlWriter.WriteStartDocument();


                    while (xmlReader.Read())
                    {
                        XmlNodeType nodeType = xmlReader.NodeType;
                        if (nodeType == XmlNodeType.Element)
                        {
                            if (!xmlReader.IsEmptyElement)
                                currentElement = XmlElementInfo.GetInfo(xmlReader, currentElement);
                            WriteXmlElementInfo(currentElement);

                            isRoot = !isRoot.HasValue;
                            if (ignoredElement == null && ignoredElements.Contains(xmlReader.LocalName))
                            {
                                ignoredElement = currentElement;
                                ignoreText = true;
                            }
                        }
                        if (nodeType == XmlNodeType.EndElement)
                        {

                            if (currentElement != null &&
                                ignoreText && 
                                currentElement.Name == ignoredElement.Name &&
                                currentElement.Depth == ignoredElement.Depth)
                                //if (ignoredElements.Contains(xmlReader.LocalName))
                            {
                                ignoredElement = null;
                                ignoreText = false;
                            }
                            if (currentElement != null)
                                currentElement = currentElement.Parent;
                            WriteXmlElementInfo(currentElement);

                        }
                        if (nodeType == XmlNodeType.Text)
                        {
                            if (ignoreText)
                            {
                                Transformace.SerializeNode(xmlReader, xmlWriter);
                                continue;
                            }


                            string text = xmlReader.Value;

                            if (text.IndexOf(' ') == -1)
                            {
                                xmlWriter.WriteElementString("w", NsTei, text);
                            }
                            else
                            {
                                _capacity = text.Length;
                                StringBuilder stringBuilder = new StringBuilder(_capacity);
                                foreach (char c in text)
                                {
                                    if (c == ' ' || c == '\u0001' || c == '\t') //mezera a pevná mezera
                                    {
                                        WriteWordElement(ref stringBuilder, xmlWriter, _capacity, id++, XmlIdFormat);
                                        xmlWriter.WriteStartElement("c");
                                        xmlWriter.WriteAttributeString("type", "space");
                                        xmlWriter.WriteAttributeString("space", NsXml, "preserve");
                                        if (c == '\t')
                                            xmlWriter.WriteString("\t");
                                        else
                                            xmlWriter.WriteString(" ");
                                        xmlWriter.WriteEndElement();
                                    }
                                    else
                                    {
                                        if (punctation.IndexOf(c) > -1)
                                        {
                                            WriteWordElement(ref stringBuilder, xmlWriter, _capacity, id++, XmlIdFormat);
                                            xmlWriter.WriteElementString("pc", NsTei, String.Format("{0}", c));
                                        }
                                        else
                                        {
                                            stringBuilder.Append(c);
                                        }
                                    }
                                }
                                //výpis posledního slova uloženého v bufferu
                                WriteWordElement(ref stringBuilder, xmlWriter, 0, id++, XmlIdFormat);
                            }
                        }
                        else
                        {
                            Transformace.SerializeNode(xmlReader, xmlWriter);
                            if (isRoot.HasValue && isRoot.Value)
                            {
                                xmlWriter.WriteAttributeString("xmlns", "nlp", null, NsNlp);
                                isRoot = false;
                            }
                        }
                    }

                }
            }
        }

        private void WriteXmlElementInfo(XmlElementInfo xmlElementInfo)
        {
            return;
            if (xmlElementInfo != null)
                // ReSharper disable LocalizableElement
                Console.WriteLine("{0}, {1}, {2}, {3}", xmlElementInfo.Name, xmlElementInfo.Prefix, xmlElementInfo.Depth, xmlElementInfo.GetXPath());
            // ReSharper restore LocalizableElement
        }

        private static void WriteWordElement(ref StringBuilder stringBuilder, XmlWriter xmlWriter, int length, int id, string xmlIdFormat)
        {
            if (stringBuilder.Length > 0)
            {
                xmlWriter.WriteStartElement("w", NsTei);
                if(xmlIdFormat != null)
                    xmlWriter.WriteAttributeString("xml", "id", NsXml, String.Format(xmlIdFormat, id));
                xmlWriter.WriteString(stringBuilder.ToString());
                xmlWriter.WriteEndElement();
                stringBuilder = new StringBuilder(length);
            }
        }
    }
}