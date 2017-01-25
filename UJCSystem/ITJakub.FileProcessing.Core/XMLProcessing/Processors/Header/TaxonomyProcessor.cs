using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class TaxonomyProcessor : ConcreteInstanceProcessorBase<CategoryData>
    {
        public TaxonomyProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "taxonomy"; }
        }

        protected override IEnumerable<ConcreteInstanceProcessorBase<CategoryData>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<CategoryData>>
                {
                    Container.Resolve<CategoryProcessor>()
                };
            }
        }

        protected override void ProcessElement(BookData bookData, CategoryData parentCategory, XmlReader xmlReader)
        {
            var xmlId = xmlReader.GetAttribute("xml:id");
            if (xmlId != null && xmlId.Equals("output"))
            {
                base.ProcessElement(bookData, parentCategory, xmlReader);
            }
        }
    }
}