using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class CategoryDescriptionProcessor : ConcreteInstanceListProcessorBase<Category>
    {
        public CategoryDescriptionProcessor(XsltTransformationManager xsltTransformationManager, IKernel container) : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "catDesc"; }
        }

        protected override void ProcessElement(BookVersion bookVersion, Category category, XmlReader xmlReader)
        {
            category.Description = GetInnerContentAsString(xmlReader);
        }
    }
}