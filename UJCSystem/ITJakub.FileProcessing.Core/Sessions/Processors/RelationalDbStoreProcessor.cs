using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Entities.Enums;
using ITJakub.DataEntities.Database.Exceptions;
using ITJakub.DataEntities.Database.Repositories;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.FileProcessing.Core.Sessions.Processors
{
    public class RelationalDbStoreProcessor : IResourceProcessor
    {
        private readonly BookVersionRepository m_bookVersionRepository;
        private readonly CategoryRepository m_categoryRepository;

        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

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

            try
            {
                m_categoryRepository.SetBookTypeToRootCategoryIfNotKnown(bookEntity.Book.BookType, category);
            }
            catch (BookTypeIsAlreadyAssociatedWithAnotherCategoryException ex)
            {
                m_log.Error(ex.Message);
            }
            
        }
    }
}