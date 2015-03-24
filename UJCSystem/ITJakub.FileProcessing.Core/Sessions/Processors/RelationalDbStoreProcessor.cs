using System.Collections.Generic;
using System.Linq;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts.Resources;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class RelationalDbStoreProcessor : IResourceProcessor
    {
        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly CategoryRepository m_categoryRepository;

        public RelationalDbStoreProcessor(BookVersionRepository bookVersionRepository, CategoryRepository categoryRepository)
        {
            m_bookVersionRepository = bookVersionRepository;
            m_categoryRepository = categoryRepository;
        }

        public void Process(ResourceSessionDirector resourceDirector)
        {
            var bookEntity = resourceDirector.GetSessionInfoValue<BookVersion>(SessionInfo.BookVersionEntity);
            var trans = resourceDirector.Resources.Where(x => x.ResourceType == ResourceType.Transformation);
            if (bookEntity.Transformations == null)
            {
                bookEntity.Transformations = new List<Transformation>();
            }
            
            foreach (var transResource in trans)
            {
                bookEntity.Transformations.Add(new Transformation
                {
                    IsDefaultForBookType = false,
                    Description = string.Empty,
                    Name = transResource.FileName,
                    OutputFormat = OutputFormat.Html,
                    ResourceLevel = ResourceLevel.Version
                });
            }

            m_bookVersionRepository.Create(bookEntity);

            var category = bookEntity.Book.Category;
            while (category.ParentCategory != null)
            {
                category = category.ParentCategory;
            }
            m_categoryRepository.SetBookTypeToRootCategoryIfNotKnown(bookEntity.Book.BookType, category);
        }
    }
}