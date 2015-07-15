using System.Collections.Generic;
using System.IO;
using Castle.Windsor;
using ITJakub.ITJakubService.Core;
using ITJakub.ITJakubService.Core.Resources;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Resources;
using ITJakub.Shared.Contracts.Searching;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Shared.Contracts.Searching.Results;

namespace ITJakub.ITJakubService.Services
{
    public class ItJakubServiceManager : IItJakubService
    {
        private readonly UserManager m_userManager;
        private readonly BookManager m_bookManager;
        private readonly AuthorManager m_authorManager;
        private readonly ResourceManager m_resourceManager;
        private readonly SearchManager m_searchManager;
        private readonly CardFileManager m_cardFileManager;        
        private readonly WindsorContainer m_container = Container.Current;

        public ItJakubServiceManager()
        {
            m_userManager = m_container.Resolve<UserManager>();
            m_bookManager = m_container.Resolve<BookManager>();
            m_authorManager = m_container.Resolve<AuthorManager>();
            m_resourceManager = m_container.Resolve<ResourceManager>();
            m_searchManager = m_container.Resolve<SearchManager>();
            m_cardFileManager = m_container.Resolve<CardFileManager>();
        }

        public IEnumerable<AuthorDetailContract> GetAllAuthors()
        {
            return m_authorManager.GetAllAuthors();
        }

        public int CreateAuthor(string name)
        {
            return m_authorManager.CreateAuthor(name);
        }

        public BookInfoContract GetBookInfo(string bookGuid)
        {
            return m_bookManager.GetBookInfo(bookGuid);
        }

        public BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType)
        {
            return m_searchManager.GetBooksWithCategoriesByBookType(bookType);
        }

        public string GetBookPageByXmlId(string bookGuid, string pageXmlId, OutputFormatEnumContract resultFormat, BookTypeEnumContract bookTypeContract)
        {
            return m_bookManager.GetBookPageByXmlId(bookGuid, pageXmlId, resultFormat, bookTypeContract);
        }

        public IEnumerable<BookPageContract> GetBookPageList(string bookGuid)
        {
            return m_bookManager.GetBookPagesList(bookGuid);
        }

        public IEnumerable<BookContentItemContract> GetBookContent(string bookGuid)
        {
            return m_bookManager.GetBookContent(bookGuid);
        }

        public void AddResource(UploadResourceContract resourceInfoSkeleton)
        {
            m_resourceManager.AddResource(resourceInfoSkeleton);
        }

        public bool ProcessSession(string resourceSessionId, string uploadMessage)
        {
            return m_resourceManager.ProcessSession(resourceSessionId, uploadMessage);
        }

        public IEnumerable<SearchResultContract> Search(string term)
        {
            return m_searchManager.Search(term);
        }

        public IEnumerable<SearchResultContract> SearchBooksWithBookType(string term, BookTypeEnumContract bookType)
        {
            return m_searchManager.SearchBooksWithBookType(term, bookType);
        }

        public IEnumerable<SearchResultContract> GetBooksByBookType(BookTypeEnumContract bookType)
        {
            return m_searchManager.GetBooksByBookType(bookType);
        }

        public Stream GetBookPageImage(BookPageImageContract bookPageImageContract)
        {
            return m_bookManager.GetBookPageImage(bookPageImageContract);
        }

        public IEnumerable<SearchResultContract> SearchByCriteria(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            return m_searchManager.SearchByCriteria(searchCriterias);
        }
        #region CardFile methods
        public IEnumerable<CardFileContract> GetCardFiles()
        {
            return m_cardFileManager.GetCardFiles();
        }
        
        public IEnumerable<BucketShortContract> GetBuckets(string cardFileId)
        {
            return m_cardFileManager.GetBuckets(cardFileId);
        }        
        public IEnumerable<BucketShortContract> GetBucketsWithHeadword(string cardFileId, string headword)
        {
            return m_cardFileManager.GetBucketsByHeadword(cardFileId, headword);
        }        

        public IEnumerable<CardContract> GetCards(string cardFileId, string bucketId)
        {
            return m_cardFileManager.GetCards(cardFileId, bucketId);
        }

        public IEnumerable<CardShortContract> GetCardsShort(string cardFileId, string bucketId)
        {
            return m_cardFileManager.GetCardsShort(cardFileId, bucketId);
        }

        public CardContract GetCard(string cardFileId, string bucketId, string cardId)
        {
            return m_cardFileManager.GetCard(cardFileId, bucketId, cardId);
        }

        public Stream GetImage(string cardFileId, string bucketId, string cardId, string imageId, ImageSizeEnum imageSize)
        {
            return m_cardFileManager.GetImage(cardFileId, bucketId, cardId, imageId, imageSize);
        }
        
        #endregion
        
        public IList<string> GetTypeaheadAuthors(string query)
        {
            return m_searchManager.GetTypeaheadAuthors(query);
        }

        public IList<string> GetTypeaheadTitles(string query)
        {
            return m_searchManager.GetTypeaheadTitles(query);
        }

        public IList<string> GetTypeaheadDictionaryHeadwords(string query)
        {
            return m_searchManager.GetTypeaheadDictionaryHeadwords(query);
        }

        public IList<string> GetTypeaheadAuthorsByBookType(string query, BookTypeEnumContract bookType)
        {
            return m_searchManager.GetTypeaheadAuthorsByBookType(query, bookType);
        }

        public IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnumContract bookType)
        {
            return m_searchManager.GetTypeaheadTitlesByBookType(query, bookType);
        }
        public int GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            return m_searchManager.GetHeadwordCount(selectedCategoryIds, selectedBookIds);
        }

        public IList<HeadwordContract> GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, int page, int pageSize)
        {
            return m_searchManager.GetHeadwordList(selectedCategoryIds, selectedBookIds, page, pageSize);
        }

        public int GetHeadwordPageNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query, int pageSize)
        {
            return m_searchManager.GetHeadwordPageNumber(selectedCategoryIds, selectedBookIds, query, pageSize);
        }

        public HeadwordSearchResultContract GetHeadwordSearchResultCount(string query)
        {
            return m_searchManager.GetHeadwordSearchResultCount(query);
        }

        public IList<HeadwordContract> SearchHeadword(string query, IList<string> dictionaryGuidList, int page, int pageSize)
        {
            return m_searchManager.SearchHeadword(query, dictionaryGuidList, page, pageSize);
        }

        public string GetDictionaryEntryByXmlId(string bookGuid, string xmlEntryId, OutputFormatEnumContract resultFormat)
        {
            return m_bookManager.GetDictionaryEntryByXmlId(bookGuid, xmlEntryId, resultFormat);
        }
    }
}