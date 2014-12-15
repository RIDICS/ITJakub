using ITJakub.DataEntities.Database.Entities;
using ITJakub.DataEntities.Database.Repositories;

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
            m_bookVersionRepository.Create(resourceDirector.GetSessionInfoValue<BookVersion>(SessionInfo.BookVersionEntity));
        }
    }
}