using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Resources;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Shared.Contracts.Searching.Results;
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

        public BookInfoWithPagesContract GetBookInfoWithPages(string bookId)
        {
            try
            {
                return Channel.GetBookInfoWithPages(bookId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookInfoWithPages failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetBookInfoWithPages timeouted with: {0}", ex);
                throw;
            }
        }

        public string GetBookPageByXmlId(string documentId, string pageXmlId, OutputFormatEnumContract resultFormat,
            BookTypeEnumContract bookTypeContract)
        {
            try
            {
                return Channel.GetBookPageByXmlId(documentId, pageXmlId, resultFormat, bookTypeContract);
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

        public Stream GetBookPageImage(string bookXmlId, int position)
        {
            try
            {
                return Channel.GetBookPageImage(bookXmlId, position);
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

        public Stream GetHeadwordImage(string bookXmlId, string bookVersionXmlId, string fileName)
        {
            try
            {
                return Channel.GetHeadwordImage(bookXmlId, bookVersionXmlId, fileName);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordImage failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordImage failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordImage timeouted with: {0}", ex);
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
                return Channel.GetCard(cardFileId, bucketId, cardId);
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

        public Stream GetImage(string cardFileId, string bucketId, string cardId, string imageId,
            ImageSizeEnum imageSize)
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

        public IList<string> GetTypeaheadDictionaryHeadwords(IList<int> selectedCategoryIds, IList<long> selectedBookIds,
            string query)
        {
            try
            {
                return Channel.GetTypeaheadDictionaryHeadwords(selectedCategoryIds, selectedBookIds, query);
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

        public IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnumContract bookType,
            IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            try
            {
                return Channel.GetTypeaheadTitlesByBookType(query, bookType, selectedCategoryIds, selectedBookIds);
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

        public int GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            try
            {
                return Channel.GetHeadwordCount(selectedCategoryIds, selectedBookIds);
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

        public HeadwordListContract GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds,
            int start, int count)
        {
            try
            {
                return Channel.GetHeadwordList(selectedCategoryIds, selectedBookIds, start, count);
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

        public long GetHeadwordRowNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            try
            {
                return Channel.GetHeadwordRowNumber(selectedCategoryIds, selectedBookIds, query);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordRowNumber failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordRowNumber failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordRowNumber timeouted with: {0}", ex);
                throw;
            }
        }

        public long GetHeadwordRowNumberById(IList<int> selectedCategoryIds, IList<long> selectedBookIds,
            string headwordBookId, string headwordEntryXmlId)
        {
            try
            {
                return Channel.GetHeadwordRowNumberById(selectedCategoryIds, selectedBookIds, headwordBookId,
                    headwordEntryXmlId);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordRowNumberById failed with: {0}", ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordRowNumberById failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetHeadwordRowNumberById timeouted with: {0}", ex);
                throw;
            }
        }

        public HeadwordListContract SearchHeadwordByCriteria(IEnumerable<SearchCriteriaContract> searchCriterias,
            DictionarySearchTarget searchTarget)
        {
            try
            {
                return Channel.SearchHeadwordByCriteria(searchCriterias, searchTarget);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchHeadwordByCriteria failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchHeadwordByCriteria failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchHeadwordByCriteria timeouted with: {0}", ex);
                throw;
            }
        }

        public int SearchHeadwordByCriteriaResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias,
            DictionarySearchTarget searchTarget)
        {
            try
            {
                return Channel.SearchHeadwordByCriteriaResultsCount(searchCriterias, searchTarget);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchHeadwordByCriteriaResultsCount failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchHeadwordByCriteriaResultsCount failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchHeadwordByCriteriaResultsCount timeouted with: {0}", ex);
                throw;
            }
        }

        public int SearchCriteriaResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            try
            {
                return Channel.SearchCriteriaResultsCount(searchCriterias);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchCriteriaResultsCount failed with: {0}", ex);
                throw;
            }

            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchCriteriaResultsCount failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("SearchCriteriaResultsCount timeouted with: {0}", ex);
                throw;
            }
        }

        public string GetDictionaryEntryByXmlId(string bookGuid, string xmlEntryId,
            OutputFormatEnumContract resultFormat)
        {
            try
            {
                return Channel.GetDictionaryEntryByXmlId(bookGuid, xmlEntryId, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetDictionaryEntryByXmlId failed with: {0}", ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetDictionaryEntryByXmlId failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetDictionaryEntryByXmlId timeouted with: {0}", ex);
                throw;
            }
        }

        public string GetDictionaryEntryFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookGuid,
            string xmlEntryId,
            OutputFormatEnumContract resultFormat)
        {
            try
            {
                return Channel.GetDictionaryEntryFromSearch(searchCriterias, bookGuid, xmlEntryId, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetDictionaryEntryFromSearch failed with: {0}", ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetDictionaryEntryFromSearch failed with: {0}", ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("GetDictionaryEntryFromSearch timeouted with: {0}", ex);
                throw;
            }
        }

        public PageListContract GetSearchEditionsPageList(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            try
            {
                return Channel.GetSearchEditionsPageList(searchCriterias);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public string GetEditionPageFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookXmlId,
            string pageXmlId,
            OutputFormatEnumContract resultFormat)
        {
            try
            {
                return Channel.GetEditionPageFromSearch(searchCriterias, bookXmlId, pageXmlId, resultFormat);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public void CreateAnonymousFeedback(string feedback, string name, string email)
        {
            try
            {
                Channel.CreateAnonymousFeedback(feedback, name, email);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public void CreateAnonymousFeedbackForHeadword(string feedback, string bookXmlId, string versionXmlId, string entryXmlId, string name, string email)
        {
            try
            {
                Channel.CreateAnonymousFeedbackForHeadword(feedback, bookXmlId, versionXmlId, entryXmlId, name, email);
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        public List<FeedbackContract> GetAllFeedback()
        {
            try
            {
               return Channel.GetAllFeedback();
            }
            catch (CommunicationException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (TimeoutException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
            catch (ObjectDisposedException ex)
            {
                if (m_log.IsErrorEnabled)
                    m_log.ErrorFormat("{0} failed with: {1}", GetCurrentMethod(), ex);
                throw;
            }
        }

        private string GetCurrentMethod([CallerMemberName] string methodName = null)
        {
            return methodName;
        }      
    }
}