using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace ITJakub.Core.XMLOperations
{
    public abstract class XslTransformationBase
    {
        public XslTransformationBase()
        {
            InnerTransformation = LoadTransformation();
        }

        public abstract string SourceFile { get; }
        protected XslCompiledTransform InnerTransformation { get; private set; }


        public string Transform(string xml)
        {
            StringWriter writer = new StringWriter();
            XmlReader xmlReadB = new XmlTextReader(new StringReader(xml));

            InnerTransformation.Transform(xmlReadB, null, writer);

            return writer.ToString();
        }

        protected XslCompiledTransform LoadTransformation()
        {
            XslCompiledTransform result = new XslCompiledTransform(true);
            result.Load(SourceFile);
            return result;
        }
    }

    public class DictionaryXslt : XslTransformationBase
    {
        //public override string SourceFile { get { return "D:\\Pool\\ITJakub\\trunk\\UJCSystem\\ITJakub.Core\\XMLOperations\\XSLTransformations\\Dictionaries\\Dictionaries.xsl"; } }
        public override string SourceFile { get { return "assembly://ITJakub.Core/ITJakub.Core.XMLOperations.XSLTransformations.Dictionaries.Dictionaries.xsl"; } }
    }
}