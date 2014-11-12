using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
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

        protected override void ProcessElement(Category category, XmlReader xmlReader)
        {
            category.Description = GetInnerContentAsString(xmlReader);
        }
    }
}