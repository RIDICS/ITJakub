using System.Xml;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class TitleProcessor : ListProcessorBase
    {
        protected override string NodeName
        {
            get { return "title"; }
        }

        public override void Process(BookVersion bookVersion, XmlTextReader xmlReader)
        {
            string title = GetInnerContentAsString(xmlReader.ReadSubtree());
        }
    }
}