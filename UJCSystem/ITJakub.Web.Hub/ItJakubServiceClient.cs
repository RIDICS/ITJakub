using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;
using log4net;

namespace ITJakub.Web.Hub
{
    public class ItJakubServiceClient : ClientBase<IItJakubService>, IItJakubService
    {
        private static readonly ILog m_log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ItJakubServiceClient()
        {
            if (m_log.IsDebugEnabled)
                m_log.DebugFormat("MainServiceClient created.");
        }

        public IEnumerable<AuthorDetailContract> GetAllAuthors()
        {
            try
            {
                return Channel.GetAllAuthors();
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetAllAuthors failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetAllAuthors timeouted with: {0}", ex);
                throw;
            }
        }

        public BookInfoContract GetBookInfo(string bookId)
        {
            try
            {
                return Channel.GetBookInfo(bookId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookInfo failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookInfo timeouted with: {0}", ex);
                throw;
            }
        }

        public string GetBookPageByName(string documentId, string pageName, OutputFormatEnumContract resultFormat)
        {
            try
            {
                return Channel.GetBookPageByName(documentId, pageName, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByName failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByName timeouted with: {0}", ex);
                throw;
            }
        }
        public string GetBookPageByXmlId(string documentId, string pageXmlId, OutputFormatEnumContract resultFormat)
        {
            try
            {
                return Channel.GetBookPageByXmlId(documentId, pageXmlId, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByXmlId failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByXmlId timeouted with: {0}", ex);
                throw;
            }
        }

        public string GetBookPagesByName(string documentId, string startPageName, string endPageName, OutputFormatEnumContract resultFormat)
        {
            try
            {
                return Channel.GetBookPagesByName(documentId, startPageName, endPageName, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPagesByName failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPagesByName timeouted with: {0}", ex);
                throw;
            }
        }

        public string GetBookPageByPosition(string documentId, int position, OutputFormatEnumContract resultFormat)
        {
            try
            {
                return Channel.GetBookPageByPosition(documentId, position, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByPosition failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageByPosition timeouted with: {0}", ex);
                throw;
            }
        }

        public IEnumerable<BookPageContract> GetBookPageList(string documentId)
        {
            try
            {
                return Channel.GetBookPageList(documentId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageList failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageList timeouted with: {0}", ex);
                throw;
            }
        }

        public IEnumerable<BookContentItemContract> GetBookContent(string documentId)
        {
            try
            {
                return Channel.GetBookContent(documentId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookContent failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookContent timeouted with: {0}", ex);
                throw;
            }
        }

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            try
            {
                Channel.AddResource(resourceInfoSkeleton);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AddResource failed with: {0}", ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AddResource failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("AddResource timeouted with: {0}", ex);
                throw;
            }
        }

        public bool ProcessSession(string sessionId, string uploadMessage)
        {
            try
            {
                return Channel.ProcessSession(sessionId, uploadMessage);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("ProcessSession failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("ProcessSession failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("ProcessSession timeouted with: {0}", ex);
                throw;
            }
        }

        public IEnumerable<SearchResultContract> Search(string term)
        {
            try
            {
                return Channel.Search(term);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("Search timeouted with: {0}", ex);
                throw;
            }
        }

        public IEnumerable<SearchResultContract> SearchBooksWithBookType(string term, BookTypeEnumContract bookType)
        {
            try
            {
                return Channel.SearchBooksWithBookType(term, bookType);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchBooksWithBookType failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchBooksWithBookType failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchBooksWithBookType timeouted with: {0}", ex);
                throw;
            }
        }

        public IEnumerable<SearchResultContract> GetBooksByBookType(BookTypeEnumContract bookType)
        {
            try
            {
                return Channel.GetBooksByBookType(bookType);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksByBookType failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksByBookType failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksByBookType timeouted with: {0}", ex);
                throw;
            }
        }

        public BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType)
        {
            try
            {
                return Channel.GetBooksWithCategoriesByBookType(bookType);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksWithCategoriesByBookType failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksWithCategoriesByBookType failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBooksWithCategoriesByBookType timeouted with: {0}", ex);
                throw;
            }
        }

        public Stream GetBookPageImage(BookPageImageContract bookPageImageContract)
        {
            try
            {
                return Channel.GetBookPageImage(bookPageImageContract);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageImage failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageImage failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookPageImage timeouted with: {0}", ex);
                throw;
            }
        }

        public IEnumerable<SearchResultContract> SearchByCriteria(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            try
            {
                return Channel.SearchByCriteria(searchCriterias);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchByCriteria failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchByCriteria failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchByCriteria timeouted with: {0}", ex);
                throw;
            }
        }

        public IEnumerable<CardFileContract> GetCardFiles()
        {
            try
            {
                return Channel.GetCardFiles();
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCardFiles failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCardFiles failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCardFiles timeouted with: {0}", ex);
                throw;
            }
        }
        
        public IEnumerable<BucketShortContract> GetBuckets(string cardFileId)
        {
            try
            {
                return Channel.GetBuckets(cardFileId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBuckets failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBuckets failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBuckets timeouted with: {0}", ex);
                throw;
            }
        }        
        public IEnumerable<BucketShortContract> GetBucketsWithHeadword(string cardFileId, string headword)
        {
            try
            {
                return Channel.GetBucketsWithHeadword(cardFileId, headword);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBucketsWithHeadword failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBucketsWithHeadword failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBucketsWithHeadword timeouted with: {0}", ex);
                throw;
            }
        }

        public IEnumerable<CardContract> GetCards(string cardFileId, string bucketId)
        {
            try
            {
                return Channel.GetCards(cardFileId, bucketId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCards failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCards failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCards timeouted with: {0}", ex);
                throw;
            }
        }
        public IEnumerable<CardShortContract> GetCardsShort(string cardFileId, string bucketId)
        {
            try
            {
                return Channel.GetCardsShort(cardFileId, bucketId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCardsShort failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCardsShort failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCardsShort timeouted with: {0}", ex);
                throw;
            }
        }

        public CardContract GetCard(string cardFileId, string bucketId, string cardId)
        {
            try
            {
                return Channel.GetCard(cardFileId, bucketId,cardId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCard failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCard failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetCard timeouted with: {0}", ex);
                throw;
            }
        }

        public Stream GetImage(string cardFileId, string bucketId, string cardId, string imageId, ImageSizeEnum imageSize)
        {
            try
            {
                return Channel.GetImage(cardFileId, bucketId, cardId, imageId, imageSize);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetImage failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetImage failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetImage timeouted with: {0}", ex);
                throw;
            }
        }


        public IList<string> GetTypeaheadAuthors(string query)
        {
            try
            {
                return Channel.GetTypeaheadAuthors(query);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadAuthors failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadAuthors failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadAuthors timeouted with: {0}", ex);
                throw;
            }
        }

        public IList<string> GetTypeaheadTitles(string query)
        {
            try
            {
                return Channel.GetTypeaheadTitles(query);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadTitles failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadTitles failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadTitles timeouted with: {0}", ex);
                throw;
            }
        }

        public IList<string> GetTypeaheadDictionaryHeadwords(string query)
        {
            try
            {
                return Channel.GetTypeaheadDictionaryHeadwords(query);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadDictionaryHeadwords failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadDictionaryHeadwords failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadDictionaryHeadwords timeouted with: {0}", ex);
                throw;
            }
        }
        
        public IList<string> GetTypeaheadAuthorsByBookType(string query, BookTypeEnumContract bookType)
        {
            try
            {
                return Channel.GetTypeaheadAuthorsByBookType(query, bookType);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadAuthorsByBookType failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadAuthorsByBookType failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadAuthorsByBookType timeouted with: {0}", ex);
                throw;
            }
        }

        public IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnumContract bookType)
        {
            try
            {
                return Channel.GetTypeaheadTitlesByBookType(query, bookType);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadTitlesByBookType failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadTitlesByBookType failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetTypeaheadTitlesByBookType timeouted with: {0}", ex);
                throw;
            }
        }

        public int GetHeadwordCount(IList<long> selectedBookIds)
        {
            try
            {
                return Channel.GetHeadwordCount(selectedBookIds);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordCount failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordCount failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordCount timeouted with: {0}", ex);
                throw;
            }
        }

        public IList<HeadwordContract> GetHeadwordList(IList<long> selectedBookIds, int page, int pageSize)
        {
            try
            {
                return Channel.GetHeadwordList(selectedBookIds, page, pageSize);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordList failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordList failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordList timeouted with: {0}", ex);
                throw;
            }
        }

        public HeadwordSearchResultContract GetHeadwordSearchResultCount(string query)
        {
            try
            {
                return Channel.GetHeadwordSearchResultCount(query);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordSearchResultCount failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordSearchResultCount failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordSearchResultCount timeouted with: {0}", ex);
                throw;
            }
        }
        public IList<HeadwordContract> SearchHeadword(string query, IList<string> dictionaryGuidList, int page, int pageSize)
        {
            try
            {
                return Channel.SearchHeadword(query, dictionaryGuidList, page, pageSize);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchHeadword failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchHeadword failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchHeadword timeouted with: {0}", ex);
                throw;
            }
        }
    }
}