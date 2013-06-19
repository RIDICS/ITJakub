namespace ITJakub.Core.XMLOperations
{
    public abstract class XslTransformationBase
    {
        public XslTransformationBase()
        {
            
        }

        public abstract string SourceFile { get; }

        
    }

    public class DictionaryXslt : XslTransformationBase
    {
        public override string SourceFile { get { return "Dictionaries.xsl"; } }
    }
}