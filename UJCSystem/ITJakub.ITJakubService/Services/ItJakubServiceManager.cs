using System.Collections.Generic;
using System.IO;
using Castle.Windsor;
using ITJakub.ITJakubService.Core;
using ITJakub.ITJakubService.Core.Resources;
using ITJakub.ITJakubService.DataContracts;
using ITJakub.ITJakubService.DataContracts.Contracts;
using ITJakub.ITJakubService.DataContracts.Contracts.Favorite;
using ITJakub.Shared.Contracts;
using ITJakub.Shared.Contracts.Favorites;
using ITJakub.Shared.Contracts.News;
using ITJakub.Shared.Contracts.Notes;
using ITJakub.Shared.Contracts.Searching.Criteria;
using ITJakub.Shared.Contracts.Searching.Results;

namespace ITJakub.ITJakubService.Services
{
    public class ItJakubServiceManager : IItJakubService
    {
        private readonly AuthorManager m_authorManager;

        private readonly BookManager m_bookManager;
        private readonly CardFileManager m_cardFileManager;
        private readonly WindsorContainer m_container = Container.Current;
        private readonly FavoriteManager m_favoriteManager;
        private readonly FeedbackManager m_feedbackManager;
        private readonly NewsManager m_newsManager;
        private readonly PermissionManager m_permissionManager;
        private readonly ResourceManager m_resourceManager;
        private readonly SearchManager m_searchManager;
        private readonly UserManager m_userManager;

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
            m_favoriteManager = m_container.Resolve<FavoriteManager>();
        }

        public IEnumerable<AuthorDetailContract> GetAllAuthors()
        {
            return m_authorManager.GetAllAuthors();
        }

        public BookInfoWithPagesContract GetBookInfoWithPages(string bookGuid)
        {
            return m_bookManager.GetBookInfoWithPages(bookGuid);
        }

        public SearchResultDetailContract GetBookDetailInfoById(long bookId)
        {
            return m_searchManager.GetBookDetailInfoById(bookId);
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

        public bool HasBookPageByXmlId(string bookGuid, string pageXmlId)
        {
            return m_bookManager.HasBookPageByXmlId(bookGuid, pageXmlId);
        }

        public string GetBookPageByXmlId(string bookGuid, string pageXmlId, OutputFormatEnumContract resultFormat, BookTypeEnumContract bookTypeContract)
        {
            return m_bookManager.GetBookPageByXmlId(bookGuid, pageXmlId, resultFormat, bookTypeContract);
        }

        public long GetBookIdByXmlId(string bookGuid)
        {
            return m_bookManager.GetBookIdByXmlId(bookGuid);
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

        public IList<SearchResultContract> Search(string term)
        {
            return m_searchManager.Search(term);
        }

        public bool HasBookImage(string bookXmlId, string versionId)
        {
            return m_bookManager.HasBookImage(bookXmlId, versionId);
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

        public IList<string> GetTypeaheadDictionaryHeadwords(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query, BookTypeEnumContract? bookType)
        {
            return m_searchManager.GetTypeaheadDictionaryHeadwords(selectedCategoryIds, selectedBookIds, query, bookType);
        }

        public IList<string> GetTypeaheadAuthorsByBookType(string query, BookTypeEnumContract bookType)
        {
            return m_searchManager.GetTypeaheadAuthorsByBookType(query, bookType);
        }

        public IList<string> GetTypeaheadTitlesByBookType(string query, BookTypeEnumContract bookType, IList<int> selectedCategoryIds,
            IList<long> selectedBookIds)
        {
            return m_searchManager.GetTypeaheadTitlesByBookType(query, bookType, selectedCategoryIds, selectedBookIds);
        }

        public IList<string> GetTypeaheadTermsByBookType(string query, BookTypeEnumContract bookType, IList<int> selectedCategoryIds,
            IList<long> selectedBookIds)
        {
            return m_searchManager.GetTypeaheadTermsByBookType(query, bookType, selectedCategoryIds, selectedBookIds);
        }

        public int GetHeadwordCount(IList<int> selectedCategoryIds, IList<long> selectedBookIds, BookTypeEnumContract bookType)
        {
            return m_searchManager.GetHeadwordCount(selectedCategoryIds, selectedBookIds, bookType);
        }

        public HeadwordListContract GetHeadwordList(IList<int> selectedCategoryIds, IList<long> selectedBookIds, int start, int count, BookTypeEnumContract bookType)
        {
            return m_searchManager.GetHeadwordList(selectedCategoryIds, selectedBookIds, start, count, bookType);
        }

        public long GetHeadwordRowNumber(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string query, BookTypeEnumContract bookType)
        {
            return m_searchManager.GetHeadwordRowNumber(selectedCategoryIds, selectedBookIds, query, bookType);
        }

        public long GetHeadwordRowNumberById(IList<int> selectedCategoryIds, IList<long> selectedBookIds, string headwordBookId, string headwordEntryXmlId, BookTypeEnumContract bookType)
        {
            return m_searchManager.GetHeadwordRowNumberById(selectedCategoryIds, selectedBookIds, headwordBookId,
                headwordEntryXmlId, bookType);
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

        public string GetDictionaryEntryFromSearch(IEnumerable<SearchCriteriaContract> searchCriterias, string bookGuid, string xmlEntryId,
            OutputFormatEnumContract resultFormat, BookTypeEnumContract bookType)
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

        public AudioBookSearchResultContract GetAudioBookDetailInfoById(long bookId)
        {
            return m_searchManager.GetAudioBookDetailInfoById(bookId);
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

        public IList<SpecialPermissionContract> GetSpecialPermissionsForUser()
        {
            return m_permissionManager.GetSpecialPermissionsForUser();
        }

        public IList<SpecialPermissionContract> GetSpecialPermissionsForUserByType(SpecialPermissionCategorizationEnumContract permissionType)
        {
            return m_permissionManager.GetSpecialPermissionsForUserByType(permissionType);
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
        
        #region news

        public void CreateNewsSyndicationItem(string title, string content, string url, NewsTypeContract itemType, string username)
        {
            m_newsManager.CreateNewSyndicationItem(title, content, url, itemType, username);
        }

        public IList<TermCategoryContract> GetTermCategoriesWithTerms()
        {
            return m_bookManager.GetTermCategoriesWithTerms();
        }

        public string GetBookEditionNote(long bookId, OutputFormatEnumContract outputFormat)
        {
            return m_bookManager.GetBookEditionNote(bookId, outputFormat);
        }

        #endregion

        public void Dispose()
        {
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

        #region Favorite Items

        public IList<FavoriteLabelContract> GetFavoriteLabels(int latestLabelCount)
        {
            return m_favoriteManager.GetFavoriteLabels(latestLabelCount);
        }

        public long CreateFavoriteLabel(string name, string color)
        {
            return m_favoriteManager.CreateFavoriteLabel(name, color, false);
        }

        public void UpdateFavoriteLabel(long labelId, string name, string color)
        {
            m_favoriteManager.UpdateFavoriteLabel(labelId, name, color);
        }

        public void DeleteFavoriteLabel(long labelId)
        {
            m_favoriteManager.DeleteFavoriteLabel(labelId);
        }

        public IList<FavoriteBaseInfoContract> GetFavoriteItems(long? labelId, FavoriteTypeContract? filterByType, string filterByTitle, FavoriteSortContract sort, int start, int count)
        {
            return m_favoriteManager.GetFavoriteItems(labelId, filterByType, filterByTitle, sort, start, count);
        }

        public int GetFavoriteItemsCount(long? labelId, FavoriteTypeContract? filterByType, string filterByTitle)
        {
            return m_favoriteManager.GetFavoriteItemsCount(labelId, filterByType, filterByTitle);
        }

        public IList<FavoriteQueryContract> GetFavoriteQueries(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle, int start, int count)
        {
            return m_favoriteManager.GetFavoriteQueries(labelId, bookType, queryType, filterByTitle, start, count);
        }

        public int GetFavoriteQueriesCount(long? labelId, BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string filterByTitle)
        {
            return m_favoriteManager.GetFavoriteQueriesCount(labelId, bookType, queryType, filterByTitle);
        }

        public List<PageBookmarkContract> GetPageBookmarks(string bookId)
        {
            return m_favoriteManager.GetPageBookmarks(bookId);
        }
        
        public IList<HeadwordBookmarkContract> GetHeadwordBookmarks()
        {
            return m_favoriteManager.GetHeadwordBookmarks();
        }

        public FavoriteFullInfoContract GetFavoriteItem(long id)
        {
            return m_favoriteManager.GetFavoriteItem(id);
        }

        public void AddHeadwordBookmark(string bookXmlId, string entryXmlId)
        {
            m_favoriteManager.AddHeadwordBookmark(bookXmlId, entryXmlId);
        }

        public void RemoveHeadwordBookmark(string bookXmlId, string entryXmlId)
        {
            m_favoriteManager.RemoveHeadwordBookmark(bookXmlId, entryXmlId);
        }

        public IList<FavoriteBookInfoContract> GetFavoriteLabeledBooks(IList<long> bookIds)
        {
            return m_favoriteManager.GetFavoriteLabeledBooks(bookIds);
        }

        public IList<FavoriteCategoryContract> GetFavoriteLabeledCategories(IList<int> categoryIds)
        {
            return m_favoriteManager.GetFavoriteLabeledCategories(categoryIds);
        }

        public IList<FavoriteLabelWithBooksAndCategories> GetFavoriteLabelsWithBooksAndCategories(BookTypeEnumContract bookType)
        {
            return m_favoriteManager.GetFavoriteLabelsWithBooksAndCategories(bookType);
        }

        public void UpdateFavoriteItem(long id, string title)
        {
            m_favoriteManager.UpdateFavoriteItem(id, title);
        }

        public void DeleteFavoriteItem(long id)
        {
            m_favoriteManager.DeleteFavoriteItem(id);
        }

        public IList<long> CreateFavoriteBook(long bookId, string title, IList<long> labelIds)
        {
            return m_favoriteManager.CreateFavoriteBook(bookId, title, labelIds);
        }

        public IList<long> CreateFavoriteCategory(int categoryId, string title, IList<long> labelIds)
        {
            return m_favoriteManager.CreateFavoriteCategory(categoryId, title, labelIds);
        }

        public IList<long> CreateFavoriteQuery(BookTypeEnumContract bookType, QueryTypeEnumContract queryType, string query, string title, IList<long> labelIds)
        {
            return m_favoriteManager.CreateFavoriteQuery(bookType, queryType, query, title, labelIds);
        }

        public long CreatePageBookmark(string bookXmlId, string pageXmlId, string title, long? labelId)
        {
            return m_favoriteManager.CreatePageBookmark(bookXmlId, pageXmlId, title, labelId);
        }

        #endregion

        #region Feedback

        public void CreateFeedback(string feedback, string username, FeedbackCategoryEnumContract category)
        {
            m_feedbackManager.CreateFeedback(feedback, username, category);
        }

        public void CreateFeedbackForHeadword(string feedback, string bookXmlId, string versionXmlId, string entryXmlId,
            string username)
        {
            m_feedbackManager.CreateFeedbackForHeadword(feedback, bookXmlId, versionXmlId, entryXmlId, username);
        }

        #endregion
    }
}