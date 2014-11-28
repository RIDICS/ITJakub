using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class CatRefProcessor : ListProcessorBase
    {
        private readonly CategoryRepository m_categoryRepository;

        public CatRefProcessor(CategoryRepository categoryRepository, XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
            m_categoryRepository = categoryRepository;
        }

        protected override string NodeName
        {
            get { return "catRef"; }
        }

        protected override void ProcessAttributes(BookVersion bookVersion, XmlReader xmlReader)
        {
            var target = xmlReader.GetAttribute("target");
            if (target.StartsWith("#"))
            {
                target = target.Remove(0, 1);
            }
            bookVersion.Book.Category = m_categoryRepository.FindByXmlId(target);
        }
    }
}