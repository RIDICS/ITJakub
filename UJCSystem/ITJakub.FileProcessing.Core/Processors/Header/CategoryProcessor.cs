using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XSLT;

namespace ITJakub.FileProcessing.Core.Processors.Header
{
    public class CategoryProcessor : ConcreteInstanceProcessorBase<Category>
    {
        private readonly CategoryRepository m_categoryRepository;

        public CategoryProcessor(CategoryRepository categoryRepository,
            XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
            m_categoryRepository = categoryRepository;
        }

        protected override string NodeName
        {
            get { return "category"; }
        }


        protected override IEnumerable<ConcreteInstanceProcessorBase<Category>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<Category>>
                {
                    Container.Resolve<CategoryProcessor>(),
                    Container.Resolve<CategoryDescriptionProcessor>(),
                };
            }
        }

        protected override void ProcessElement(Category parentCategory, XmlReader xmlReader)
        {
            string xmlId = xmlReader.GetAttribute("xml:id");
            var category = new Category
            {
                XmlId = xmlId,
                ParentCategory = parentCategory,
            };
            m_categoryRepository.SaveOrUpdate(category);
            base.ProcessElement(category, xmlReader);
            m_categoryRepository.SaveOrUpdate(category);
        }
    }
}