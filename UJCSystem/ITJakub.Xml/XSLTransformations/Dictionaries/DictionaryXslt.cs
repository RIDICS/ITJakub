using ITJakub.Xml.XMLOperations;

namespace ITJakub.Xml.XSLTransformations.Dictionaries
{
    public class DictionaryXslt : XslTransformationBase
    {
        public override string SourceFile
        {
            get { return "Dictionaries.xsl"; }
        }
    }

    public class UniversalXslt : XslTransformationBase
    {
        public override string SourceFile
        {
            get { return "TextsAndDictionaries.xsl"; }
        }
    }
}