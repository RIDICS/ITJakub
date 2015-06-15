using System.Collections.Generic;
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

        public CatRefProcessor(CategoryRepository categoryRepository,
            XsltTransformationManager xsltTransformationManager, IKernel container)
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
            var targetAttribute = xmlReader.GetAttribute("target");
            var targets = targetAttribute.Split(' ');
            var foundFirstCategory = false;
            foreach (var target in targets)
            {
                if (!target.StartsWith("#")) continue;
                var categoryXmlId = target.Remove(0, 1);
                if (bookVersion.Book.LastVersion.Categories == null)
                {
                    bookVersion.Book.LastVersion.Categories = new List<Category>();
                }
                var category = m_categoryRepository.FindByXmlId(categoryXmlId);
                if (category == null) continue;
                bookVersion.Book.LastVersion.Categories.Add(category);
                if (!foundFirstCategory)
                {
                    bookVersion.Book.LastVersion.DefaultBookType = m_categoryRepository.FindBookTypeByCategory(category);
                    foundFirstCategory = true;
                }
            }
        }
    }
}