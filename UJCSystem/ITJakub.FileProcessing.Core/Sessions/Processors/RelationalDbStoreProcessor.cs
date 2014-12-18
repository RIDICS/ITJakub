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

        public RelationalDbStoreProcessor(BookVersionRepository bookVersionRepository)
        {
            m_bookVersionRepository = bookVersionRepository;
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
                    OutputFormat = OutputFormat.Html
                });
            }

            m_bookVersionRepository.Create(bookEntity);
        }
    }
}