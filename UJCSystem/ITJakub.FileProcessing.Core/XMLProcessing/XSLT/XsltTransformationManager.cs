using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace ITJakub.FileProcessing.Core.XMLProcessing.XSLT
{
    public class XsltTransformationManager
    {
        private readonly XslCompiledTransform m_toStringTransformation;

        public XsltTransformationManager(string toStringXsltName)
        {
            m_toStringTransformation = new XslCompiledTransform();
            using (Stream strm = Assembly.GetExecutingAssembly().GetManifestResourceStream(toStringXsltName))
            using (XmlReader reader = XmlReader.Create(strm))
            {
                m_toStringTransformation.Load(reader);
            }
        }

        public string TransformToString(XmlReader xmlReader)
        {
            var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, ConformanceLevel = ConformanceLevel.Fragment };
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
            {
                m_toStringTransformation.Transform(xmlReader, null, xmlWriter);
            }
            return stringWriter.ToString();
        }
    }
}