using System;
using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class CategoryProcessor : ConcreteInstanceProcessorBase<Category>
    {
        private readonly CategoryRepository m_categoryRepository;
        private readonly PermissionRepository m_permissionRepository;

        public CategoryProcessor(CategoryRepository categoryRepository, PermissionRepository permissionRepository,
            XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
            m_categoryRepository = categoryRepository;
            m_permissionRepository = permissionRepository;
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

        protected override void ProcessElement(BookVersion bookVersion, Category parentCategory, XmlReader xmlReader)
        {
            string xmlId = xmlReader.GetAttribute("xml:id");
            var category = m_categoryRepository.FindByXmlId(xmlId);
            if (category == null) { 
                category = new Category
                {
                    XmlId = xmlId,
                    ParentCategory = parentCategory
                };

                if (parentCategory != null)
                {
                    category.BookType = parentCategory.BookType;
                    category.Path = string.Format("{0}{1}/", parentCategory.Path, xmlId);
                }
                else
                {
                    category.Path = string.Format("/{0}/", xmlId);
                }

                m_categoryRepository.SaveOrUpdate(category);
                var newlyCreatedCategory = m_categoryRepository.FindByXmlId(category.XmlId);
                var newAutoimportPermission = new AutoImportCategoryPermission
                {
                    Category = newlyCreatedCategory,
                    AutoImportIsAllowed = true
                };
                m_permissionRepository.CreateSpecialPermission(newAutoimportPermission);
            }

            base.ProcessElement(bookVersion, category, xmlReader);
            m_categoryRepository.SaveOrUpdate(category);
        }
    }
}