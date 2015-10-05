using System.Collections.Generic;
using System.IO;
using Castle.Windsor;
using ITJakub.ITJakubService.Core;
using ITJakub.ITJakubService.Core.Resources;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.News;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Shared.Contracts.Searching.Results;

namespace ITJakub.ITJakubService.Services
{
    public class ItJakubServiceManager : IItJakubService
    {
        private readonly WindsorContainer m_container = Container.Current;
        
        private readonly BookManager m_bookManager;
        private readonly AuthorManager m_authorManager;
        private readonly ResourceManager m_resourceManager;
        private readonly SearchManager m_searchManager;
        private readonly CardFileManager m_cardFileManager;        
        private readonly FeedbackManager m_feedbackManager;
        private readonly PermissionManager m_permissionManager;
        private readonly UserManager m_userManager;    
        private readonly NewsManager m_newsManager;

        public ItJakubServiceManager()
        {            
            m_bookManager = m_container.Resolve<BookManager>();
            m_authorManager = m_container.Resolve<AuthorManager>();
            m_resourceManager = m_container.Resolve<ResourceManager>();
            m_searchManager = m_container.Resolve<SearchManager>();
            m_feedbackManager = m_container.Resolve<FeedbackManager>();
            m_cardFileManager = m_container.Resolve<CardFileManager>();
            m_permissionManager = m_container.Resolve<PermissionManager>();
            m_userManager = m_container.Resolve<UserManager>();
            m_newsManager = m_container.Resolve<NewsManager>();
         
        }

        public IEnumerable<AuthorDetailContract> GetAllAuthors()
        {
            return m_authorManager.GetAllAuthors();
        }

        public BookInfoWithPagesContract GetBookInfoWithPages(string bookGuid)
        {
            return m_bookManager.GetBookInfoWithPages(bookGuid);
        }

        public BookTypeSearchResultContract GetBooksWithCategoriesByBookType(BookTypeEnumContract bookType)
        {
            return m_searchManager.GetBooksWithCategoriesByBookType(bookType);
        }

        public IList<CategoryContract> GetRootCategories()
        {
            return m_searchManager.GetRootCategories();
        }

        public CategoryContentContract GetCategoryContentForGroup(int groupId, int categoryId)
        {
            return m_permissionManager.GetCategoryContentForGroup(groupId, categoryId);
        }

        public CategoryContentContract GetAllCategoryContent(int categoryId)
        {
            return m_permissionManager.GetAllCategoryContent(categoryId);
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

        public bool ProcessSession(string resourceSessionId, string uploadMessage)
        {
            return m_resourceManager.ProcessSession(resourceSessionId, uploadMessage);
        }

        public IEnumerable<SearchResultContract> Search(string term)
        {
            return m_searchManager.Search(term);
        }
        
        public Stream GetBookPageImage(string bookXmlId, int position)
        {
            return m_bookManager.GetBookPageImage(bookXmlId, position);
        }

        public Stream GetHeadwordImage(string bookXmlId, string bookVersionXmlId, string fileName)
        {
            return m_bookManager.GetHeadwordImage(bookXmlId, bookVersionXmlId, fileName);
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
            
        public IList<UserContract> GetTypeaheadUsers(string query)
        {
            return m_searchManager.GetTypeaheadUsers(query);
        }

        public IList<GroupContract> GetTypeaheadGroups(string query)
        {
            return m_searchManager.GetTypeaheadGroups(query);
        }

        public IList<string> GetTypeaheadTitles(string query)
        {
            return m_searchManager.GetTypeaheadTitles(query);
        }

        public IList<string> GetTypeaheadDictionaryHeadwords(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            return m_searchManager.GetTypeaheadDictionaryHeadwords(selectedCategoryIds, selectedBookIds, query);
        }

        public IList<string> GetTypeaheadAuthorsByBookType(string query, BookTypeEnumContract bookType)
        {
            return m_searchManager.GetTypeaheadAuthorsByBookType(query, bookType);
        }

        public IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnumContract bookType, IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            return m_searchManager.GetTypeaheadTitlesByBookType(query, bookType, selectedCategoryIds, selectedBookIds);
        }
        public IList<string> GetTypeaheadTermsByBookType(string query, BookTypeEnumContract bookType, IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            return m_searchManager.GetTypeaheadTermsByBookType(query, bookType, selectedCategoryIds, selectedBookIds);
        }

        public int GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds)
        {
            return m_searchManager.GetHeadwordCount(selectedCategoryIds, selectedBookIds);
        }

        public HeadwordListContract GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, int start, int count)
        {
            return m_searchManager.GetHeadwordList(selectedCategoryIds, selectedBookIds, start, count);
        }

        public long GetHeadwordRowNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query)
        {
            return m_searchManager.GetHeadwordRowNumber(selectedCategoryIds, selectedBookIds, query);
        }

        public long GetHeadwordRowNumberById(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string headwordBookId, string headwordEntryXmlId)
        {
            return m_searchManager.GetHeadwordRowNumberById(selectedCategoryIds, selectedBookIds, headwordBookId,
                headwordEntryXmlId);
        }

        public HeadwordListContract SearchHeadwordByCriteria(IEnumerable<SearchCriteriaContract> searchCriterias, DictionarySearchTarget searchTarget)
        {
            return m_searchManager.SearchHeadwordByCriteria(searchCriterias, searchTarget);
        }

        public int SearchHeadwordByCriteriaResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias, DictionarySearchTarget searchTarget)
        {
            return m_searchManager.SearchHeadwordByCriteriaResultsCount(searchCriterias, searchTarget);
        }

        public int SearchCriteriaResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            return m_searchManager.SearchCriteriaResultsCount(searchCriterias);
        }

        public string GetDictionaryEntryByXmlId(string bookGuid, string xmlEntryId, OutputFormatEnumContract resultFormat, BookTypeEnumContract bookType)
        {
            return m_bookManager.GetDictionaryEntryByXmlId(bookGuid, xmlEntryId, resultFormat, bookType);
        }

        public string GetDictionaryEntryFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookGuid, string xmlEntryId, OutputFormatEnumContract resultFormat, BookTypeEnumContract bookType)
        {
            return m_searchManager.GetDictionaryEntryFromSearch(searchCriterias, bookGuid, xmlEntryId, resultFormat, bookType);
        }

        public PageListContract GetSearchEditionsPageList(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            return m_searchManager.GetSearchEditionsPageList(searchCriterias);
        }

        public CorpusSearchResultContractList GetCorpusSearchResults(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            return m_searchManager.GetCorpusSearchResults(searchCriterias);
        }

        public int GetCorpusSearchResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            return m_searchManager.GetCorpusSearchResultsCount(searchCriterias);
        }

        public string GetEditionPageFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookXmlId,
             string pageXmlId, OutputFormatEnumContract resultFormat)
        {
            return m_searchManager.GetEditionPageFromSearch(searchCriterias, bookXmlId, pageXmlId, resultFormat);
        }
        
        public void CreateAnonymousFeedback(string feedback, string name, string email, FeedbackCategoryEnumContract feedbackCategory)
        {
            m_feedbackManager.CreateAnonymousFeedback(feedback, name, email, feedbackCategory);
        }
        
        public void CreateAnonymousFeedbackForHeadword(string feedback, string bookXmlId, string versionXmlId, string entryXmlId,
            string name, string email)
        {
            m_feedbackManager.CreateAnonymousFeedbackForHeadword(feedback, bookXmlId, versionXmlId, entryXmlId, name, email);
        }

        public List<FeedbackContract> GetFeedbacks(FeedbackCriteriaContract feedbackSearchCriteria)
        {
            return m_feedbackManager.GetFeedbacks(feedbackSearchCriteria);
        }

        public int GetFeedbacksCount(FeedbackCriteriaContract feedbackSearchCriteria)
        {
            return m_feedbackManager.GetFeedbacksCount(feedbackSearchCriteria);
        }

        public void DeleteFeedback(long feedbackId)
        {
            m_feedbackManager.DeleteFeedback(feedbackId);
        }

        public AudioBookSearchResultContractList GetAudioBooksSearchResults(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            return m_searchManager.GetAudioBookSearchResults(searchCriterias);
        }

        public int GetAudioBooksSearchResultsCount(IEnumerable<SearchCriteriaContract> searchCriterias)
        {
            return m_searchManager.GetAudioBooksSearchResultsCount(searchCriterias);
        }

        public IList<TermContract> GetTermsOnPage(string bookXmlId, string pageXmlId)
        {
            return m_bookManager.GetTermsOnPage(bookXmlId, pageXmlId);
        }

        public IList<GroupContract> GetGroupsByUser(int userId)
        {
            return m_permissionManager.GetGroupsByUser(userId);
        }

        public IList<UserContract> GetUsersByGroup(int groupId)
        {
            return m_permissionManager.GetUsersByGroup(groupId);
        }

        public void AddUserToGroup(int userId, int groupId)
        {
            m_permissionManager.AddUserToGroup(userId, groupId);
        }

        public void RemoveUserFromGroup(int userId, int groupId)
        {
            m_permissionManager.RemoveUserFromGroup(userId, groupId);
        }

        public GroupContract CreateGroup(string name, string description)
        {
            return m_permissionManager.CreateGroup(name, description);
        }

        public UserDetailContract GetUserDetail(int userId)
        {
            return m_userManager.GetUserDetail(userId);
        }

        public GroupDetailContract GetGroupDetail(int groupId)
        {
            return m_permissionManager.GetGroupDetail(groupId);
        }

        public void DeleteGroup(int groupId)
        {
            m_permissionManager.DeleteGroup(groupId);
        }

        public void AddBooksAndCategoriesToGroup(int groupId, IList<long> bookIds, IList<int> categoryIds)
        {
            m_permissionManager.AddBooksAndCategoriesToGroup(groupId, bookIds, categoryIds);
        }

        public void RemoveBooksAndCategoriesFromGroup(int groupId, IList<long> bookIds, IList<int> categoryIds)
        {
            m_permissionManager.RemoveBooksAndCategoriesFromGroup(groupId, bookIds, categoryIds);
        }

        public IList<SpecialPermissionContract> GetSpecialPermissions()
        {
            return m_permissionManager.GetSpecialPermissions();
        }
        public IList<SpecialPermissionContract> GetSpecialPermissionsForGroup(int groupId)
        {
            return m_permissionManager.GetSpecialPermissionsForGroup(groupId);
        }

        public IList<SpecialPermissionContract> GetSpecialPermissionsForUser(int userId)
        {
            return m_permissionManager.GetSpecialPermissionsForUser(userId);
        }

        public IList<SpecialPermissionContract> GetSpecialPermissionsForUserByType(int userId, SpecialPermissionCategorizationEnumContract permissionType)
        {
            return m_permissionManager.GetSpecialPermissionsForUserByType(userId, permissionType);
        }

        public void AddSpecialPermissionsToGroup(int groupId, IList<int> specialPermissionIds)
        {
            m_permissionManager.AddSpecialPermissionsToGroup(groupId, specialPermissionIds);
        }

        public void RemoveSpecialPermissionsFromGroup(int groupId, IList<int> specialPermissionIds)
        {
            m_permissionManager.RemoveSpecialPermissionsFromGroup(groupId, specialPermissionIds);
        }

        public List<NewsSyndicationItemContract> GetWebNewsSyndicationItems(int start, int count)
        {
            return m_newsManager.GetWebNewsSyndicationItems(start, count);
        }

        public int GetWebNewsSyndicationItemCount()
        {
            return m_newsManager.GetWebNewsSyndicationItemCount();
        }

    }
}