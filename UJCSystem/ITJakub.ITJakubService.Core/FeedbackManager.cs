using ITJakub.DataEntities.Database.Repositories;

namespace ITJakub.ITJakubService.Core
{
    public class FeedbackManager
    {
        private readonly BookVersionRepository m_bookVersionRepository;

        public FeedbackManager(BookVersionRepository bookVersionRepository)
        {
            m_bookVersionRepository = bookVersionRepository;
        }

        public void AddHeadwordFeedback(string bookXmlId, string bookVersionXmlId, string entryXmlId, string name, string email, string content, bool publicationAgreement)
        {
            var headword = m_bookVersionRepository.GetFirstHeadwordInfo(bookXmlId, entryXmlId, bookVersionXmlId);
        }
    }
}