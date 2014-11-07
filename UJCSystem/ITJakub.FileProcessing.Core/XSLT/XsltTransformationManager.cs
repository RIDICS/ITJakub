using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;

namespace ITJakub.FileProcessing.Core.XSLT
{
    public class XsltTransformationManager
    {
        private readonly XslCompiledTransform m_transformation;

        public XsltTransformationManager()  //TODO refactor loading assembly
        {
            m_transformation = new XslCompiledTransform();
            using (Stream strm = Assembly.GetExecutingAssembly().GetManifestResourceStream("ITJakub.FileProcessing.Core.XSLT.CommonTEI.xsl"))
            using (XmlReader reader = XmlReader.Create(strm))
            {
                m_transformation.Load(reader);
            }
        }

        public string TransformToString(XmlReader xmlReader)
        {
            var stringWriter = new StringWriter();
            var settings = new XmlWriterSettings { OmitXmlDeclaration = true, ConformanceLevel = ConformanceLevel.Fragment };
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
            {
                m_transformation.Transform(xmlReader, null, xmlWriter);
            }
            return stringWriter.ToString();
        }
    }
}