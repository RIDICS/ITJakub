using System.Collections.Generic;
using System.Xml;
using Castle.MicroKernel;
using ITJakub.FileProcessing.Core.Data;
using ITJakub.FileProcessing.Core.XMLProcessing.XSLT;

namespace ITJakub.FileProcessing.Core.XMLProcessing.Processors.Header
{
    public class CategoryProcessor : ConcreteInstanceProcessorBase<CategoryData>
    {
        public CategoryProcessor(XsltTransformationManager xsltTransformationManager, IKernel container)
            : base(xsltTransformationManager, container)
        {
        }

        protected override string NodeName
        {
            get { return "category"; }
        }


        protected override IEnumerable<ConcreteInstanceProcessorBase<CategoryData>> ConcreteSubProcessors
        {
            get
            {
                return new List<ConcreteInstanceProcessorBase<CategoryData>>
                {
                    Container.Resolve<CategoryProcessor>(),
                    Container.Resolve<CategoryDescriptionProcessor>(),
                };
            }
        }

        protected override void ProcessElement(BookData bookData, CategoryData parentCategory, XmlReader xmlReader)
        {
            string xmlId = xmlReader.GetAttribute("xml:id");
            
            var category = new CategoryData
            {
                XmlId = xmlId,
                SubCategories = new List<CategoryData>()
            };

            if (parentCategory != null)
            {
                parentCategory.SubCategories.Add(category);
            }
            else
            {
                bookData.AllCategoriesHierarchy.Add(category);
            }

            //TODO move creating AutoImportCategoryPermission to place, where Category is inserted to database
            //var newlyCreatedCategory = m_categoryRepository.FindByXmlId(category.XmlId);
            //var newAutoimportPermission = new AutoImportCategoryPermission
            //{
            //    Category = newlyCreatedCategory,
            //    AutoImportIsAllowed = true
            //};
            //m_permissionRepository.CreateSpecialPermission(newAutoimportPermission);
            

            base.ProcessElement(bookData, category, xmlReader);
        }
    }
}