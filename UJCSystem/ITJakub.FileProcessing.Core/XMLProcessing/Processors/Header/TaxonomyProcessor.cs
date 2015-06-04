using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class TaxonomyProcessor : ConcreteInstanceProcessorBase<Category>
    {
        public TaxonomyProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "taxonomy"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<Category>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<Category>>
                {
                    Container.Resolve<CategoryProcessor>()
                };
            }
        }

        protected override void ProcessElement(BookVersion bookVersion, Category parentCategory, XmlReader xmlReader)
        {
            var xmlId = xmlReader.GetAttribute("xml:id");
            if (xmlId != null && xmlId.Equals("output"))
            {
                base.ProcessElement(bookVersion, parentCategory, xmlReader);
            }
        }
    }
}