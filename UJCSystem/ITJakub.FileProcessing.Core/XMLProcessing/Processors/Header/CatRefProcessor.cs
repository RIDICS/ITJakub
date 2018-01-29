using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class CatRefProcessor : ListProcessorBase
    {
        public CatRefProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "catRef"; }
        }

        protected override void ProcessAttributes(BookData bookData, XmlReader xmlReader)
        {
            var targetAttribute = xmlReader.GetAttribute("target");
            var targets = targetAttribute.Split(' ');
            
            foreach (var target in targets)
            {
                if (!target.StartsWith("#")) continue;
                var categoryXmlId = target.Remove(0, 1);
                
                bookData.CategoryXmlIds.Add(categoryXmlId);
            }
        }
    }
}