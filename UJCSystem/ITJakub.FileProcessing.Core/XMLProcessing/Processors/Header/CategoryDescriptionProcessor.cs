using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class CategoryDescriptionProcessor : ConcreteInstanceListProcessorBase<CategoryData>
    {
        public CategoryDescriptionProcessor(XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "catDesc"; }
        }

        protected override void ProcessElement(BookData bookData, CategoryData category, XmlReader xmlReader)
        {
            category.Description = GetInnerContentAsString(xmlReader);
        }
    }
}